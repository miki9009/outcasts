using UnityEngine;

public class Track : MonoBehaviour
{
    public BezierCurve curve;
    public Transform startAnchor;
    public Transform endAnchor;
    public bool raiseEvent = true;
    public BoxCollider Trigger { get; set; }

    private void Awake()
    {
        Trigger = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.gameObject.layer == Layers.Character)
        {
            var characterWagon = other.transform.parent.GetComponent<CharacterWagon>();
            if(characterWagon != null && characterWagon.previousTrack!=this)
            {
                characterWagon.nextTrack = this;
                characterWagon.endPointForward = endAnchor.forward;
                if (characterWagon.track == null)
                {
                    characterWagon.track = this;
                }
                OnTrackReached();              
            }
        }
    }

    public static event System.Action<Track> TrackReached;
    void OnTrackReached()
    {
        if(raiseEvent)
            TrackReached?.Invoke(this);
    }

    public Vector3 GetPosition(float pos)
    {
        return curve.GetPointAt(pos);
    }

    public Quaternion GetRotation(float pos)
    {
        return Quaternion.LookRotation(curve.GetDirection(pos));
    }
}