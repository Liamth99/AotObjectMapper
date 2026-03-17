```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
AMD Ryzen 5 7600 3.47GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.104
  [Host]     : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v4
  Job-XFHCDR : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v4

WarmupCount=10  

```
| Method              | Categories | Mean     | Error   | StdDev  | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------- |----------- |---------:|--------:|--------:|------:|--------:|----------:|------------:|
| Manual              | Baseline   | 257.7 ns | 2.28 ns | 2.13 ns |  1.00 |    0.01 |   1.21 KB |        1.00 |
| &#39;&gt; AotObjectMapper&#39; | Source Gen | 272.1 ns | 2.45 ns | 2.29 ns |  1.06 |    0.01 |   1.34 KB |        1.10 |
| Mapster             | Reflection | 630.9 ns | 4.62 ns | 4.32 ns |  2.45 |    0.03 |   1.34 KB |        1.11 |
| Mapperly            | Source Gen | 697.7 ns | 1.22 ns | 1.14 ns |  2.71 |    0.02 |   1.86 KB |        1.54 |
| AutoMapper          | Reflection | 782.6 ns | 0.95 ns | 0.89 ns |  3.04 |    0.02 |   1.23 KB |        1.01 |
