﻿#if LESSTHAN_NET40 || NETSTANDARD1_0

// BASEDON: https://github.com/dotnet/corefx/blob/master/src/System.ComponentModel.Annotations/src/System/ComponentModel/DataAnnotations/LocalizableString.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    ///     A helper class for providing a localizable string property.
    ///     This class is currently compiled in both System.Web.dll and System.ComponentModel.DataAnnotations.dll.
    /// </summary>
    internal class LocalizableString
    {
        private readonly string _propertyName;
        private Func<string?>? _cachedResult;
        private string? _propertyValue;
        private Type? _resourceType;

        /// <summary>
        ///     Constructs a localizable string, specifying the property name associated
        ///     with this item.  The <paramref name="propertyName" /> value will be used
        ///     within any exceptions thrown as a result of localization failures.
        /// </summary>
        /// <param name="propertyName">
        ///     The name of the property being localized.  This name
        ///     will be used within exceptions thrown as a result of localization failures.
        /// </param>
        public LocalizableString(string propertyName)
        {
            _propertyName = propertyName;
        }

        /// <summary>
        ///     Gets or sets the resource type to be used for localization.
        /// </summary>
        public Type? ResourceType
        {
            get => _resourceType;
            set
            {
                if (_resourceType == value)
                {
                    return;
                }

                ClearCache();
                _resourceType = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value of this localizable string.  This value can be
        ///     either the literal, non-localized value, or it can be a resource name
        ///     found on the resource type supplied to <see cref="GetLocalizableValue" />.
        /// </summary>
        public string? Value
        {
            get => _propertyValue;
            set
            {
                if (string.Equals(_propertyValue, value, StringComparison.Ordinal))
                {
                    return;
                }

                ClearCache();
                _propertyValue = value;
            }
        }

        /// <summary>
        ///     Gets the potentially localized value.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ResourceType" /> has been specified and <see cref="Value" /> is not
        ///     null, then localization will occur and the localized value will be returned.
        ///     <para>
        ///         If <see cref="ResourceType" /> is null then <see cref="Value" /> will be returned
        ///         as a literal, non-localized string.
        ///     </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if localization fails.  This can occur if <see cref="ResourceType" /> has been
        ///     specified, <see cref="Value" /> is not null, but the resource could not be
        ///     accessed.  <see cref="ResourceType" /> must be a public class, and <see cref="Value" />
        ///     must be the name of a public static string property that contains a getter.
        /// </exception>
        /// <returns>
        ///     Returns the potentially localized value.
        /// </returns>
        public string? GetLocalizableValue()
        {
            // Return the cached result
            var cachedResult = _cachedResult ??= GetCachedResult();
            return cachedResult();
        }

        /// <summary>
        ///     Clears any cached values, forcing <see cref="GetLocalizableValue" /> to
        ///     perform evaluation.
        /// </summary>
        private void ClearCache()
        {
            _cachedResult = null;
        }

        private Func<string?> GetCachedResult()
        {
            // If the property value is null, then just cache that value
            // If the resource type is null, then property value is literal, so cache it
            if (_propertyValue == null || _resourceType == null)
            {
                return () => _propertyValue;
            }

            var resourceTypeInfo = _resourceType.GetTypeInfo();

            // Get the property from the resource type for this resource key
            var property = _resourceType.GetProperty(_propertyValue);

            // We need to detect bad configurations so that we can throw exceptions accordingly
            // Make sure we found the property and it's the correct type, and that the type itself is public
            // If the property is not configured properly, then throw a missing member exception
            if (!resourceTypeInfo.IsVisible || property == null || property.PropertyType != typeof(string) || !IsGetterPublicAndStatic())
            {
                return () => throw new InvalidOperationException($"Localization failed ({_propertyName}, {_resourceType.FullName}, _propertyValue)");
            }

            // We have a valid property, so cache the resource
            return () => (string)property.GetValue(null, index: null);

            bool IsGetterPublicAndStatic()
            {
                // Ensure the getter for the property is available as public static
                var getter = property!.GetGetMethod();
                return getter?.IsPublic == true && getter.IsStatic;
            }
        }
    }
}

#endif