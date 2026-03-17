```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
AMD Ryzen 5 7600 3.47GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.104
  [Host]     : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v4
  Job-XFHCDR : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v4

WarmupCount=10  

```
| Method              | Categories | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------- |----------- |----------:|----------:|----------:|------:|--------:|----------:|------------:|
| Manual              | Baseline   |  7.419 ns | 0.1357 ns | 0.1269 ns |  1.00 |    0.02 |      88 B |        1.00 |
| Mapperly            | Source Gen |  7.996 ns | 0.1085 ns | 0.1015 ns |  1.08 |    0.02 |      88 B |        1.00 |
| &#39;&gt; AotObjectMapper&#39; | Source Gen |  9.298 ns | 0.1044 ns | 0.0977 ns |  1.25 |    0.02 |      88 B |        1.00 |
| Mapster             | Reflection | 13.365 ns | 0.1472 ns | 0.1376 ns |  1.80 |    0.03 |      88 B |        1.00 |
| AutoMapper          | Reflection | 46.234 ns | 0.4950 ns | 0.4630 ns |  6.23 |    0.12 |      88 B |        1.00 |
