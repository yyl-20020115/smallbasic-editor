// <copyright file="SmallBasicCompilation.cs" company="MIT License">
// Licensed under the MIT License. See LICENSE file in the project root for license information.
// </copyright>


using System;
using System.Collections.Generic;
using SmallBasic.Compiler.Binding;
using SmallBasic.Compiler.Diagnostics;
using SmallBasic.Compiler.Parsing;
using SmallBasic.Compiler.Scanning;
using SmallBasic.Compiler.Services;

namespace SmallBasic.Compiler;
public sealed class SmallBasicCompilation
{
    private readonly DiagnosticBag diagnostics;
    private readonly bool isRunningOnDesktop;

    private readonly Scanner scanner;
    private readonly Parser parser;
    private readonly Binder binder;

    private readonly Lazy<RuntimeAnalysis> lazyAnalysis;

    public SmallBasicCompilation(string text)
#if IsBuildingForDesktop
       : this(text, isRunningOnDesktop: true)
#else
       : this(text, isRunningOnDesktop: false)
#endif
    {
    }

    public SmallBasicCompilation(string text, bool isRunningOnDesktop)
    {
        this.diagnostics = new DiagnosticBag();
        this.isRunningOnDesktop = isRunningOnDesktop;

        this.Text = text;

        this.scanner = new Scanner(this.Text, this.diagnostics);
        this.parser = new Parser(this.scanner.Tokens, this.diagnostics);
        this.binder = new Binder(this.parser.SyntaxTree, this.diagnostics, isRunningOnDesktop);

        this.lazyAnalysis = new Lazy<RuntimeAnalysis>(() => new RuntimeAnalysis(this));
    }

    public string Text { get; private set; }

    public RuntimeAnalysis Analysis => this.lazyAnalysis.Value;

    public IReadOnlyList<Diagnostic> Diagnostics => this.diagnostics.Contents;

    internal BoundStatementBlock MainModule => this.binder.MainModule;

    internal IReadOnlyDictionary<string, BoundSubModule> SubModules => this.binder.SubModules;

    public MonacoCompletionItem[] ProvideCompletionItems(TextPosition position) => CompletionItemProvider.Provide(this.parser, this.binder, position);

    public string[] ProvideHover(TextPosition position) => HoverProvider.Provide(this.diagnostics, this.parser, position);
}
