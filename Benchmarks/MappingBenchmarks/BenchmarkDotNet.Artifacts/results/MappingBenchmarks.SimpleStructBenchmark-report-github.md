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
| Manual              | Baseline   |  5.701 ns | 0.0529 ns | 0.0469 ns |  1.00 |    0.01 |         - |          NA |
| Mapperly            | Source Gen |  7.775 ns | 0.1776 ns | 0.1900 ns |  1.36 |    0.03 |         - |          NA |
| &#39;&gt; AotObjectMapper&#39; | Source Gen |  9.977 ns | 0.1194 ns | 0.1117 ns |  1.75 |    0.02 |         - |          NA |
| Mapster             | Reflection | 26.458 ns | 0.5483 ns | 0.5631 ns |  4.64 |    0.10 |      88 B |          NA |
| AutoMapper          | Reflection | 63.482 ns | 1.2783 ns | 1.2554 ns | 11.14 |    0.23 |      88 B |          NA |
