using UnityEngine;
using Vuforia;
using System.Collections;

public class ImageTracking : MonoBehaviour, ITrackableEventHandler {
	
	public GameObject childObject; 
    private TrackableBehaviour mTrackableBehaviour;
	private bool state = false;
	
	void Start () {
		mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
	}

    void Update() 
    {
        childObject.SetActive(state);
    }
	
	public void OnTrackableStateChanged(
                                    TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED)
        {
            state = true;
        }
        else
        {
            state = false;
        }
    }
	
}