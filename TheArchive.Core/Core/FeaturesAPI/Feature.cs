﻿using System;
using System.Collections.Generic;
using TheArchive.Core.FeaturesAPI.Settings;
using TheArchive.Core.FeaturesAPI.Components;
using TheArchive.Core.Models;
using TheArchive.Interfaces;
using static TheArchive.Utilities.Utils;
using TheArchive.Utilities;

namespace TheArchive.Core.FeaturesAPI
{
    /// <summary>
    /// An enableable / disableable feature
    /// </summary>
    public abstract class Feature
    {
        private string _identifier = null;
        public string Identifier => _identifier ??= this.GetType().Name;
        public bool IsHidden => FeatureInternal.HideInModSettings;
        public bool BelongsToGroup => Group != null;
        public bool HasAdditionalSettings => FeatureInternal.HasAdditionalSettings;
        public bool AllAdditionalSettingsAreHidden => FeatureInternal.AllAdditionalSettingsAreHidden;
        public IEnumerable<FeatureSettingsHelper> SettingsHelpers => FeatureInternal.Settings;
        public void RequestRestart() => FeatureManager.RequestRestart(this);
        public void RevokeRestartRequest() => FeatureManager.RevokeRestartRequest(this);

        protected void RequestDisable(string reason = null) => FeatureInternal.RequestDisable(reason);

        /// <summary>
        /// True if this <see cref="Feature"/> is controled via code<br/>
        /// (button disabled in Mod Settings!)
        /// </summary>
        public bool IsAutomated => FeatureInternal.AutomatedFeature;

        /// <summary>
        /// Does what it says it does
        /// </summary>
        public bool DisableModSettingsButton => FeatureInternal.DisableModSettingsButton;

        /// <summary>
        /// Logging interface for this <see cref="Feature"/>
        /// </summary>
        public IArchiveLogger FeatureLogger => FeatureInternal.FeatureLoggerInstance;

        /// <summary>
        /// If the <see cref="Feature"/> is currently enabled.
        /// </summary>
        public bool Enabled { get; internal set; } = false;

        public bool AppliesToThisGameBuild => !FeatureInternal.InternalDisabled;

        public RundownFlags AppliesToRundowns => FeatureInternal.Rundowns;

        /// <summary>
        /// Information about the current game build.
        /// </summary>
        public static GameBuildInfo BuildInfo { get; internal set; }

        /// <summary>
        /// The <see cref="Feature"/>s Name<br/>
        /// used in Mod Settings
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// A text describing this <see cref="Feature"/><br/>
        /// used in Mod Settings
        /// </summary>
        public virtual string Description { get; set; } = string.Empty;

        /// <summary>
        /// Used to group multiple settings together under one header<br/>
        /// used in Mod Settings
        /// </summary>
        public virtual string Group => null;

        /// <summary>
        /// If set, prevents calling of <see cref="OnEnable"/> and <see cref="OnDisable"/> methods and only switches the config state of this <see cref="Feature"/>.
        /// </summary>
        public virtual bool RequiresRestart => false;

        /// <summary>
        /// If set, prevents calling of <see cref="OnEnable"/> on startup once.<br/>
        /// <see cref="OnEnable"/> gets called normally after that.
        /// </summary>
        public virtual bool SkipInitialOnEnable => false;

        /// <summary>
        /// If the <see cref="Feature"/> requires a UnityEngine AudioListener Component setup on the LocalPlayer GameObject
        /// </summary>
        public virtual bool RequiresUnityAudioListener => false;

        [Obsolete($"Remove or use {nameof(InlineSettingsIntoParentMenu)} instead if inlining is intended")]
        public virtual bool PlaceSettingsInSubMenu => false;

        /// <summary>
        /// If this <see cref="Feature"/>s settings should be put inside of íts parent menu in the Mod Settings menu
        /// </summary>
        public virtual bool InlineSettingsIntoParentMenu => false;

        /// <summary>
        /// Called once upon application start before <see cref="Init"/> and before any patches have been loaded
        /// </summary>
        /// <returns>If the <see cref="Feature"/> should be inited</returns>
        public virtual bool ShouldInit()
        {
            return true;
        }

        /// <summary>
        /// Called once upon application start and after all patches have been loaded
        /// </summary>
        public virtual void Init()
        {
            
        }

        /// <summary>
        /// Called every time the feature gets enabled
        /// </summary>
        public virtual void OnEnable()
        {

        }

        /// <summary>
        /// Called every time the feature gets disabled
        /// </summary>
        public virtual void OnDisable()
        {

        }

        /// <summary>
        /// Called once after the game data has initialized
        /// </summary>
        public virtual void OnGameDataInitialized()
        {

        }

        /// <summary>
        /// Called everytime the game is focused or unfocused
        /// </summary>
        /// <param name="focus"></param>
        public virtual void OnApplicationFocusChanged(bool focus)
        {
            
        }

        /// <summary>
        /// Called once after datablocks have been loaded
        /// </summary>
        public virtual void OnDatablocksReady()
        {

        }

