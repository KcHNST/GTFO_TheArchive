﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TheArchive.Core.FeaturesAPI.Settings
{
    public class EnumListSetting : FeatureSetting
    {
        public string[] Options { get; }
        public Dictionary<string, object> Map { get; private set; } = new Dictionary<string, object>();
        public Type EnumType { get; }
        public EnumListSetting(FeatureSettingsHelper featureSettingsHelper, PropertyInfo prop, object instance, string debug_path = "") : base(featureSettingsHelper, prop, instance, debug_path)
        {
            EnumType = Type.GenericTypeArguments[0];
            Options = Enum.GetNames(EnumType);

            foreach (var option in Options)
            {
                Map.Add(option, Enum.Parse(EnumType, option));
            }
        }

        public object GetEnumValueFor(string option)
        {
            if (Map.TryGetValue(option, out var val))
            {
                return val;
            }

            return null;
        }

        public IList GetList()
        {
            return GetValue() as IList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns>If the value is now present in the list</returns>
        public bool ToggleInList(object value)
        {
            if (RemoveFromList(value))
                return false;
            AddToList(value);
            return true;
        }

        public void AddToList(object value)
        {
            GetList().Add(value);
            FeatureManager.Instance.OnFeatureSettingChanged(this);
        }

        public bool RemoveFromList(object value)
        {
            var list = GetList();
            if (list.Contains(value))
            {
                list.Remove(value);
                FeatureManager.Instance.OnFeatureSettingChanged(this);
                return true;
            }
            return false;
        }

        public object[] CurrentSelectedValues()
        {
            var list = GetList();
            var array = new object[list.Count];
            list.CopyTo(array, 0);
            return array;
        }
    }
}
