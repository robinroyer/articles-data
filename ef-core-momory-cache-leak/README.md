EF Core memory leak

===

- use `backgroundJob` in config with `leak` or `no-leak`.
- go to http://localhost:5240/WeatherForecast to get weather forecasts




ForecastJobWithLeak.Work:
| Method |  Loop |         Mean |          Error |        StdDev |         Gen 0 |       Gen 1 |       Gen 2 |  Allocated |
|------- |------ |-------------:|---------------:|--------------:|--------------:|------------:|------------:|-----------:|
|   Work |   100 |     465.8 ms |       731.4 ms |      40.09 ms |     2000.0000 |           - |           - |      39 MB |
|   Work |   500 |   4,438.0 ms |    17,558.7 ms |     962.45 ms |    58000.0000 |   1000.0000 |           - |     914 MB |
|   Work |  1000 |  13,344.0 ms |    66,832.6 ms |   3,663.32 ms |   231000.0000 |   7000.0000 |           - |   3,631 MB |
|   Work |  5000 | 223,459.5 ms | 1,423,820.1 ms |  78,044.38 ms |  5515000.0000 |  99000.0000 |  48000.0000 |  90,249 MB |
|   Work | 10000 | 787,672.9 ms | 4,755,206.5 ms | 260,648.91 ms | 21847000.0000 | 279000.0000 | 192000.0000 | 360,742 MB |


ForecastJobWithoutLeak.Work:

| Method |  Loop |        Mean |       Error |    StdDev | Ratio |      Gen 0 |     Gen 1 | Allocated |
|------- |------ |------------:|------------:|----------:|------:|-----------:|----------:|----------:|
|   Work |   100 |    434.6 ms |    222.9 ms |  12.22 ms |  1.00 |          - |         - |     10 MB |
|   Work |   500 |  1,927.2 ms |    249.6 ms |  13.68 ms |  1.00 |  3000.0000 |         - |     50 MB |
|   Work |  1000 |  3,828.6 ms |    486.7 ms |  26.68 ms |  1.00 |  6000.0000 |         - |    100 MB |
|   Work |  5000 | 19,264.2 ms |  2,996.4 ms | 164.25 ms |  1.00 | 30000.0000 | 1000.0000 |    502 MB |
|   Work | 10000 | 38,891.4 ms | 11,711.1 ms | 641.92 ms |  1.00 | 61000.0000 |         - |  1,004 MB |


// * Legends *
  Loop      : Value of the 'Loop' parameter
  Mean      : Arithmetic mean of all measurements
  Error     : Half of 99.9% confidence interval
  Ratio     : Mean of the ratio distribution ([Current]/[Baseline])
  Gen 0     : GC Generation 0 collects per 1000 operations
  Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ms      : 1 Millisecond (0.001 sec)
