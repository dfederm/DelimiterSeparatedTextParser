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
            var reader = new DsvReader(memory.Span, valueDelimeter, recordDelimeter);
            this.indexData = ParseIndexData(reader);
        }

        public int RecordsLength => this.indexData.Count;

        public int GetRecordLength(int record) => this.indexData[record].Count;

        public ReadOnlySpan<char> GetValue(int record, int value)
        {
            var (index, length) = Decode(this.indexData[record][value]);
            return this.memory.Span.Slice(index, length);
        }

        private static List<List<long>> ParseIndexData(DsvReader reader)
        {
            var indexData = new List<List<long>>();

            // Intially just use the default list capacity.
            // https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/Collections/Generic/List.cs#L24
            var capacity = 4;

            while (reader.MoveNextRecord())
            {
                var recordIndexData = new List<long>(capacity);
                while (reader.MoveNextValue())
                {
                    var value = reader.Current;
                    recordIndexData.Add(Encode(reader.CurrentIndex, value.Length));
                }

                indexData.Add(recordIndexData);

                // Assume the next row will have the same number of records as this one.
                capacity = recordIndexData.Count;
            }

            return indexData;
        }

        // See: https://stackoverflow.com/questions/827252/c-sharp-making-one-int64-from-two-int32s
        private static long Encode(int left, int right) => (long)left << 32 | (uint)right;

        private static (int, int) Decode(long value) => ((int)(value >> 32), (int)(value & 0xffffffffL));
    }
}
