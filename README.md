# DelimiterSeparatedTextParser
Performant delimeter-separated value (CSV, TSV, etc.) text parser.

# Benchmark Results
``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3903987 Hz, Resolution=256.1484 ns, Timer=TSC
.NET Core SDK=2.1.301
  [Host]     : .NET Core 2.1.1 (CoreCLR 4.6.26606.02, CoreFX 4.6.26606.05), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.1 (CoreCLR 4.6.26606.02, CoreFX 4.6.26606.05), 64bit RyuJIT


```
|                       Method | DataSize |           Mean |         Error |        StdDev | Scaled | ScaledSD |      Gen 0 |     Gen 1 |     Gen 2 |   Allocated |
|----------------------------- |--------- |---------------:|--------------:|--------------:|-------:|---------:|-----------:|----------:|----------:|------------:|
| **DelimiterSeparatedTextReader** |    **Large** |  **24,929.372 us** |   **341.7452 us** |   **319.6686 us** |   **1.00** |     **0.00** |          **-** |         **-** |         **-** |         **0 B** |
|                FastCsvParser |    Large |  58,659.486 us |   507.6749 us |   474.8794 us |   2.35 |     0.03 | 11437.5000 |  187.5000 |         - |  48360656 B |
|           CsvTextFieldParser |    Large | 105,256.008 us |   990.7970 us |   926.7921 us |   4.22 |     0.06 | 44437.5000 |         - |         - | 186400144 B |
|                    CsvHelper |    Large | 196,541.906 us | 1,421.1963 us | 1,259.8534 us |   7.89 |     0.11 | 42312.5000 |         - |         - | 177611728 B |
|                              |          |                |               |               |        |          |            |           |           |             |
| **DelimiterSeparatedTextParser** |    **Large** |  **59,266.792 us** | **1,169.8543 us** | **1,251.7309 us** |   **1.00** |     **0.00** |  **2750.0000** | **1312.5000** |  **437.5000** |  **16499337 B** |
|                  StringSplit |    Large |  89,254.220 us |   336.0341 us |   280.6036 us |   1.51 |     0.03 | 19687.5000 | 4687.5000 | 1625.0000 |  88000057 B |
|                TinyCsvParser |    Large | 160,947.603 us | 2,219.3522 us | 2,075.9832 us |   2.72 |     0.07 | 64875.0000 | 4687.5000 | 1000.0000 |  25674926 B |
|                  FileHelpers |    Large | 265,223.622 us |   933.2992 us |   779.3470 us |   4.48 |     0.09 | 15187.5000 | 6500.0000 | 2062.5000 |  85322864 B |
|                              |          |                |               |               |        |          |            |           |           |             |
| **DelimiterSeparatedTextReader** |   **Medium** |     **244.256 us** |     **3.5335 us** |     **3.3053 us** |   **1.00** |     **0.00** |          **-** |         **-** |         **-** |        **96 B** |
|                FastCsvParser |   Medium |     686.830 us |     9.0364 us |     8.4526 us |   2.81 |     0.05 |   166.0156 |   83.0078 |   83.0078 |    815328 B |
|           CsvTextFieldParser |   Medium |   1,050.241 us |    16.9236 us |    15.8303 us |   4.30 |     0.08 |   443.3594 |         - |         - |   1864144 B |
|                    CsvHelper |   Medium |   1,957.324 us |    28.4206 us |    26.5846 us |   8.01 |     0.15 |   425.7813 |         - |         - |   1789016 B |
|                              |          |                |               |               |        |          |            |           |           |             |
| **DelimiterSeparatedTextParser** |   **Medium** |     **375.222 us** |     **3.1718 us** |     **2.6486 us** |   **1.00** |     **0.00** |    **38.0859** |    **1.4648** |         **-** |    **160936 B** |
|                  StringSplit |   Medium |     394.238 us |     4.1344 us |     3.6650 us |   1.05 |     0.01 |   190.4297 |   47.3633 |         - |    879984 B |
|                TinyCsvParser |   Medium |   2,311.955 us |     9.8217 us |     9.1872 us |   6.16 |     0.05 |   500.0000 |  246.0938 |   11.7188 |    330044 B |
|                  FileHelpers |   Medium |   2,014.732 us |    38.6427 us |    47.4567 us |   5.37 |     0.13 |   164.0625 |   82.0313 |         - |    871837 B |
|                              |          |                |               |               |        |          |            |           |           |             |
| **DelimiterSeparatedTextReader** |    **Small** |       **2.494 us** |     **0.0361 us** |     **0.0338 us** |   **1.00** |     **0.00** |     **0.0191** |         **-** |         **-** |        **96 B** |
|                FastCsvParser |    Small |      22.896 us |     0.3485 us |     0.3260 us |   9.18 |     0.17 |    41.6565 |   41.6565 |   41.6565 |    208784 B |
|           CsvTextFieldParser |    Small |      10.506 us |     0.1106 us |     0.0981 us |   4.21 |     0.07 |     4.4708 |         - |         - |     18784 B |
|                    CsvHelper |    Small |      21.624 us |     0.2660 us |     0.2488 us |   8.67 |     0.15 |     6.9885 |         - |         - |     29392 B |
|                              |          |                |               |               |        |          |            |           |           |             |
| **DelimiterSeparatedTextParser** |    **Small** |       **3.985 us** |     **0.0724 us** |     **0.0677 us** |   **1.00** |     **0.00** |     **0.4959** |         **-** |         **-** |      **2104 B** |
|                  StringSplit |    Small |       3.825 us |     0.0751 us |     0.1003 us |   0.96 |     0.03 |     2.0905 |         - |         - |      8784 B |
|                TinyCsvParser |    Small |   1,050.156 us |    12.7396 us |    11.2933 us | 263.60 |     5.07 |    25.3906 |   11.7188 |         - |     76532 B |
|                  FileHelpers |    Small |     680.903 us |    12.8899 us |    13.2370 us | 170.91 |     4.25 |     6.8359 |    2.9297 |         - |     31812 B |
