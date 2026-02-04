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
| Manual              | Baseline   |  5.483 ns | 0.0299 ns | 0.0265 ns |  1.00 |    0.01 |         - |          NA |
| Mapperly            | Source Gen |  7.229 ns | 0.0416 ns | 0.0389 ns |  1.32 |    0.01 |         - |          NA |
| &#39;&gt; AotObjectMapper&#39; | Source Gen |  8.085 ns | 0.0392 ns | 0.0367 ns |  1.47 |    0.01 |         - |          NA |
| Mapster             | Reflection | 40.341 ns | 0.5335 ns | 0.4990 ns |  7.36 |    0.09 |      88 B |          NA |
| AutoMapper          | Reflection | 55.085 ns | 0.3228 ns | 0.2695 ns | 10.05 |    0.07 |      88 B |          NA |
