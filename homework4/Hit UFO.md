# Hit UFO

1. 首先建立一个飞碟的预设，存在Resources/Prefabs目录下，飞碟预设属性如下：

   ![](C:\Users\spreed\Desktop\飞碟.png)

2. 设置摄像机属性，如下图：

   ![像](C:\Users\spreed\Desktop\摄像机.png)

3. 设计并实现如下类：

* Director   导演类，控制场景的转换

  ```c#
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  public class Director : System.Object {
      private static Director director;
      public SceneControl currentSceneCtrol { get; set; }
      
      private Director() { }

      public static Director getInstance()
      {
          if (director == null)
          {
              director = new Director();
          }
          return director;
      }
  }

  ```

* SceneControl   场景基类，包含所有场景的基本动作

  ```c#
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  public interface SceneControl {
      //加载场景资源
      void LoadResource();
  }
  ```

* CCSceneControlFirst   第一个场景类，负责控制第一个场景的动作

  ```c#
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  public enum GameState { start, running, gameover}

  public class CCSceneControlFirst : MonoBehaviour, SceneControl {


      CCActionManager manager;
      DiskFactory factory;
      ScoreRecorder recorder;

      private Queue<GameObject> diskQueue = new Queue<GameObject>();
      private float intervalTime;
      private int round;
      public GameState state;



      protected CCSceneControlFirst() { }

      // Use this for initialization
      void Awake() {
          manager = SingleTon<CCActionManager>.Instance;
          factory = SingleTon<DiskFactory>.Instance;
          recorder = SingleTon<ScoreRecorder>.Instance;
          Director.getInstance().currentSceneCtrol = this;
          GameState state = GameState.start;
          round = 0;
          intervalTime = 0.5f;
      }
  	
  	// Update is called once per frame
  	void Update () {
          if (state == GameState.running)
          {
              Debug.Log(intervalTime);
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

  ```

* DiskFactory   飞碟工厂，负责产生和回收飞碟

  ```c#
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  public class DiskFactory : MonoBehaviour {

      private GameObject diskPrefab;

      private Queue<GameObject> free = new Queue<GameObject>();
      private List<GameObject> used = new List<GameObject>();

      protected DiskFactory() { }

      private void Awake()
      {
          diskPrefab = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/disk"), Vector3.zero, Quaternion.identity);
          diskPrefab.SetActive(false);
      }

      public GameObject GetDisk(int round)
      {
          GameObject disk = null;
          int type = 0;
          if (free.Count != 0)
          {
              disk = free.Dequeue();
          } else
          {
              disk = GameObject.Instantiate<GameObject>(diskPrefab, Vector3.zero, Quaternion.identity);
              disk.AddComponent<DiskData>();
          }
          switch (round)
          {
              case 1:
                  type = 1;
                  break;
              case 2:
                  type = Random.Range(0, 200);
                  break;
              case 3:
                  type = Random.Range(0, 1000);
                  break;
          }
          float y = UnityEngine.Random.Range(0.0f, 4.0f);
          if (type <= 100)
          {
              setType(disk, Color.red, 8.0f, y);
          } else if(type <= 400) {
              setType(disk, Color.yellow, 10.0f, y);
          }else {
              setType(disk, Color.blue, 15.0f, y);
          }
          disk.GetComponent<DiskData>().name = disk.GetInstanceID().ToString();
          disk.SetActive(false);
          used.Add(disk);
          return disk;
      }

      private void setType(GameObject disk, Color color, float hspeed, float vspeed)
      {
          disk.GetComponent<DiskData>().color = color;
          disk.GetComponent<Renderer>().material.color = color;
          disk.GetComponent<DiskData>().horizontalSpeed = hspeed;
          disk.GetComponent<DiskData>().verticalSpeed = vspeed;
          int x = (UnityEngine.Random.Range(-1.0f, 1.0f) < 0 ? -1 : 1);
          float y = UnityEngine.Random.Range(0.0f, 4.0f);
          disk.GetComponent<DiskData>().direction = x;
          disk.transform.position = new Vector3(x * 7, y, 0);
      }

      public void FreeDisk(GameObject disk)
      {
          foreach(GameObject i in used)
          {
              if(i.GetInstanceID() == disk.GetInstanceID())
              {
                  i.SetActive(false);
                  free.Enqueue(i);
                  used.Remove(i);
                  break;
              }
          }
      }

      public void Reset()
      {
          used.Clear();
      }
  }
  ```

* DiskData   包含飞碟的基本属性

  ```c#
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  public class DiskData : MonoBehaviour
  {
      public Color color;
      public float horizontalSpeed;
      public int direction;
      public float verticalSpeed;
  }
  ```

* ScoreRecorder   积分器，负责记录分数

  ```c#
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
  ```

