// <copyright file="DsvParserTests.cs" company="David Federman">
// Copyright (c) David Federman. All rights reserved.
// </copyright>

namespace DelimiterSeparatedTextParser.Tests
{
    using System;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DsvParserTests
    {
        [TestMethod]
        public void SquareTable()
        {
            const string ValueDelimeter = "|";
            const string RecordDelimeter = "\r\n";
            const int NumRecords = 3;
            const int NumValues = 4;

            var builder = CreateTable(ValueDelimeter, RecordDelimeter, NumRecords, NumValues);

            var parser = CreateParser(builder, ValueDelimeter, RecordDelimeter);

            var numRecords = parser.RecordsLength;
            Assert.AreEqual(NumRecords, numRecords);
            for (var recordNum = 0; recordNum < numRecords; recordNum++)
            {
                var numValues = parser.GetRecordLength(recordNum);
                Assert.AreEqual(NumValues, numValues);
                for (var valueNum = 0; valueNum < numValues; valueNum++)
                {
                    var value = parser.GetValue(recordNum, valueNum);
                    Assert.AreEqual($"Value {recordNum}-{valueNum}", value.ToString());
                }
            }
        }

        [TestMethod]
        public void JaggedTable()
        {
            const string ValueDelimeter = "|";
            const string RecordDelimeter = "\r\n";
            const int NumRecords = 4;
            const int NumValuesOddRow = 3;
            const int NumValuesEvenRow = 5;

            var builder = new StringBuilder(128);
            for (var recordNum = 0; recordNum < NumRecords; recordNum++)
            {
                if (recordNum > 0)
                {
                    builder.Append(RecordDelimeter);
                }

                var numValues = recordNum % 2 == 0 ? NumValuesEvenRow : NumValuesOddRow;
                AddRecord(builder, ValueDelimeter, recordNum, numValues);
            }

            var parser = CreateParser(builder, ValueDelimeter, RecordDelimeter);

            var numRecords = parser.RecordsLength;
            Assert.AreEqual(NumRecords, numRecords);
            for (var recordNum = 0; recordNum < numRecords; recordNum++)
            {
                var numValues = parser.GetRecordLength(recordNum);
                Assert.AreEqual(recordNum % 2 == 0 ? NumValuesEvenRow : NumValuesOddRow, numValues);
                for (var valueNum = 0; valueNum < numValues; valueNum++)
                {
                    var value = parser.GetValue(recordNum, valueNum);
                    Assert.AreEqual($"Value {recordNum}-{valueNum}", value.ToString());
                }
            }
        }

        [TestMethod]
        public void LongDelimeters()
        {
            const string ValueDelimeter = "This is a very long and terrible value delimeter";
            const string RecordDelimeter = "This is a very long and terrible record delimeter";
            const int NumRecords = 3;
            const int NumValues = 4;

            var builder = CreateTable(ValueDelimeter, RecordDelimeter, NumRecords, NumValues);

            var parser = CreateParser(builder, ValueDelimeter, RecordDelimeter);

            var numRecords = parser.RecordsLength;
            Assert.AreEqual(NumRecords, numRecords);
            for (var recordNum = 0; recordNum < numRecords; recordNum++)
            {
                var numValues = parser.GetRecordLength(recordNum);
                Assert.AreEqual(NumValues, numValues);
                for (var valueNum = 0; valueNum < numValues; valueNum++)
                {
                    var value = parser.GetValue(recordNum, valueNum);
                    Assert.AreEqual($"Value {recordNum}-{valueNum}", value.ToString());
                }
            }
        }

        [TestMethod]
        public void LargeTable()
        {
            const string ValueDelimeter = "|";
            const string RecordDelimeter = "\r\n";
            const int NumRecords = 10_000;
            const int NumValues = 100;

            var builder = CreateTable(ValueDelimeter, RecordDelimeter, NumRecords, NumValues);

            var parser = CreateParser(builder, ValueDelimeter, RecordDelimeter);

            var numRecords = parser.RecordsLength;
            Assert.AreEqual(NumRecords, numRecords);
            for (var recordNum = 0; recordNum < numRecords; recordNum++)
            {
                var numValues = parser.GetRecordLength(recordNum);
                Assert.AreEqual(NumValues, numValues);
                for (var valueNum = 0; valueNum < numValues; valueNum++)
                {
                    var value = parser.GetValue(recordNum, valueNum);
                    Assert.AreEqual($"Value {recordNum}-{valueNum}", value.ToString());
                }
            }
        }

        [TestMethod]
        public void CsvConvenienceSubclass()
        {
            const int NumRecords = 3;
            const int NumValues = 4;

            var builder = CreateTable(CsvParser.ValueDelimeter, CsvParser.RecordDelimeter, NumRecords, NumValues);

            var parser = new CsvParser(new ReadOnlyMemory<char>(builder.ToString().ToCharArray()));

            var numRecords = parser.RecordsLength;
            Assert.AreEqual(NumRecords, numRecords);
            for (var recordNum = 0; recordNum < numRecords; recordNum++)
            {
                var numValues = parser.GetRecordLength(recordNum);
                Assert.AreEqual(NumValues, numValues);
                for (var valueNum = 0; valueNum < numValues; valueNum++)
                {
                    var value = parser.GetValue(recordNum, valueNum);
                    Assert.AreEqual($"Value {recordNum}-{valueNum}", value.ToString());
                }
            }
        }

        [TestMethod]
        public void TsvConvenienceSubclass()
        {
            const int NumRecords = 3;
            const int NumValues = 4;

            var builder = CreateTable(TsvParser.ValueDelimeter, TsvParser.RecordDelimeter, NumRecords, NumValues);

            var parser = new TsvParser(new ReadOnlyMemory<char>(builder.ToString().ToCharArray()));

            var numRecords = parser.RecordsLength;
            Assert.AreEqual(NumRecords, numRecords);
            for (var recordNum = 0; recordNum < numRecords; recordNum++)
            {
                var numValues = parser.GetRecordLength(recordNum);
                Assert.AreEqual(NumValues, numValues);
                for (var valueNum = 0; valueNum < numValues; valueNum++)
                {
                    var value = parser.GetValue(recordNum, valueNum);
                    Assert.AreEqual($"Value {recordNum}-{valueNum}", value.ToString());
                }
            }
        }

        private static DsvParser CreateParser(StringBuilder builder, string valueDelimeter, string recordDelimeter)
            => new DsvParser(
                new ReadOnlyMemory<char>(builder.ToString().ToCharArray()),
                valueDelimeter.AsSpan(),
                recordDelimeter.AsSpan());

        private static StringBuilder CreateTable(
            string valueDelimeter,
            string recordDelimeter,
            int numRecords,
            int numValues)
        {
            var builder = new StringBuilder(128);
            for (var recordNum = 0; recordNum < numRecords; recordNum++)
            {
                if (recordNum > 0)
                {
                    builder.Append(recordDelimeter);
                }

                AddRecord(builder, valueDelimeter, recordNum, numValues);
            }

            return builder;
        }

        private static void AddRecord(
            StringBuilder builder,
            string valueDelimeter,
            int recordNum,
            int numValues)
        {
            for (var valueNum = 0; valueNum < numValues; valueNum++)
            {
                if (valueNum > 0)
                {
                    builder.Append(valueDelimeter);
                }

                builder.Append("Value ");
                builder.Append(recordNum);
                builder.Append("-");
                builder.Append(valueNum);
            }
        }
    }
}
