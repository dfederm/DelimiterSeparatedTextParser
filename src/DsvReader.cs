// <copyright file="DsvReader.cs" company="David Federman">
// Copyright (c) David Federman. All rights reserved.
// </copyright>

namespace DelimiterSeparatedTextParser
{
    using System;

    /// <summary>
    /// Delimiter-separated value reader.
    /// </summary>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public ref struct DsvReader
#pragma warning restore CA1815
    {
        private readonly ReadOnlySpan<char> valueDelimeter;

        private readonly ReadOnlySpan<char> recordDelimeter;

        // The remainder of the unconsumed data
        private ReadOnlySpan<char> span;

        // The index of the span in the global buffer
        private int index;

        // The remainder of the unconsumed data for this record
        private ReadOnlySpan<char> recordSpan;

        // The index of the span in the global buffer
        private int recordIndex;

        public DsvReader(
            ReadOnlySpan<char> span,
            ReadOnlySpan<char> valueDelimeter,
            ReadOnlySpan<char> recordDelimeter)
        {
            this.span = span;
            this.index = 0;

            this.recordSpan = ReadOnlySpan<char>.Empty;
            this.recordIndex = 0;

            this.valueDelimeter = valueDelimeter;
            this.recordDelimeter = recordDelimeter;

            this.Current = ReadOnlySpan<char>.Empty;
            this.CurrentIndex = -1;
        }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        public ReadOnlySpan<char> Current { get; private set; }

        /// <summary>
        /// Gets the global index of the current value.
        /// </summary>
        public int CurrentIndex { get; private set; }

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
            if (this.span.IsEmpty)
            {
                this.Current = ReadOnlySpan<char>.Empty;
                return false;
            }

            var recordDelimeterIndex = this.span.IndexOf(this.recordDelimeter);
            if (recordDelimeterIndex == -1)
            {
                // The rest of the span is the final value.
                this.recordSpan = this.span;
                this.recordIndex = this.index;

                this.span = ReadOnlySpan<char>.Empty;
                this.index = -1;
            }
            else
            {
                this.recordSpan = this.span.Slice(0, recordDelimeterIndex);
                this.recordIndex = this.index;

                var moveAmount = recordDelimeterIndex + this.recordDelimeter.Length;
                this.span = this.span.Slice(moveAmount);
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
            if (this.recordSpan.IsEmpty)
            {
                this.Current = ReadOnlySpan<char>.Empty;
                this.CurrentIndex = -1;
                return false;
            }

            var valueDelimeterIndex = this.recordSpan.IndexOf(this.valueDelimeter);
            if (valueDelimeterIndex == -1)
            {
                // The rest of the span is the final value.
                this.Current = this.recordSpan;
                this.CurrentIndex = this.recordIndex;

                this.recordSpan = ReadOnlySpan<char>.Empty;
                this.recordIndex = -1;
            }
            else
            {
                this.Current = this.recordSpan.Slice(0, valueDelimeterIndex);
                this.CurrentIndex = this.recordIndex;

                var moveAmount = valueDelimeterIndex + this.valueDelimeter.Length;
                this.recordSpan = this.recordSpan.Slice(moveAmount);
                this.recordIndex += moveAmount;
            }

            return true;
        }
    }
}
