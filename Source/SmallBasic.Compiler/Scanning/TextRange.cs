﻿// <copyright file="TextRange.cs" company="MIT License">
// Licensed under the MIT License. See LICENSE file in the project root for license information.
// </copyright>

namespace SmallBasic.Compiler.Scanning;

using System;
using System.Diagnostics;
using SmallBasic.Compiler.Services;

[DebuggerDisplay("{ToDisplayString()}")]
public readonly struct TextRange : IEquatable<TextRange>
{
    internal TextRange(TextPosition start, TextPosition end)
    {
        this.Start = start;
        this.End = end;
    }

    public TextPosition Start { get; }

    public TextPosition End { get; }

    public static implicit operator TextRange(in (TextPosition Start, TextPosition End) tuple)
    {
        return new TextRange(tuple.Start, tuple.End);
    }

    public static bool operator ==(TextRange left, TextRange right) => left.Start == right.Start && left.End == right.End;

    public static bool operator !=(TextRange left, TextRange right) => !(left == right);

    public override bool Equals(object obj) => obj is TextRange other && this == other;

    public override int GetHashCode() => this.Start.GetHashCode() ^ this.End.GetHashCode();

    public bool Equals(TextRange other) => this == other;

    public string ToDisplayString() => $"({this.Start.ToDisplayString()}, {this.End.ToDisplayString()})";

    public bool Contains(in TextPosition position) => this.Start <= position && position <= this.End;

    public MonacoRange ToMonacoRange() => new MonacoRange
    {
        startLineNumber = this.Start.Line + 1,
        startColumn = this.Start.Column + 1,
        endLineNumber = this.End.Line + 1,
        endColumn = this.End.Column + 2
    };
}
