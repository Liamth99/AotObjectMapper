```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
AMD Ryzen 5 7600 2.99GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.104
  [Host]     : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v4
  Job-XFHCDR : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v4

WarmupCount=10  

```
| Method              | Categories | Mean     | Error   | StdDev  | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------- |----------- |---------:|--------:|--------:|------:|--------:|----------:|------------:|
| Manual              | Baseline   | 252.6 ns | 2.59 ns | 2.42 ns |  1.00 |    0.01 |   1.21 KB |        1.00 |
| &#39;&gt; AotObjectMapper&#39; | Source Gen | 260.8 ns | 2.20 ns | 1.95 ns |  1.03 |    0.01 |   1.34 KB |        1.10 |
| Mapster             | Reflection | 623.3 ns | 1.98 ns | 1.85 ns |  2.47 |    0.02 |   1.34 KB |        1.11 |
| Mapperly            | Source Gen | 679.9 ns | 0.97 ns | 0.91 ns |  2.69 |    0.03 |   1.86 KB |        1.54 |
| AutoMapper          | Reflection | 806.9 ns | 1.02 ns | 0.95 ns |  3.20 |    0.03 |   1.23 KB |        1.01 |
