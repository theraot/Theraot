// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
#if NET35
    /// <summary>
    /// Stores information needed to emit debugging symbol information for a
    /// source file, in particular the file name and unique language identifier.
    /// </summary>
    public class SymbolDocumentInfo
    {
        private readonly string _fileName;

        internal SymbolDocumentInfo(string fileName)
        {
            ContractUtils.RequiresNotNull(fileName, "fileName");
            _fileName = fileName;
        }

        /// <summary>
        /// The source file name.
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
        }

        /// <summary>
        /// Returns the language's unique identifier, if any.
        /// </summary>
        public virtual Guid Language
        {
            get { return Guid.Empty; }
        }

        /// <summary>
        /// Returns the language vendor's unique identifier, if any.
        /// </summary>
        public virtual Guid LanguageVendor
        {
            get { return Guid.Empty; }
        }

        internal static readonly Guid DocumentType_Text = new Guid(0x5a869d0b, 0x6611, 0x11d3, 0xbd, 0x2a, 0, 0, 0xf8, 8, 0x49, 0xbd);

        /// <summary>
        /// Returns the document type's unique identifier, if any.
        /// Defaults to the guid for a text file.
        /// </summary>
        public virtual Guid DocumentType
        {
            get { return DocumentType_Text; }
        }
    }

    internal sealed class SymbolDocumentWithGuids : SymbolDocumentInfo
    {
        private readonly Guid _language;
        private readonly Guid _vendor;
        private readonly Guid _documentType;

        internal SymbolDocumentWithGuids(string fileName, ref Guid language)
            : base(fileName)
        {
            _language = language;
            _documentType = SymbolDocumentInfo.DocumentType_Text;
        }

        internal SymbolDocumentWithGuids(string fileName, ref Guid language, ref Guid vendor)
            : base(fileName)
        {
            _language = language;
            _vendor = vendor;
            _documentType = SymbolDocumentInfo.DocumentType_Text;
        }

        internal SymbolDocumentWithGuids(string fileName, ref Guid language, ref Guid vendor, ref Guid documentType)
            : base(fileName)
        {
            _language = language;
            _vendor = vendor;
            _documentType = documentType;
        }

        public override Guid Language
        {
            get { return _language; }
        }

        public override Guid LanguageVendor
        {
            get { return _vendor; }
        }

        public override Guid DocumentType
        {
            get { return _documentType; }
        }
    }

#endif
}
