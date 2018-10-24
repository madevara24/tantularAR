using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour {
    [SerializeField] List<Sprite> listOfLoadingSprite;
    [SerializeField] Image imgLoad;
    private int currentSpriteId = 0;
    // Use this for initialization
    void Start () {
        StartCoroutine(initLoading());
        StartCoroutine(initMainMenu());
    }
    private IEnumerator initLoading() {
        yield return new WaitForSeconds(0.5f);
        changeLoadSprite();
        StartCoroutine(initLoading());
    }

    private IEnumerator initMainMenu() {
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene("Main Menu");
    }
    private void changeLoadSprite() {
        currentSpriteId++;
        if (currentSpriteId >= listOfLoadingSprite.Count)
            currentSpriteId = 0;
        imgLoad.GetComponent<Image>().sprite = listOfLoadingSprite[currentSpriteId];
    }
}
