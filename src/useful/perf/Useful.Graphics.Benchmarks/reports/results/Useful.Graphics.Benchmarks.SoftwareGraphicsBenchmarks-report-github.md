```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6899)
Intel Core i5-14600K 3.50GHz, 1 CPU, 20 logical and 14 physical cores
.NET SDK 10.0.100-rc.2.25502.107
  [Host]     : .NET 10.0.0 (10.0.0-rc.2.25502.107, 10.0.25.50307), X64 RyuJIT x86-64-v3 [AttachedDebugger]
  DefaultJob : .NET 10.0.0 (10.0.0-rc.2.25502.107, 10.0.25.50307), X64 RyuJIT x86-64-v3


```
| Method                   | Mean           | Error       | StdDev      | Median         |
|------------------------- |---------------:|------------:|------------:|---------------:|
| Clear                    | 16,155.5444 ns | 321.1457 ns | 570.8361 ns | 16,273.7106 ns |
| DrawCircle               |    990.5093 ns |   1.6226 ns |   1.5178 ns |    990.4737 ns |
| DrawCircleFilled         | 32,103.2625 ns | 174.8102 ns | 163.5176 ns | 32,053.2532 ns |
| DrawImage                |    197.8967 ns |   1.8652 ns |   1.7447 ns |    198.1472 ns |
| DrawImageCentre          |    182.1974 ns |   0.9721 ns |   0.9093 ns |    182.2170 ns |
| DrawLine                 |    400.7281 ns |   1.7382 ns |   1.5409 ns |    400.3074 ns |
| DrawPixel                |      0.3810 ns |   0.0023 ns |   0.0020 ns |      0.3812 ns |
| DrawPolygon              |     45.7267 ns |   0.0591 ns |   0.0524 ns |     45.7083 ns |
| DrawPolygonFilled        |     51.3833 ns |   0.2796 ns |   0.2478 ns |     51.3314 ns |
| DrawRectangle            |      9.7440 ns |   0.0230 ns |   0.0216 ns |      9.7438 ns |
| DrawRectangleCentre      |     14.2877 ns |   0.0240 ns |   0.0187 ns |     14.2890 ns |
| DrawRectangleFilled      |     30.4512 ns |   0.0577 ns |   0.0540 ns |     30.4279 ns |
| DrawTextCentreWhitespace |      0.9419 ns |   0.0465 ns |   0.0497 ns |      0.9239 ns |
| DrawTextLeftWhitespace   |      0.0073 ns |   0.0069 ns |   0.0065 ns |      0.0055 ns |
| DrawTextRightWhitespace  |      0.0000 ns |   0.0000 ns |   0.0000 ns |      0.0000 ns |
| DrawTriangle             |      7.4046 ns |   0.0181 ns |   0.0160 ns |      7.4028 ns |
| DrawTriangleFilled       |     50.7456 ns |   0.9701 ns |   0.8599 ns |     50.8703 ns |
| Initialize               |     34.8874 ns |   0.7119 ns |   0.6659 ns |     34.7256 ns |
| IsInitialized            |      0.0000 ns |   0.0000 ns |   0.0000 ns |      0.0000 ns |
| Scale                    |      0.0016 ns |   0.0015 ns |   0.0014 ns |      0.0016 ns |
| ScreenHeight             |      0.0075 ns |   0.0063 ns |   0.0059 ns |      0.0065 ns |
| ScreenUpdate             |      0.0017 ns |   0.0023 ns |   0.0022 ns |      0.0006 ns |
| ScreenWidth              |      0.0009 ns |   0.0012 ns |   0.0011 ns |      0.0005 ns |
| SetClipRegion            |      0.0000 ns |   0.0000 ns |   0.0000 ns |      0.0000 ns |
