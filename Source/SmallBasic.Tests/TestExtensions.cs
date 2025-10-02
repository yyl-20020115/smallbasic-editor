﻿// <copyright file="TestExtensions.cs" company="MIT License">
// Licensed under the MIT License. See LICENSE file in the project root for license information.
// </copyright>

namespace SmallBasic.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using SmallBasic.Compiler;
using SmallBasic.Compiler.Binding;
using SmallBasic.Compiler.Diagnostics;
using SmallBasic.Compiler.Runtime;
using SmallBasic.Editor.Libraries;
using SmallBasic.Utilities;

internal static class TestExtensions
{
    public static SmallBasicCompilation VerifyDiagnostics(this SmallBasicCompilation compilation, params Diagnostic[] diagnostics)
    {
        string[] textLines = Regex.Split(compilation.Text, @"\r?\n");
        string expected = SerializeDiagnostics(textLines, diagnostics);
        string actual = SerializeDiagnostics(textLines, compilation.Diagnostics);

        expected.Should().Be(actual);
        return compilation;
    }

    public static async Task<SmallBasicEngine> VerifyExecutionState(this SmallBasicCompilation compilation, ExecutionState executionState, ExecutionMode mode = ExecutionMode.RunToEnd)
    {
        compilation.VerifyDiagnostics();

        SmallBasicEngine engine = new SmallBasicEngine(compilation, new LibrariesCollection());
        engine.Mode = mode;
        await engine.Execute().ConfigureAwait(false);
        engine.State.Should().Be(executionState);

        return engine;
    }

    public static Task<SmallBasicEngine> VerifyRealRuntime(this SmallBasicCompilation compilation, string memoryContents = default)
    {
        return VerifyRuntimeAux(compilation, new LibrariesCollection(), memoryContents);
    }

    public static async Task<SmallBasicEngine> VerifyLoggingRuntime(this SmallBasicCompilation compilation, string expectedLog = default, string memoryContents = default)
    {
        StringBuilder log = new StringBuilder();
        var engine = await VerifyRuntimeAux(compilation, new LoggingEngineLibraries(log), memoryContents).ConfigureAwait(false);

        if (!expectedLog.IsDefault())
        {
            (Environment.NewLine + log.ToString()).Should().Be(expectedLog);
        }

        return engine;
    }

    private static async Task<SmallBasicEngine> VerifyRuntimeAux(this SmallBasicCompilation compilation, IEngineLibraries libraries, string memoryContents)
    {
        compilation.VerifyDiagnostics();

        SmallBasicEngine engine = new SmallBasicEngine(compilation, libraries);

        while (engine.State != ExecutionState.Terminated)
        {
            engine.State.Should().Be(ExecutionState.Running, "loggers cannot move to another state");
            await engine.Execute().ConfigureAwait(false);
        }

        DebuggerSnapshot snapshot = engine.GetSnapshot();
        snapshot.ExecutionStack.Should().BeEmpty();

        if (!memoryContents.IsDefault())
        {
            string values = Environment.NewLine
                + snapshot.Memory.Select(pair => $"{pair.Key} = {pair.Value.ToDisplayString()}").Join(Environment.NewLine);

            values.Should().Be(memoryContents);
        }

        return engine;
    }

    private static string SerializeDiagnostics(string[] textLines, IEnumerable<Diagnostic> diagnostics)
    {
        return diagnostics.Select(diagnostic =>
        {
            diagnostic.Range.Start.Line.Should().Be(diagnostic.Range.End.Line, "multiline diagnostics are not supported yet");

            int line = diagnostic.Range.Start.Line;
            int start = diagnostic.Range.Start.Column;
            int end = diagnostic.Range.End.Column;

            List<string> constructorArgs = new List<string>()
            {
                $"DiagnosticCode.{diagnostic.Code}",
                diagnostic.Range.ToDisplayString()
            };

            constructorArgs.AddRange(diagnostic.Args.Select(arg => $@"""{arg}"""));

            return $@"
                // {(textLines.Length > line ? textLines[line] : string.Empty)}
                // {new string(' ', start)}{new string('^', end - start + 1)}
                // {diagnostic.ToDisplayString()}
                new Diagnostic({constructorArgs.Join(", ")})";
        }).Join(",") + Environment.NewLine;
    }
}
