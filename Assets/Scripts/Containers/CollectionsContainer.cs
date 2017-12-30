using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class CollectionsContainer : DataContainer
{
    public CollectionData data;
    public override IData Data
    {
        get
        {
            return data;
        }
        set
        {
            data = (CollectionData)value;
        }
    }

    [Serializable]
    public class CollectionData : IData
    {
        public string id;
        public string ID
        {
            get
            {
                return id;
            }
        }

        public int coins;
        public int emmeralds;
    }



}

