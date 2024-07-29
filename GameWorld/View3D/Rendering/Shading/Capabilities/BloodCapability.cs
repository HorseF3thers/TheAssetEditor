﻿using GameWorld.WpfWindow.ResourceHandling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.GameFormats.RigidModel;
using Shared.GameFormats.RigidModel.Types;
using Shared.GameFormats.WsModel;


namespace GameWorld.Core.Rendering.Shading.Capabilities
{
    public class BloodCapability : ICapability
    {
        public bool UseBlood { get; set; } = true;
        public Vector2 UvScale { get; set; } = new Vector2(1);
        public TextureInput BloodMask { get; set; } = new TextureInput(TextureType.Blood);
        public float PreviewBlood { get; set; } = 0;

        public void Apply(Effect effect, ResourceLibrary resourceLibrary)
        {
            //effect.Parameters["UseBlood"].SetValue(UseBlood);
            //
            //BloodMask.Apply(effect, resourceLibrary);
        }

        public void Initialize(WsModelMaterialFile? wsModelMaterial, RmvModel model)
        {
            CapabilityHelper.SetTextureFromModel(model, wsModelMaterial, BloodMask);
        }
    }
}
