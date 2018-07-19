// <copyright file="TsvParser.cs" company="David Federman">
// Copyright (c) David Federman. All rights reserved.
// </copyright>

namespace DelimiterSeparatedTextParser
{
    using System;

    public sealed class TsvParser : DsvParser
    {
        public TsvParser(string str)
            : base(new TsvReader(str))
        {
        }

        public TsvParser(string str, string recordDelimeter)
            : base(new TsvReader(str, recordDelimeter))
        {
        }

        public TsvParser(ReadOnlyMemory<char> memory)
            : base(new TsvReader(memory))
        {
        }

        public TsvParser(ReadOnlyMemory<char> memory, string recordDelimeter)
            : base(new TsvReader(memory, recordDelimeter))
        {
        }
    }
}
