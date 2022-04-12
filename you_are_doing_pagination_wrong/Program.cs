// See https://aka.ms/new-console-template for more information
namespace YouAreDoingPaginationWrong;

using Bogus;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;



public class ObjectsContext : DbContext
{
    public DbSet<TheObject> objects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(
            "Server=127.0.0.1;Uid=root;Pwd=root;TreatTinyAsBoolean=true;Database=Pagination;",
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
    public static void Main()
    {
        var maxRange = 5000;
        var rangeBulk = 200;
        Randomizer.Seed = new Random(8675309);

        using var context = new ObjectsContext();

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
}

