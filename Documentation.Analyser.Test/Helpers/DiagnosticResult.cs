// <copyright file="DiagnosticResult.cs" company="Palantir (Pty) Ltd">
// Copyright (c) Palantir (Pty) Ltd. All rights reserved.
// </copyright>

namespace Documentation.Analyser.Test.Helpers
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Struct that stores information about a Diagnostic appearing in a source
    /// </summary>
    public struct DiagnosticResult
    {
        private DiagnosticResultLocation[] locations;

        /// <summary>
        /// Gets or sets the result locations.
        /// </summary>
        public DiagnosticResultLocation[] Locations
        {
            get
            {
                return this.locations ?? (this.locations = new DiagnosticResultLocation[] { });
            }

            set
            {
                this.locations = value;
            }
        }

        /// <summary>
        /// Gets or sets the severity of the diagnostic.
        /// </summary>
        public DiagnosticSeverity Severity { get; set; }

        /// <summary>
        /// Gets or sets Diagnostic id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        public string Path
        {
            get
            {
                return this.Locations.Length > 0 ? this.Locations[0].Path : string.Empty;
            }
        }

        /// <summary>
        /// Gets The error line.
        /// </summary>
        public int Line
        {
            get
            {
                return this.Locations.Length > 0 ? this.Locations[0].Line : -1;
            }
        }

        /// <summary>
        /// Gets The error column
        /// </summary>
        public int Column
        {
            get
            {
                return this.Locations.Length > 0 ? this.Locations[0].Column : -1;
            }
        }
    }
}