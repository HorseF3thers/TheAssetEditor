﻿using System.Windows;
using Shared.Core.PackFiles.Models;

namespace Shared.Core.ToolCreation
{
    public interface EditorInterfaces
    {
        string DisplayName { get; set; }
        
        void Close();
    }

    public interface ISaveableEditor
    {
        bool Save();
        bool HasUnsavedChanges { get; set; }
    }

    public interface IFileEditor
    {
        PackFile CurrentFile { get; }
        public void LoadFile(PackFile file);
    }

    public interface IEditorCreator
    {
        void CreateFromFile(PackFile file, EditorEnums? preferedEditor = null);
        void Create(EditorEnums editor, Action<EditorInterfaces>? onInitializeCallback = null);
        Window CreateWindow(PackFile packFile, EditorEnums? preferedEditor = null);
    }

    public delegate void EditorSavedDelegate(PackFile newFile);
}