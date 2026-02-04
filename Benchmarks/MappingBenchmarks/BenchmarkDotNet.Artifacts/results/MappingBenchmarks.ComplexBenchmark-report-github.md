```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7623/25H2/2025Update/HudsonValley2)
AMD Ryzen 5 7600 3.80GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v4
  Job-XFHCDR : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v4

WarmupCount=10  

```
| Method              | Categories | Mean      | Error    | StdDev   | Median    | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------- |----------- |----------:|---------:|---------:|----------:|------:|--------:|----------:|------------:|
| Manual              | Baseline   |  19.01 ns | 0.078 ns | 0.073 ns |  18.98 ns |  1.00 |    0.01 |     176 B |        1.00 |
| &#39;&gt; AotObjectMapper&#39; | Source Gen |  60.44 ns | 0.979 ns | 0.818 ns |  60.45 ns |  3.18 |    0.04 |     520 B |        2.95 |
| Mapster             | Reflection | 146.78 ns | 0.649 ns | 0.507 ns | 146.82 ns |  7.72 |    0.04 |     520 B |        2.95 |
| Mapperly            | Source Gen | 209.22 ns | 4.081 ns | 6.354 ns | 205.97 ns | 11.00 |    0.33 |     920 B |        5.23 |
| AutoMapper          | Reflection | 245.16 ns | 0.367 ns | 0.325 ns | 245.13 ns | 12.89 |    0.05 |     464 B |        2.64 |
