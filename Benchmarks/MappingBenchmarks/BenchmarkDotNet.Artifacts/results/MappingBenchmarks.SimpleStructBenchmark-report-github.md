```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
AMD Ryzen 5 7600 2.99GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.104
  [Host]     : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v4
  Job-XFHCDR : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v4

WarmupCount=10  

```
| Method              | Categories | Mean      | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
|-------------------- |----------- |----------:|----------:|----------:|------:|----------:|------------:|
| Manual              | Baseline   |  5.165 ns | 0.0005 ns | 0.0004 ns |  1.00 |         - |          NA |
| Mapperly            | Source Gen |  7.986 ns | 0.0096 ns | 0.0085 ns |  1.55 |         - |          NA |
| &#39;&gt; AotObjectMapper&#39; | Source Gen | 10.256 ns | 0.0703 ns | 0.0658 ns |  1.99 |         - |          NA |
| Mapster             | Reflection | 25.322 ns | 0.0158 ns | 0.0132 ns |  4.90 |      88 B |          NA |
| AutoMapper          | Reflection | 53.965 ns | 0.0165 ns | 0.0146 ns | 10.45 |      88 B |          NA |
