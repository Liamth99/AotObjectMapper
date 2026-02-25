```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
AMD Ryzen 5 7600 2.99GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.103
  [Host]     : .NET 10.0.3 (10.0.3, 42.42.42.42424), X64 RyuJIT x86-64-v4
  Job-XFHCDR : .NET 10.0.3 (10.0.3, 42.42.42.42424), X64 RyuJIT x86-64-v4

WarmupCount=10  

```
| Method              | Categories | Mean      | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
|-------------------- |----------- |----------:|----------:|----------:|------:|----------:|------------:|
| Manual              | Baseline   |  5.278 ns | 0.0004 ns | 0.0003 ns |  1.00 |         - |          NA |
| Mapperly            | Source Gen |  7.733 ns | 0.0161 ns | 0.0151 ns |  1.47 |         - |          NA |
| &#39;&gt; AotObjectMapper&#39; | Source Gen |  9.201 ns | 0.0369 ns | 0.0345 ns |  1.74 |         - |          NA |
| Mapster             | Reflection | 25.296 ns | 0.0400 ns | 0.0374 ns |  4.79 |      88 B |          NA |
| AutoMapper          | Reflection | 53.054 ns | 0.0260 ns | 0.0230 ns | 10.05 |      88 B |          NA |
