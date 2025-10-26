```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6899)
Intel Core i5-14600K 3.50GHz, 1 CPU, 20 logical and 14 physical cores
.NET SDK 10.0.100-rc.2.25502.107
  [Host]     : .NET 10.0.0 (10.0.0-rc.2.25502.107, 10.0.25.50307), X64 RyuJIT x86-64-v3 [AttachedDebugger]
  Job-CNUJVU : .NET 10.0.0 (10.0.0-rc.2.25502.107, 10.0.25.50307), X64 RyuJIT x86-64-v3

InvocationCount=1  UnrollFactor=1  

```
| Method            | Mean        | Error      | StdDev    | Median        | Allocated |
|------------------ |------------:|-----------:|----------:|--------------:|----------:|
| KeyDown           |   175.64 ns |  16.737 ns |  43.20 ns |   200.0000 ns |         - |
| KeyUp             |   766.33 ns |  72.001 ns | 210.03 ns |   700.0000 ns |      48 B |
| IsPressedKey      |   362.11 ns |  25.510 ns |  73.19 ns |   300.0000 ns |         - |
| IsPressedModifier |   505.00 ns |  41.851 ns | 123.40 ns |   500.0000 ns |      48 B |
| LastPressed       |   240.00 ns |  27.093 ns |  77.73 ns |   200.0000 ns |         - |
| ClearPressed      |    30.00 ns |  15.620 ns |  46.06 ns |     0.0000 ns |         - |
| PollNoEvents      |    93.67 ns |   9.428 ns |  24.50 ns |   100.0000 ns |         - |
| PollWithEvents    | 2,158.95 ns | 104.373 ns | 299.47 ns | 2,000.0000 ns |    1600 B |
