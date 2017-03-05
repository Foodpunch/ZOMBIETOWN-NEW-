using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CopyAlphaScript : MonoBehaviour {

    [SerializeField] Image parentImage;
    Image currImage;

	// Use this for initialization
	void Start () {
        parentImage = parentImage.GetComponent<Image>();
        currImage = GetComponent<Image>();
	
	}
	
	// Update is called once per frame
	void Update () {
        currImage.color = new Color(currImage.color.r, currImage.color.g, currImage.color.b, parentImage.color.a);
	
	}
}
