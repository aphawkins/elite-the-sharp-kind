```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6899)
Intel Core i5-14600K 3.50GHz, 1 CPU, 20 logical and 14 physical cores
.NET SDK 10.0.100-rc.2.25502.107
  [Host]     : .NET 10.0.0 (10.0.0-rc.2.25502.107, 10.0.25.50307), X64 RyuJIT x86-64-v3 [AttachedDebugger]
  DefaultJob : .NET 10.0.0 (10.0.0-rc.2.25502.107, 10.0.25.50307), X64 RyuJIT x86-64-v3


```
| Method           | Mean            | Error       | StdDev      |
|----------------- |----------------:|------------:|------------:|
| Clear            |  15,340.8988 ns | 304.6746 ns | 396.1631 ns |
| DrawCircle       |     997.4499 ns |   2.9922 ns |   2.7989 ns |
| DrawCircleFilled | 134,649.7754 ns | 948.1352 ns | 886.8863 ns |
| DrawLine         |   1,860.9377 ns |  21.2752 ns |  19.9009 ns |
| DrawPixel        |       0.3790 ns |   0.0032 ns |   0.0030 ns |
