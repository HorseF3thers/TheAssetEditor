﻿using Common;
using Filetypes.RigidModel.LodHeader;
using Serilog;
using System;
using System.IO;

namespace Filetypes.RigidModel
{
    public class RmvRigidModel
    {
        public RmvModelHeader Header { get; private set; }
        public RmvLodHeader[] LodHeaders { get; private set; }
        public RmvSubModel[][] MeshList { get; private set; }
        public string FileName { get; private set; }

        public RmvRigidModel(byte[] data, string fileName)
        { 
            ILogger logger = Logging.Create<RmvRigidModel>();
            logger.Here().Information($"Loading Rmv2RigidModel: {fileName}");
            if (data.Length == 0)
                throw new Exception("Trying to load Rmv2RigidModel with no data, data size = 0");

            FileName = fileName;
            Header = LoadModelHeader(data);
            LodHeaders = LoadLodHeaders(data);

            MeshList = new RmvSubModel[Header.LodCount][];
            for (int lodIndex = 0; lodIndex < Header.LodCount; lodIndex++)
            {
                var lodMeshCount = LodHeaders[lodIndex].MeshCount;
                MeshList[lodIndex] = new RmvSubModel[lodMeshCount];
            
                var sizeOffset = 0;
                for (int meshIndex = 0; meshIndex < lodMeshCount; meshIndex++)
                {
                    var offset = LodHeaders[lodIndex].FirstMeshOffset + sizeOffset;
                    MeshList[lodIndex][meshIndex] = new RmvSubModel(data, (int)offset);
                    sizeOffset += (int)MeshList[lodIndex][meshIndex].Header.ModelSize;
                }
            }

            logger.Here().Information("Loading done");
        }

        RmvModelHeader LoadModelHeader(byte[] data)
        {
           return ByteHelper.ByteArrayToStructure<RmvModelHeader>(data, 0);
        }

        int GetLodHeaderSize()
        {
            if (Header.Version == 6)
                return ByteHelper.GetSize(typeof(Rmv2LodHeader_V6));
            else if(Header.Version == 7)
                return ByteHelper.GetSize(typeof(Rmv2LodHeader_V7));
            
            throw new Exception("Unknown rmv2 version - " + Header.Version);
        }

        RmvLodHeader[] LoadLodHeaders(byte[] data)
        {
            var offset = ByteHelper.GetSize(typeof(RmvModelHeader));
            var lodHeaderSize = GetLodHeaderSize();

            var lodHeaders = new RmvLodHeader[Header.LodCount];
            for (int i = 0; i < Header.LodCount; i++)
            {
                RmvLodHeader header;
                if (Header.Version == 6)
                    header = ByteHelper.ByteArrayToStructure<Rmv2LodHeader_V6>(data, offset + lodHeaderSize*i);
                else
                    header = ByteHelper.ByteArrayToStructure<Rmv2LodHeader_V7>(data, offset + lodHeaderSize * i);

                lodHeaders[i] = header;
            }

            return lodHeaders;
        }

        public void SaveToByteArray(BinaryWriter writer)
        {
            if (Header.Version != 7)
                throw new Exception("Not a know version - can not save");

            writer.Write(ByteHelper.GetBytes(Header));

            for (int i = 0; i < LodHeaders.Length; i++)
                writer.Write(ByteHelper.GetBytes((Rmv2LodHeader_V7)LodHeaders[i]));

            for (int lodIndex = 0; lodIndex < Header.LodCount; lodIndex++)
            {
                var modelList = MeshList[lodIndex];
                for(var modelIndex = 0; modelIndex < modelList.Length; modelIndex++)
                {
                    var model = modelList[modelIndex];
                    writer.Write(ByteHelper.GetBytes(model.Header));
                    for (var attachmentPointIndex = 0; attachmentPointIndex < model.AttachmentPoints.Count; attachmentPointIndex++)
                        writer.Write(ByteHelper.GetBytes(model.AttachmentPoints[attachmentPointIndex]));

                    for (var textureIndex = 0; textureIndex < model.Textures.Count; textureIndex++)
                        writer.Write(ByteHelper.GetBytes(model.Textures[textureIndex]));

                    model.Mesh.SaveToByteArray(writer, model.Header.VertextType);
                }
            }
        }

    }
}
