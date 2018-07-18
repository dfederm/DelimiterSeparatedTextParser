// <copyright file="DsvReaderTests.cs" company="David Federman">
// Copyright (c) David Federman. All rights reserved.
// </copyright>

namespace DelimiterSeparatedTextParser.Tests
{
    using System;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DsvReaderTests
    {
        [TestMethod]
        public void SquareTable()
        {
            const string ValueDelimeter = "|";
            const string RecordDelimeter = "\r\n";
            const int NumRecords = 3;
            const int NumValues = 4;

            var builder = CreateTable(ValueDelimeter, RecordDelimeter, NumRecords, NumValues);
            var reader = CreateReader(builder, ValueDelimeter, RecordDelimeter);

            Assert.IsTrue(reader.Current.IsEmpty);
            Assert.AreEqual(-1, reader.CurrentIndex);

            for (var recordNum = 0; recordNum < NumRecords; recordNum++)
            {
                Assert.IsTrue(reader.MoveNextRecord());

                Assert.IsTrue(reader.Current.IsEmpty);
                Assert.AreEqual(-1, reader.CurrentIndex);

                for (var valueNum = 0; valueNum < NumValues; valueNum++)
                {
                    Assert.IsTrue(reader.MoveNextValue());
                    var value = reader.Current;
                    Assert.AreEqual($"Value {recordNum}-{valueNum}", value.ToString());
                }

                Assert.IsFalse(reader.MoveNextValue());
                Assert.IsTrue(reader.Current.IsEmpty);
                Assert.AreEqual(-1, reader.CurrentIndex);
            }

            Assert.IsFalse(reader.MoveNextRecord());
            Assert.IsTrue(reader.Current.IsEmpty);
            Assert.AreEqual(-1, reader.CurrentIndex);
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

            var reader = CreateReader(builder, ValueDelimeter, RecordDelimeter);

            Assert.IsTrue(reader.Current.IsEmpty);
            Assert.AreEqual(-1, reader.CurrentIndex);

            for (var recordNum = 0; recordNum < NumRecords; recordNum++)
            {
                Assert.IsTrue(reader.MoveNextRecord());

                Assert.IsTrue(reader.Current.IsEmpty);
                Assert.AreEqual(-1, reader.CurrentIndex);

                var numValues = recordNum % 2 == 0 ? NumValuesEvenRow : NumValuesOddRow;
                for (var valueNum = 0; valueNum < numValues; valueNum++)
                {
                    Assert.IsTrue(reader.MoveNextValue());
                    var value = reader.Current;
                    Assert.AreEqual($"Value {recordNum}-{valueNum}", value.ToString());
                }

                Assert.IsFalse(reader.MoveNextValue());
                Assert.IsTrue(reader.Current.IsEmpty);
                Assert.AreEqual(-1, reader.CurrentIndex);
            }

            Assert.IsFalse(reader.MoveNextRecord());
            Assert.IsTrue(reader.Current.IsEmpty);
            Assert.AreEqual(-1, reader.CurrentIndex);
        }

        [TestMethod]
        public void LongDelimeters()
        {
            const string ValueDelimeter = "This is a very long and terrible value delimeter";
            const string RecordDelimeter = "This is a very long and terrible record delimeter";
            const int NumRecords = 3;
            const int NumValues = 4;

            var builder = CreateTable(ValueDelimeter, RecordDelimeter, NumRecords, NumValues);

            var reader = CreateReader(builder, ValueDelimeter, RecordDelimeter);

            Assert.IsTrue(reader.Current.IsEmpty);
            Assert.AreEqual(-1, reader.CurrentIndex);

            for (var recordNum = 0; recordNum < NumRecords; recordNum++)
            {
                Assert.IsTrue(reader.MoveNextRecord());

                Assert.IsTrue(reader.Current.IsEmpty);
                Assert.AreEqual(-1, reader.CurrentIndex);

                for (var valueNum = 0; valueNum < NumValues; valueNum++)
                {
                    Assert.IsTrue(reader.MoveNextValue());
                    var value = reader.Current;
                    Assert.AreEqual($"Value {recordNum}-{valueNum}", value.ToString());
                }

                Assert.IsFalse(reader.MoveNextValue());
                Assert.IsTrue(reader.Current.IsEmpty);
                Assert.AreEqual(-1, reader.CurrentIndex);
            }

            Assert.IsFalse(reader.MoveNextRecord());
            Assert.IsTrue(reader.Current.IsEmpty);
            Assert.AreEqual(-1, reader.CurrentIndex);
        }

        [TestMethod]
        public void LargeTable()
        {
            const string ValueDelimeter = "|";
            const string RecordDelimeter = "\r\n";
            const int NumRecords = 10_000;
            const int NumValues = 100;

            var builder = CreateTable(ValueDelimeter, RecordDelimeter, NumRecords, NumValues);

            var reader = CreateReader(builder, ValueDelimeter, RecordDelimeter);

            Assert.IsTrue(reader.Current.IsEmpty);
            Assert.AreEqual(-1, reader.CurrentIndex);

            for (var recordNum = 0; recordNum < NumRecords; recordNum++)
            {
                Assert.IsTrue(reader.MoveNextRecord());

                Assert.IsTrue(reader.Current.IsEmpty);
                Assert.AreEqual(-1, reader.CurrentIndex);

                for (var valueNum = 0; valueNum < NumValues; valueNum++)
                {
                    Assert.IsTrue(reader.MoveNextValue());
                    var value = reader.Current;
                    Assert.AreEqual($"Value {recordNum}-{valueNum}", value.ToString());
                }

                Assert.IsFalse(reader.MoveNextValue());
                Assert.IsTrue(reader.Current.IsEmpty);
                Assert.AreEqual(-1, reader.CurrentIndex);
            }

            Assert.IsFalse(reader.MoveNextRecord());
            Assert.IsTrue(reader.Current.IsEmpty);
            Assert.AreEqual(-1, reader.CurrentIndex);
        }

        private static DsvReader CreateReader(StringBuilder builder, string valueDelimeter, string recordDelimeter)
            => new DsvReader(
                builder.ToString().AsSpan(),
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
