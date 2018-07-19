// <copyright file="TsvReader.cs" company="David Federman">
// Copyright (c) David Federman. All rights reserved.
// </copyright>

namespace DelimiterSeparatedTextParser
{
    using System;

    public sealed class TsvReader : DsvReader
    {
        public TsvReader(string str)
            : base(str.AsMemory(), ValueDelimeter, RecordDelimeter)
        {
        }

        public TsvReader(string str, string recordDelimeter)
            : base(str.AsMemory(), ValueDelimeter, recordDelimeter)
        {
        }

        public TsvReader(ReadOnlyMemory<char> memory)
            : base(memory, ValueDelimeter, RecordDelimeter)
        {
        }

        public TsvReader(ReadOnlyMemory<char> memory, string recordDelimeter)
            : base(memory, ValueDelimeter, recordDelimeter)
        {
        }

        public static string ValueDelimeter => "\t";

        public static string RecordDelimeter => Environment.NewLine;
    }
}
