using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationBehaviour : MonoBehaviour {

    [SerializeField] int informationType;
    public int getInformationType() {
        return informationType;
    }
    public const int ENUM_INFORMATION_DESCRIPTION = 0;
    public const int ENUM_INFORMATION_DIMENSION = 1;
    public const int ENUM_INFORMATION_LOCATION = 2;
    [SerializeField] Sprite informationSprite;
    public Sprite getInformatonSprite() {
        return informationSprite;
    }
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
