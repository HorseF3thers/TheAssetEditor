﻿using System.Windows;
using System.Windows.Controls;

namespace Shared.Core.ToolCreation
{
    public interface IToolFactory
    {
        IEditorViewModel Create(string fullFileName, bool useDefaultTool = false);
        ViewModel Create<ViewModel>() where ViewModel : IEditorViewModel;
        Window CreateAsWindow(IEditorViewModel viewModel);
        void DestroyEditor(IEditorViewModel instance);
        Type GetViewTypeFromViewModel(Type viewModelType);
        void RegisterTool<ViewModel, View>(IPackFileToToolSelector toolSelector = null)
            where ViewModel : IEditorViewModel
            where View : Control;
    }
}
