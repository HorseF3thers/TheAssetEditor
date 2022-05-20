﻿using CommonControls.Common;
using Microsoft.Xna.Framework;
using Serilog;
using System;
using View3D.SceneNodes;

namespace View3D.Animation.AnimationChange
{
    public class DockEquipmentRule : AnimationChangeRule
    {
        ILogger _logger = Logging.Create<CopyRootTransform>();
        bool _hasError = false;

        int _equipmentSlotToDock;
        AnimationClip _dockAnimation;
        ISkeletonProvider _skeletonProvider;
        float _startTime;
        float _endTime;
        int _dockTargetkBoneId;
        Matrix _offset;

        public DockEquipmentRule(int dockTargetkBoneId, int equipmentSlotToDock, AnimationClip dockAnimation, ISkeletonProvider skeletonProvider, float startTime, float endTime)
        {
            _dockTargetkBoneId = dockTargetkBoneId;           
            _dockAnimation = dockAnimation;
            _skeletonProvider = skeletonProvider;
            _startTime = startTime;
            _endTime = endTime;

            try
            {
                _equipmentSlotToDock = skeletonProvider.Skeleton.GetBoneIndexByName("be_prop_" + (equipmentSlotToDock - 1));
                var offsetFrame = AnimationSampler.Sample(0, _skeletonProvider.Skeleton, _dockAnimation);
                _offset = offsetFrame.GetSkeletonAnimatedWorldDiff(_skeletonProvider.Skeleton, _equipmentSlotToDock, _dockTargetkBoneId);
            }
            catch (Exception e)
            {
                _logger.Here().Error($"Error in {nameof(DockEquipmentRule)} - {e.Message}");
                _hasError = true;
            }
        }

        public override void ApplyWorldTransform(AnimationFrame frame, float time)
        {
            if (_hasError)
                return;

            try
            {
                if (time >= _startTime)
                {
                    var offsetFrame = AnimationSampler.Sample(0, _skeletonProvider.Skeleton, _dockAnimation);
                    _offset = offsetFrame.GetSkeletonAnimatedWorldDiff(_skeletonProvider.Skeleton, _dockTargetkBoneId, _equipmentSlotToDock);

                    var propTransform = _skeletonProvider.Skeleton.GetAnimatedWorldTranform(_dockTargetkBoneId);
                    frame.BoneTransforms[_equipmentSlotToDock].WorldTransform = _offset * propTransform;
                }
            }
            catch (Exception e)
            {
                _logger.Here().Error($"Error in {nameof(DockEquipmentRule)} - {e.Message}");
                _hasError = true;
            }
        }
    }
}