* SSActionManeger   动作管理基类，包含基本的动作管理方法

  ```c#
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  public class SSActionManager : MonoBehaviour {

      private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();        //保存所以已经注册的动作  
      private List<SSAction> waitingAdd = new List<SSAction>();                           //动作的等待队列，在这个对象保存的动作会稍后注册到动作管理器里  
      private List<int> waitingDelete = new List<int>();                                  //动作的删除队列，在这个对象保存的动作会稍后删除  


      // Use this for initialization  
      protected void Start()
      {

      }

      // Update is called once per frame  
      protected void Update()
      {
          //把等待队列里所有的动作注册到动作管理器里  
          foreach (SSAction ac in waitingAdd) actions[ac.GetInstanceID()] = ac;
          waitingAdd.Clear();

          //管理所有的动作，如果动作被标志为删除，则把它加入删除队列，被标志为激活，则调用其对应的Update函数  
          foreach (KeyValuePair<int, SSAction> kv in actions)
          {
              SSAction ac = kv.Value;
              if (ac.destroy)
              {
                  waitingDelete.Add(ac.GetInstanceID());
              }
              else if (ac.enable)
              {
                  ac.Update();
              }
          }

          //把删除队列里所有的动作删除  
          foreach (int key in waitingDelete)
          {
              SSAction ac = actions[key];
              actions.Remove(key);
              DestroyObject(ac);
          }
          waitingDelete.Clear();
      }

      //初始化一个动作  
      public void RunAction(GameObject gameobject, SSAction action, SSActionCallback manager)
      {
          gameobject.SetActive(true);
          action.gameobject = gameobject;
          action.callback = manager;
          waitingAdd.Add(action);
          action.Start();
      }

      public void Reset()
      {
          actions.Clear();
          waitingAdd.Clear();
          waitingDelete.Clear();
      }
  }
  ```

* CCActionManager   第一个场景的动作管理类，负责第一个场景中的动作管理

  ```c#
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  public class CCActionManager : SSActionManager, SSActionCallback {

      DiskFactory factory;

      public void ThrowDisk(GameObject disk)
      {
          CCFlyAction action = ScriptableObject.CreateInstance<CCFlyAction>();
          RunAction(disk, action, this);
      }

      protected void Start()
      {
          factory = SingleTon<DiskFactory>.Instance;
      }

      public void SSActionEvent(SSAction source)
      {
          if (source is CCFlyAction)
          {
              factory.FreeDisk(source.gameobject);
              source.destroy = true;
              source.enable = false;
          }
      }


  }
  ```

* SSAction   动作基类，包含一切动作的基本属性

  ```c#
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  public class SSAction : ScriptableObject
  {

      public bool enable = false;
      public bool destroy = false;

      public GameObject gameobject { get; set; }
      public SSActionCallback callback { get; set; }

      protected SSAction() { }

      public virtual void Start()
      {
          throw new System.NotImplementedException();
      }

      // Update is called once per frame  
      public virtual void Update()
      {
          throw new System.NotImplementedException();
      }

      public void reset()
      {
          enable = false;
          destroy = false;
          gameobject = null;
          callback = null;
      }
  }

  ```

* CCFlyAction   飞行动作类，描述了飞碟飞行的动作

  ```c#
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  public class CCFlyAction : SSAction {
      float acceleration;			//acceleration是重力加速度

      float horizontalSpeed;		//horizontalSpeed是飞碟水平方向的速度 
      float verticalSpeed;		//verticalSpeed是飞碟垂直方向的速度

      int direction;				//飞碟的飞行方向

      float time;					//已经飞行的时间

      public override void Start()
      {
          enable = true;
          acceleration = 9.8f;
          time = 0;
          horizontalSpeed = gameobject.GetComponent<DiskData>().horizontalSpeed;
          verticalSpeed = gameobject.GetComponent<DiskData>().verticalSpeed;
          direction = gameobject.GetComponent<DiskData>().direction;
      }

      // Update is called once per frame  
      public override void Update()
      {
          if (gameobject.activeSelf)
          { 
              time += Time.deltaTime;		//计算飞碟的累计飞行时间

              gameobject.transform.Translate(-Vector3.down * (verticalSpeed - acceleration) * time * Time.deltaTime);			//飞碟在数值方向的运动

              gameobject.transform.Translate(Vector3.left * direction * horizontalSpeed * Time.deltaTime);			//飞碟在水平方向的运动 

              if (this.gameobject.transform.position.y < -4)	//当飞碟的y坐标比-4小时，飞碟落地 
              {
                  this.destroy = true;
                  this.enable = false;
                  this.callback.SSActionEvent(this);
              }
          }

      }


  }

  ```

* SSActionCallback   动作与动作管理器的交互接口

  ```c#
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  public interface SSActionCallback {
      void SSActionEvent(SSAction source);
  }

  ```

* SingleTon   所有单例类的模板类

  ```c#
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;


  public class SingleTon<T> : MonoBehaviour where T : MonoBehaviour
  {

      protected static T instance;

      public static T Instance
      {
          get
          {
              if (instance == null)
              {
                  instance = (T)FindObjectOfType(typeof(T));
              }
              return instance;
          }
      }
  }
  ```

* UserAction   负责展现用户界面

  ```c#
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

  ```

4. 规则介绍：
   * 一共有三轮游戏，点击Start按钮开始游戏，每轮有10个飞碟
   * 红色飞碟1分，黄色2分，蓝色3分，速度由慢到快
   * 第一轮只会有红色飞碟，第二轮会又红色和黄色（出现概率均为50%），第三轮三种都有（出现概率为别为10%、30%、60%）
   * 飞碟水平初速度、出现高度、出现间隔均随机
   * 游戏中可以点击Stop按钮终止游戏
   * 游戏结束后可以点击Reset按钮重置游戏
5. [演示视频（录制软件原因，视频中未显示鼠标）](http://littlefish33.cn:8080/UploadFile/showVideo.jsp)

