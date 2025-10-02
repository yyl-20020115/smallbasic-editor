﻿// <copyright file="StringExtensions.cs" company="MIT License">
// Licensed under the MIT License. See LICENSE file in the project root for license information.
// </copyright>

namespace SmallBasic.Utilities;

using System;
using System.Collections.Generic;
using System.Globalization;

public static class StringExtensions
{
    public static string Join(this IEnumerable<string> enumerable, string separator = "") => string.Join(separator, enumerable);

    public static string ToLowerFirstChar(this string value) => char.ToLowerInvariant(value[0]) + value.Substring(1);

    public static string RemovePrefix(this string value, string prefix) 
        => !value.StartsWith(prefix, StringComparison.CurrentCulture)
            ? throw new ArgumentException($"Value '{value}' does not start with prefix '{prefix}'.")
            : value.Substring(prefix.Length);

    public static string RemoveSuffix(this string value, string suffix) 
        => !value.EndsWith(suffix, StringComparison.CurrentCulture)
            ? throw new ArgumentException($"Value '{value}' does not end with suffix '{suffix}'.")
            : value.Substring(0, value.Length - suffix.Length);

    public static string ToDisplayString(this string value) => value;

    public static string ToDisplayString(this char value) => value.ToString(CultureInfo.CurrentCulture);

    public static string ToDisplayString(this int value) => value.ToString(CultureInfo.CurrentCulture);
}
