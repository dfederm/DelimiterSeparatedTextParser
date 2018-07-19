// <copyright file="CsvReader.cs" company="David Federman">
// Copyright (c) David Federman. All rights reserved.
// </copyright>

namespace DelimiterSeparatedTextParser
{
    using System;

    public sealed class CsvReader : DsvReader
    {
        public CsvReader(string str)
            : base(str.AsMemory(), ValueDelimeter, RecordDelimeter)
        {
        }

        public CsvReader(string str, string recordDelimeter)
            : base(str.AsMemory(), ValueDelimeter, recordDelimeter)
        {
        }

        public CsvReader(ReadOnlyMemory<char> memory)
            : base(memory, ValueDelimeter, RecordDelimeter)
        {
        }

        public CsvReader(ReadOnlyMemory<char> memory, string recordDelimeter)
            : base(memory, ValueDelimeter, recordDelimeter)
        {
        }

        public static string ValueDelimeter => ",";

        public static string RecordDelimeter => Environment.NewLine;
    }
}
