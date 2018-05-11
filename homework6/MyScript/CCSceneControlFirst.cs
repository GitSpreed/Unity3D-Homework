using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { start, running, gameover}

public class CCSceneControlFirst : MonoBehaviour, SceneControl {


    public IActionManager manager;
    CharacterFactory factory;
    ScoreRecorder recorder;

    public GameState state;

    protected CCSceneControlFirst() { }

    // Use this for initialization
    void Awake() {
        manager = SingleTon<CCActionManager>.Instance;
        factory = SingleTon<CharacterFactory>.Instance;
        recorder = SingleTon<ScoreRecorder>.Instance;
        Director.getInstance().currentSceneCtrol = this;
        GameState state = GameState.start;
    }
	
	// Update is called once per frame
	void Update () {
        if (state == GameState.running)
        {
            //Debug.Log(intervalTime);
            if (diskQueue.Count == 0)
            {
                if (round == 4)
                {
                    state = GameState.gameover;
                }
                else
                {
                    NextRound();
                }
            }
            if (intervalTime <= 0)
            {
                ThrowDisk();
                intervalTime = UnityEngine.Random.Range(0f, 4f);
            }
            else
            {
                intervalTime -= Time.deltaTime;
            }
        }
		

	}

    private void NextRound()
    {
        round++;
        for(int i = 0; i < 10; i++)
        {
            diskQueue.Enqueue(factory.GetDisk(round));
        }
    }

    private void ThrowDisk()
    {
        manager.ThrowDisk(diskQueue.Dequeue());
    }

    public void hit (Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            DiskData data = hit.collider.gameObject.GetComponent<DiskData>();

            if (data)
            {
                recorder.Record(hit.collider.gameObject);
            }
            hit.collider.gameObject.transform.position = new Vector3(0, -5, 0);
        }
    }

    public void Reset()
    {
        state = GameState.start;
        round = 0;
        intervalTime = 0.5f;
        diskQueue.Clear();
        manager.Reset();
        factory.Reset();
        recorder.Reset();
    }

    public int getRound()
    {
        return round;
    }

    public void LoadResource()
    {
       
    }
}
