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
        private DiagnosticResultLocation[] _locations;

        /// <summary>
        /// the result locations.
        /// </summary>
        public DiagnosticResultLocation[] Locations
        {
            get
            {
                return this._locations ?? (this._locations = new DiagnosticResultLocation[] { });
            }

            set
            {
                this._locations = value;
            }
        }

        /// <summary>
        /// The severity of the diagnostic.
        /// </summary>
        public DiagnosticSeverity Severity { get; set; }

        /// <summary>
        /// Diagnostic id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// the file path.
        /// </summary>
        public string Path
        {
            get
            {
                return this.Locations.Length > 0 ? this.Locations[0].Path : string.Empty;
            }
        }

        /// <summary>
        /// The error line.
        /// </summary>
        public int Line
        {
            get
            {
                return this.Locations.Length > 0 ? this.Locations[0].Line : -1;
            }
        }

        /// <summary>
        /// The error column
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