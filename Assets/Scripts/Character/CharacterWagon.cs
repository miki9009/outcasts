using UnityEngine;


public class CharacterWagon : CharacterMovementPlayer, ILocalPlayer
{
    public Track track;
    public Track nextTrack;
    public Track previousTrack;
 
    public float curTrackPos = 0;

    public float wagonSpeed = 1;

    public override bool IsPlayer
    {
        get
        {
            return true;
        }
    }

    protected override void Initialize()
    {
        Controller.Instance.gameCamera.SetTarget(transform);
        Controller.Instance.gameCamera.ResetView();
    }

    protected override void Inputs()
    {

    }

    protected override void Rotation()
    {

    }

    protected override void FixedUpdate()
    {
        curTrackPos += Time.deltaTime * wagonSpeed;
        if (curTrackPos >= 1 && nextTrack != null)
        {
            previousTrack = track;
            track = nextTrack;
            curTrackPos = curTrackPos % 1;
        }
        if (track != null)
        {
            transform.position = track.GetPosition(curTrackPos);
            transform.rotation = track.GetRotation(curTrackPos);
        }
        else
        {
            Debug.Log("track was null");
        }


    }

    protected override void Update()
    {

    }
}