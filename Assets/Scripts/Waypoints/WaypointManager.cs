using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public Dictionary<int,Waypoint> waypoints;
    public Waypoint currentWaypoint;
    static WaypointManager instance;
    public static WaypointManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GameObject("Waypoint Manager", typeof(WaypointManager)).GetComponent<WaypointManager>();
                instance.waypoints = new Dictionary<int, Waypoint>();
            }
            return instance;
        }
    }
}