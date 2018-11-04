
using Engine;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager: MonoBehaviour
{

    public int prefabsCount;
    public int prewarmNumber = 5;
    public int startTrackIndex = 4;
    public GameObject[] prefabs;

    Track foreTrack;
    List<Track> spare;
    Queue<Track> queue;
    bool initialized = false;
    Track startTrack;

    private void Awake()
    {
        Level.LevelLoaded += Init;

        Track.TrackReached += OnNewTrackReached;
        Character.CharacterCreated += SetToTrack;
        GameManager.LevelClear += Restart;
    }


    private void Init()
    {
        spare = new List<Track>();
        queue = new Queue<Track>();
        int k = 0;
        for (int i = 0; i < prefabs.Length; i++)
        {
            for (int j = 0; j < prefabsCount; j++)
            {
                spare.Add(Instantiate(prefabs[i], new Vector3(-100000,-100000,-100000),prefabs[i].transform.rotation).GetComponent<Track>());
                spare[spare.Count - 1].name = k.ToString();
                k++;
            }
        }
        for (int i = 0; i < prewarmNumber; i++)
        {
            InitTrack(i);
        }

        initialized = true;

    }

    void SetToTrack(Character character)
    {
        character.transform.position = startTrack.transform.position;
    }

    private void OnDestroy()
    {
        Track.TrackReached -= OnNewTrackReached;
        Character.CharacterCreated -= SetToTrack;
        GameManager.LevelClear -= Restart;
    }

    void OnNewTrackReached(Track track)
    {
        var t = Dequeue();
        spare.Add(t);
        t.gameObject.SetActive(false);
        var newLast = spare[Random.Range(0, spare.Count-1)];
        newLast.gameObject.SetActive(true);
        Transform anchor = foreTrack.endAnchor;      
        newLast.transform.position = anchor.position;
        newLast.transform.rotation = anchor.rotation;
        foreTrack = newLast;
        spare.Remove(foreTrack);
        Enqueue(foreTrack);
    }

    void InitTrack(int i)
    {
        var newLast = Instantiate(prefabs[0]).GetComponent<Track>();
        newLast.gameObject.SetActive(true);
        Transform anchor = transform;
        newLast.transform.position = anchor.position;
        newLast.transform.rotation = anchor.rotation;
        foreTrack = newLast;
        spare.Remove(foreTrack);
        Enqueue(foreTrack);
        if (i == startTrackIndex)
            startTrack = foreTrack;
    }

    void Enqueue(Track track)
    {
        Debug.Log("Enqueue " + track.name);
        queue.Enqueue(track);
    }

    void Restart()
    {
        initialized = false;
        foreach (var track in spare)
        {
            if (track != null)
                Destroy(track.gameObject);
        }
        foreach (var track in queue)
        {
            if (track != null)
                Destroy(track.gameObject);
        }

    }

    Track Dequeue()
    {
        var t = queue.Dequeue();
        Debug.Log("Dequeue " + t.name);
        return t;
    }
}