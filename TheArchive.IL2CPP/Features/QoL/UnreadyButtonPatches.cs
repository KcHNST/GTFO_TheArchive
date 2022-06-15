﻿using CellMenu;
using SNetwork;
using TheArchive.Core;
using TheArchive.Core.Attributes;
using static TheArchive.Utilities.Utils;

namespace TheArchive.Features.QoL
{
    [RundownConstraint(RundownFlags.RundownOne, RundownFlags.RundownFive)]
    internal class UnreadyButtonPatches : Feature
    {
        public override string Name => "Unready Button";

        [ArchivePatch(typeof(CM_PageLoadout), "UpdateReadyButton")]
        internal static class CM_PageLoadout_UpdateReadyButtonPatch
        {
#if IL2CPP
            public static void Postfix(CM_PageLoadout __instance)
            {
                var readyButton = __instance.m_readyButton;
#else
            public static void Postfix(CM_TimedButton ___m_readyButton)
            {
                var readyButton = ___m_readyButton;
#endif
                if (!SNet.IsInLobby)
                    return;

                if (SNet.IsMaster)
                    return;

                if (GameStateManager.IsReady)
                {
                    readyButton.SetText("UNREADY");
                    readyButton.SetButtonEnabled(true);
                }
            }

        }
    }
}