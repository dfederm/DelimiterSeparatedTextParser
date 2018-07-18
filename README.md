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
| **DelimiterSeparatedTextReader** |    **Large** |  **16,349.969 us** |   **178.4392 us** |   **166.9121 us** |   **1.00** |     **0.00** |          **-** |         **-** |         **-** |         **0 B** |
|                FastCsvParser |    Large |  57,649.310 us |   504.1050 us |   446.8759 us |   3.53 |     0.04 | 11437.5000 |  187.5000 |         - |  48360656 B |
|           CsvTextFieldParser |    Large | 103,398.356 us |   812.2300 us |   759.7604 us |   6.32 |     0.08 | 44437.5000 |         - |         - | 186400144 B |
|                    CsvHelper |    Large | 194,618.607 us | 1,076.8708 us | 1,007.3055 us |  11.90 |     0.13 | 42312.5000 |         - |         - | 177611728 B |
|                              |          |                |               |               |        |          |            |           |           |             |
| **DelimiterSeparatedTextParser** |    **Large** |  **53,862.981 us** |   **840.9631 us** |   **745.4918 us** |   **1.00** |     **0.00** |  **2812.5000** | **1500.0000** |  **500.0000** |  **16498940 B** |
|                  StringSplit |    Large |  87,535.705 us |   701.1808 us |   655.8849 us |   1.63 |     0.02 | 19562.5000 | 4562.5000 | 1500.0000 |  88000046 B |
|                TinyCsvParser |    Large | 157,429.493 us | 2,083.2643 us | 1,948.6865 us |   2.92 |     0.05 | 64875.0000 | 4250.0000 | 1000.0000 |  25674400 B |
|                  FileHelpers |    Large | 259,745.145 us |   804.0339 us |   752.0937 us |   4.82 |     0.07 | 15187.5000 | 6500.0000 | 2062.5000 |  85322480 B |
|                              |          |                |               |               |        |          |            |           |           |             |
| **DelimiterSeparatedTextReader** |   **Medium** |     **161.729 us** |     **2.0281 us** |     **1.8971 us** |   **1.00** |     **0.00** |          **-** |         **-** |         **-** |         **0 B** |
|                FastCsvParser |   Medium |     640.133 us |     3.1721 us |     2.4766 us |   3.96 |     0.05 |   166.0156 |   83.0078 |   83.0078 |    815328 B |
|           CsvTextFieldParser |   Medium |   1,024.653 us |    17.4747 us |    16.3458 us |   6.34 |     0.12 |   443.3594 |         - |         - |   1864144 B |
|                    CsvHelper |   Medium |   1,953.924 us |    23.0131 us |    21.5265 us |  12.08 |     0.19 |   425.7813 |         - |         - |   1789016 B |
|                              |          |                |               |               |        |          |            |           |           |             |
| **DelimiterSeparatedTextParser** |   **Medium** |     **321.522 us** |     **4.6362 us** |     **4.3367 us** |   **1.00** |     **0.00** |    **38.0859** |    **1.4648** |         **-** |    **160840 B** |
|                  StringSplit |   Medium |     382.312 us |     2.5442 us |     2.2554 us |   1.19 |     0.02 |   190.4297 |   47.3633 |         - |    879984 B |
|                TinyCsvParser |   Medium |   2,249.190 us |    12.3942 us |    10.9871 us |   7.00 |     0.10 |   500.0000 |  242.1875 |   11.7188 |    330045 B |
|                  FileHelpers |   Medium |   1,901.365 us |    36.5730 us |    39.1327 us |   5.91 |     0.14 |   166.0156 |   82.0313 |         - |    871837 B |
|                              |          |                |               |               |        |          |            |           |           |             |
| **DelimiterSeparatedTextReader** |    **Small** |       **1.647 us** |     **0.0107 us** |     **0.0095 us** |   **1.00** |     **0.00** |          **-** |         **-** |         **-** |         **0 B** |
|                FastCsvParser |    Small |      22.443 us |     0.1352 us |     0.1198 us |  13.62 |     0.10 |    41.6565 |   41.6565 |   41.6565 |    208784 B |
|           CsvTextFieldParser |    Small |      10.202 us |     0.0972 us |     0.0909 us |   6.19 |     0.06 |     4.4708 |         - |         - |     18784 B |
|                    CsvHelper |    Small |      21.441 us |     0.1885 us |     0.1764 us |  13.02 |     0.13 |     6.9885 |         - |         - |     29392 B |
|                              |          |                |               |               |        |          |            |           |           |             |
| **DelimiterSeparatedTextParser** |    **Small** |       **3.385 us** |     **0.0255 us** |     **0.0238 us** |   **1.00** |     **0.00** |     **0.4768** |         **-** |         **-** |      **2008 B** |
|                  StringSplit |    Small |       3.703 us |     0.0244 us |     0.0228 us |   1.09 |     0.01 |     2.0905 |         - |         - |      8784 B |
|                TinyCsvParser |    Small |   1,018.881 us |     6.4023 us |     5.6755 us | 301.01 |     2.60 |    25.3906 |   12.6953 |    0.9766 |     76532 B |
|                  FileHelpers |    Small |     664.814 us |     7.7928 us |     6.5074 us | 196.41 |     2.28 |     6.8359 |    2.9297 |         - |     31812 B |
