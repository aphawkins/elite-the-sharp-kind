```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6899)
Intel Core i5-14600K 3.50GHz, 1 CPU, 20 logical and 14 physical cores
.NET SDK 10.0.100-rc.2.25502.107
  [Host]     : .NET 10.0.0 (10.0.0-rc.2.25502.107, 10.0.25.50307), X64 RyuJIT x86-64-v3 [AttachedDebugger]
  DefaultJob : .NET 10.0.0 (10.0.0-rc.2.25502.107, 10.0.25.50307), X64 RyuJIT x86-64-v3


```
| Method           | Mean            | Error         | StdDev        | Median          |
|----------------- |----------------:|--------------:|--------------:|----------------:|
| Clear            |  16,606.5855 ns |   307.9335 ns |   288.0412 ns |  16,605.2246 ns |
| DrawCircle       |   1,046.4827 ns |     6.3503 ns |     5.9401 ns |   1,047.9958 ns |
| DrawCircleFilled | 135,979.1829 ns | 1,861.5248 ns | 1,741.2715 ns | 135,765.5518 ns |
| DrawImage        |     197.6575 ns |     0.7449 ns |     0.6603 ns |     197.3906 ns |
| DrawImageCentre  |     182.0331 ns |     0.4272 ns |     0.3996 ns |     182.1143 ns |
| DrawLine         |   2,507.2296 ns |   151.2974 ns |   446.1037 ns |   2,780.0663 ns |
| DrawPixel        |       0.3800 ns |     0.0041 ns |     0.0038 ns |       0.3791 ns |
| DrawPolygon      |     214.2701 ns |     4.0927 ns |     4.3792 ns |     215.5464 ns |
