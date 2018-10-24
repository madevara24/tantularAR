using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetector : MonoBehaviour {
    private bool tap, swipeUp, swipeDown, swipeLeft, swipeRight, pinchIn, pinchOut;
    private Vector2 startTouch, swipeDelta;
    private Vector2 touchOneStartPos, touchZeroStartPos;
    private float pinchStartMag, pinchCurrentMag, pinchDeltaMag;
    private bool isDraging = false;
    private bool isPinching = false;

    public Vector2 SwipeDelta { get { return swipeDelta; } }
    public float PinchDeltaMag { get { return pinchDeltaMag; } }
    public float PinchStartMag { get { return pinchStartMag; } }
    public float PinchCurrentMag { get { return pinchCurrentMag; } }
    public bool SwipeUp { get { return swipeUp; } }
    public bool SwipeDown { get { return swipeDown; } }
    public bool SwipeLeft { get { return swipeLeft; } }
    public bool SwipeRight { get { return swipeRight; } }
    public bool Tap { get { return tap; } }
    public bool PinchIn { get { return pinchIn; } }
    public bool PinchOut { get { return pinchOut; } }
    public bool IsDraging { get { return isDraging; } }
    public bool IsPinching { get { return isPinching; } }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        tap = swipeUp = swipeDown = swipeRight = swipeLeft = pinchIn = pinchOut = false;

        //GET INPUT
        if (Input.touches.Length == 1) {
            if (Input.touches[0].phase == TouchPhase.Began) {
                isDraging = true;
                tap = true;
                startTouch = Input.touches[0].position;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled) {
                isDraging = false;
                ResetSwipe();
            }
        }else if(Input.touches.Length >  1) {
            if(Input.touches[0].phase == TouchPhase.Began && Input.touches[1].phase == TouchPhase.Began) {
                isPinching = true;
                touchZeroStartPos = Input.touches[0].position - Input.touches[0].deltaPosition;
                touchOneStartPos = Input.touches[1].position - Input.touches[1].deltaPosition;
            }
            else if(Input.touches[0].phase == TouchPhase.Ended || Input.touches[1].phase == TouchPhase.Ended || 
                Input.touches[0].phase == TouchPhase.Canceled || Input.touches[1].phase == TouchPhase.Canceled) {
                isPinching = false;
                ResetPinch();
            }
        }

        //CALCULATE PINCH DISTANCE
        if (isPinching) {
            if(Input.touches.Length == 2) {
                pinchStartMag = (touchZeroStartPos - touchOneStartPos).magnitude;
                pinchCurrentMag = (Input.touches[0].position - Input.touches[1].position).magnitude;
                pinchDeltaMag = pinchCurrentMag / pinchStartMag;
            }
        }

        //CALCULATE SWIPE DISTANCE
        swipeDelta = Vector2.zero;
        if (isDraging) {
            if (Input.touches.Length == 1)
                swipeDelta = Input.touches[0].position - startTouch;
        }

        //CROSS THE ZONE
        if (swipeDelta.magnitude > 10) {
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if(Mathf.Abs(x) > Mathf.Abs(y)) {
                if (x < 0)
                    swipeLeft = true;
                else
                    swipeRight = true;
            }
            else {
                if (y < 0)
                    swipeDown = true;
                else
                    swipeUp = true;
            }
            //LEFT OR RIGHT
            
        }
    }

    public void ResetSwipe() {
        startTouch = swipeDelta = Vector2.zero;
        isDraging = false;
    }

    public void ResetPinch() {
        pinchDeltaMag = 0f;
        isPinching = false;
    }
}
