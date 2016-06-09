// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Dynamic.Utils;
using Theraot.Core;

namespace System.Linq.Expressions
{
#if NET35

    /// <summary>
    /// Used to denote the target of a <see cref="GotoExpression"/>.
    /// </summary>
    public sealed class LabelTarget
    {
        private readonly Type _type;
        private readonly string _name;

        internal LabelTarget(Type type, string name)
        {
            _type = type;
            _name = name;
        }

        /// <summary>
        /// Gets the name of the label.
        /// </summary>
        /// <remarks>The label's name is provided for information purposes only.</remarks>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The type of value that is passed when jumping to the label
        /// (or System.Void if no value should be passed).
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public Type Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="Object"/>. 
        /// </summary>
        /// <returns>A <see cref="String"/> that represents the current <see cref="Object"/>. </returns>
        public override string ToString()
        {
            return String.IsNullOrEmpty(this.Name) ? "UnamedLabel" : this.Name;
        }
    }

#endif
}
