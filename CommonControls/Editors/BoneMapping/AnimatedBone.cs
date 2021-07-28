﻿using Common;
using CommonControls.Common;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CommonControls.Editors.BoneMapping
{
    [DebuggerDisplay("AnimatedBone - {Name} -> {MappedBoneName}")]
    public class AnimatedBone : NotifyPropertyChangedImpl
    {
        public NotifyAttr<bool> IsVisible { get; set; } = new NotifyAttr<bool>(true);
        public NotifyAttr<string> Name { get; set; } = new NotifyAttr<string>("");
        public NotifyAttr<int> BoneIndex { get; set; } = new NotifyAttr<int>(-1);
        public ObservableCollection<AnimatedBone> Children { get; set; } = new ObservableCollection<AnimatedBone>();

        public NotifyAttr<bool> IsUsedByCurrentModel { get; set; } = new NotifyAttr<bool>(false);
        public NotifyAttr<string> MappedBoneName { get; set; } = new NotifyAttr<string>(null);
        public NotifyAttr<int> MappedBoneIndex { get; set; } = new NotifyAttr<int>(-1);

        // Meta data
        public Vector3 BonePosOffset { get; set; } = new Vector3(0);
        public Vector3 BoneRotOffset { get; set; } = new Vector3(0);
        public float BoneScaleOffset { get; set; } = 1;

        public AnimatedBone(int index, string name)
        {
            BoneIndex.Value = index;
            Name.Value = name;
        }

        public void ClearMapping()
        {
            MappedBoneName.Value = "";
            MappedBoneIndex.Value = -1;

            foreach (var child in Children)
                child.ClearMapping();
        }

        public AnimatedBone GetFromBoneId(int i)
        {
            if (BoneIndex.Value == i)
                return this;

            foreach (var child in Children)
            {
                var res = child.GetFromBoneId(i);
                if (res != null)
                    return res;
            }

            return null;
        }
    }
}