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
| Manual              | Baseline   |  6.655 ns | 0.0449 ns | 0.0420 ns |  1.00 |    0.01 |         - |          NA |
| Mapperly            | Source Gen |  7.437 ns | 0.0905 ns | 0.0846 ns |  1.12 |    0.01 |         - |          NA |
| &#39;&gt; AotObjectMapper&#39; | Source Gen |  9.946 ns | 0.0972 ns | 0.0862 ns |  1.49 |    0.02 |         - |          NA |
| Mapster             | Reflection | 26.411 ns | 0.2394 ns | 0.2122 ns |  3.97 |    0.04 |      88 B |          NA |
| AutoMapper          | Reflection | 61.789 ns | 0.7921 ns | 0.7409 ns |  9.28 |    0.12 |      88 B |          NA |
