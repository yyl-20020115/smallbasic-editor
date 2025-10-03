﻿// <copyright file="EditPage.cs" company="MIT License">
// Licensed under the MIT License. See LICENSE file in the project root for license information.
// </copyright>

namespace SmallBasic.Editor.Components.Pages.Edit
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Blazor.Components;
    using Microsoft.AspNetCore.Blazor.Services;
    using SmallBasic.Editor.Components.Layout;
    using SmallBasic.Editor.Components.Toolbox;
    using SmallBasic.Editor.Interop;
    using SmallBasic.Editor.Store;
    using SmallBasic.Utilities.Resources;

    public sealed class EditPage : MainLayout
    {
        public static void Inject(TreeComposer composer)
        {
            composer.Inject<EditPage>();
        }

        protected override void ComposeBody(TreeComposer composer)
        {
            composer.Element("edit-page", body: () =>
            {
                LibraryExplorer.Inject(composer);

                composer.Element("main-space", body: () =>
                {
                    composer.Element("editor-space", body: () =>
                    {
                        MonacoEditor.Inject(composer, isReadOnly: false);
                    });

                    ErrorsSpace.Inject(composer);
                });
            });
        }

        protected override void ComposeLeftActions(TreeComposer composer)
        {
            Actions.Action(composer, "new", EditorResources.Actions_New, onClick: () => JSInteropUtil.Monaco.ClearEditor(EditorResources.Actions_ClearEverythingConfirmation));
            Actions.Separator(composer);
            Actions.Action(composer, "save", EditorResources.Actions_Save, onClick: JSInteropUtil.Monaco.SaveToFile);
            Actions.Action(composer, "open", EditorResources.Actions_Open, onClick: () => JSInteropUtil.Monaco.OpenFile(EditorResources.Actions_ClearEverythingConfirmation));
            Actions.DisabledAction(composer, "import", EditorResources.Actions_Import, message: EditorResources.Actions_DisabledMessage_ComingSoon);
            Actions.DisabledAction(composer, "publish", EditorResources.Actions_Publish, message: EditorResources.Actions_DisabledMessage_ComingSoon);
            Actions.Separator(composer);
            Actions.Action(composer, "cut", EditorResources.Actions_Cut, onClick: JSInteropUtil.Monaco.Cut);
            Actions.Action(composer, "copy", EditorResources.Actions_Copy, onClick: JSInteropUtil.Monaco.Copy);
            Actions.Action(composer, "paste", EditorResources.Actions_Paste, onClick: JSInteropUtil.Monaco.Paste);
            Actions.Separator(composer);
            Actions.Action(composer, "undo", EditorResources.Actions_Undo, onClick: JSInteropUtil.Monaco.Undo);
            Actions.Action(composer, "redo", EditorResources.Actions_Redo, onClick: JSInteropUtil.Monaco.Redo);
        }

        protected override void ComposeRightActions(TreeComposer composer)
        {
            EditPageExeuctionActions.Inject(composer);
        }
    }

    public sealed class EditPageExeuctionActions : SmallBasicComponent, IDisposable
    {
        public void Dispose()
        {
            CompilationStore.CodeChanged -= this.StateHasChanged;
        }

        internal static void Inject(TreeComposer composer)
        {
            composer.Inject<EditPageExeuctionActions>();
        }

        protected override void OnInit()
        {
            CompilationStore.CodeChanged += this.StateHasChanged;
        }

        protected override void ComposeTree(TreeComposer composer)
        {
            if (CompilationStore.Compilation.Diagnostics.Any())
            {
                string message = string.Format(CultureInfo.CurrentCulture, EditorResources.Errors_Count, CompilationStore.Compilation.Diagnostics.Count);
                Actions.DisabledAction(composer, "debug", EditorResources.Actions_Debug, message: message);
                Actions.DisabledAction(composer, "run", EditorResources.Actions_Run, message: message);
            }
            else
            {
                Actions.Action(composer, "debug", EditorResources.Actions_Debug, onClick: () =>
                {
                    NavigationStore.NagivateTo(NavigationStore.PageId.Debug);
                    return Task.CompletedTask;
                });

                Actions.Action(composer, "run", EditorResources.Actions_Run, onClick: () =>
                {
                    NavigationStore.NagivateTo(NavigationStore.PageId.Run);
                    return Task.CompletedTask;
                });
            }
        }
    }
}
