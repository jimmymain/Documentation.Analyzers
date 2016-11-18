// <copyright file="DiagnosticResultLocation.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Helpers
{
    using System;

    /// <summary>
    /// Location where the diagnostic appears, as determined by path, line number, and column number.
    /// </summary>
    public struct DiagnosticResultLocation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticResultLocation"/> struct.
        /// the diagnostic (error) result location.
        /// </summary>
        /// <param name="path">the path.</param>
        /// <param name="line">the line.</param>
        /// <param name="column">the column.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public DiagnosticResultLocation(string path, int line, int column)
        {
            if (line < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(line), @"line must be >= -1");
            }

            if (column < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(line), @"column must be >= -1");
            }

            this.Path = path;
            this.Line = line;
            this.Column = column;
        }

        /// <summary>
        /// the path.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// the line.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// the column.
        /// </summary>
        public int Column { get; }
    }
}