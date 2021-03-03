﻿using System.Collections.Generic;
using View3D.Rendering;
using View3D.SceneNodes;

namespace View3D.Components.Component.Selection
{
    public class ObjectSelectionState : ISelectionState
    {
        public event SelectionStateChanged SelectionChanged;
        public GeometrySelectionMode Mode => GeometrySelectionMode.Object;

        List<ISelectable> _selectionList { get; set; } = new List<ISelectable>();

        public void ModifySelection(ISelectable newSelectionItem, bool onlyRemove)
        {
            if (_selectionList.Contains(newSelectionItem))
                _selectionList.Remove(newSelectionItem);
            else if (!onlyRemove)
                _selectionList.Add(newSelectionItem);

            SelectionChanged?.Invoke(this);
        }

        public List<ISelectable> CurrentSelection() 
        { 
            return _selectionList; 
        }

        public void Clear()
        {
            if (_selectionList.Count != 0)
            {
                _selectionList.Clear();
                SelectionChanged?.Invoke(this);
            }
        }

        public ISelectionState Clone()
        {
            return new ObjectSelectionState()
            {
                _selectionList = new List<ISelectable>(_selectionList)
            };
        }
        public int SelectionCount()
        {
            return _selectionList.Count;
        }

        public ISelectable GetSingleSelectedObject()
        {
            if (_selectionList.Count != 1)
                return null;
            return _selectionList[0];
        }

        public List<ISelectable> SelectedObjects()
        {
            return _selectionList;
        }
    }
}

