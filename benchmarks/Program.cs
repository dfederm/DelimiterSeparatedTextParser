// <copyright file="Program.cs" company="David Federman">
// Copyright (c) David Federman. All rights reserved.
// </copyright>

namespace DelimiterSeparatedTextParser.Benchmarks
{
    using System;
    using System.IO;
    using System.Text;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Running;
    using NotVisualBasic.FileIO;
    using TinyCsvParser;
    using CsvParser = DelimiterSeparatedTextParser.CsvParser;
    using FastCsvParser = global::CsvParser.CsvReader;

    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory, BenchmarkLogicalGroupRule.ByParams)]
    [MemoryDiagnoser]
    public class Program
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int ValueLength = 10;
        private const int NumValues = 10;

        private static readonly char[] RecordDelimeterChars = CsvParser.RecordDelimeter.ToCharArray();
        private static readonly string[] RecordDelimeterStrings = { CsvParser.RecordDelimeter };

        private static readonly char ValueDelimeterChar = CsvParser.ValueDelimeter[0];
        private static readonly char[] ValueDelimeterChars = CsvParser.ValueDelimeter.ToCharArray();

        private string str;
        private ReadOnlyMemory<char> memory;
        private byte[] bytes;

        [Params("Small", "Medium", "Large")]
        public string DataSize { get; set; }

        public static void Main() => BenchmarkRunner.Run<Program>();

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(27);

            int numRecords;
            switch (this.DataSize)
            {
                case "Small":
                {
                    numRecords = 10;
                    break;
                }

                case "Medium":
                {
                    numRecords = 1000;
                    break;
                }

                case "Large":
                {
                    numRecords = 100000;
                    break;
                }

                default:
                {
                    throw new InvalidOperationException($"Unrecognized DataSize: {this.DataSize}");
                }
            }

            var arrLength = (numRecords * ((NumValues * ValueLength) + ((NumValues - 1) * CsvParser.ValueDelimeter.Length))) + ((numRecords - 1) * CsvParser.RecordDelimeter.Length);
            var arr = new char[arrLength];
            var index = 0;

            for (var recordNum = 0; recordNum < numRecords; recordNum++)
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
            this.str = new string(arr);
            this.bytes = Encoding.UTF8.GetBytes(arr);
        }

        [BenchmarkCategory("Reader")]
        [Benchmark(Baseline = true)]
        public int DelimiterSeparatedTextReader()
        {
            var totalLength = 0;
            var reader = new DsvReader(this.memory.Span, CsvParser.ValueDelimeter.AsSpan(), CsvParser.RecordDelimeter.AsSpan());

            while (reader.MoveNextRecord())
            {
                while (reader.MoveNextValue())
                {
                    var value = reader.Current;
                    totalLength += value.Length;
                }
            }

            return totalLength;
        }

        [BenchmarkCategory("Parser")]
        [Benchmark(Baseline = true)]
        public int DelimiterSeparatedTextParser()
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

        [BenchmarkCategory("Parser")]
        [Benchmark]
        public int StringSplit()
        {
            var totalLength = 0;

            var records = this.str.Split(RecordDelimeterChars);
            var numRecords = records.Length;
            for (var recordNum = 0; recordNum < numRecords; recordNum++)
            {
                var values = records[recordNum].Split(ValueDelimeterChars);
                var numValues = values.Length;
                for (var valueNum = 0; valueNum < numValues; valueNum++)
                {
                    var value = values[valueNum];
                    totalLength += value.Length;
                }
            }

            return totalLength;
        }

        [BenchmarkCategory("Parser")]
        [Benchmark(Description = "TinyCsvParser")]
        public int TinyCsvParserBenchmark()
        {
            var totalLength = 0;

            var csvParserOptions = new CsvParserOptions(false, ValueDelimeterChar, 4, true);
            var csvReaderOptions = new CsvReaderOptions(RecordDelimeterStrings);
            var csvMapper = new TinyCsvRecordMapping();
            var csvParser = new CsvParser<Record>(csvParserOptions, csvMapper);

            var results = csvParser.ReadFromString(csvReaderOptions, this.str);
            foreach (var result in results)
            {
                var record = result.Result;
                totalLength += record.Value0.Length;
                totalLength += record.Value1.Length;
                totalLength += record.Value2.Length;
                totalLength += record.Value3.Length;
                totalLength += record.Value4.Length;
                totalLength += record.Value5.Length;
                totalLength += record.Value6.Length;
                totalLength += record.Value7.Length;
                totalLength += record.Value8.Length;
                totalLength += record.Value9.Length;
            }

            return totalLength;
        }

        [BenchmarkCategory("Reader")]
        [Benchmark(Description = "FastCsvParser")]
        public int FastCsvParser()
        {
            var totalLength = 0;

            using (var stream = new MemoryStream(this.bytes))
            using (var parser = new FastCsvParser(stream, Encoding.UTF8))
            {
                while (parser.MoveNext())
                {
                    var values = parser.Current;
                    var numValues = values.Count;
                    for (var valueNum = 0; valueNum < numValues; valueNum++)
                    {
                        var value = values[valueNum];
                        totalLength += value.Length;
                    }
                }
            }

            return totalLength;
        }

        [BenchmarkCategory("Reader")]
        [Benchmark(Description = "CsvTextFieldParser")]
        public int CsvTextFieldParser()
        {
            var totalLength = 0;

            using (var csvReader = new StringReader(this.str))
            using (var parser = new CsvTextFieldParser(csvReader))
            {
                while (!parser.EndOfData)
                {
                    var values = parser.ReadFields();
                    var numValues = values.Length;
                    for (var valueNum = 0; valueNum < numValues; valueNum++)
                    {
                        var value = values[valueNum];
                        totalLength += value.Length;
                    }
                }
            }

            return totalLength;
        }

        [BenchmarkCategory("Reader")]
        [Benchmark(Description = "CsvHelper")]
        public int CsvHelper()
        {
            var totalLength = 0;

            using (var reader = new StringReader(this.str))
            using (var csv = new CsvHelper.CsvReader(reader))
            {
                while (csv.Read())
                {
                    var numValues = csv.Context.ColumnCount;
                    for (var valueNum = 0; valueNum < numValues; valueNum++)
                    {
                        var value = csv[valueNum];
                        totalLength += value.Length;
                    }
                }
            }

            return totalLength;
        }

        [BenchmarkCategory("Parser")]
        [Benchmark(Description = "FileHelpers")]
        public int FileHelpersBenchmark()
        {
            var totalLength = 0;

            var engine = new FileHelpers.FileHelperEngine<Record>();

            var records = engine.ReadString(this.str);
            var numRecords = records.Length;
            for (var recordNum = 0; recordNum < numRecords; recordNum++)
            {
                var record = records[recordNum];
                totalLength += record.Value0.Length;
                totalLength += record.Value1.Length;
                totalLength += record.Value2.Length;
                totalLength += record.Value3.Length;
                totalLength += record.Value4.Length;
                totalLength += record.Value5.Length;
                totalLength += record.Value6.Length;
                totalLength += record.Value7.Length;
                totalLength += record.Value8.Length;
                totalLength += record.Value9.Length;
            }

            return totalLength;
        }

        private sealed class TinyCsvRecordMapping : TinyCsvParser.Mapping.CsvMapping<Record>
        {
            public TinyCsvRecordMapping()
            {
                this.MapProperty(0, x => x.Value0);
                this.MapProperty(1, x => x.Value1);
                this.MapProperty(2, x => x.Value2);
                this.MapProperty(3, x => x.Value3);
                this.MapProperty(4, x => x.Value4);
                this.MapProperty(5, x => x.Value5);
                this.MapProperty(6, x => x.Value6);
                this.MapProperty(7, x => x.Value7);
                this.MapProperty(8, x => x.Value8);
                this.MapProperty(9, x => x.Value9);
            }
        }

        [FileHelpers.DelimitedRecord(",")]
        private sealed class Record
        {
            public string Value0 { get; set; }

            public string Value1 { get; set; }

            public string Value2 { get; set; }

            public string Value3 { get; set; }

            public string Value4 { get; set; }

            public string Value5 { get; set; }

            public string Value6 { get; set; }

            public string Value7 { get; set; }

            public string Value8 { get; set; }

            public string Value9 { get; set; }
        }
    }
}