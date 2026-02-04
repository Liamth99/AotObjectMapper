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
| &#39;&gt; AotObjectMapper&#39; | Source Gen |  6.151 ns | 0.0536 ns | 0.0475 ns |  0.91 |    0.02 |      88 B |        1.00 |
| Manual              | Baseline   |  6.744 ns | 0.1651 ns | 0.1544 ns |  1.00 |    0.03 |      88 B |        1.00 |
| Mapperly            | Source Gen |  6.908 ns | 0.0467 ns | 0.0414 ns |  1.02 |    0.02 |      88 B |        1.00 |
| Mapster             | Reflection | 11.560 ns | 0.0813 ns | 0.0721 ns |  1.71 |    0.04 |      88 B |        1.00 |
| AutoMapper          | Reflection | 44.702 ns | 0.3323 ns | 0.2946 ns |  6.63 |    0.15 |      88 B |        1.00 |
