1.操作与总结

* 参考 Fantasy Skybox FREE 构建自己的游戏场景

  ![1](https://github.com/GitSpreed/Unity3D-Homework/blob/master/homework3/picture/1.png?raw=true)

* 写一个简单的总结，总结游戏对象的使用

  一个游戏场景由很多个游戏对象组成，每个游戏对象是场景中的一小部分，如一棵树，一棵草，每个游戏对象都有自己的属性和方法，能实现一定的功能。其中，一些常用的游戏对象类型有Empty对象、Camera对象、Light对象和Audio对象等。

2. 编程实践

   牧师与魔鬼动作分离版

   * 动作基类与事件回调接口

     ```c#
     public enum SSActionEventType : int { Started, Competeted }  
       
     public interface ISSActionCallback  
     {  
         void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,  
             int intParam = 0, string strParam = null, Object objectParam = null);  
     }  
       
     public class SSAction : ScriptableObject  
     {  
         public bool enable = true;  
         public bool destroy = false;  
       
         public GameObject gameobject { get; set; }  
         public Transform transform { get; set; }  
         public ISSActionCallback callback { get; set; }  
       
         protected SSAction() { }  
       
         public virtual void Start()  
         {  
             throw new System.NotImplementedException();  
         }  
       
         public virtual void Update()  
         {  
             throw new System.NotImplementedException();  
         }  
     }  
     ```

   * 上船的动作

     ```c#
     public class CCGetOnTheBoat : SSAction  
     {  
         public GenGameObect model;  
       
         public static CCGetOnTheBoat GetSSAction()  
         {  
             CCGetOnTheBoat action = ScriptableObject.CreateInstance<CCGetOnTheBoat>();  
             return action;  
         }  
         // Use this for initialization  
         public override void Start()  
         {  
             model = (GenGameObect)SSDirector.getInstance().currentModel;  
         }  
       
         // Update is called once per frame  
         public override void Update()  
         {  
             if (model.boatCapacity() != 0)  
             {  
                 if (model.boat_position == 0)  
                 {  
                 	if(gameobject.transform.tag == "Priests"){
                 		priests_right.pop();
                     }else{
                          devils_right.pop();
                     }
                 }  
                 else if (model.boat_position == 1)  
                 {  
                     if(gameobject.transform.tag == "Priests"){
                 		priests_left.pop();
                     }else{
                          devils_left.pop();
                     }
                 }  
       
                 gameobject.transform.parent = model.boat_obj.transform;  
       
                 if (model.boat[0] == null)  
                 {  
                     model.boat[0] = gameobject;     
                     this.transform.localPosition = new Vector3(0, 1.2f, 0.3f);  
                 }  
                 else if (model.boat[1] == null)  
                 {  
                     model.boat[1] = gameobject;  
                     this.transform.localPosition = new Vector3(0, 1.2f, -0.3f);  
                 }  
             }    
             this.destroy = true;  
             this.callback.SSActionEvent(this);  
         }  
     }  
     ```

   * 下船动作

     ```c#
     public class CCGetOffBoat : SSAction {  
       
         public int side;  
         public GenGameObect model;  
       
         public static CCGetOffBoat GetSSAction(int side)  
         {  
             CCGetOffBoat action = ScriptableObject.CreateInstance<CCGetOffBoat>();  
             action.side = side;  
             return action;  
         }  
         // Use this for initialization  
         public override void Start()  
         {  
             model = (GenGameObect)SSDirector.getInstance().currentScenceController;  
         }  
       
         // Update is called once per frame  
         public override void Update()  
         {  
             if (model.boat[side] != null)  
             {  
                 model.boat[side].transform.parent = null;  
                 if (model.boat_position == 1)  
                 {  
                       
                     if (model.boat[side].transform.tag == "Priest")  
                     {  
                         priests_left.push(boat[side]);
                     }  
                     else if (model.boat[side].transform.tag == "Devil")  
                     {  
                         devils_left.push(boat[side]);
                     }  
                 }  
                 else if (model.boat_position == 0)  
                 {  
                     if (model.boat[side].transform.tag == "Priest")  
                     {  
                         priests_right.push(boat[side]);  
                     }  
                     else if (model.boat[side].transform.tag == "Devil")  
                     {  
                         devils_right.push(boat[side]);
                     }  
                 }  
                 model.boat[side] = null;  
             }  
             model.check();  
             this.destroy = true;  
             this.callback.SSActionEvent(this);  
         }  
     }  
     ```

   * 船的移动

     ```c#
     public class CCBoatMoveing : SSAction {  
         public GenGameObect model;  
       
         public static CCBoatMoveing GetSSAction()  
         {  
             CCBoatMoveing action = ScriptableObject.CreateInstance<CCBoatMoveing>();  
             return action;  
         }  
         // Use this for initialization  
         public override void Start()  
         {  
             model = (GenGameObect)SSDirector.getInstance().currentScenceController;  
         }  
       
         // Update is called once per frame  
         public override void Update()  
         {  
             if (model.boat_position == 1)  
             {  
                 model.boat_position = 0;  
                 while (this.transform.position != model.boatStartPos)  
                     this.transform.position = Vector3.MoveTowards(this.transform.position, model.boat_Right_Position, 1);  
             }  
             else if (model.boat_position == 0)  
             {  
                 model.boat_position = 1;  
                 while (this.transform.position != model.boatEndPos)  
                     this.transform.position = Vector3.MoveTowards(this.transform.position, model.boat_Left_Position, 1);  
             }  
             model.check();  
             this.destroy = true;  
             this.callback.SSActionEvent(this);  
         }  
     }  
     ```

   * 动作管理基类

     ```c#
     public class SSActionManager : MonoBehaviour {  
         private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();  
         private List<SSAction> waitingAdd = new List<SSAction>();  
         private List<int> waitingDelete = new List<int>();  
       
         // Use this for initialization  
         void Start()  
         {  
       
         }  
       
         // Update is called once per frame  
         protected void Update()  
         {  
             foreach (SSAction ac in waitingAdd) actions[ac.GetInstanceID()] = ac;  
             waitingAdd.Clear();  
       
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
       
             foreach (int key in waitingDelete)  
             {  
                 SSAction ac = actions[key]; actions.Remove(key); DestroyObject(ac);  
             }  
             waitingDelete.Clear();  
         }  
       
         public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager)  
         {  
             action.gameobject = gameobject;  
             action.transform = gameobject.transform;  
             action.callback = manager;  
             waitingAdd.Add(action);  
             action.Start();  
         }  
     }  
     ```

   * 具体的动作管理类

     ```c#
     public class CCActionManager : SSActionManager, ISSActionCallback  
     {  
         public GenGameObect model;  
         public CCGetOnTheBoat geton;  
         public CCGetOffBoat getoff;  
         public CCBoatMoveing moving;  
       

         float width, height;

         float castw(float scale)
         {
             return (Screen.width - width) / scale;
         }

         float casth(float scale)
         {
             return (Screen.height - height) / scale;
         }

         void Start()
         {
             model = Controller.GetInstance();
         }


         // Use this for initialization  
         protected void Start () {  
             model = (GenGameObect)SSDirector.getInstance().currentScenceController;  
             model.actionManager = this;  
         }  
           
         // Update is called once per frame  
         protected  void OnGUI () {  
             GUIStyle myStyle = new GUIStyle
             {
                 fontSize = 25,
                 fontStyle = FontStyle.Bold
             };

             width = Screen.width / 12;
             height = Screen.height / 12;

             if (model.game != 0)
             {
                 if (model.game == 1)
                 {
                     GUI.Label(new Rect(castw(2f), casth(6f), width, height), "Win!", myStyle);
                 }
                 else
                 {
                     GUI.Label(new Rect(castw(2f), casth(6f), width, height), "Lose!", myStyle);
                 }

                 if (GUI.Button(new Rect(castw(2f), casth(1f), width, height), "Reset"))
                 {
                     model.reset();
                 }
             }
             else
             {
                 if (GUI.Button(new Rect(castw(2f), casth(6f), width, height), "Move"))
                 {
                     moving = CCBoatMoveing.GetSSAction();
                     this.RunAction(model.boat_obj, moving, this);
                 }
                 if (GUI.Button(new Rect(castw(1.1f), casth(4f), width, height), "On"))
                 {
                     if(devils_right.Count != 0){
                         geton = CCGetOnTheBoat.GetSSAction();
                         this.RunAction(model.devils_right.top(), geton, this);
                     }
                 }
                 if (GUI.Button(new Rect(castw(1.3f), casth(4f), width, height), "On"))
                 {
                     if(priests_right.Count != 0){
                         geton = CCGetOnTheBoat.GetSSAction();
                         this.RunAction(model.priests_right.top(), geton, this);
                     }
                 }
                 if (GUI.Button(new Rect(castw(10.5f), casth(4f), width, height), "On"))
                 {
                     if(devils_left.Count != 0){
                         geton = CCGetOnTheBoat.GetSSAction();
                         this.RunAction(model.devils_left.top(), geton, this);
                     }
                 }
                 if (GUI.Button(new Rect(castw(4.3f), casth(4f), width, height), "On"))
                 {
                     if(priests_left.Count != 0){
                         geton = CCGetOnTheBoat.GetSSAction();
                         this.RunAction(model.priests_left.top(), geton, this);
                     }
                 }
                 if (GUI.Button(new Rect(castw(1.7f), casth(1.3f), width, height), "Off"))
                 {
                     getoff = CCGetOffBoat.GetSSAction(0);
                     this.RunAction(model.boat[0], getoff, this);
                 }
                 if (GUI.Button(new Rect(castw(2.5f), casth(1.3f), width, height), "Off"))
                 {
                     getoff = CCGetOffBoat.GetSSAction(1);
                     this.RunAction(model.boat[1], getoff, this);
                 }
             }  
         }  
       
         public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,  
             int intParam = 0, string strParam = null, Object objectParam = null)  
         {  
             
         }  
     }  
     ```

   * GenGameObject:

     ```c#
     public class GenGameObject : MonoBehaviour
     {

         Stack<GameObject> priests_right = new Stack<GameObject>();
         Stack<GameObject> priests_left = new Stack<GameObject>();
         Stack<GameObject> devils_right = new Stack<GameObject>();
         Stack<GameObject> devils_left = new Stack<GameObject>();

         GameObject[] boat = new GameObject[2];
         GameObject boat_obj;
         public float speed = 100f;

         int game = 0;
         int boat_position = 0;

         Vector3 coast_Right_Position = new Vector3(0, 0, -12);
         Vector3 coast_Left_Position = new Vector3(0, 0, 12);
         Vector3 boat_Right_Position = new Vector3(0, 0, -6.5f);
         Vector3 boat_Left_Position = new Vector3(0, 0, 6.5f);

         float gap = 1.5f;
         Vector3 priest_Right_Position = new Vector3(0, 0.7f, -11f);
         Vector3 priest_Left_Position = new Vector3(0, 0.7f, 8f);
         Vector3 devil_Right_Position = new Vector3(0, 0.7f, -16f);
         Vector3 devil_Left_Position = new Vector3(0, 0.7f, 13f);


         void Start()
         {
             loadSrc();
         }

         void loadSrc()
         { 
             Instantiate(Resources.Load("Prefabs/Coast"), coast_Right_Position, Quaternion.identity);
             Instantiate(Resources.Load("Prefabs/Coast"), coast_Left_Position, Quaternion.identity);

             boat_obj = Instantiate(Resources.Load("Prefabs/Boat"), boat_Right_Position, Quaternion.identity) as GameObject;
       
             for (int i = 0; i < 3; ++i)
             {
                 priests_right.Push(Instantiate(Resources.Load("Prefabs/Priest")) as GameObject);
                 devils_right.Push(Instantiate(Resources.Load("Prefabs/Devil")) as GameObject);
             }
         }

         int boatCapacity()
         {
             int capacity = 0;
             for (int i = 0; i < 2; ++i)
             {
                 if (boat[i] == null) capacity++;
             }
             return capacity;
         }

         void setCharacterPositions(Stack<GameObject> stack, Vector3 vec)
         {
             GameObject[] array = stack.ToArray();
             for (int i = 0; i < stack.Count; ++i)
             {
                 array[i].transform.position = new Vector3(vec.x, vec.y, vec.z + gap * i);
             }
         }

         void check()
         {
             int priests = 0, devils = 0;

             if (priests_left.Count == 3 && devils_left.Count == 3)
             {
                 game = 1;
                 return;
             }

             if (boat_position == 0)
             {
                 priests = priests_right.Count;
                 devils = devils_right.Count;
             }
             else if (boat_position == 1)
             {
                 priests = priests_left.Count;
                 devils = devils_left.Count;
             }

             for (int i = 0; i < 2; ++i)
             {
                 if (boat[i] != null && boat[i].tag == "Priest") priests++;
                 else if (boat[i] != null && boat[i].tag == "Devil") devils++;
             }

             Debug.Log(priests);
             Debug.Log(-devils);

             if ((priests != 0 && priests != 3) && priests != devils)
             {
                 game = 2;
             }
         }

         void setPositon()
         {
             setCharacterPositions(priests_right, priest_Right_Position);
             setCharacterPositions(priests_left, priest_Left_Position);
             setCharacterPositions(devils_right, devil_Right_Position);
             setCharacterPositions(devils_left, devil_Left_Position);
         }


         void Update()
         {
             setPositon();
         }

         public void Reset()
         {
             while (priests_left.Count != 0)
             {
                 priests_right.Push(priests_left.Pop());
             }

             while (devils_left.Count != 0)
             {
                 devils_right.Push(devils_left.Pop());
             }

             for (int i = 0; i < 2; ++i)
             {
                 if (boat[i] != null && boat[i].tag == "Priest")
                 {
                     priests_right.Push(boat[i]);
                     boat[i] = null;
                 }
                 else if (boat[i] != null && boat[i].tag == "Devil")
                 {
                     devils_right.Push(boat[i]);
                     boat[i] = null;
                 }
             }
             boat_obj.transform.position = boat_Right_Position;
         }
     }
     ```

     ​

