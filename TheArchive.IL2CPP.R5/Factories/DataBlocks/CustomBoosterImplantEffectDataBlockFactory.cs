﻿using GameData;
using System;
using TheArchive.Interfaces;
using TheArchive.Managers;
using TheArchive.Models.DataBlocks;

namespace TheArchive.IL2CPP.R5.Factories.DataBlocks
{
    public class CustomBoosterImplantEffectDataBlockFactory : IBaseGameConverter<CustomBoosterImplantEffectDataBlock>
    {
        public CustomBoosterImplantEffectDataBlock FromBaseGame(object baseGame, CustomBoosterImplantEffectDataBlock existingCT = null)
        {
            var baseBlock = (BoosterImplantEffectDataBlock) baseGame;

            var customBlock = existingCT ?? new CustomBoosterImplantEffectDataBlock();

            customBlock = (CustomBoosterImplantEffectDataBlock) ImplementationInstanceManager.FromBaseGameConverter<CustomGameDataBlockBase>(baseGame, customBlock);

            customBlock.BoosterEffectCategory = baseBlock.BoosterEffectCategory;
            customBlock.Description = baseBlock.Description;
            customBlock.DescriptionNegative = baseBlock.DescriptionNegative;
            customBlock.Effect = (int) baseBlock.Effect;
            customBlock.PublicName = baseBlock.PublicName;
            customBlock.PublicShortName = baseBlock.PublicShortName;

            return customBlock;
        }

        public Type GetBaseGameType() => typeof(BoosterImplantEffectDataBlock);

        public Type GetCustomType() => typeof(CustomBoosterImplantEffectDataBlock);

        public object ToBaseGame(CustomBoosterImplantEffectDataBlock customType, object existingBaseGame = null) => throw new NotImplementedException();
    }
}