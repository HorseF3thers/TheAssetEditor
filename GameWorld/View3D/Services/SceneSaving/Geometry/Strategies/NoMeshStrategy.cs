﻿using GameWorld.Core.SceneNodes;
using GameWorld.Core.Services.SceneSaving.Geometry;

namespace GameWorld.Core.Services.SceneSaving.Geometry.Strategies
{
    public class NoMeshStrategy : IGeometryStrategy
    {
        public GeometryStrategy StrategyId => GeometryStrategy.Rmv8;
        public string Name => "None";
        public string Description => "Dont generate a mesh";
        public bool IsAvailable => true;

        public NoMeshStrategy()
        {
        }

        public void Generate(MainEditableNode mainNode, GeometrySaveSettings saveSettings)
        {
        }
    }
}
