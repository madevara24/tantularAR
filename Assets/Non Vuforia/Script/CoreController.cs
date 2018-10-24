using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using Vuforia;

public class CoreController : MonoBehaviour {
    [SerializeField] List<GameObject> listOfSuryaStambhaTargetObject;
    [SerializeField] List<GameObject> listOfSimphonionTargetObject;
    [SerializeField] List<GameObject> listOfGarudeyaTargetObject;
    [SerializeField] List<GameObject> listOfTargetObject;

    [SerializeField] List<GameObject> listOfPhotoObjects;
    [SerializeField] List<GameObject> listOfGarudeyaPhotoObjects;
    [SerializeField] List<GameObject> listOfSuryaStambhaPhotoObject;
    [SerializeField] List<GameObject> listOfSimphonionPhotoObject;

    [SerializeField] GameObject canvasMain;
    [SerializeField] GameObject canvasPhoto;

    [SerializeField] Button btnInstruction;
    [SerializeField] Button btnAbout;
    [SerializeField] Button btnInformation;
    [SerializeField] Button btnCloseInformation;
    [SerializeField] Button btnSound;
    [SerializeField] Button btnMenu;

    [SerializeField] UnityEngine.UI.Image imgAbout;
    [SerializeField] UnityEngine.UI.Image imgObjectInformationBackground;
    [SerializeField] UnityEngine.UI.Image imgObjectInformationDescription;
    [SerializeField] UnityEngine.UI.Image imgObjectInformationDimension;
    [SerializeField] UnityEngine.UI.Image imgObjectInformationLocation;
    [SerializeField] Text txtObjectInformation;

    [SerializeField] Sprite menuOpen;
    [SerializeField] Sprite menuClose;

    [SerializeField] Text txtDebug;
    private void trace(string _message = "") {
        txtDebug.text = _message;
    }

    private GameObject displayedObjectPrefab = null;
    private GameObject extraObjectPrefab = null;
    [SerializeField] List<GameObject> listOfDisplayedPhotoObjects;
    private Vector3 objectPositionOffset = new Vector3(0, -2f, 12f);
    private Vector3 garudeyaSiwaPositionOffset = new Vector3(-12.2f, -14.5f, 19f);
    private Vector3 garudeyaGarudaPositionOffset = new Vector3(-12.2f, -10.9f, 19f);
    private Vector3 garudeyaRaksasaPositionOffset = new Vector3(-12.2f, 0, 19f);

    #region Var Image Name
    private string currentTargetImageName;
    public void setCurrentTargetImageName(string _name) {
        if (_name == "")
            currentTargetImageName = null;
        else
            currentTargetImageName = _name;
        Debug.Log("Target Image Name = " + currentTargetImageName);
    }
    public string getCurrentTargetImageName() {
        return currentTargetImageName;
    }
    #endregion
    #region Var Current Object ID
    private int currentObjectId;
    public void setCurrentObjectId(int _id = 0) {
        currentObjectId = _id;
    }
    public int getCurrentObjectId() {
        return currentObjectId;
    }
    #endregion
    #region Var Current Object Information ID
    private int currentObjectInformationId;
    public void setCurrentObjectInformationId(int _id = 0){
        currentObjectInformationId = _id;
    }
    public int getCurrentObjectInformationId() {
        return currentObjectInformationId;
    }
    #endregion

    private AnalogController analogController;
    private SwipeDetector swipeDetector;
    private ScreenShotShare screenShotShare;

    private string displayedInformation;
    private bool showingInformation = false;
    private bool photoMode = false;

    private const string ENUM_IMAGE_TARGET_GARUDEYA = "imageTarget_garudeya";
    private const string ENUM_IMAGE_TARGET_SIMPHONION = "imageTarget_simphonion";
    private const string ENUM_IMAGE_TARGET_SURYA_STAMBHA = "imageTarget_suryaStambha";
    
	// Use this for initialization
	void Start () {
        swipeDetector = GetComponent<SwipeDetector>();
        screenShotShare = GetComponent<ScreenShotShare>();
        canvasPhoto.gameObject.SetActive(false);
        displayedObjectPrefab = null;
        photoMode = false;
        posAnchor = objectPositionOffset;
        //imgObjectInformationDescription.GetComponent<UnityEngine.UI.Image>().sprite = info.getInformatonSprite();
    }
	
