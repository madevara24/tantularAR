using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Vuforia;

public class AnalogController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

    private UnityEngine.UI.Image bgImg;
    private UnityEngine.UI.Image joystickImg;
    public Vector3 inputVector;
    float angle, degrees;

    void Start() {
        bgImg = GetComponent<UnityEngine.UI.Image>();
        joystickImg = transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        inputVector = Vector3.zero;
        angle = degrees = 0;
    }
    public void OnDrag(PointerEventData _ped) {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            bgImg.rectTransform, _ped.position, _ped.pressEventCamera, out pos)) {
            pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);

            float x = (bgImg.rectTransform.pivot.x == 1) ? pos.x * 2 + 1 : pos.x * 2 - 1;
            float y = (bgImg.rectTransform.pivot.y == 1) ? pos.y * 2 + 1 : pos.y * 2 - 1;

            inputVector = new Vector3(x, 0, y);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
            angle = Mathf.Atan2(x, y);
            degrees = angle * 180 / Mathf.PI;

            Debug.Log(inputVector + " | " + angle + " | " + degrees);

            joystickImg.rectTransform.anchoredPosition = new Vector3(
                inputVector.x * (bgImg.rectTransform.sizeDelta.x / 3),
                inputVector.z * (bgImg.rectTransform.sizeDelta.y / 3));
        }
    }

    public void OnPointerDown(PointerEventData _ped) {
        OnDrag(_ped);
    }

    public void OnPointerUp(PointerEventData _ped) {
        inputVector = Vector3.zero;
        joystickImg.rectTransform.anchoredPosition = Vector3.zero;
    }

    public float getAxisHorizontal() {
        return inputVector.x;
    }

    public float getAxisVertical() {
        return inputVector.z;
    }

    public float getRotationDegree() {
        return degrees;
    }
}
