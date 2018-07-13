// <copyright file="CsvParser.cs" company="David Federman">
// Copyright (c) David Federman. All rights reserved.
// </copyright>

namespace DelimiterSeparatedTextParser
{
    using System;

    public sealed class CsvParser : DsvParser
    {
        public CsvParser(ReadOnlyMemory<char> memory)
            : base(memory, ",".AsSpan(), Environment.NewLine.AsSpan())
        {
        }
    }
}
