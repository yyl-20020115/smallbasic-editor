﻿// <copyright file="VariablesAndSubModulesCollector.cs" company="MIT License">
// Licensed under the MIT License. See LICENSE file in the project root for license information.
// </copyright>

namespace SmallBasic.Compiler.Binding;

using System.Collections.Generic;

internal sealed class VariablesAndSubModulesCollector : BaseBoundNodeVisitor
{
    private readonly HashSet<string> names = [];

    public VariablesAndSubModulesCollector(Binder binder)
    {
        this.Visit(binder.MainModule);
        foreach (var subModule in binder.SubModules.Values)
        {
            this.Visit(subModule);
        }
    }

    public IReadOnlyCollection<string> Names => this.names;

    private protected override void VisitArrayAssignmentStatement(BoundArrayAssignmentStatement node)
    {
        this.names.Add(node.Array.Name);
        base.VisitArrayAssignmentStatement(node);
    }

    private protected override void VisitVariableAssignmentStatement(BoundVariableAssignmentStatement node)
    {
        this.names.Add(node.Variable.Name);
        base.VisitVariableAssignmentStatement(node);
    }

    private protected override void VisitSubModule(BoundSubModule node)
    {
        this.names.Add(node.Name);
        base.VisitSubModule(node);
    }
}
