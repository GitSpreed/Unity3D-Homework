using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UerAction : MonoBehaviour {

    CCSceneControlFirst control;
    ScoreRecorder recorder;
	// Use this for initialization
	void Start () {
        control = SingleTon<CCSceneControlFirst>.Instance;
        recorder = SingleTon<ScoreRecorder>.Instance;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {
        GUIStyle myStyle = new GUIStyle
        {
            fontSize = 25,
            fontStyle = FontStyle.Bold
        };
        GUI.Label(new Rect(500, 0, 400, 400), "Round " + control.getRound().ToString(),  myStyle);
        GUI.Label(new Rect(900, 0, 400, 400), "Score: " + recorder.getScore().ToString(), myStyle);
        if (control.state == GameState.start)
        {
            if (GUI.Button(new Rect(500, 100, 90, 60), "Start"))
            {
                control.state = GameState.running;
            }
        } else if(control.state == GameState.running) { 
        
            if (Input.GetButtonDown("Fire1"))
            {
                Vector3 position = Input.mousePosition;
                control.hit(position);
            }
            if (GUI.Button(new Rect(500, 100, 90, 60), "Stop"))
            {
                control.state = GameState.gameover;
            }
        } else {
            if (GUI.Button(new Rect(500, 100, 90, 60), "Reset"))
            {
                //GUI.Label(new Rect(500, 0, 400, 400), "Your score is " + recorder.getScore().ToString(), myStyle);
                control.Reset();
            }
        }
    }
}
