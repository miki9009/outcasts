
using System.Collections.Generic;
using UnityEngine;

public class TrackManager: MonoBehaviour
{

    public int prefabsCount;
    public int prewarmNumber = 5;
    public GameObject[] prefabs;

    Track foreTrack;
    List<Track> spare;
    Queue<Track> queue;
    bool initialized = false;

    private void Start()
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
            OnNewTrackReached(spare[0]);
        }
        Track.TrackReached += OnNewTrackReached;
        initialized = true;
        Character.CharacterCreated += SetToTrack;
    }

    void SetToTrack(Character character)
    {
        int i = 0;
        foreach (var track in queue)
        {
            if (i == 3)
                character.transform.position = track.transform.position;
            i++;
        }
    }

    private void OnDestroy()
    {
        Track.TrackReached -= OnNewTrackReached;
        Character.CharacterCreated -= SetToTrack;
    }

    void OnNewTrackReached(Track track)
    {
        if (initialized)
        {
            var t = Dequeue();
            spare.Add(t);
            t.gameObject.SetActive(false);
        }

        var newLast = spare[Random.Range(0, spare.Count-1)];
        newLast.gameObject.SetActive(true);
        Transform anchor = null;
        if(foreTrack == null)
        {
            anchor = transform;
        }
        else
        {
            anchor = foreTrack.endAnchor;
        }
        newLast.transform.position = anchor.position;
        newLast.transform.rotation = anchor.rotation;
        foreTrack = newLast;
        spare.Remove(foreTrack);
        Enqueue(foreTrack);
    }

    void Enqueue(Track track)
    {
        Debug.Log("Enqueue " + track.name);
        queue.Enqueue(track);
    }

    Track Dequeue()
    {
        var t = queue.Dequeue();
        Debug.Log("Dequeue " + t.name);
        return t;
    }
}