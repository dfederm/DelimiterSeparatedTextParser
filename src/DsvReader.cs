// <copyright file="DsvReader.cs" company="David Federman">
// Copyright (c) David Federman. All rights reserved.
// </copyright>

namespace DelimiterSeparatedTextParser
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Delimiter-separated value reader.
    /// </summary>
    public class DsvReader
    {
        private readonly string valueDelimeter;

        private readonly string recordDelimeter;

        // The remainder of the unconsumed data. It does not unclude the current value.
        private ReadOnlyMemory<char> memory;

        // The index of the span in the global buffer.
        // This will always be the index of the next value, not the current value.
        private int index;

        // Whether the read is currently inside of a record.
        // If false, MoveNextValue should always return false until MoveNextRecord is called.
        private bool isInRecord;

        public DsvReader(string str, string valueDelimeter, string recordDelimeter)
            : this(str.AsMemory(), valueDelimeter, recordDelimeter)
        {
        }

        public DsvReader(ReadOnlyMemory<char> memory, string valueDelimeter, string recordDelimeter)
        {
            this.memory = memory;
            this.index = 0;

            this.valueDelimeter = valueDelimeter;
            this.recordDelimeter = recordDelimeter;

            this.Current = ReadOnlyMemory<char>.Empty;
            this.CurrentIndex = -1;
        }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        public ReadOnlyMemory<char> Current { get; private set; }

        /// <summary>
        /// Gets the global index of the current value.
        /// </summary>
        public int CurrentIndex { get; private set; }

        // Exposed interally for DsvParser to get at it
        internal ReadOnlyMemory<char> Memory => this.memory;

        /// <summary>
        /// Moves to the next record.
        /// </summary>
        /// <remarks>
        /// This method needs to be called before <see cref="Current"/> or <see cref="CurrentIndex"/> will have values.
        /// </remarks>
        /// <returns>True if there is a new record, false if the end of the data has been reached.</returns>
        public bool MoveNextRecord()
        {
            // Data is completely consumed
            if (this.memory.IsEmpty)
            {
                this.Current = ReadOnlyMemory<char>.Empty;
                this.CurrentIndex = -1;
                return false;
            }

            if (this.isInRecord)
            {
                while (this.MoveNextValue())
                {
                    // No-op. We're just reading the rest of the record.
                }
            }

            if (!this.isInRecord)
            {
                this.isInRecord = true;
                return true;
            }

            return true;
        }

        /// <summary>
        /// Moves to the next value.
        /// </summary>
        /// <remarks>
        /// This method needs to be called before <see cref="Current"/> or <see cref="CurrentIndex"/> will have values.
        /// </remarks>
        /// <returns>True if there is a new value, false if the end of the record data has been reached.</returns>
        public bool MoveNextValue()
        {
            // Current record is completely consumed
            if (!this.isInRecord)
            {
                this.Current = ReadOnlyMemory<char>.Empty;
                this.CurrentIndex = -1;
                return false;
            }

            var span = this.memory.Span;
            var valueDelimeterSpan = this.valueDelimeter.AsSpan();
            var recordDelimeterSpan = this.recordDelimeter.AsSpan();

            var length = span.Length;
            var i = 0;
            while (i < length)
            {
                if (IsAtDelimeter(span, valueDelimeterSpan, i, length))
                {
                    // Found value delimeter
                    this.Current = this.memory.Slice(0, i);
                    this.CurrentIndex = this.index;

                    var moveAmount = i + valueDelimeterSpan.Length;
                    this.memory = this.memory.Slice(moveAmount);
                    this.index += moveAmount;

                    this.isInRecord = true;
                    return true;
                }

                if (IsAtDelimeter(span, recordDelimeterSpan, i, length))
                {
                    // Found record delimeter
                    this.Current = this.memory.Slice(0, i);
                    this.CurrentIndex = this.index;

                    var moveAmount = i + recordDelimeterSpan.Length;
                    this.memory = this.memory.Slice(moveAmount);
                    this.index += moveAmount;

                    this.isInRecord = false;
                    return true;
                }

                i++;
            }

            // Hit the end of the data, the rest of the span is the final value.
            this.Current = this.memory;
            this.CurrentIndex = this.index;

            this.memory = ReadOnlyMemory<char>.Empty;
            this.index = -1;

            this.isInRecord = false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAtDelimeter(ReadOnlySpan<char> span, ReadOnlySpan<char> delimeter, int i, int length)
        {
            // Check the first character to short-circuit the condition
            if (span[i] != delimeter[0])
            {
                return false;
            }

            switch (delimeter.Length)
            {
                // If the delimeter is only 1 character, no need to check more
                case 1:
                {
                    return true;
                }

                // Special-case a delimeter length of 2 a well
                case 2:
                {
                    return i + 1 <= length
                           && span[i + 1] == delimeter[1];
                }

                // If more than 2, use SequenceEqual
                default:
                {
                    return i + delimeter.Length < length
                           && span.Slice(i, delimeter.Length).SequenceEqual(delimeter);
                }
            }
        }
    }
}
