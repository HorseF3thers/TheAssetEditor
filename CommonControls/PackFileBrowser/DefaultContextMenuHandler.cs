﻿using CommonControls.Services;
using System.Collections.ObjectModel;

namespace CommonControls.PackFileBrowser
{
    public class DefaultContextMenuHandler : ContextMenuHandler
    {
        public DefaultContextMenuHandler(PackFileService service) : base(service)
        { }

        public override void Create(TreeNode node)
        {
            _selectedNode = node;
            if (node == null)
            {
                Items = new ObservableCollection<ContextMenuItem>();
                return;
            }

            var newContextMenu = new ObservableCollection<ContextMenuItem>();

            if (node.NodeType == NodeType.Root)
            {
                if (node.FileOwner.IsCaPackFile)
                {
                    Additem(ContextItems.Close, newContextMenu);
                }
                else
                {
                    if (_packFileService.GetEditablePack() != node.FileOwner)
                    {
                        Additem(ContextItems.SetAsEditabelPack, newContextMenu);
                        AddSeperator(newContextMenu);
                    }

                    var addFolder = Additem(ContextItems.Add, newContextMenu);
                    Additem(ContextItems.AddFiles, addFolder);
                    Additem(ContextItems.AddDirectory, addFolder);
                    AddSeperator(newContextMenu);

                    Additem(ContextItems.Expand, newContextMenu);
                    AddSeperator(newContextMenu);
                    Additem(ContextItems.Save, newContextMenu);
                    Additem(ContextItems.SaveAs, newContextMenu);
                    Additem(ContextItems.Close, newContextMenu);
                }
            }

            if (node.NodeType == NodeType.Directory)
            {
                if (_packFileService.GetEditablePack() != node.FileOwner)
                    Additem(ContextItems.CopyToEditablePack, newContextMenu);
                if (!node.FileOwner.IsCaPackFile)
                {
                    AddSeperator(newContextMenu);
                    Additem(ContextItems.Rename, newContextMenu);
                    Additem(ContextItems.Delete, newContextMenu);
                    AddSeperator(newContextMenu);

                }
                Additem(ContextItems.Expand, newContextMenu);
            }

            if (node.NodeType == NodeType.File)
            {
                if (_packFileService.GetEditablePack() != node.FileOwner)
                    Additem(ContextItems.CopyToEditablePack, newContextMenu);
                if (!node.FileOwner.IsCaPackFile)
                {
                    AddSeperator(newContextMenu);
                    Additem(ContextItems.Duplicate, newContextMenu);
                    Additem(ContextItems.Rename, newContextMenu);
                    Additem(ContextItems.Delete, newContextMenu);
                    AddSeperator(newContextMenu);

                }
                Additem(ContextItems.CopyFullPath, newContextMenu);
                AddSeperator(newContextMenu);

                var openFolder = Additem(ContextItems.Open, newContextMenu);
                Additem(ContextItems.OpenWithKitbasher, openFolder);
                Additem(ContextItems.OpenWithTextEditor, openFolder);
                Additem(ContextItems.OpenWithPreview, openFolder);
            }

            Items = newContextMenu;
        }
    }
}