        /// <summary>
        /// Called once every frame whenever the feature is enabled
        /// </summary>
        public virtual void Update()
        {

        }

        /// <summary>
        /// Called once every frame whenever the feature is enabled after all <see cref="Update"/>s have been called
        /// </summary>
        public virtual void LateUpdate()
        {

        }

        /// <summary>
        /// Called everytime after a setting has been changed
        /// </summary>
        /// <param name="setting">The changed setting</param>
        public virtual void OnFeatureSettingChanged(FeatureSetting setting)
        {
            
        }

        /// <summary>
        /// Called everytime the gamestate changes<br/>
        /// Cast to <c>eGameStateName</c> or define a new instance method <c>OnGameStateChanged(eGameStateName state)</c>
        /// </summary>
        /// <param name="state">The state</param>
        public virtual void OnGameStateChanged(int state)
        {

        }

        /// <summary>
        /// Called whenever an area is culled / unculled<br />
        /// Cast to the first parameter to <c>LG_Area</c> or define a new instance method <c>OnAreaCull(LG_Area area, bool active)</c>
        /// </summary>
        /// <param name="lgArea">LG_Area that is affected</param>
        /// <param name="active">if rendered or not</param>
        public virtual void OnAreaCull(object lgArea, bool active)
        {

        }

        /// <summary>
        /// Called whenever a <see cref="FButton"/> from this <see cref="Feature"/>s mod settings menu has been pressed.
        /// </summary>
        /// <param name="setting"></param>
        public virtual void OnButtonPressed(ButtonSetting setting)
        {

        }

        /// <summary>
        /// Called whenever the application quits
        /// <br/>
        /// Gets executed right before <seealso cref="OnDisable"/> is called
        /// </summary>
        public virtual void OnQuit()
        {

        }

        internal FeatureInternal FeatureInternal { get; set; }
        public static bool IsPlayingModded => ArchiveMod.IsPlayingModded;
        public static bool DevMode => ArchiveMod.Settings.FeatureDevMode;
        public static bool GameDataInited { get; internal set; } = false;
        public static bool IsApplicationFocused { get; internal set; } = false;
        public static bool IsApplicationQuitting { get; internal set; } = false;
        public static bool DataBlocksReady { get; internal set; } = false;
        /// <summary>Cast to eGameStateName</summary>
        public static int CurrentGameState { get; internal set; } = 0;
        /// <summary>Cast to eGameStateName</summary>
        public static int PreviousGameState { get; internal set; } = 0;

        internal static void SetupIs()
        {
            Is.R1 = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownOne);
            Is.R1OrLater = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownOne.ToLatest());
            Is.R2 = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownTwo);
            Is.R2OrLater = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownTwo.ToLatest());
            Is.R3 = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownThree);
            Is.R3OrLater = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownThree.ToLatest());
            Is.R4 = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownFour);
            Is.R4OrLater = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownFour.ToLatest());
            Is.R5 = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownFive);
            Is.R5OrLater = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownFive.ToLatest());
            Is.R6 = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownSix);
            Is.R6OrLater = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownSix.ToLatest());
            Is.R7 = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownSeven);
            Is.R7OrLater = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownSeven.ToLatest());
            Is.A1 = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownAltOne);
            Is.A1OrLater = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownAltOne.ToLatest());
            Is.A2 = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownAltTwo);
            Is.A2OrLater = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownAltTwo.ToLatest());
            Is.A3 = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownAltThree);
            Is.A3OrLater = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownAltThree.ToLatest());
            Is.A4 = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownAltFour);
            Is.A4OrLater = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownAltFour.ToLatest());
            Is.A5 = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownAltFive);
            Is.A5OrLater = BuildInfo.Rundown.IsIncludedIn(RundownFlags.RundownAltFive.ToLatest());
        }

        /// <summary>
        /// If the current game version is [...]
        /// </summary>
        public static class Is
        {
            public static bool R1 { get; internal set; }
            public static bool R1OrLater { get; internal set; }
            public static bool R2 { get; internal set; }
            public static bool R2OrLater { get; internal set; }
            public static bool R3 { get; internal set; }
            public static bool R3OrLater { get; internal set; }
            public static bool R4 { get; internal set; }
            public static bool R4OrLater { get; internal set; }
            public static bool R5 { get; internal set; }
            public static bool R5OrLater { get; internal set; }
            public static bool R6 { get; internal set; }
            public static bool R6OrLater { get; internal set; }
            public static bool R7 { get; internal set; }
            public static bool R7OrLater { get; internal set; }
            public static bool A1 { get; internal set; }
            public static bool A1OrLater { get; internal set; }
            public static bool A2 { get; internal set; }
            public static bool A2OrLater { get; internal set; }
            public static bool A3 { get; internal set; }
            public static bool A3OrLater { get; internal set; }
            public static bool A4 { get; internal set; }
            public static bool A4OrLater { get; internal set; }
            public static bool A5 { get; internal set; }
            public static bool A5OrLater { get; internal set; }
        }
    }
}
