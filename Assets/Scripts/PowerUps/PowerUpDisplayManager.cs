using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PowerUpDisplayManager : MonoBehaviour
{
    public GameObject prefab;
    public HashSet<PowerUpDisplay> displays;

    public static PowerUpDisplayManager Instance
    {
        get; private set;
    }

    private void Awake()
    {
        Instance = this;
        displays = new HashSet<PowerUpDisplay>();
        GameManager.OnLevelLoaded += ClearPowerUps;
    }

    void ClearPowerUps()
    {
        PowerUp.activePowerUps.Clear();
    }

    private void OnDestroy()
    {
        GameManager.OnLevelLoaded -= ClearPowerUps;
    }

    public void AddDisplay(CollectionType type, int time, PowerUp powerUp)
    {
        PowerUpDisplay d = null;
        try
        {
            d = displays.SingleOrDefault(x => x.type == type);
        }
        catch { }
        if (d == null)
        {
            var obj = Instantiate(prefab, transform);
            var display = obj.GetComponent<PowerUpDisplay>();
            display.image.sprite = CollectionDisplayManager.Instance.collectionSprites[(int)type];
            display.time = time;
            display.powerUp = powerUp;
            display.ConvertTime();
            display.type = type;
            displays.Add(display);
        }
        else
        {
            d.time = time;
            d.ConvertTime();
            d.powerUp = powerUp;
        }
    }
}