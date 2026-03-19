```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
AMD Ryzen 5 7600 2.99GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.104
  [Host]     : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v4
  Job-XFHCDR : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v4

WarmupCount=10  

```
| Method              | Categories | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------- |----------- |----------:|----------:|----------:|------:|--------:|----------:|------------:|
| Manual              | Baseline   |  7.448 ns | 0.1133 ns | 0.1060 ns |  1.00 |    0.02 |      88 B |        1.00 |
| Mapperly            | Source Gen |  8.246 ns | 0.1981 ns | 0.2034 ns |  1.11 |    0.03 |      88 B |        1.00 |
| &#39;&gt; AotObjectMapper&#39; | Source Gen |  9.390 ns | 0.0872 ns | 0.0816 ns |  1.26 |    0.02 |      88 B |        1.00 |
| Mapster             | Reflection | 13.627 ns | 0.1655 ns | 0.1549 ns |  1.83 |    0.03 |      88 B |        1.00 |
| AutoMapper          | Reflection | 40.367 ns | 0.1220 ns | 0.1141 ns |  5.42 |    0.08 |      88 B |        1.00 |
