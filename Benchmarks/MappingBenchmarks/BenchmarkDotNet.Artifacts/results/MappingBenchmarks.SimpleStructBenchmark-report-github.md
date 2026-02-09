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
| Manual              | Baseline   |  5.480 ns | 0.0290 ns | 0.0271 ns |  1.00 |    0.01 |         - |          NA |
| Mapperly            | Source Gen |  7.303 ns | 0.0346 ns | 0.0324 ns |  1.33 |    0.01 |         - |          NA |
| &#39;&gt; AotObjectMapper&#39; | Source Gen |  9.623 ns | 0.0340 ns | 0.0302 ns |  1.76 |    0.01 |         - |          NA |
| Mapster             | Reflection | 24.579 ns | 0.0521 ns | 0.0487 ns |  4.49 |    0.02 |      88 B |          NA |
| AutoMapper          | Reflection | 54.340 ns | 0.1868 ns | 0.1747 ns |  9.92 |    0.06 |      88 B |          NA |
