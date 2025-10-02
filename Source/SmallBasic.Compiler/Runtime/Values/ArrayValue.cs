﻿// <copyright file="ArrayValue.cs" company="MIT License">
// Licensed under the MIT License. See LICENSE file in the project root for license information.
// </copyright>


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmallBasic.Compiler.Runtime;
public sealed class ArrayValue : BaseValue, IReadOnlyDictionary<string, BaseValue>
{
    private readonly Dictionary<string, BaseValue> contents;

    public ArrayValue()
    {
        this.contents = new Dictionary<string, BaseValue>(StringComparer.OrdinalIgnoreCase);
    }

    public ArrayValue(Dictionary<string, BaseValue> contents)
    {
        this.contents = contents;
    }

    public IEnumerable<string> Keys => this.contents.Keys;

    public IEnumerable<BaseValue> Values => this.contents.Values;

    public int Count => this.contents.Count;

    public BaseValue this[string key] => this.contents[key];

    public void SetIndex(string key, BaseValue value)
    {
        this.contents[key] = value;
    }

    public void RemoveIndex(string key)
    {
        if (this.contents.ContainsKey(key))
        {
            this.contents.Remove(key);
        }
    }

    public bool ContainsKey(string key) => this.contents.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, BaseValue>> GetEnumerator() => this.contents.GetEnumerator();

    public bool TryGetValue(string key, out BaseValue value) => this.contents.TryGetValue(key, out value);

    public override string ToDisplayString()
    {
        StringBuilder builder = new StringBuilder();

        void escape(string value)
        {
            foreach (char ch in value)
            {
                switch (ch)
                {
                    case ';':
                    case '=':
                    case '\\':
                        builder.Append("\\");
                        break;
                }

                builder.Append(ch);
            }
        }

        foreach (KeyValuePair<string, BaseValue> pair in this.contents)
        {
            builder.Append($"{pair.Key}=");
            escape(pair.Value.ToDisplayString());
            builder.Append(";");
        }

        return builder.ToString();
    }

    IEnumerator IEnumerable.GetEnumerator() => this.contents.GetEnumerator();

    internal override bool ToBoolean() => false;

    internal override decimal ToNumber() => 0;

    internal override ArrayValue ToArray() => this;
}
