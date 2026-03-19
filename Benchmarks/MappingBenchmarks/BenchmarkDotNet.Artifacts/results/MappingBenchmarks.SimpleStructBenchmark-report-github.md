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
| Manual              | Baseline   |  5.188 ns | 0.0023 ns | 0.0022 ns |  1.00 |    0.00 |         - |          NA |
| Mapperly            | Source Gen |  6.750 ns | 0.0064 ns | 0.0057 ns |  1.30 |    0.00 |         - |          NA |
| &#39;&gt; AotObjectMapper&#39; | Source Gen |  9.259 ns | 0.0094 ns | 0.0078 ns |  1.78 |    0.00 |         - |          NA |
| Mapster             | Reflection | 25.342 ns | 0.0135 ns | 0.0127 ns |  4.88 |    0.00 |      88 B |          NA |
| AutoMapper          | Reflection | 57.334 ns | 0.2510 ns | 0.2348 ns | 11.05 |    0.04 |      88 B |          NA |
