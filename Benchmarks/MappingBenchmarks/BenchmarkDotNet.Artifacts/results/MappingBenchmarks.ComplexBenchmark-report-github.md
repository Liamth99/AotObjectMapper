```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7623/25H2/2025Update/HudsonValley2)
AMD Ryzen 5 7600 3.80GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v4
  Job-XFHCDR : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v4

WarmupCount=10  

```
| Method              | Categories | Mean     | Error   | StdDev  | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------- |----------- |---------:|--------:|--------:|------:|--------:|----------:|------------:|
| &#39;&gt; AotObjectMapper&#39; | Source Gen | 218.6 ns | 0.46 ns | 0.43 ns |  0.92 |    0.00 |   1.34 KB |        1.10 |
| Manual              | Baseline   | 237.3 ns | 1.20 ns | 1.06 ns |  1.00 |    0.01 |   1.21 KB |        1.00 |
| Mapster             | Reflection | 600.5 ns | 3.56 ns | 3.33 ns |  2.53 |    0.02 |   1.34 KB |        1.11 |
| Mapperly            | Source Gen | 638.2 ns | 1.89 ns | 1.77 ns |  2.69 |    0.01 |   1.86 KB |        1.54 |
| AutoMapper          | Reflection | 803.7 ns | 1.86 ns | 1.65 ns |  3.39 |    0.02 |   1.23 KB |        1.01 |
