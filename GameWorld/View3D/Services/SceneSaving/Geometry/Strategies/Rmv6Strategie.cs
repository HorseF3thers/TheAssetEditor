﻿using GameWorld.Core.SceneNodes;
using Shared.GameFormats.RigidModel;

namespace GameWorld.Core.Services.SceneSaving.Geometry.Strategies
{
    public class Rmw6Strategy : IGeometryStrategy
    {
        private readonly SceneSaverService _sceneSaverService;
        public string Name => "Rmv6";
        public string Description => "";
        public bool IsAvailable => true;

        public GeometryStrategy StrategyId => GeometryStrategy.Rmv6;

        public Rmw6Strategy(SceneSaverService sceneSaverService)
        {
            _sceneSaverService = sceneSaverService;
        }

        public void Generate(MainEditableNode mainNode, GeometrySaveSettings saveSettings)
        {
            _sceneSaverService.Save(saveSettings.OutputName, mainNode, RmvVersionEnum.RMV2_V6, saveSettings);
        }
    }
}
