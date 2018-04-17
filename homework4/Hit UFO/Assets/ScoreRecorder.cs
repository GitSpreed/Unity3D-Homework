using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreRecorder : MonoBehaviour {

    private int score;

    private Dictionary<Color, int> scoreTable = new Dictionary<Color, int>();

    protected ScoreRecorder() { }

    // Use this for initialization  
    void Start()
    {
        score = 0;
        scoreTable.Add(Color.red, 1);
        scoreTable.Add(Color.yellow, 2);
        scoreTable.Add(Color.blue, 3);
    }

    public void Record(GameObject disk)
    {
        score += scoreTable[disk.GetComponent<DiskData>().color];
    }

    public int getScore()
    {
        return score;
    }
   
    public void Reset()
    {
        score = 0;
    }
}
