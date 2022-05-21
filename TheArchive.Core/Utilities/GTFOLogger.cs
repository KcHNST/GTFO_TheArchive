﻿using MelonLoader;
using System;
using System.Collections.Generic;

namespace TheArchive.Utilities
{
    public static class GTFOLogger
    {
        internal static MelonLogger.Instance Logger { private get; set; }

        private static readonly HashSet<string> _ignoreList = new HashSet<string>() {
            "show crosshair",
            "Setting and getting Body Position/Rotation, IK Goals, Lookat and BoneLocalRotation should only be done in OnAnimatorIK or OnStateIK"
        };

        public static void Ignore(string str) => _ignoreList.Add(str);

        public static void Log(string message)
        {
            if (_ignoreList.Contains(message)) return;
            Logger.Msg(message);
        }

        public static void Warn(string message)
        {
            if (_ignoreList.Contains(message)) return;
            Logger.Warning(message);
        }

        public static void Error(string message)
        {
            if (_ignoreList.Contains(message)) return;
            Logger.Error(message);
        }
    }
}
