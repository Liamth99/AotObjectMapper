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
| Manual              | Baseline   | 254.9 ns | 2.02 ns | 1.89 ns |  1.00 |    0.01 |   1.21 KB |        1.00 |
| &#39;&gt; AotObjectMapper&#39; | Source Gen | 265.9 ns | 1.85 ns | 1.73 ns |  1.04 |    0.01 |   1.34 KB |        1.10 |
| Mapster             | Reflection | 627.2 ns | 2.46 ns | 2.18 ns |  2.46 |    0.02 |   1.34 KB |        1.11 |
| Mapperly            | Source Gen | 679.3 ns | 0.52 ns | 0.49 ns |  2.67 |    0.02 |   1.86 KB |        1.54 |
| AutoMapper          | Reflection | 772.5 ns | 1.86 ns | 1.74 ns |  3.03 |    0.02 |   1.23 KB |        1.01 |
