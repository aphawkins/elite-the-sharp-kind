```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.7171)
Intel Core i5-14600K 3.50GHz, 1 CPU, 20 logical and 14 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3 [AttachedDebugger]
  Job-CNUJVU : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3

InvocationCount=1  UnrollFactor=1  

```
| Method            | Mean        | Error      | StdDev     | Median        | Allocated |
|------------------ |------------:|-----------:|-----------:|--------------:|----------:|
| KeyDown           |   175.61 ns |  16.293 ns |  43.208 ns |   200.0000 ns |         - |
| KeyUp             |   722.22 ns |  44.525 ns | 130.584 ns |   700.0000 ns |      48 B |
| IsPressedKey      |   440.00 ns |  45.220 ns | 133.333 ns |   500.0000 ns |         - |
| IsPressedModifier |   532.32 ns |  45.649 ns | 133.881 ns |   500.0000 ns |      48 B |
| LastPressed       |   241.24 ns |  26.209 ns |  76.038 ns |   200.0000 ns |         - |
| ClearPressed      |    49.00 ns |  18.353 ns |  54.114 ns |     0.0000 ns |         - |
| PollNoEvents      |   100.00 ns |   0.000 ns |   0.000 ns |   100.0000 ns |         - |
| PollWithEvents    | 2,331.11 ns | 116.291 ns | 324.174 ns | 2,300.0000 ns |    1600 B |
