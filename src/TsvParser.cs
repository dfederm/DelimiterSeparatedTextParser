// <copyright file="TsvParser.cs" company="David Federman">
// Copyright (c) David Federman. All rights reserved.
// </copyright>

namespace DelimiterSeparatedTextParser
{
    using System;

    public sealed class TsvParser : DsvParser
    {
        public TsvParser(ReadOnlyMemory<char> memory)
            : base(memory, "\t".AsSpan(), Environment.NewLine.AsSpan())
        {
        }
    }
}
