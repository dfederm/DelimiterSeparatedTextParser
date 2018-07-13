// <copyright file="DsvParser.cs" company="David Federman">
// Copyright (c) David Federman. All rights reserved.
// </copyright>

namespace DelimiterSeparatedTextParser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Delimiter-separated value parser.
    /// </summary>
    public class DsvParser
    {
        private readonly ReadOnlyMemory<char> memory;

        // Outer list is each record. Inner list is each entry. The final value is an encoded value representing the index and length the value exists at in the memory buffer.
        private readonly List<List<long>> indexData;

        public DsvParser(ReadOnlyMemory<char> memory, ReadOnlySpan<char> valueDelimeter, ReadOnlySpan<char> recordDelimeter)
        {
            this.memory = memory;
            this.indexData = ParseIndexData(memory.Span, valueDelimeter, recordDelimeter);
        }

        public int RecordsLength => this.indexData.Count;

        public int GetRecordLength(int record) => this.indexData[record].Count;

        public ReadOnlySpan<char> GetValue(int record, int value)
        {
            var (index, length) = Decode(this.indexData[record][value]);
            return this.memory.Span.Slice(index, length);
        }

        private static List<List<long>> ParseIndexData(ReadOnlySpan<char> span, ReadOnlySpan<char> valueDelimeter, ReadOnlySpan<char> recordDelimeter)
        {
            var indexData = new List<List<long>>();

            // Intially just use the default list capacity.
            // https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/Collections/Generic/List.cs#L24
            var capacity = 4;

            // Keep track of what we're consuming so we can directly work on the span.
            var currentIndex = 0;

            while (!span.IsEmpty)
            {
                var recordIndexData = new List<long>(capacity);

                ReadOnlySpan<char> record;
                var recordDelimeterIndex = span.IndexOf(recordDelimeter);
                if (recordDelimeterIndex == -1)
                {
                    // The rest of the memory is the final record.
                    record = span;
                    span = ReadOnlySpan<char>.Empty;
                }
                else
                {
                    record = span.Slice(0, recordDelimeterIndex);
                    span = span.Slice(recordDelimeterIndex + recordDelimeter.Length);
                }

                ParseRecord(record, currentIndex, recordIndexData, valueDelimeter);

                indexData.Add(recordIndexData);
                currentIndex += recordDelimeterIndex + recordDelimeter.Length;

                // Assume the next row will have the same number of records as this one.
                capacity = recordIndexData.Count;
            }

            return indexData;
        }

        private static void ParseRecord(ReadOnlySpan<char> record, int recordIndex, List<long> indexData, ReadOnlySpan<char> valueDelimeter)
        {
            var index = recordIndex;
            while (!record.IsEmpty)
            {
                ReadOnlySpan<char> value;
                var valueDelimeterIndex = record.IndexOf(valueDelimeter);
                if (valueDelimeterIndex == -1)
                {
                    // The rest of the record is the final value.
                    value = record;
                    record = ReadOnlySpan<char>.Empty;
                }
                else
                {
                    value = record.Slice(0, valueDelimeterIndex);
                    record = record.Slice(valueDelimeterIndex + valueDelimeter.Length);
                }

                indexData.Add(Encode(index, value.Length));
                index += valueDelimeterIndex + valueDelimeter.Length;
            }
        }

        // See: https://stackoverflow.com/questions/827252/c-sharp-making-one-int64-from-two-int32s
        private static long Encode(int left, int right) => (long)left << 32 | (uint)right;

        private static (int, int) Decode(long value) => ((int)(value >> 32), (int)(value & 0xffffffffL));
    }
}