	// Update is called once per frame
	void Update () {
        if (displayedObjectPrefab != null && !displayedObjectPrefab.GetComponent<ObjectBehaviour>().IsVideo && !displayedObjectPrefab.GetComponent<ObjectBehaviour>().IsGlobe) {
            checkSwipe();
            checkPinch();
        }
        if (photoMode) {
            updateMovement();
        }
	}
    #region Analog Control
    private int xTrans = 0, yTrans = 0;
    private float xVel = 0, yVel = 0;
    private void updateMovement() {
        for (int i = 0; i < listOfDisplayedPhotoObjects.Count; i++) {
            listOfDisplayedPhotoObjects[i].transform.Translate(updateVelocity(), Space.World);
        }
    }
    private Vector3 updateVelocity() {
        analogController = GameObject.Find("PhotoAnalog").GetComponent<AnalogController>();
        Vector3 velocity = Vector3.zero;
        velocity.x = analogController.getAxisHorizontal() / 10;
        velocity.y = analogController.getAxisVertical() / 10;
        if (velocity.magnitude > 1)
            velocity.Normalize();
        return velocity;
    }
    #endregion
    #region Swipe Input
    private float yDegree, startDegree = 0, endDegree, startScale, endScale;
    private bool swipeLocked = false, scaleLocked = false;
    private Vector3 posAnchor, scaleAnchor;
    private void checkSwipe() {
        if (swipeDetector.SwipeLeft) {
            yDegree = swipeDetector.SwipeDelta.magnitude / 10;
            if (!swipeLocked) {
                swipeLocked = true;
                if (showingInformation)
                    swipeInformation(true);
            }
            if (!showingInformation)
                swipeObject();
        }

        if (swipeDetector.SwipeRight) {
            yDegree = -swipeDetector.SwipeDelta.magnitude / 10;
            if (!swipeLocked) {
                swipeLocked = true;
                if (showingInformation)
                    swipeInformation(true);
            }
            if (!showingInformation)
                swipeObject();
        }

        if (!swipeDetector.IsDraging && swipeLocked) {
            swipeLocked = false;
        }
    }
    private void checkPinch() {
        if (!swipeDetector.IsPinching) {
            if (!scaleLocked) {
                startScale = endScale;
                scaleLocked = true;
            }
        }
        else {
            if ((swipeDetector.PinchDeltaMag * displayedObjectPrefab.transform.localScale.x) >= scaleAnchor.x &&
                (swipeDetector.PinchDeltaMag * displayedObjectPrefab.transform.localScale.x) <= (scaleAnchor.x * 2)) {
                displayedObjectPrefab.transform.localScale = new Vector3(displayedObjectPrefab.transform.localScale.x * swipeDetector.PinchDeltaMag,
                                                    displayedObjectPrefab.transform.localScale.y * swipeDetector.PinchDeltaMag,
                                                    displayedObjectPrefab.transform.localScale.z * swipeDetector.PinchDeltaMag);
            }
        }
    }
    private void swipeObject() {
        if (!photoMode) {
            endDegree = yDegree + startDegree;
            displayedObjectPrefab.transform.rotation = Quaternion.Euler(0, 180 + endDegree, 0);
            //displayedObjectPrefab.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }
    private void swipeInformation(bool _next) {
        if (!swipeLocked &&  _next && currentObjectInformationId < displayedObjectPrefab.GetComponent<ObjectBehaviour>().getInformationBehaviourLength() - 1)
            currentObjectInformationId++;
        else if (!swipeLocked && !_next && currentObjectInformationId > 0)
                currentObjectInformationId--;
        setObjectInformationImage();
    }
    #endregion
    #region Target Images Controller
    public void onDetectTargetImage(string _name = "") {
        if (!photoMode) {
            Debug.Log("Target Image " + _name + " Detected");
            btnInformation.gameObject.SetActive(true);
            setCurrentTargetImageName(_name);
            initTargetObjectDatabase();
            setCurrentObjectId();
            initTargetObject();
            setCurrentObjectInformationId();
            setObjectInformationDisplay(false);
        }
    }
    public void onLostTargetImage() {
        Debug.Log("Target Image Lost");
        btnInformation.gameObject.SetActive(false);
        setObjectInformationDisplay(false);
    }
    #endregion
    #region Object Information
    private void setObjectInformationDisplay(bool _active) {
        if (displayedObjectPrefab.GetComponent<ObjectBehaviour>().IsGlobe) {
            btnCloseInformation.gameObject.SetActive(false);
            btnSound.gameObject.SetActive(false);
        }
        else {
            btnCloseInformation.gameObject.SetActive(_active);
            btnSound.gameObject.SetActive(_active);
        }
        if (_active)
            chooseObjectInformationImageDisplay();
        else
            closeAllObjectInformationImage();
        btnInformation.gameObject.SetActive(!_active);
        //imgObjectInformationBackground.gameObject.SetActive(_active);
        //txtObjectInformation.gameObject.SetActive(_active);
    }
    private void setDisplayInformationText() {
        txtObjectInformation.text = displayedObjectPrefab.GetComponent<ObjectBehaviour>().getObjectInformation(currentObjectInformationId);
    }
    private void setObjectInformationImage() {
        Sprite objectInfo = displayedObjectPrefab.GetComponent<ObjectBehaviour>().getObjectInformationBehaviourSprite(currentObjectInformationId);
        switch (displayedObjectPrefab.GetComponent<ObjectBehaviour>().getObjectInformationBehaviourType(currentObjectInformationId)) {
            case InformationBehaviour.ENUM_INFORMATION_DESCRIPTION:
                imgObjectInformationDescription.GetComponent<UnityEngine.UI.Image>().sprite = objectInfo;
                break;
            case InformationBehaviour.ENUM_INFORMATION_DIMENSION:
                imgObjectInformationDimension.GetComponent<UnityEngine.UI.Image>().sprite = objectInfo;
                break;
            case InformationBehaviour.ENUM_INFORMATION_LOCATION:
                imgObjectInformationLocation.GetComponent<UnityEngine.UI.Image>().sprite = objectInfo;
                showingInformation = true;
                break;
            default:
                break;
        }
        setObjectInformationDisplay(showingInformation);
    }
    private void closeAllObjectInformationImage() {
        imgObjectInformationDescription.gameObject.SetActive(false);
        imgObjectInformationDimension.gameObject.SetActive(false);
        imgObjectInformationLocation.gameObject.SetActive(false);
    }
    private void chooseObjectInformationImageDisplay() {
        switch (displayedObjectPrefab.GetComponent<ObjectBehaviour>().getObjectInformationBehaviourType(currentObjectInformationId)) {
            case InformationBehaviour.ENUM_INFORMATION_DESCRIPTION:
                imgObjectInformationDescription.gameObject.SetActive(true);
                imgObjectInformationDimension.gameObject.SetActive(false);
                imgObjectInformationLocation.gameObject.SetActive(false);
                break;
            case InformationBehaviour.ENUM_INFORMATION_DIMENSION:
                imgObjectInformationDescription.gameObject.SetActive(false);
                imgObjectInformationDimension.gameObject.SetActive(true);
                imgObjectInformationLocation.gameObject.SetActive(false);
                break;
            case InformationBehaviour.ENUM_INFORMATION_LOCATION:
                imgObjectInformationDescription.gameObject.SetActive(false);
                imgObjectInformationDimension.gameObject.SetActive(false);
                imgObjectInformationLocation.gameObject.SetActive(true);
                break;
            default:
                closeAllObjectInformationImage();
                break;
        }
    }
    #endregion
    #region Target Object Controller
    private void initTargetObjectDatabase() {
        listOfTargetObject = null;
        listOfPhotoObjects = null;
        switch (currentTargetImageName) {
            case ENUM_IMAGE_TARGET_SURYA_STAMBHA:
                listOfTargetObject = listOfSuryaStambhaTargetObject;
                listOfPhotoObjects = listOfSuryaStambhaPhotoObject;
                Debug.Log("Loading database Surya Stambha");
                break;
            case ENUM_IMAGE_TARGET_SIMPHONION:
                listOfTargetObject = listOfSimphonionTargetObject;
                listOfPhotoObjects = listOfSimphonionPhotoObject;
                Debug.Log("Loading database Simphonion");
                break;
            case ENUM_IMAGE_TARGET_GARUDEYA:
                listOfTargetObject = listOfGarudeyaTargetObject;
                listOfPhotoObjects = listOfGarudeyaPhotoObjects;
                Debug.Log("Loading database Garudeya");
                break;
            default:
                break;
        }
    }
    private void initTargetObject() {
        Debug.Log("Init object number " + currentObjectId);
        attachTargetObject(currentObjectId);
        //setDisplayInformationText();
        setObjectInformationImage();
    }
    private void attachTargetObject(int _id) {
        DestroyImmediate(displayedObjectPrefab);
        DestroyImmediate(extraObjectPrefab);
        Vector3 position = listOfTargetObject[_id].GetComponent<ObjectBehaviour>().getSpawnPosition();
        Vector3 rotation = new Vector3(0, 180, 0);
        if (listOfTargetObject[_id].GetComponent<ObjectBehaviour>().IsGlobe)
            rotation = listOfTargetObject[_id].GetComponent<ObjectBehaviour>().getSpawnRotation();
        if (position == Vector3.zero)
            position = objectPositionOffset;
        if (!listOfTargetObject[_id].GetComponent<ObjectBehaviour>().IsVideo) {
            if (currentTargetImageName == ENUM_IMAGE_TARGET_GARUDEYA) {
                switch (currentObjectId) {
                    case 3:
                        extraObjectPrefab = Instantiate(listOfTargetObject[0], garudeyaSiwaPositionOffset, Quaternion.Euler(rotation));
                        extraObjectPrefab.transform.localScale = new Vector3(5, 5, 5);
                        break;
                    case 4:
                        extraObjectPrefab = Instantiate(listOfTargetObject[0], garudeyaGarudaPositionOffset, Quaternion.Euler(rotation));
                        extraObjectPrefab.transform.localScale = new Vector3(5, 5, 5);
                        break;
                    case 5:
                        extraObjectPrefab = Instantiate(listOfTargetObject[0], garudeyaRaksasaPositionOffset, Quaternion.Euler(rotation));
                        extraObjectPrefab.transform.localScale = new Vector3(5, 5, 5);
                        break;
                }
            }
            displayedObjectPrefab = Instantiate(listOfTargetObject[_id], position, Quaternion.Euler(rotation));
        }
        else {
            displayedObjectPrefab = Instantiate(listOfTargetObject[_id], Vector3.zero, Quaternion.Euler(0, 0, 0));
            displayedObjectPrefab.GetComponent<VideoPlayer>().targetCamera = GameObject.Find("ARCamera").GetComponent<Camera>();
            //displayedObjectPrefab.GetComponent<VideoPlayer>().SetTargetAudioSource();
            displayedObjectPrefab.GetComponent<VideoPlayer>().Play();
        }
        //displayedObjectPrefab.
        //displayedObjectPrefab.transform.rotation = Quaternion.Euler(0, 180, 0);
        scaleAnchor = displayedObjectPrefab.transform.localScale;
        startScale = scaleAnchor.x;
        startDegree = displayedObjectPrefab.transform.rotation.y;
    }
    #endregion

    #region Button Functions
    //BUTTON FUNCTIONS #################################################################################################################
    public void onPressMenuButton() {
        btnInstruction.gameObject.SetActive(!btnInstruction.gameObject.activeSelf);
        btnAbout.gameObject.SetActive(!btnAbout.gameObject.activeSelf);
        if (btnAbout.gameObject.activeSelf)
            btnMenu.GetComponent<UnityEngine.UI.Image>().sprite = menuOpen;
        else
            btnMenu.GetComponent<UnityEngine.UI.Image>().sprite = menuClose;
    }

    public void onPressAboutButton() {
        imgAbout.gameObject.SetActive(!imgAbout.gameObject.activeSelf);
        //displayedObjectPrefab.GetComponent<VideoPlayer>().Play();
        //onDetectTargetImage("vuforia_tarmac");
        /*displayedObjectPrefab = Instantiate(testPrefab, objectPositionOffset, Quaternion.Euler(0, 180, 0));
        Sprite objectInfo = displayedObjectPrefab.GetComponent<ObjectBehaviour>().getObjectInformationBehaviourSprite(currentObjectInformationId);
        switch (displayedObjectPrefab.GetComponent<ObjectBehaviour>().getObjectInformationBehaviourType(currentObjectInformationId)) {
            case InformationBehaviour.ENUM_INFORMATION_DESCRIPTION:
                imgObjectInformationDescription.GetComponent<UnityEngine.UI.Image>().sprite = objectInfo;
                break;
            case InformationBehaviour.ENUM_INFORMATION_DIMENSION:
                imgObjectInformationDimension.GetComponent<UnityEngine.UI.Image>().sprite = objectInfo;
                break;
            case InformationBehaviour.ENUM_INFORMATION_LOCATION:
                imgObjectInformationLocation.GetComponent<UnityEngine.UI.Image>().sprite = objectInfo;
                break;
            default:
                break;
        }*/
    }

    public void onPressInstructionButton() {
        SceneManager.LoadScene("Instruction");
    }

    public void onPressInformationButton() {
        setObjectInformationDisplay(true);
        showingInformation = true;
    }

    public void onPressCloseInformationButton() {
        setObjectInformationDisplay(false);
        showingInformation = false;
    }

    public void onPressSoundButton() {

    }

    public void onPressPhotoButton() {
        DestroyImmediate(displayedObjectPrefab);
        DestroyImmediate(extraObjectPrefab);
        photoMode = true;
        canvasPhoto.SetActive(true);
        canvasMain.SetActive(false);
        Debug.Log(listOfPhotoObjects.Count);
        for (int i = 0; i < listOfPhotoObjects.Count; i++) {
            displayedObjectPrefab = Instantiate(listOfPhotoObjects[i], listOfPhotoObjects[i].GetComponent<ObjectBehaviour>().getSpawnPosition(), Quaternion.Euler(0, 180, 0));
            if (listOfPhotoObjects.Count == 3 && i == 0)
                displayedObjectPrefab.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            if (listOfPhotoObjects.Count == 3 && i == 1)
                displayedObjectPrefab.transform.rotation = Quaternion.Euler(new Vector3(0, 163, 0));
            if (i == 2) {
                displayedObjectPrefab.transform.rotation = Quaternion.Euler(new Vector3(0, 206, 0));
                displayedObjectPrefab.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            }
            listOfDisplayedPhotoObjects.Add(displayedObjectPrefab);
            Debug.Log(listOfDisplayedPhotoObjects.Count);
        }
    }

    public void onPressReplayButton() {
        displayedObjectPrefab.GetComponent<ObjectBehaviour>().playAnimation();
    }

    public void onPressBackButton() {
        SceneManager.LoadScene("Main Menu");
    }

    public void onPressNextObjectButton() {
        showingInformation = false;
        if (currentObjectId < listOfTargetObject.Count - 1) {
            currentObjectId++;
            initTargetObject();
        }
    }

    public void onPressPrevObjectButton() {
        showingInformation = false;
        if (currentObjectId > 0) {
            currentObjectId--;
            initTargetObject();
        }
    }

    public void onPressSwapCameraButton() {
        swapCamera();
        //onPressPhotoButton();
    }

    public void onPressCaptureButton() {
        screenShotShare.Share();
    }

    public void onPressClosePhotoModeButton() {
        photoMode = false;
        restartCamera(CameraDevice.CameraDirection.CAMERA_BACK);
        for (int i = 0; i < listOfDisplayedPhotoObjects.Count; i++) {
            DestroyImmediate(listOfDisplayedPhotoObjects[i]);
        }
        canvasMain.SetActive(true);
        canvasPhoto.SetActive(false);
        initTargetObject();
    }
    //BUTTON FUNCTIONS #################################################################################################################
    #endregion
    #region Swap Camera
    private void swapCamera() {
        if (cameraFacingBack())
            restartCamera(CameraDevice.CameraDirection.CAMERA_FRONT);
        else
            restartCamera(CameraDevice.CameraDirection.CAMERA_BACK);

    }
    private bool cameraFacingBack() {
        if (CameraDevice.Instance.GetCameraDirection() == CameraDevice.CameraDirection.CAMERA_BACK)
            return true;
        else
            return false;
    }
    private void restartCamera(CameraDevice.CameraDirection _newDirection) {
        CameraDevice.Instance.Stop();
        CameraDevice.Instance.Deinit();
        CameraDevice.Instance.Init(_newDirection);
        CameraDevice.Instance.Start();
    }
    #endregion
}
