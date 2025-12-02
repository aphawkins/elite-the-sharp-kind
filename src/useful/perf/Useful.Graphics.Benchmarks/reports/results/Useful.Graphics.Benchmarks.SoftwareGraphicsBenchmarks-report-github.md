```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.7171)
Intel Core i5-14600K 3.50GHz, 1 CPU, 20 logical and 14 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3 [AttachedDebugger]
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3


```
| Method                   | Mean           | Error         | StdDev      | Median         |
|------------------------- |---------------:|--------------:|------------:|---------------:|
| Clear                    | 15,597.5695 ns |   307.2480 ns | 459.8743 ns | 15,501.2619 ns |
| DrawCircle               |  1,016.9959 ns |     3.0189 ns |   2.8239 ns |  1,016.8596 ns |
| DrawCircleFilled         | 32,495.3593 ns |   104.9782 ns |  98.1967 ns | 32,504.9805 ns |
| DrawImage                |    114.4347 ns |     0.2438 ns |   0.2280 ns |    114.4164 ns |
| DrawImageCentre          |    113.2688 ns |     0.3616 ns |   0.3382 ns |    113.2474 ns |
| DrawLine                 |    509.2106 ns |     2.1833 ns |   1.8232 ns |    509.5561 ns |
| DrawPixel                |      0.3820 ns |     0.0045 ns |   0.0042 ns |      0.3814 ns |
| DrawPolygon              |     46.6815 ns |     0.1032 ns |   0.0966 ns |     46.6867 ns |
| DrawPolygonFilled        |     47.9369 ns |     0.2766 ns |   0.2587 ns |     47.9574 ns |
| DrawRectangle            |      9.7927 ns |     0.0305 ns |   0.0285 ns |      9.7881 ns |
| DrawRectangleCentre      |     14.4054 ns |     0.0379 ns |   0.0316 ns |     14.4037 ns |
| DrawRectangleFilled      |     26.8073 ns |     0.2253 ns |   0.1997 ns |     26.8569 ns |
| DrawTextCentreWhitespace |      0.9616 ns |     0.0118 ns |   0.0110 ns |      0.9615 ns |
| DrawTextLeftWhitespace   |      0.0005 ns |     0.0010 ns |   0.0010 ns |      0.0000 ns |
| DrawTextRightWhitespace  |      0.0005 ns |     0.0010 ns |   0.0010 ns |      0.0000 ns |
| DrawTriangle             |      7.1304 ns |     0.0139 ns |   0.0130 ns |      7.1281 ns |
| DrawTriangleFilled       |     47.3249 ns |     0.1979 ns |   0.1851 ns |     47.3422 ns |
| Create                   | 72,571.3876 ns | 1,113.4182 ns | 987.0162 ns | 72,826.7639 ns |
| Scale                    |      0.0015 ns |     0.0024 ns |   0.0023 ns |      0.0004 ns |
| ScreenHeight             |      0.0000 ns |     0.0000 ns |   0.0000 ns |      0.0000 ns |
| ScreenUpdate             |      0.1909 ns |     0.0030 ns |   0.0028 ns |      0.1905 ns |
| ScreenWidth              |      0.0008 ns |     0.0016 ns |   0.0013 ns |      0.0002 ns |
| SetClipRegion            |      0.0036 ns |     0.0043 ns |   0.0040 ns |      0.0028 ns |
