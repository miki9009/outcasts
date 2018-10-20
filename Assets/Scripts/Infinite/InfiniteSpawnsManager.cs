using UnityEngine;
using UnityEngine.SceneManagement;

public class InfiniteSpawnsManager : MonoBehaviour
{
    InfiniteManager infiniteManager;
    private static SpawnsConfig spawnsConfig;

    private void Awake()
    {
        spawnsConfig = Controller.SpawnsConfig;
        SpawnManager.Enable = true;

        GameManager.GameReady += () =>
        {
            Debug.Log("Active Scene: " + SceneManager.GetActiveScene().name);
            foreach (var spawnsCollection in spawnsConfig.spawnCollections)
            {
                foreach (var spawnsettings in spawnsCollection.spawnSettings)
                {
                    Debug.Log("Adding spawn name: " + spawnsettings.spawnName);
                    SpawnManager.AddSpawn(spawnsettings.spawnName, spawnsettings.prefab, spawnsettings.numberOfSpawns);
                }
            }
        };
    }

    private void Start()
    {
        infiniteManager = InfiniteManager.Instance;
        infiniteManager.ChunkPlaced += AssignSpawns;
        PrepareContainers();
    }

    SpawnsConfig.SpawnCollection[] spawnCollection;
    void PrepareContainers()
    {
        spawnCollection = new SpawnsConfig.SpawnCollection[100];
        int index = 0;
        foreach (var collection in spawnsConfig.spawnCollections)
        {
            for (int i = 0; i < collection.chances; i++)
            {
                if (index < 100)
                {
                    spawnCollection[index] = collection;
                    index++;
                }
                else
                {
                    Debug.LogError("Container is full, but manager still wanted to add a collection for: " + collection.collectionName);
                }

            }
            collection.SettingsCollection = new SpawnsConfig.SpawnSettings[100];
            int j = 0;
            foreach (var settings in collection.spawnSettings)
            {
                for (int i = 0; i < settings.chances; i++)
                {
                    if (j < 100)
                    {
                        collection.SettingsCollection[j] = settings;
                        j++;
                    }
                    else
                    {
                        Debug.LogError("Container is full, but manager still wanted to add a setting for: " + settings.spawnType);
                    }
                }
            }
        }
    }

    void AssignSpawns(Chunk chunk)
    {
        var spawningPlace = chunk.spawningPlace;
        var setting = ChooseSpawn();
        if(setting == null)
        {
            Debug.Log("Setting was null");
            return;
        }
        Debug.Log("Spawn name: " + setting.spawnType);

        Transform[] row = spawningPlace.rowLeft;
        for (int i = 0; i < 3; i++)
        {
            int count = 0;
            for (int j = 0; j < spawningPlace.rowLeft.Length; j++)
            {
                if (count < setting.amountSpawnAtOnce)
                {
                    var obj = SpawnManager.GetSpawn(setting.spawnName);
                    obj.transform.position = row[count].position;
                    spawningPlace.activeSpawns.Push(obj.GetComponent<IPoolObject>());
                    count++;
                }
            }
            if (row == spawningPlace.rowLeft)
            {
                row = spawningPlace.rowCenter;
            }
            else if (row == spawningPlace.rowCenter)
            {
                row = spawningPlace.rowRight;
            }
        }
    }

    public SpawnsConfig.SpawnSettings ChooseSpawn()
    {
        int range = Random.Range(0, 100);
        var collection = spawnCollection[range];
        range = Random.Range(0, 100);
        return collection.SettingsCollection[range];
    }

    private void OnDestroy()
    {
        SpawnManager.Enable = false;
    }
}