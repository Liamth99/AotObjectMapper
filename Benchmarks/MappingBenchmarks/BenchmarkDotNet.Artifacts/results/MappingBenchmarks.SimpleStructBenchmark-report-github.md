```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
AMD Ryzen 5 7600 3.47GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.104
  [Host]     : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v4
  Job-XFHCDR : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v4

WarmupCount=10  

```
| Method              | Categories | Mean      | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
|-------------------- |----------- |----------:|----------:|----------:|------:|----------:|------------:|
| Manual              | Baseline   |  5.214 ns | 0.0055 ns | 0.0052 ns |  1.00 |         - |          NA |
| Mapperly            | Source Gen |  6.776 ns | 0.0390 ns | 0.0365 ns |  1.30 |         - |          NA |
| &#39;&gt; AotObjectMapper&#39; | Source Gen |  9.199 ns | 0.0334 ns | 0.0296 ns |  1.76 |         - |          NA |
| Mapster             | Reflection | 26.246 ns | 0.0146 ns | 0.0129 ns |  5.03 |      88 B |          NA |
| AutoMapper          | Reflection | 53.171 ns | 0.0162 ns | 0.0143 ns | 10.20 |      88 B |          NA |
