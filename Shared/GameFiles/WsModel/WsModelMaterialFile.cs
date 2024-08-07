﻿using System.Text;
using System.Xml;
using Shared.Core.PackFiles.Models;
using Shared.GameFormats.RigidModel;
using Shared.GameFormats.RigidModel.Types;

namespace Shared.GameFormats.WsModel
{
    public class WsModelMaterialParam
    { 
        public required string Name { get; set; }
        public required string Type { get; set; }
        public required string Value { get; set; }
    }

    public class WsModelMaterialFile
    {
        public bool Alpha { get; set; } = false;
        public Dictionary<TextureType, string> Textures { get; set; } = [];
        public UiVertexFormat VertexType { get; set; } = UiVertexFormat.Unknown;
        public string Name { get; set; } = string.Empty;

        public List<WsModelMaterialParam> Parameters { get; set; } = [];
        public string ShaderPath { get; set; } = string.Empty;

        public WsModelMaterialFile(PackFile pf)
        {
            var buffer = pf.DataSource.ReadData();
            var xmlString = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            var doc = new XmlDocument();
            doc.LoadXml(xmlString);

            ExtractParameters(doc);
            ExtractShaderName(doc);
            ExtractInformationFromName(doc);
            ExtractTextures(doc);
        }

        private void ExtractShaderName(XmlDocument doc)
        {
            var node = doc.SelectSingleNode(@"/material/shader");
            if (node == null)
                return;
            ShaderPath = node.InnerText;
        }

        void ExtractParameters(XmlDocument doc)
        {
            var parameterNodes = doc.SelectNodes(@"/material/params/param");
            if (parameterNodes == null)
                return;

            foreach (XmlNode paramNode in parameterNodes)
            {
                var paramName = paramNode.SelectSingleNode("name")!.InnerText;
                var paramType = paramNode.SelectSingleNode("type")!.InnerText;
                var paramValue = paramNode.SelectSingleNode("value")!.InnerText;

                Parameters.Add(new WsModelMaterialParam() { Name = paramName, Type = paramType, Value = paramValue });
            }
        }

        void ExtractInformationFromName(XmlDocument doc)
        {
            var nameNode = doc.SelectSingleNode(@"/material/name");
            if (nameNode == null)
                return;
          
            Name = nameNode.InnerText;
            if (Name.Contains("alpha_on", StringComparison.InvariantCultureIgnoreCase))
                Alpha = true;

            if (Name.Contains("weighted4", StringComparison.InvariantCultureIgnoreCase))
                VertexType = UiVertexFormat.Cinematic;
            else if (Name.Contains("weighted2", StringComparison.InvariantCultureIgnoreCase))
                VertexType = UiVertexFormat.Weighted;
            else if (Name.Contains("weighted_standard_4", StringComparison.InvariantCultureIgnoreCase))
                VertexType = UiVertexFormat.Cinematic;
            else if (Name.Contains("weighted_standard_2", StringComparison.InvariantCultureIgnoreCase))
                VertexType = UiVertexFormat.Weighted;
            else
                VertexType = UiVertexFormat.Static;
        }

        void ExtractTextures(XmlDocument doc)
        {
            var textureNodes = doc.SelectNodes(@"/material/textures/texture");
            foreach (XmlNode node in textureNodes)
            {
                var slotNode = node.SelectSingleNode("slot");
                var pathNode = node.SelectSingleNode("source");

                var texturePath = "";
                if (pathNode == null)
                    texturePath = node.InnerText;
                else
                    texturePath = pathNode.InnerText;

                var textureSlotName = slotNode.InnerText;

                if (textureSlotName.Contains("diffuse", StringComparison.InvariantCultureIgnoreCase))
                    Textures[TextureType.Diffuse] = texturePath;
                if (textureSlotName.Contains("gloss", StringComparison.InvariantCultureIgnoreCase))
                    Textures[TextureType.Gloss] = texturePath;
                if (textureSlotName.Contains("mask", StringComparison.InvariantCultureIgnoreCase))
                    Textures[TextureType.Mask] = texturePath;
                if (textureSlotName.Contains("normal", StringComparison.InvariantCultureIgnoreCase))
                    Textures[TextureType.Normal] = texturePath;
                if (textureSlotName.Contains("specular", StringComparison.InvariantCultureIgnoreCase))
                    Textures[TextureType.Specular] = texturePath;
                if (textureSlotName.Contains("base_colour", StringComparison.InvariantCultureIgnoreCase))
                    Textures[TextureType.BaseColour] = texturePath;
                if (textureSlotName.Contains("material_map", StringComparison.InvariantCultureIgnoreCase))
                    Textures[TextureType.MaterialMap] = texturePath;
            }
        }
    }
}
