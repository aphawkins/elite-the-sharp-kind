```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6899)
Intel Core i5-14600K 3.50GHz, 1 CPU, 20 logical and 14 physical cores
.NET SDK 10.0.100-rc.2.25502.107
  [Host]     : .NET 10.0.0 (10.0.0-rc.2.25502.107, 10.0.25.50307), X64 RyuJIT x86-64-v3 [AttachedDebugger]
  DefaultJob : .NET 10.0.0 (10.0.0-rc.2.25502.107, 10.0.25.50307), X64 RyuJIT x86-64-v3


```
| Method          | Mean       | Error     | StdDev    |
|---------------- |-----------:|----------:|----------:|
| SolidPlanet     | 138.534 μs | 1.2825 μs | 1.1369 μs |
| WireframePlanet |   1.155 μs | 0.0047 μs | 0.0044 μs |
| FractalPlanet   | 171.375 μs | 0.4663 μs | 0.4362 μs |
| StripedPlanet   | 170.588 μs | 0.2306 μs | 0.2044 μs |
