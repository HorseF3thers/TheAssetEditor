﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Shared.Core.PackFiles;
using Shared.Core.PackFiles.Models;
using Shared.Ui.BaseDialogs.PackFileBrowser;

namespace CommonControls.PackFileBrowser
{
    public partial class PackFileBrowserWindow : Window, IDisposable
    {
        public PackFile SelectedFile { get; set; }
        public PackFileBrowserViewModel ViewModel { get; set; }

        public PackFileBrowserWindow(IPackFileService packfileService) => Create(packfileService);

        public PackFileBrowserWindow(IPackFileService packfileService, string[] extentions)
        {
            Create(packfileService);
            ViewModel.Filter.SetExtentions(extentions.ToList());
        }

        void Create(IPackFileService packfileService)
        {
            ViewModel = new PackFileBrowserViewModel(packfileService, null);
            ViewModel.ContextMenu = new OpenFileContextMenuHandler(packfileService);
            ViewModel.FileOpen += ViewModel_FileOpen;
            ViewModel.Filter.AutoExapandResultsAfterLimitedCount = 50;
            InitializeComponent();
            DataContext = this;

            PreviewKeyDown += HandleEsc;
        }

        public new bool ShowDialog() => (this as Window).ShowDialog() == true && SelectedFile != null;

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void ViewModel_FileOpen(PackFile file)
        {
            SelectedFile = file;
            if (DialogResult != true)
                DialogResult = true;
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectedFile = ViewModel.SelectedItem?.Item;
            DialogResult = true;
            Close();
        }

        public void Dispose()
        {
            PreviewKeyDown -= HandleEsc;
            ViewModel.FileOpen -= ViewModel_FileOpen;
            ViewModel.Dispose();
            ViewModel = null;
            DataContext = null;
        }
    }
}
