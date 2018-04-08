using UnityEngine;
using Engine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

[DefaultExecutionOrder(1000)]
public class DataManager : MonoBehaviour
{


    //[PopUp(new string[] { "jeden", "dwa" })]

    static DataManager instance;
    public static event Action Saved;
    public static event Action Loaded;
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = GameObject.FindObjectOfType<DataManager>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    instance = new GameObject("DataManager").AddComponent<DataManager>();
                }
            }
            return instance;
        }
    }

    public string dataFileName;

    Dictionary<string, IData> datas;

    private void Awake()
    {
        instance = this;
    }

    public void LoadData()
    {
        try
        {
            datas = (Dictionary<string, IData>)Data.Load(dataFileName);
            Debug.Log("Loaded Succesfully");
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return;
        }

        var dataComponents = GameObject.FindObjectsOfType<DataContainer>().ToList();
        IData data;
        dataComponents.ForEach(o =>
        {
            if (datas.TryGetValue(o.Data.ID, out data))
            {
                o.Data = data;
            }
        });
        if (Loaded != null)
        {
            Loaded();
        }

    }

    public void SaveData()
    {
        CastData();
        Data.Save(dataFileName, datas);
    }

    public IData GetData(string ID)
    {
        IData data = null;
        if (datas != null)
        {
            if (datas.TryGetValue(ID, out data))
            {
                return data;
            }
        }
        else
        {
            datas = (Dictionary<string, IData>)Data.Load(dataFileName);
            if (datas != null)
            {
                if (datas.TryGetValue(ID, out data))
                {
                    return data;
                }
            }
        }
        if (datas == null)
        {
            CastData();
            if (datas != null)
            {
                if (datas.TryGetValue(ID, out data))
                {
                    return data;
                }
            }
        }
        Debug.Log("Data is null");
        return data;
    }

    void CastData()
    {
        datas = new Dictionary<string, IData>();
        var dataComponents = GameObject.FindObjectsOfType<DataContainer>().ToList();
        if (dataComponents != null && dataComponents.Count > 0)
        {
            dataComponents.ForEach(o =>
            {
                if (o.Data != null)
                {
                    datas.Add(o.Data.ID, o.Data);
                }
            });
        }
        else
        {
            Debug.Log("DataComponents was null or 0");
        }
    }

    public static class Containers
    {
        public const string SETTINGS = "Settings";
    }
}

