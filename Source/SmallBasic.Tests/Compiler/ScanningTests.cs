// <copyright file="ScanningTests.cs" company="MIT License">
// Licensed under the MIT License. See LICENSE file in the project root for license information.
// </copyright>

namespace SmallBasic.Tests.Compiler;

using SmallBasic.Compiler;
using SmallBasic.Compiler.Diagnostics;
using Xunit;

public sealed class ScanningTests : IClassFixture<CultureFixture>
{
    [Fact]
    public void ItReportsUnterminatedStringLiterals()
    {
        new SmallBasicCompilation(@"
x = ""name").VerifyDiagnostics(
            // x = "name
            //     ^^^^^
            // This string is missing its right double quotes.
            new Diagnostic(DiagnosticCode.UnterminatedStringLiteral, ((1, 4), (1, 8))));
    }

    [Fact]
    public void ItReportsMultipleUnterminatedStringLiterals()
    {
        new SmallBasicCompilation(@"
x = ""name
y = ""another").VerifyDiagnostics(
            // x = "name
            //     ^^^^^
            // This string is missing its right double quotes.
            new Diagnostic(DiagnosticCode.UnterminatedStringLiteral, ((1, 4), (1, 8))),
            // y = "another
            //     ^^^^^^^^
            // This string is missing its right double quotes.
            new Diagnostic(DiagnosticCode.UnterminatedStringLiteral, ((2, 4), (2, 11))));
    }

    [Fact]
    public void ItReportsUnrecognizedCharactersOnStartOfLine()
    {
        new SmallBasicCompilation(@"
$").VerifyDiagnostics(
            // $
            // ^
            // I don't understand this character '$'.
            new Diagnostic(DiagnosticCode.UnrecognizedCharacter, ((1, 0), (1, 0)), "$"));
    }

    [Fact]
    public void ItReportsMultipleUnrecognizedCharacters()
    {
        new SmallBasicCompilation(@"
x = ____^
ok = ""value $ value""
not_ok = 6 $
' $ still ok").VerifyDiagnostics(
            // x = ____^
            //         ^
            // I don't understand this character '^'.
            new Diagnostic(DiagnosticCode.UnrecognizedCharacter, ((1, 8), (1, 8)), "^"),
            // not_ok = 6 $
            //            ^
            // I don't understand this character '$'.
            new Diagnostic(DiagnosticCode.UnrecognizedCharacter, ((3, 11), (3, 11)), "$"));
    }
}
