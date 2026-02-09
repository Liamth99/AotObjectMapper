```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7623/25H2/2025Update/HudsonValley2)
AMD Ryzen 5 7600 3.80GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v4
  Job-XFHCDR : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v4

WarmupCount=10  

```
| Method              | Categories | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------- |----------- |----------:|----------:|----------:|------:|--------:|----------:|------------:|
| Manual              | Baseline   |  6.543 ns | 0.0509 ns | 0.0476 ns |  1.00 |    0.01 |      88 B |        1.00 |
| Mapperly            | Source Gen |  7.404 ns | 0.0957 ns | 0.0895 ns |  1.13 |    0.02 |      88 B |        1.00 |
| &#39;&gt; AotObjectMapper&#39; | Source Gen |  8.076 ns | 0.0876 ns | 0.0776 ns |  1.23 |    0.01 |      88 B |        1.00 |
| Mapster             | Reflection | 11.566 ns | 0.0604 ns | 0.0565 ns |  1.77 |    0.02 |      88 B |        1.00 |
| AutoMapper          | Reflection | 47.693 ns | 0.1559 ns | 0.1458 ns |  7.29 |    0.06 |      88 B |        1.00 |
