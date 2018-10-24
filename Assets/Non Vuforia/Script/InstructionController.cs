using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InstructionController : MonoBehaviour {
    [SerializeField] Button btnOk;
    [SerializeField] List<Image> listOfInstructionImage;
    private SwipeDetector swipeDetector;
    private int shownInstructionId = 0;
    private bool swipeLock = false;
    private void Start() {
        swipeDetector = GetComponent<SwipeDetector>();
        setInstructionImage();
    }
    private void Update() {
        if (shownInstructionId == 2)
            btnOk.gameObject.SetActive(true);
        else
            btnOk.gameObject.SetActive(false);

        if(swipeDetector.SwipeLeft && !swipeLock && shownInstructionId < 2) {
            swipeLock = true;
            shownInstructionId++;
            setInstructionImage();
        }
        if(swipeDetector.SwipeRight && !swipeLock && shownInstructionId > 0) {
            swipeLock = true;
            shownInstructionId--;
            setInstructionImage();
        }
        if (!swipeDetector.IsDraging)
            swipeLock = false;
    }
    private void setInstructionImage() {
        for (int i = 0; i < 3; i++) {
            if(i==shownInstructionId)
                listOfInstructionImage[i].gameObject.SetActive(true);
            else
                listOfInstructionImage[i].gameObject.SetActive(false);
        }
    }
    public void onPressOkButton() {
        SceneManager.LoadScene("AR Camera");
    }
    public void onPressBackButton() {
        SceneManager.LoadScene("Main Menu");
    }
}
