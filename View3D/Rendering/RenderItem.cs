﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using View3D.Components.Component;
using View3D.Components.Gizmo;
using View3D.Rendering.Geometry;

namespace View3D.Rendering
{
    public class RenderItem : ITransformable
    {
        public BasicEffect DefaultEffect { get; set; }
        public BasicEffect WireframeEffect { get; set; }
        public BasicEffect SelectedFacesEffect { get; set; }

        public Matrix ModelMatrix { get; private set; } = Matrix.Identity;
        public IGeometry Geometry { get; set; }
        public string Name { get; set; } = "";


        public IGeometry _selectedVertexGeo;

        public RenderItem(IGeometry geo, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Geometry = geo;
            _position = position;
            _orientation = rotation;
            _scale = scale;
            UpdateMatrix();
        }

        Quaternion _orientation = Quaternion.Identity;
        Vector3 _position = Vector3.Zero;
        Vector3 _scale = Vector3.One;

        public Vector3 Position { get { return _position; } set { _position = value; UpdateMatrix();  } }
        public Vector3 Scale { get { return _scale; } set { _scale = value; UpdateMatrix();  } }
        public Quaternion Orientation { get { return _orientation; } set { _orientation = value; UpdateMatrix();  } }

        void UpdateMatrix()
        {
            ModelMatrix = Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(Orientation) * Matrix.CreateTranslation(Position);
        }

        public void DrawWireframeOverlay(GraphicsDevice device, Matrix parentWorldMatrix, CommonShaderParameters shaderParams)
        {
            WireframeEffect.Projection = shaderParams.Projection;
            WireframeEffect.View = shaderParams.View;
            WireframeEffect.World = ModelMatrix;
            Geometry.ApplyMesh(WireframeEffect, device);
        }

        public void DrawSelectedFaces(GraphicsDevice device, Matrix parentWorldMatrix, CommonShaderParameters shaderParams, List<int> faces)
        {
            SelectedFacesEffect.Projection = shaderParams.Projection;
            SelectedFacesEffect.View = shaderParams.View;
            SelectedFacesEffect.World = ModelMatrix;
            Geometry.ApplyMeshPart(SelectedFacesEffect, device, faces);
        }

        public void DrawVertexes(GraphicsDevice device, Matrix parentWorldMatrix, CommonShaderParameters shaderParams)
        {
            SelectedFacesEffect.Projection = shaderParams.Projection;
            SelectedFacesEffect.View = shaderParams.View;
            //var p = Vector3.Zero * ModelMatrix ;
            for (int i = 0; i < Geometry.VertexCount(); i++)
            {
                SelectedFacesEffect.World = Matrix.CreateScale(0.1f) * Matrix.CreateFromQuaternion(Orientation) * Matrix.CreateTranslation(Vector3.Transform(Geometry.GetVertex(i), ModelMatrix));
                _selectedVertexGeo.ApplyMesh(SelectedFacesEffect, device);
            }
        }

        public void DrawBasic(GraphicsDevice device, Matrix parentWorldMatrix, CommonShaderParameters shaderParams)
        {
            DefaultEffect.Projection = shaderParams.Projection;
            DefaultEffect.View = shaderParams.View;
            DefaultEffect.World = ModelMatrix;
            Geometry.ApplyMesh(DefaultEffect, device);

            //DrawVertexes(device, parentWorldMatrix, shaderParams);
        }

        //public void DrawCinematic(GraphicsDevice device, Matrix parentWorldMatrix, CommonShaderParameters shaderParams)
        //{ }
    }

    public static class RenderItemHelper
    {
        public static void CreateDefaultShaders(RenderItem item, GraphicsDevice device)
        {
            item.DefaultEffect = new BasicEffect(device);
            item.DefaultEffect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
            item.DefaultEffect.EnableDefaultLighting(); 

            item.WireframeEffect = new BasicEffect(device);
            item.WireframeEffect.DiffuseColor = Vector3.Zero;

            item.SelectedFacesEffect = new BasicEffect(device);
            item.SelectedFacesEffect.DiffuseColor = new Vector3(1, 0, 0);
            item.SelectedFacesEffect.SpecularColor = new Vector3(1, 0, 0);
            item.SelectedFacesEffect.EnableDefaultLighting();


            item._selectedVertexGeo = new CubeMesh(device);
        }

        public static RenderItem CreateRenderItem(IGeometry geo, Vector3 position, Vector3 scale, string name, GraphicsDevice graphicsDevice)
        {
            var item = new RenderItem(geo, position, Quaternion.Identity, scale);
            item.Name = name;
            CreateDefaultShaders(item, graphicsDevice);
            return item;
        }
    }
}