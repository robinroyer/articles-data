// See https://aka.ms/new-console-template for more information
namespace YouAreDoingPaginationWrong;

using Bogus;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data;
using Microsoft.Data.Analysis;



public class ObjectsContext : DbContext
{
    public DbSet<TheObject> objects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(
            "Server=127.0.0.1;Uid=root;Pwd=root;TreatTinyAsBoolean=true;Database=Pagination;ConnectionTimeout=500",
            MariaDbServerVersion.LatestSupportedServerVersion);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}

public class TheObject
{
    public static string[] Tenants = new []
    {
        "tenant-1",
        "tenant-2",
        "tenant-3",
        "tenant-4",
    };
    public int Id { get; set; }
    public string Content { get; set; }
    public string Tenant { get; set; }
    public DateTime CreationDate { get; set; }

    public override string ToString()
    {
        return $"[{Id}][{Tenant}][{CreationDate}] : {Content.Substring(0, 25)}";
    }
}


public class TheObjectFactory
{
    private static int id = 1;
    private Faker<TheObject> _fake = new Faker<TheObject>()
        .RuleFor(
            o => o.Content,
            f => f.Random.String(200))
        .RuleFor(
            o => o.Tenant,
            f => f.PickRandom(TheObject.Tenants))
        .RuleFor(
            o => o.Id,
            f => id++)
        .RuleFor(
            o => o.CreationDate,
            f => f.Date.Between(
                new DateTime(2020, 1, 1),
                new DateTime(2022, 12, 31)));
    public TheObject Generate()
    {
        return _fake.Generate();
    }
}



public class MainClass
{

    private static ObjectsContext GetContext()
    {
        var ctxt =  new ObjectsContext();
        ctxt.Database.SetCommandTimeout(60 * 10);
        return ctxt;
    }


    private static void Populate()
    {
        var maxRange = 200;
        var rangeBulk = 50000;
        Randomizer.Seed = new Random(8675309);

        using var context = GetContext();

        var @count = context.objects.AsNoTracking().Count();
        Console.WriteLine($"Hello, World! {@count} objects" );

        var factory = new TheObjectFactory();


        var bulks = Enumerable
            .Range(0, maxRange)
            .Select(_ => Enumerable
                .Range(0, rangeBulk)
                .Select(_ => factory.Generate())
                .ToArray());
        
        foreach (var bulk in bulks)
        {
            context.objects.AddRange(bulk);
            context.SaveChanges();
        }

        @count = context.objects.AsNoTracking().Count();
        Console.WriteLine($"Bye, World! {@count} objects" );
    }



    private static async Task<IEnumerable<TheObject>> OffsetPagination(
        int offset = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        using var context = GetContext();

        return await context
            .objects
            .AsNoTracking()
            .OrderBy(x => x.CreationDate)
            .ThenBy(x => x.Id)
            .Skip(offset)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    private static async Task<DataFrame> OffsetAllRowsIteration(
        List<int> offsetIdx,
        CancellationToken cancellationToken = default)
    {
        var offsetColumnName = "offset";
        var offsetColumn = new PrimitiveDataFrameColumn<int>(offsetColumnName);
        var timeColumnName = "offset-pagination";
        var timePaginationColumn = new PrimitiveDataFrameColumn<TimeSpan>(timeColumnName);

        var df = new DataFrame(offsetColumn, timePaginationColumn);
        
        try
        {
            IEnumerable<TheObject> page;

            foreach(var idx in offsetIdx)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var before = DateTime.UtcNow;
                page = await OffsetPagination(idx, cancellationToken: cancellationToken);
                var after = DateTime.UtcNow;

                var spendTime = after - before;
                df.Append(
                    new KeyValuePair<string, object>[]
                    {
                        new KeyValuePair<string, object>(offsetColumnName, idx),
                        new KeyValuePair<string, object>(timeColumnName, spendTime),
                    },
                    inPlace: true);
            }
        }
        catch(Exception e)
        {
            Console.Error.WriteLine($"offset interrupted {e.Message}");
        }

        return df;
    }


    private static async Task<IEnumerable<TheObject>> SeekPagination(
        DateTime? lastCreationDate = default,
        int? lastId = default,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        using var context = GetContext();

        IQueryable<TheObject> queryable = context
            .objects
            .AsNoTracking()
            .OrderBy(x => x.CreationDate)
            .ThenBy(x => x.Id);

        if (lastCreationDate != null && lastId != null)
        {
            queryable = queryable
                .Where(x =>
                    (x.CreationDate == lastCreationDate && x.Id > lastId)
                    || x.CreationDate > lastCreationDate);
        }

        return await queryable
            .Take(take)
            .ToListAsync(cancellationToken);
    }


    private static async Task<DataFrame> SeekAllRowsIteration(
        List<int> offsetIdx,
        CancellationToken cancellationToken = default)
    {
        var offsetColumnName = "offset";
        var offsetColumn = new PrimitiveDataFrameColumn<int>(offsetColumnName);
        var timeColumnName = "seek-pagination";
        var timePaginationColumn = new PrimitiveDataFrameColumn<TimeSpan>(timeColumnName);

        var df = new DataFrame(offsetColumn, timePaginationColumn);
        
        try
        {
            IEnumerable<TheObject> page;
            DateTime? lastCreationDate = null;
            int? lastId = null;
            int offset = 0;

            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                var before = DateTime.UtcNow;
                page = await SeekPagination(lastCreationDate, lastId, cancellationToken: cancellationToken);
                var after = DateTime.UtcNow;

                if (offsetIdx.Contains(offset))
                {
                    var spendTime = after - before;
                    df.Append(
                        new KeyValuePair<string, object>[]
                        {
                            new KeyValuePair<string, object>(offsetColumnName, offset),
                            new KeyValuePair<string, object>(timeColumnName, spendTime),
                        },
                        inPlace: true);
                }

                offset += page.Count();
                lastId = page.LastOrDefault()?.Id ?? null;
                lastCreationDate = page.LastOrDefault()?.CreationDate ?? null;
            } while (page.Any());
        }
        catch(Exception e)
        {
            Console.Error.WriteLine($"seek interrupted: {e.Message}");
        }

        return df;
    }

    public static async Task Main()
    {
        Console.Error.WriteLine("begin of application");

        var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (s, arg) =>
        {
            Console.Error.WriteLine("begin of interruption");
            cts.Cancel();
            arg.Cancel = true;
        };


        // Populate();

        var offsetIdx = new List<int>()
        {
            10, 100, 1000, 10000, 50000, 100000, 200000, 500000, 1000000, 5000000, 10000000
        };

        var dfSeekTask = SeekAllRowsIteration(offsetIdx, cancellationToken: cts.Token);
        var dfOffsetTask = OffsetAllRowsIteration(offsetIdx, cancellationToken: cts.Token);

        var dfSeek = await dfSeekTask;
        var dfOffset = await dfOffsetTask;

        Console.WriteLine(dfSeek.ToString());
        Console.WriteLine(dfOffset.ToString());

        var df = dfSeek.Merge<string>(dfOffset, "offset", "offset");
        Console.WriteLine(df.ToString());

        DataFrame.WriteCsv(df, "/tmp/dataframe-pagination.csv");


        // TODO:1 find a way to mesure the query + the whole iteration process (dataframe ?)
        // TODO:2 add a switch on program parms to test different things
        // TODO:3 run the test with plot to write article
        // TODO:4 write article
        // TODO: 5 apply to qarnot
    }
}

