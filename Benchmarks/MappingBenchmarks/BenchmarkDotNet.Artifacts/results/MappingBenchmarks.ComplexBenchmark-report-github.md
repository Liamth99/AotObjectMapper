```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
AMD Ryzen 5 7600 2.99GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.103
  [Host]     : .NET 10.0.3 (10.0.3, 42.42.42.42424), X64 RyuJIT x86-64-v4
  Job-XFHCDR : .NET 10.0.3 (10.0.3, 42.42.42.42424), X64 RyuJIT x86-64-v4

WarmupCount=10  

```
| Method              | Categories | Mean     | Error   | StdDev  | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------- |----------- |---------:|--------:|--------:|------:|--------:|----------:|------------:|
| Manual              | Baseline   | 251.5 ns | 1.46 ns | 1.36 ns |  1.00 |    0.01 |   1.21 KB |        1.00 |
| &#39;&gt; AotObjectMapper&#39; | Source Gen | 265.1 ns | 0.94 ns | 0.84 ns |  1.05 |    0.01 |   1.34 KB |        1.10 |
| Mapster             | Reflection | 623.1 ns | 1.96 ns | 1.84 ns |  2.48 |    0.01 |   1.34 KB |        1.11 |
| Mapperly            | Source Gen | 682.6 ns | 1.05 ns | 0.98 ns |  2.71 |    0.01 |   1.86 KB |        1.54 |
| AutoMapper          | Reflection | 806.4 ns | 1.47 ns | 1.38 ns |  3.21 |    0.02 |   1.23 KB |        1.01 |
