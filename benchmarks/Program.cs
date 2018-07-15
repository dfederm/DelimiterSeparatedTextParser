// <copyright file="Program.cs" company="David Federman">
// Copyright (c) David Federman. All rights reserved.
// </copyright>

namespace DelimiterSeparatedTextParser.Benchmarks
{
    using System;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    [MemoryDiagnoser]
    public class Program
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int ValueLength = 10;
        private const int NumValues = 10;

        private ReadOnlyMemory<char> memory;

        [Params(100, 1000, 10000)]
        public int NumRecords { get; set; }

        public static void Main() => BenchmarkRunner.Run<Program>();

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(27);

            var arrLength = (this.NumRecords * ((NumValues * ValueLength) + ((NumValues - 1) * CsvParser.ValueDelimeter.Length))) + ((this.NumRecords - 1) * CsvParser.RecordDelimeter.Length);
            var arr = new char[arrLength];
            var index = 0;

            for (var recordNum = 0; recordNum < this.NumRecords; recordNum++)
            {
                if (recordNum > 0)
                {
                    for (var i = 0; i < CsvParser.RecordDelimeter.Length; i++)
                    {
                        arr[index++] = CsvParser.RecordDelimeter[i];
                    }
                }

                for (var valueNum = 0; valueNum < NumValues; valueNum++)
                {
                    if (valueNum > 0)
                    {
                        for (var i = 0; i < CsvParser.ValueDelimeter.Length; i++)
                        {
                            arr[index++] = CsvParser.ValueDelimeter[i];
                        }
                    }

                    for (var i = 0; i < ValueLength; i++)
                    {
                        arr[index++] = Chars[random.Next(Chars.Length)];
                    }
                }
            }

            if (index != arrLength)
            {
                throw new InvalidOperationException("Did not set up correctly");
            }

            this.memory = new ReadOnlyMemory<char>(arr);
        }

        [Benchmark]
        public int Parse()
        {
            var totalLength = 0;
            var parser = new CsvParser(this.memory);

            var numRecords = parser.RecordsLength;
            for (var recordNum = 0; recordNum < numRecords; recordNum++)
            {
                var numValues = parser.GetRecordLength(recordNum);
                for (var valueNum = 0; valueNum < numValues; valueNum++)
                {
                    var value = parser.GetValue(recordNum, valueNum);
                    totalLength += value.Length;
                }
            }

            return totalLength;
        }
    }
}