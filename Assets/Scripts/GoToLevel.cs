using Engine;
using UnityEngine;

public class GoToLevel : LevelElement
{
    [LevelSelector]
    public string levelName;
    [CustomLevelSelector]
    public string customLevel;


    private void OnTriggerEnter(Collider other)
    {
        if(other.attachedRigidbody != null)
        {
            var character = other.attachedRigidbody.GetComponentInParent<Character>();
            if(character!=null && character == Character.GetLocalPlayer())
                GoToLevelAdditive();
        }
    }

    void GoToLevelAdditive()
    {
        LevelManager.ChangeLevel(levelName, customLevel);
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data!=null)
        {
            if (data.ContainsKey("LevelName"))
                levelName = (string)data["LevelName"];
            if (data.ContainsKey("CustomLevel"))
                customLevel = (string)data["CustomLevel"];
        }
    }

    public override void OnSave()
    {
        base.OnSave();
        if(data!=null)
        {
            data["LevelName"] = levelName;
            data["CustomLevel"] = customLevel;
        }
    }

}