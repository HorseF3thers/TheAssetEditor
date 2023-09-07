﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using View3D.Animation;

namespace View3D.Commands.Bone
{
    public class PasteIntoSelectedBonesTransformBoneCommand : ICommand
    {
        public string HintText => "Copy/paste bone transform";

        public bool IsMutation => true;

        AnimationClip.KeyFrame _fromFrame;
        AnimationClip _animation;
        List<AnimationClip.KeyFrame> _backupFrames = new();
        int _startingFrame;
        List<int> _selectedBones;
        bool _pastePosition = true;
        bool _pasteRotation = true;
        bool _pasteScale = true;


        public void Configure(AnimationClip.KeyFrame copyFromFrame, AnimationClip animation, int startingFrame, List<int> selectedBones = null, 
            bool pastePosition = true, bool pasteRotation = true, bool pasteScale = true)
        {
            _fromFrame = copyFromFrame;
            _animation = animation;
            _startingFrame = startingFrame;
            _selectedBones = selectedBones;
            _pastePosition = pastePosition;
            _pasteRotation = pasteRotation;
            _pasteScale = pasteScale;

            foreach (var frame in _animation.DynamicFrames)
            {
                _backupFrames.Add(frame.Clone());
            }
        }

        public void Execute()
        {
            if (_selectedBones == null) return;

            foreach (var bone in _selectedBones)
            {
                var clone = _fromFrame.Clone();
                if (_pastePosition) _animation.DynamicFrames[_startingFrame].Position[bone] = clone.Position[bone];
                if (_pasteRotation) _animation.DynamicFrames[_startingFrame].Rotation[bone] = clone.Rotation[bone];
                if (_pasteScale) _animation.DynamicFrames[_startingFrame].Scale[bone] = clone.Scale[bone];
            }
        }
        public void Undo()
        {
            _animation.DynamicFrames = _backupFrames;
        }
    }
}
