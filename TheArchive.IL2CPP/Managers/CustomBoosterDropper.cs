﻿using System;
using System.Collections.Generic;
using System.Linq;
using TheArchive.Core;
using TheArchive.Core.Managers;
using TheArchive.Interfaces;
using TheArchive.Models.Boosters;
using TheArchive.Models.DataBlocks;
using TheArchive.Utilities;

namespace TheArchive.Managers
{
    public class CustomBoosterDropper : InitSingletonBase<CustomBoosterDropper>, IInitAfterDataBlocksReady, IInitCondition
    {
        private static bool _hasBeenSetup = false;

        public CustomBoosterImplantTemplateDataBlock[] MutedTemplates { get; private set; }
        public CustomBoosterImplantTemplateDataBlock[] BoldTemplates { get; private set; }
        public CustomBoosterImplantTemplateDataBlock[] AgrressiveTemplates { get; private set; }

        public CustomBoosterImplantEffectDataBlock[] Effects { get; private set; }
        public CustomBoosterImplantConditionDataBlock[] Conditions { get; private set; }

        public bool InitCondition()
        {
            return ArchiveMod.CurrentRundown != Utils.RundownID.RundownFour;
        }

        public void Init()
        {
            if(_hasBeenSetup)
            {
                ArchiveLogger.Info($"{nameof(CustomBoosterDropper)} already setup, skipping ...");
                return;
            }
            ArchiveLogger.Info($"Setting up {nameof(CustomBoosterDropper)} ...");

            var templates = ImplementationManager.GetAllCustomDataBlocksFor<CustomBoosterImplantTemplateDataBlock>();

            MutedTemplates = templates.Where(t => t.ImplantCategory == BoosterImplantCategory.Muted).ToArray();
            BoldTemplates = templates.Where(t => t.ImplantCategory == BoosterImplantCategory.Bold).ToArray();
            AgrressiveTemplates = templates.Where(t => t.ImplantCategory == BoosterImplantCategory.Aggressive).ToArray();

            Effects = ImplementationManager.GetAllCustomDataBlocksFor<CustomBoosterImplantEffectDataBlock>();
            Conditions = ImplementationManager.GetAllCustomDataBlocksFor<CustomBoosterImplantConditionDataBlock>();

            ArchiveLogger.Msg(ConsoleColor.Magenta, $"{nameof(CustomBoosterDropper)}.{nameof(Init)}() complete, retrieved {MutedTemplates.Length} Muted, {BoldTemplates.Length} Bold and {AgrressiveTemplates.Length} Agrressive Templates as well as {Effects?.Length} Effects and {Conditions?.Length} Conditions.");
            Instance = this;
            _hasBeenSetup = true;
        }

        public void Test()
        {
            ArchiveLogger.Msg(ConsoleColor.Cyan, $"!!!!!!!!! TEST !!!!!!!!!");
        }

        public static int BOOSTER_DROP_MAX_REROLL_COUNT { get; internal set; } = 25;

        public CustomDropServerBoosterImplantInventoryItem GenerateBooster(BoosterImplantCategory category, uint[] usedIds)
        {
            CustomBoosterImplantTemplateDataBlock template;
            float weight = 1f;

            int maxUses;

            int count = 0;
            do
            {
                switch (category)
                {
                    default:
                    case BoosterImplantCategory.Muted:
                        template = MutedTemplates[UnityEngine.Random.Range(0, MutedTemplates.Length)];
                        break;
                    case BoosterImplantCategory.Bold:
                        template = BoldTemplates[UnityEngine.Random.Range(0, BoldTemplates.Length)];
                        break;
                    case BoosterImplantCategory.Aggressive:
                        template = AgrressiveTemplates[UnityEngine.Random.Range(0, AgrressiveTemplates.Length)];
                        break;
                }
                if (template == null) continue;
                if (count > BOOSTER_DROP_MAX_REROLL_COUNT) break;
                weight = 1f;
                if (template.DropWeight != 0) weight = 1 / template.DropWeight;
                count++;
            } while (template == null || UnityEngine.Random.Range(0f, 1f) > weight);

            switch (category)
            {
                default:
                case BoosterImplantCategory.Muted:
                    maxUses = 1; // 1
                    break;
                case BoosterImplantCategory.Bold:
                    maxUses = UnityEngine.Random.Range(1, 3); // 1-2
                    break;
                case BoosterImplantCategory.Aggressive:
                    maxUses = UnityEngine.Random.Range(2, 4); // 2-3
                    break;
            }

            var conditionIds = new List<uint>();
            var effects = new List<CustomBoosterImplant.Effect>();

            // Add set effects
            foreach (var fx in template.Effects)
            {
                effects.Add(new CustomBoosterImplant.Effect
                {
                    Id = fx.BoosterImplantEffect,
                    Value = UnityEngine.Random.Range(fx.MinValue, fx.MaxValue)
                });
            }

            // Choose from random effects
            foreach (var randomEffectList in template.RandomEffects)
            {
                if (randomEffectList == null || randomEffectList.Count < 1) continue;
                var fx = randomEffectList[UnityEngine.Random.Range(0, randomEffectList.Count)];

                effects.Add(new CustomBoosterImplant.Effect
                {
                    Id = fx.BoosterImplantEffect,
                    Value = UnityEngine.Random.Range(fx.MinValue, fx.MaxValue)
                });
            }

            // Add set condition
            foreach (var cond in template.Conditions)
            {
                conditionIds.Add(cond);
            }

            // Pick one of the random conditions
            if (template.RandomConditions != null && template.RandomConditions.Count > 0)
            {
                conditionIds.Add(template.RandomConditions[UnityEngine.Random.Range(0, template.RandomConditions.Count)]);
            }

            var instanceId = GenerateInstanceId(usedIds);

            var value = new CustomDropServerBoosterImplantInventoryItem(template.PersistentID, instanceId, maxUses, effects.ToArray(), conditionIds.ToArray());

            value.Category = category;

#pragma warning disable CS0618 // Type or member is obsolete
            value.Template = template;
#pragma warning restore CS0618 // Type or member is obsolete

            CustomBoosterImplantEffectDataBlock effectDB = null;
            CustomBoosterImplantConditionDataBlock conditionDB = null;
            try
            {
                if(effects.Count > 0)
                    effectDB = Effects.First(ef => ef.PersistentID == effects[0].Id);
                if(conditionIds.Count > 0)
                    conditionDB = Conditions.First(cd => cd.PersistentID == conditionIds[0]);
            }
            catch(Exception)
            {

            }
            

            ArchiveLogger.Msg(ConsoleColor.Magenta, $"Generated booster: {template.PublicName} - {effectDB?.PublicShortName} {conditionDB?.PublicShortName} ({effectDB?.PublicName} when {conditionDB?.PublicName})");

            return value;
        }

        private uint GenerateInstanceId(uint[] usedIds)
        {
            if (usedIds == null || usedIds.Length == 0) return 1;
            uint count = 1;
            while(true)
            {
                if(usedIds.Any(i => i == count)) {
                    count++;
                }
                else
                {
                    return count;
                }
            }
        }

        public void GenerateAndAddBooster(ref CustomBoosterImplantPlayerData data, BoosterImplantCategory category)
        {
            var newBooster = GenerateBooster(category, data.GetUsedIds());

            if(!data.TryAddBooster(newBooster))
            {
                ArchiveLogger.Msg($"Did not add Booster as the inventory for category {category} is full! (This message should not appear!)");
            }
        }
    }
}