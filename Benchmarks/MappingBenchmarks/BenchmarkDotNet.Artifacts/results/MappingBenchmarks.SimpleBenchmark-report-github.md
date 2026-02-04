```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7623/25H2/2025Update/HudsonValley2)
AMD Ryzen 5 7600 3.80GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v4
  Job-XFHCDR : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v4

WarmupCount=10  

```
| Method              | Categories | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------- |----------- |----------:|----------:|----------:|----------:|------:|--------:|----------:|------------:|
| &#39;&gt; AotObjectMapper&#39; | Source Gen |  6.730 ns | 0.1753 ns | 0.2570 ns |  6.600 ns |  0.85 |    0.13 |      88 B |        1.00 |
| Mapperly            | Source Gen |  8.069 ns | 0.2026 ns | 0.2412 ns |  7.945 ns |  1.02 |    0.15 |      88 B |        1.00 |
| Manual              | Baseline   |  8.100 ns | 0.4307 ns | 1.2633 ns |  7.765 ns |  1.02 |    0.22 |      88 B |        1.00 |
| Mapster             | Reflection | 12.753 ns | 0.2955 ns | 0.3284 ns | 12.720 ns |  1.61 |    0.24 |      88 B |        1.00 |
| AutoMapper          | Reflection | 45.928 ns | 0.9009 ns | 0.9640 ns | 45.602 ns |  5.80 |    0.87 |      88 B |        1.00 |
