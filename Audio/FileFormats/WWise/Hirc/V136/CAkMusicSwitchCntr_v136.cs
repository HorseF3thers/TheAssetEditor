﻿using Filetypes.ByteParsing;
using System;
using System.Collections.Generic;

namespace Audio.FileFormats.WWise.Hirc.V136
{
    public class CAkMusicSwitchCntr_v136 : HircItem
    {

        public MusicTransNodeParams MusicTransNodeParams { get; set; }
        public byte bIsContinuePlayback { get; set; }

        public uint uTreeDepth;
        public ArgumentList ArgumentList;
        public uint uTreeDataSize;
        public byte uMode;
        public AkDecisionTree AkDecisionTree;


        protected override void CreateSpesificData(ByteChunk chunk)
        {
            MusicTransNodeParams = MusicTransNodeParams.Create(chunk);
            bIsContinuePlayback = chunk.ReadByte();

            uTreeDepth = chunk.ReadUInt32();
            ArgumentList = new ArgumentList(chunk, uTreeDepth);
            uTreeDataSize = chunk.ReadUInt32();
            uMode = chunk.ReadByte();
            AkDecisionTree = new AkDecisionTree(chunk, uTreeDepth, uTreeDataSize, Size);
        }

        public override void UpdateSize() => throw new NotImplementedException();
        public override byte[] GetAsByteArray() => throw new NotImplementedException();
    }
}