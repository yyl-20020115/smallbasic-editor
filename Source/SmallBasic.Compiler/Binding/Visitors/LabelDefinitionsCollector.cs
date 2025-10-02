// <copyright file="LabelDefinitionsCollector.cs" company="MIT License">
// Licensed under the MIT License. See LICENSE file in the project root for license information.
// </copyright>

using System.Collections.Generic;
using SmallBasic.Compiler.Diagnostics;

namespace SmallBasic.Compiler.Binding;

internal sealed class LabelDefinitionsCollector : BaseBoundNodeVisitor
{
    private readonly DiagnosticBag diagnostics;
    private readonly HashSet<string> labels = [];

    public LabelDefinitionsCollector(DiagnosticBag diagnostics, BoundStatementBlock module)
    {
        this.diagnostics = diagnostics;
        this.Visit(module);
    }

    public IReadOnlyCollection<string> Labels => this.labels;

    private protected override void VisitLabelStatement(BoundLabelStatement node)
    {
        if (!this.labels.Add(node.Label))
        {
            this.diagnostics.ReportTwoLabelsWithTheSameName(node.Syntax.LabelToken.Range, node.Label);
        }
    }
}
