``` ini

BenchmarkDotNet=v0.13.1, OS=ubuntu 20.04
AMD Ryzen 5 PRO 2500U w/ Radeon Vega Mobile Gfx, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  Job-UZSFOI : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT

IterationCount=1  RunStrategy=Monitoring  

```
| Method |   N |    Mean | Error |      Gen 0 |     Gen 1 | Allocated |
|------- |---- |--------:|------:|-----------:|----------:|----------:|
|   Work | 500 | 3.439 s |    NA | 32000.0000 | 2000.0000 |    515 MB |
