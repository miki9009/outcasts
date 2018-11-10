using Engine;
using UnityEngine;

public class LevelVisualSettings : LevelElement
{
    public UnityEngine.Color color;
    public Material material;
    public Material standardVertex;


    [Range(0,1.5f)]
    public float bloomThreshold = 0.6f;
    [Range(0, 1)]
    public float materialSmoothness = 0.7f; 

    public override void OnSave()
    {
        base.OnSave();
        Engine.Colour col = color;
        data.Add("Color", col);
        data.Add("BloomThreshold", bloomThreshold);
        data.Add("Smoothness", materialSmoothness);
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data.ContainsKey("Color"))
        {
            color = (Engine.Colour)data["Color"];
            material.color = color;
        }
        if(data.ContainsKey("BloomThreshold"))
        {
            bloomThreshold = (float)data["BloomThreshold"];
            if(Controller.Instance!=null)
                Controller.Instance.bloom.threshold = bloomThreshold;
        }
        if(data.ContainsKey("Smoothness"))
        {
            try
            {
                materialSmoothness = (float)data["Smoothness"];
                material.SetFloat("_Glossiness", materialSmoothness);
            }
            catch { }

        }
    }


}