```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7623/25H2/2025Update/HudsonValley2)
AMD Ryzen 5 7600 3.80GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v4


```
| Method              | Categories | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------- |----------- |----------:|----------:|----------:|------:|--------:|----------:|------------:|
| Manual              | Baseline   |  6.901 ns | 0.1753 ns | 0.2087 ns |  1.00 |    0.04 |      88 B |        1.00 |
| Mapperly            | Source Gen |  6.923 ns | 0.0901 ns | 0.0704 ns |  1.00 |    0.03 |      88 B |        1.00 |
| &#39;&gt; AotObjectMapper&#39; | Source Gen |  7.515 ns | 0.1835 ns | 0.2184 ns |  1.09 |    0.04 |      88 B |        1.00 |
| Mapster             | Reflection | 12.107 ns | 0.2737 ns | 0.3925 ns |  1.76 |    0.08 |      88 B |        1.00 |
| AutoMapper          | Reflection | 43.652 ns | 0.8364 ns | 0.7824 ns |  6.33 |    0.21 |      88 B |        1.00 |
