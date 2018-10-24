using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectBehaviour : MonoBehaviour {
    [SerializeField] Vector3 spawnPosition = Vector3.zero;
    public Vector3 getSpawnPosition() {
        return spawnPosition;
    }
    [SerializeField] Vector3 spawnRotation = new Vector3(0, 180, 0);
    public Vector3 getSpawnRotation() {
        return spawnRotation;
    }
    [SerializeField] bool isGlobe;
    public bool IsGlobe { get { return isGlobe; } }
    [SerializeField] bool isVideo;
    public bool IsVideo{ get { return isVideo; } }
    [SerializeField] List<string> listOfObjectInformation;
    public string getObjectInformation(int _id) {
        return listOfObjectInformation[_id];
    }
    public int getInformationLength() {
        return listOfObjectInformation.Count;
    }
    [SerializeField] List<InformationBehaviour> listOfObjectInformationBehaviour;
    public InformationBehaviour getObjectInformationImage(int _id) {
        return listOfObjectInformationBehaviour[_id];
    }
    public int getObjectInformationBehaviourType(int _id) {
        return listOfObjectInformationBehaviour[_id].getInformationType();
    }
    public Sprite getObjectInformationBehaviourSprite(int _id) {
        return listOfObjectInformationBehaviour[_id].getInformatonSprite();
    }
    public int getInformationBehaviourLength() {
        return listOfObjectInformationBehaviour.Count;
    }
    private Animator animator;
    private int triggerAttack = Animator.StringToHash("triggerAttack");
    public void playAnimation() {
        animator.SetTrigger(triggerAttack);
    }

    private void Start() {
        animator = GetComponent<Animator>();
    }
}
