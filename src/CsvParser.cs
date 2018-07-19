// <copyright file="CsvParser.cs" company="David Federman">
// Copyright (c) David Federman. All rights reserved.
// </copyright>

namespace DelimiterSeparatedTextParser
{
    using System;

    public sealed class CsvParser : DsvParser
    {
        public CsvParser(string str)
            : base(new CsvReader(str))
        {
        }

        public CsvParser(string str, string recordDelimeter)
            : base(new CsvReader(str, recordDelimeter))
        {
        }

        public CsvParser(ReadOnlyMemory<char> memory)
            : base(new CsvReader(memory))
        {
        }

        public CsvParser(ReadOnlyMemory<char> memory, string recordDelimeter)
            : base(new CsvReader(memory, recordDelimeter))
        {
        }
    }
}
