// <copyright file="DsvReader.cs" company="David Federman">
// Copyright (c) David Federman. All rights reserved.
// </copyright>

namespace DelimiterSeparatedTextParser
{
    using System;

    /// <summary>
    /// Delimiter-separated value reader.
    /// </summary>
    public class DsvReader
    {
        private readonly string valueDelimeter;

        private readonly string recordDelimeter;

        // The remainder of the unconsumed data
        private ReadOnlyMemory<char> memory;

        // The index of the span in the global buffer
        private int index;

        // The remainder of the unconsumed data for this record
        private ReadOnlyMemory<char> recordMemory;

        // The index of the span in the global buffer
        private int recordIndex;

        public DsvReader(string str, string valueDelimeter, string recordDelimeter)
            : this(str.AsMemory(), valueDelimeter, recordDelimeter)
        {
        }

        public DsvReader(ReadOnlyMemory<char> memory, string valueDelimeter, string recordDelimeter)
        {
            this.memory = memory;
            this.index = 0;

            this.recordMemory = ReadOnlyMemory<char>.Empty;
            this.recordIndex = 0;

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
                return false;
            }

            var recordDelimeterIndex = this.memory.Span.IndexOf(this.recordDelimeter.AsSpan());
            if (recordDelimeterIndex == -1)
            {
                // The rest of the span is the final value.
                this.recordMemory = this.memory;
                this.recordIndex = this.index;

                this.memory = ReadOnlyMemory<char>.Empty;
                this.index = -1;
            }
            else
            {
                this.recordMemory = this.memory.Slice(0, recordDelimeterIndex);
                this.recordIndex = this.index;

                var moveAmount = recordDelimeterIndex + this.recordDelimeter.Length;
                this.memory = this.memory.Slice(moveAmount);
                this.index += moveAmount;
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
            // Record is completely consumed
            if (this.recordMemory.IsEmpty)
            {
                this.Current = ReadOnlyMemory<char>.Empty;
                this.CurrentIndex = -1;
                return false;
            }

            var valueDelimeterIndex = this.recordMemory.Span.IndexOf(this.valueDelimeter.AsSpan());
            if (valueDelimeterIndex == -1)
            {
                // The rest of the span is the final value.
                this.Current = this.recordMemory;
                this.CurrentIndex = this.recordIndex;

                this.recordMemory = ReadOnlyMemory<char>.Empty;
                this.recordIndex = -1;
            }
            else
            {
                this.Current = this.recordMemory.Slice(0, valueDelimeterIndex);
                this.CurrentIndex = this.recordIndex;

                var moveAmount = valueDelimeterIndex + this.valueDelimeter.Length;
                this.recordMemory = this.recordMemory.Slice(moveAmount);
                this.recordIndex += moveAmount;
            }

            return true;
        }
    }
}
