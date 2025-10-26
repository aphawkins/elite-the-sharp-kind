```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6899)
Intel Core i5-14600K 3.50GHz, 1 CPU, 20 logical and 14 physical cores
.NET SDK 10.0.100-rc.2.25502.107
  [Host]     : .NET 10.0.0 (10.0.0-rc.2.25502.107, 10.0.25.50307), X64 RyuJIT x86-64-v3 [AttachedDebugger]
  DefaultJob : .NET 10.0.0 (10.0.0-rc.2.25502.107, 10.0.25.50307), X64 RyuJIT x86-64-v3


```
| Method                   | Mean            | Error         | StdDev        | Median          |
|------------------------- |----------------:|--------------:|--------------:|----------------:|
| Clear                    |  14,172.5500 ns |   266.1770 ns |   235.9590 ns |  14,174.5338 ns |
| DrawCircle               |     997.5177 ns |     1.5640 ns |     1.4629 ns |     997.7718 ns |
| DrawCircleFilled         | 132,470.1709 ns | 1,269.6863 ns | 1,187.6653 ns | 132,213.2324 ns |
| DrawImage                |     196.6532 ns |     0.4958 ns |     0.4395 ns |     196.6532 ns |
| DrawImageCentre          |     181.8684 ns |     0.3856 ns |     0.3607 ns |     181.8056 ns |
| DrawLine                 |   2,097.6433 ns |    41.8217 ns |    87.2974 ns |   2,094.7521 ns |
| DrawPixel                |       0.3686 ns |     0.0020 ns |     0.0018 ns |       0.3687 ns |
| DrawPolygon              |     210.9829 ns |     1.4638 ns |     1.2976 ns |     210.8211 ns |
| DrawPolygonFilled        |      50.3071 ns |     0.2749 ns |     0.2295 ns |      50.2528 ns |
| DrawRectangle            |       9.7795 ns |     0.0363 ns |     0.0322 ns |       9.7814 ns |
| DrawRectangleCentre      |      14.3827 ns |     0.0370 ns |     0.0346 ns |      14.3894 ns |
| DrawRectangleFilled      |      30.6470 ns |     0.0479 ns |     0.0448 ns |      30.6389 ns |
| DrawTextCentreWhitespace |       0.9086 ns |     0.0098 ns |     0.0082 ns |       0.9082 ns |
| DrawTextLeftWhitespace   |       0.0000 ns |     0.0000 ns |     0.0000 ns |       0.0000 ns |
| DrawTextRightWhitespace  |       0.0035 ns |     0.0046 ns |     0.0043 ns |       0.0011 ns |
| DrawTriangle             |      56.8534 ns |     0.5400 ns |     0.5051 ns |      56.7804 ns |
| DrawTriangleFilled       |      49.4823 ns |     0.2430 ns |     0.2154 ns |      49.4663 ns |
| Initialize               |      33.9803 ns |     0.4536 ns |     0.4243 ns |      34.0195 ns |
| IsInitialized            |       0.0064 ns |     0.0028 ns |     0.0025 ns |       0.0064 ns |
| Scale                    |       0.0032 ns |     0.0035 ns |     0.0033 ns |       0.0014 ns |
| ScreenHeight             |       0.0017 ns |     0.0022 ns |     0.0019 ns |       0.0011 ns |
| ScreenUpdate             |       0.0001 ns |     0.0003 ns |     0.0003 ns |       0.0000 ns |
| ScreenWidth              |       0.0036 ns |     0.0026 ns |     0.0024 ns |       0.0032 ns |
| SetClipRegion            |       0.0039 ns |     0.0035 ns |     0.0033 ns |       0.0037 ns |
