﻿// <copyright file="HoverProviderTests.cs" company="MIT License">
// Licensed under the MIT License. See LICENSE file in the project root for license information.
// </copyright>

using System;
using System.Linq;
using FluentAssertions;
using SmallBasic.Compiler;
using SmallBasic.Compiler.Diagnostics;
using SmallBasic.Utilities.Resources;
using Xunit;

namespace SmallBasic.Tests.Services;
public sealed class HoverProviderTests : IClassFixture<CultureFixture>
{
    [Fact]
    public void NoHoverOnEmptyString()
    {
        TestForHover("$");
    }

    [Fact]
    public void NoHoverOnVariables()
    {
        TestForHover("some$thing = 6");
    }

    [Fact]
    public void HoverOnLibraryNames()
    {
        TestForHover("Text$Window.WriteLine(5)", "TextWindow", LibrariesResources.TextWindow);
    }

    [Fact]
    public void HoverOnMethodName()
    {
        TestForHover("TextWindow.Write$Line(1)", "WriteLine", LibrariesResources.TextWindow_WriteLine);
    }

    [Fact]
    public void HoverOnPropertyName()
    {
        TestForHover("x = Clock.Ti$me", "Time", LibrariesResources.Clock_Time);
    }

    [Fact]
    public void HoverOnEventName()
    {
        TestForHover(@"
Sub b
EndSub
Controls.ButtonCl$icked = b",
            "ButtonClicked",
            LibrariesResources.Controls_ButtonClicked);
    }

    [Fact]
    public void HoverOnError()
    {
        TestForHover(
            "TextWindow.No$Method()",
            new Diagnostic(DiagnosticCode.LibraryMemberNotFound, ((1, 1), (2, 2)), "TextWindow", "NoMethod").ToDisplayString());
    }

    private static void TestForHover(string text, params string[] expectedHover)
    {
        var markerCompilation = new SmallBasicCompilation(text);
        var marker = markerCompilation.Diagnostics.Single(d => d.Code == DiagnosticCode.UnrecognizedCharacter && d.Args.Single() == "$");

        var start = marker.Range.Start;
        var end = marker.Range.End;
        start.Line.Should().Be(end.Line);
        start.Column.Should().Be(end.Column);

        var compilation = new SmallBasicCompilation(text.Replace("$", string.Empty, StringComparison.CurrentCulture));
        var hover = compilation.ProvideHover((start.Line, start.Column));
        hover.Should().Equal(expectedHover);
    }
}
