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
| Mapperly            | Source Gen |  7.909 ns | 0.0687 ns | 0.0574 ns |  0.97 |    0.03 |      88 B |        1.00 |
| Manual              | Baseline   |  8.136 ns | 0.1954 ns | 0.2250 ns |  1.00 |    0.04 |      88 B |        1.00 |
| &#39;&gt; AotObjectMapper&#39; | Source Gen |  9.404 ns | 0.0713 ns | 0.0667 ns |  1.16 |    0.03 |      88 B |        1.00 |
| Mapster             | Reflection | 13.314 ns | 0.2225 ns | 0.2082 ns |  1.64 |    0.05 |      88 B |        1.00 |
| AutoMapper          | Reflection | 40.817 ns | 0.0149 ns | 0.0140 ns |  5.02 |    0.14 |      88 B |        1.00 |
