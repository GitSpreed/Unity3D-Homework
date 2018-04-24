# Hit UFO 改进版

### 游戏说明

​    在本次改进版中，有**两种动作管理类**的实现，可以通过button来切换，primary表示原来的动作管理类，physics表示新增的利用物理引擎的动作管理类，新增的动作管理类因为运用了物理引擎，所以在游戏过程中，飞碟可能会出现 **碰撞** 的情况，而原来的动作管理类则不会出现，这一点在文末的演示视频中有体现。



### 规则介绍：

- 共有三轮游戏，点击Start按钮开始游戏，每轮有10个飞碟
- 红色飞碟1分，黄色2分，蓝色3分，速度由慢到快
- 第一轮只会有红色飞碟，第二轮会又红色和黄色（出现概率均为50%），第三轮三种都有（出现概率为别为10%、30%、60%）
- 飞碟水平初速度、出现高度、出现间隔均随机
- 游戏中可以点击Stop按钮终止游戏
- 游戏结束后可以点击Reset按钮重置游戏
- **[演示视频](http://littlefish33.cn:8080/UploadFile/showVideo.jsp)**



### 改进的代码

1. 新增类PhysicalFlyAction

   该类使用刚体组件的属性来定义飞碟的运动

   ```c#
   using System.Collections;
   using System.Collections.Generic;
   using UnityEngine;

   public class PhysicalFlyAction : SSAction {

   	bool enableThrow = true;
   	Vector3 force;

   	// Use this for initialization
   	public override void Start () {
   		enable = true;
           //使用重力
   		gameobject.GetComponent<Rigidbody>().useGravity = true;
           //关闭运动学
   		gameobject.GetComponent<Rigidbody>().isKinematic = false;
           //提供初始水平速度和竖直速度的力
   		force = new Vector3(-1 * gameobject.GetComponent<DiskData>().direction * gameobject.GetComponent<DiskData>().horizontalSpeed, 
   						gameobject.GetComponent<DiskData>().verticalSpeed, 0);
   	}
   		
   	// Update is called once per frame
   	public override void Update () {
   		if(!destroy) 
   		{
   			if(enableThrow)
   			{
   				//Debug.Log ("Physical test");
                   //角转动力为0
   				gameobject.GetComponent<Rigidbody>().velocity = Vector3.zero;  
                   //初始给一个冲击力
   				gameobject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
                   enableThrow = false;
   			}
   		}
           
           //落入地面以下，回收飞碟
   		if (this.gameobject.transform.position.y < -4)
           {
               this.destroy = true;
               this.enable = false;
               this.callback.SSActionEvent(this);
           }
   	}

   }

   ```

2. 新增类PhysicalActionManager

   该类管理具有PhysicalFlyAction动作类的飞碟

   ```c#
   using System.Collections;
   using System.Collections.Generic;
   using UnityEngine;

   public class PhysicalActionManager : SSActionManager, SSActionCallback, IActionManager {

   	DiskFactory factory;

       //发射一个飞碟
   	public void ThrowDisk(GameObject disk)
       {
           PhysicalFlyAction action = ScriptableObject.CreateInstance<PhysicalFlyAction>();
           RunAction(disk, action, this);
       }

       protected new void Start()
       {
           factory = SingleTon<DiskFactory>.Instance;
       }

       //回收飞碟
       public void SSActionEvent(SSAction source)
       {
           if (source is PhysicalFlyAction)
           {
               factory.FreeDisk(source.gameobject);
               source.destroy = true;
               source.enable = false;
           }
       }

       //重置动作管理器
   	public void Reset()
   	{
   		actions.Clear();
   		waitingAdd.Clear();
   		waitingDelete.Clear();
   	}
   }

   ```

3. 新增接口IActionManager

   两种动作管理器均要实现该接口

   ```c#
   using System.Collections;
   using System.Collections.Generic;
   using UnityEngine;

   public interface IActionManager {

   	//发射一个飞碟
   	void ThrowDisk(GameObject disk);

       //重置
   	void Reset();

   }
   ```

4. 更改CCActionManager代码，增加接口IActionManager的实现

   ```c#
   using System.Collections;
   using System.Collections.Generic;
   using UnityEngine;

   //在此处增加了接口IActionManager的实现申明
   public class CCActionManager : SSActionManager, SSActionCallback, IActionManager {

       DiskFactory factory;

       public void ThrowDisk(GameObject disk)
       {
           CCFlyAction action = ScriptableObject.CreateInstance<CCFlyAction>();
           RunAction(disk, action, this);
       }

       protected new void Start()
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

       //新增Reset函数的实现
   	public void Reset()
   	{
   		actions.Clear();
   		waitingAdd.Clear();
   		waitingDelete.Clear();
   	} 
   }
   ```

5. 更改UserAction，增加切换两种动作管理器的button
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
                   control.Reset();
               }
           }
           
           //此处增加两个button，用于切换两种动作管理器
           if (GUI.Button(new Rect(0, 50, 90, 60), "Primary"))
           {
   			control.state = GameState.gameover;
               control.Reset();
               control.manager = SingleTon<CCActionManager>.Instance;
           }
           if (GUI.Button(new Rect(0, 120, 90, 60), "Physics"))
           {
   			control.state = GameState.gameover;
               control.Reset();
               control.manager = SingleTon<PhysicalActionManager>.Instance;
           }
       }
   }

   ```

6. 更改CCSenceManager，将原来的成员CCActionManager更改为IActionManager接口

   ```c#
   using System.Collections;
   using System.Collections.Generic;
   using UnityEngine;

   public enum GameState { start, running, gameover}

   public class CCSceneControlFirst : MonoBehaviour, SceneControl {

   	//更改此处的动作管理类，改为接口实现
       public IActionManager manager;
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

   ```



### 未改变代码

1. Director   导演类，控制场景的转换

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

2. SceneControl   场景基类，包含所有场景的基本动作

   ```c#
   using System.Collections;
   using System.Collections.Generic;
   using UnityEngine;

   public interface SceneControl {
       //加载场景资源
       void LoadResource();
   }
   ```

3. CCSceneControlFirst   第一个场景类，负责控制第一个场景的动作

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

4. DiskFactory   飞碟工厂，负责产生和回收飞碟

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

5. DiskData   包含飞碟的基本属性

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

6. ScoreRecorder   积分器，负责记录分数

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

7. SSActionManeger   动作管理基类，包含基本的动作管理方法

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

8. SSAction   动作基类，包含一切动作的基本属性

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

9. CCFlyAction   飞行动作类，描述了飞碟飞行的动作

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

10. SSActionCallback   动作与动作管理器的交互接口

   ```c#
   using System.Collections;
   using System.Collections.Generic;
   using UnityEngine;

   public interface SSActionCallback {
       void SSActionEvent(SSAction source);
   }

   ```

11. SingleTon   所有单例类的模板类

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

