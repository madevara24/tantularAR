using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class ImageTargetBehaviour : MonoBehaviour, ITrackableEventHandler {
    [SerializeField] Text txtDebug;
    private void trace(string _message = "") {
        txtDebug.text = _message;
    }
    private TrackableBehaviour mTrackableBehaviour;

    private void Start() {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour) {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }
    //TRACKABLE DISPATCH EVENT #########################################################################################################
    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus) {
        if (newStatus == TrackableBehaviour.Status.DETECTED || newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) {
            GameObject.Find("coreController").GetComponent<CoreController>().onDetectTargetImage(mTrackableBehaviour.TrackableName);
        }
        else if (newStatus == TrackableBehaviour.Status.NO_POSE) {
            //GameObject.Find("coreController").GetComponent<CoreController>().setCurrentTargetImageName("");
        }
    }
}
