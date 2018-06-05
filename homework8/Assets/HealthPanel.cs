using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPanel : MonoBehaviour {

    public float healthPanelOffset = 0.35f;
    public GUISkin mySkin;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        GUI.skin = mySkin;
        Vector3 worldPos = new Vector3(transform.position.x, transform.position.y + healthPanelOffset, transform.position.z);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        GUI.HorizontalSlider(new Rect(screenPos.x - 50, screenPos.y - 30, 100, 100), 50, 0, 100);
    }
}
