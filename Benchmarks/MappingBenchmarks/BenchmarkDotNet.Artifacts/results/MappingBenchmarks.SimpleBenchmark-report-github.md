```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
AMD Ryzen 5 7600 2.99GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.103
  [Host]     : .NET 10.0.3 (10.0.3, 42.42.42.42424), X64 RyuJIT x86-64-v4
  Job-XFHCDR : .NET 10.0.3 (10.0.3, 42.42.42.42424), X64 RyuJIT x86-64-v4

WarmupCount=10  

```
| Method              | Categories | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------- |----------- |----------:|----------:|----------:|------:|--------:|----------:|------------:|
| Manual              | Baseline   |  7.370 ns | 0.0119 ns | 0.0093 ns |  1.00 |    0.00 |      88 B |        1.00 |
| Mapperly            | Source Gen |  8.648 ns | 0.0934 ns | 0.0828 ns |  1.17 |    0.01 |      88 B |        1.00 |
| &#39;&gt; AotObjectMapper&#39; | Source Gen |  9.361 ns | 0.1596 ns | 0.1493 ns |  1.27 |    0.02 |      88 B |        1.00 |
| Mapster             | Reflection | 13.217 ns | 0.2262 ns | 0.2116 ns |  1.79 |    0.03 |      88 B |        1.00 |
| AutoMapper          | Reflection | 44.396 ns | 0.0219 ns | 0.0194 ns |  6.02 |    0.01 |      88 B |        1.00 |
