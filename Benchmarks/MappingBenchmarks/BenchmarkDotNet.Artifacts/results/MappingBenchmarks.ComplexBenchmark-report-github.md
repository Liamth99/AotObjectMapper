```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7623/25H2/2025Update/HudsonValley2)
AMD Ryzen 5 7600 3.80GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v4
  Job-XFHCDR : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v4

WarmupCount=10  

```
| Method              | Categories | Mean      | Error    | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------- |----------- |----------:|---------:|----------:|------:|--------:|-------:|----------:|------------:|
| Manual              | Baseline   |  21.73 ns | 0.427 ns |  0.379 ns |  1.00 |    0.02 | 0.0105 |     176 B |        1.00 |
| &#39;&gt; AotObjectMapper&#39; | Source Gen |  75.04 ns | 3.545 ns | 10.452 ns |  3.45 |    0.48 | 0.0310 |     520 B |        2.95 |
| Mapster             | Reflection | 197.89 ns | 4.523 ns | 13.122 ns |  9.11 |    0.62 | 0.0310 |     520 B |        2.95 |
| Mapperly            | Source Gen | 240.87 ns | 4.710 ns | 10.038 ns | 11.09 |    0.49 | 0.0548 |     920 B |        5.23 |
| AutoMapper          | Reflection | 243.57 ns | 4.742 ns |  4.869 ns | 11.21 |    0.29 | 0.0277 |     464 B |        2.64 |
