using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class SettingsContainer : DataContainer
{
    public Settings data;

    //public Sett data;
    public override IData Data
    {
        get
        {
            return data;
        }
        set
        {
            data = (Settings)value;
        }
    }

    [Serializable]
    public class Settings : IData
    {
        public override string ToString()
        {
            return id;
        }
        public string id;
        public string ID
        {
            get
            {
                return id;
            }
        }
        public Float2 resolution = new Float2(1920, 1080);
        public bool runBenchmark;
        public bool firstStart;
    }
}
