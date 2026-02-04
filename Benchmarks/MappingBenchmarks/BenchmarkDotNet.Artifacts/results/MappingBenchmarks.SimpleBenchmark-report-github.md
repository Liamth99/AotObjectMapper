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
| &#39;&gt; AotObjectMapper&#39; | Source Gen |  6.484 ns | 0.1402 ns | 0.3644 ns |  0.96 |    0.06 |      88 B |        1.00 |
| Manual              | Baseline   |  6.742 ns | 0.1688 ns | 0.1876 ns |  1.00 |    0.04 |      88 B |        1.00 |
| Mapperly            | Source Gen |  7.457 ns | 0.1744 ns | 0.2142 ns |  1.11 |    0.04 |      88 B |        1.00 |
| Mapster             | Reflection | 12.241 ns | 0.2819 ns | 0.3666 ns |  1.82 |    0.07 |      88 B |        1.00 |
| AutoMapper          | Reflection | 49.308 ns | 0.5412 ns | 0.4519 ns |  7.32 |    0.21 |      88 B |        1.00 |
