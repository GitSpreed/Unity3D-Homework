### 牧师与恶魔

* 列出游戏中提及的事物（Object）

  * 牧师
  * 恶魔
  * 小船
  * 河岸

* 用表格列出玩家动作表（规则表），注意，动作越少越好

  | 动作     | 条件                     |
  | -------- | ------------------------ |
  | 牧师上船 | 靠近船的河岸上有牧师     |
  | 恶魔上船 | 靠近船的河岸上有恶魔     |
  | 牧师下船 | 船上有牧师               |
  | 恶魔下船 | 船上有恶魔               |
  | 开船     | 船上至少有一个牧师或恶魔 |

* 代码展示：

  * Controller.cs

  ```c#
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;


  public enum State { LEFT, RIGHT, MOVEL, MOVER, WIN, LOSE };

  public class Controller : System.Object
  {

      private static Controller _instance = null;  
      private GenGameObject model;
      private BasicCode basic;
      public State state = State.RIGHT;

      public static Controller GetInstance()
      {
          if (_instance == null)
          {
              _instance = new Controller();
          }
          return _instance;
      }

      public void setBasicCode(BasicCode value)
      {
          basic = value;
      }

      public GenGameObject getGenGameObject()
      {
          return model;
      }

      internal void setGenGameObject(GenGameObject value)
      {
         model = value;
      }

      public void priest_Right_GetOn()
      {
          model.priestRightGetOn();
      }

      public void priest_Left_GetOn()
      {
          model.priestLeftGetOn();
      }

      public void devil_Right_GetOn()
      {
          model.devilRightGetOn();
      }

      public void devil_Left_GetOn()
      {
          model.devilLeftGetOn();
      }

      public void moveBoat()
      {
          model.moveBoat();
      }

      public void offBoat_Left()
      {
          model.getOffTheBoat(0);
      }

      public void offBoat_Right()
      {
          model.getOffTheBoat(1);
      }

      public void reset()
      {  
          state = State.RIGHT;
          model.Reset();
      }
  }

  public class BasicCode :MonoBehaviour
  {
      private void Start()
      {
          Controller controller = Controller.GetInstance();
          controller.setBasicCode(this);
      }
  }
  ```

  * GenGameObject.cs:

    ```c#
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;


    public class GenGameObject : MonoBehaviour
    {

        Stack<GameObject> priests_right = new Stack<GameObject>();
        Stack<GameObject> priests_left = new Stack<GameObject>();
        Stack<GameObject> devils_right = new Stack<GameObject>();
        Stack<GameObject> devils_left = new Stack<GameObject>();

        GameObject[] boat = new GameObject[2];
        GameObject boat_obj;
        public float speed = 100f;

        Controller controller;

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
            controller = Controller.GetInstance();
            controller.setGenGameObject(this);
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

        void getOn(GameObject obj)
        {
            if (boatCapacity() != 0)
            {
                obj.transform.parent = boat_obj.transform;
                if (boat[0] == null)
                {
                    boat[0] = obj;
                    obj.transform.localPosition = new Vector3(0, 1.2f, -0.3f);
                }
                else
                {
                    boat[1] = obj;
                    obj.transform.localPosition = new Vector3(0, 1.2f, 0.3f);
                }
            }
        }

        public void moveBoat()
        {
            if (boatCapacity() < 2)
            {
                if (controller.state == State.RIGHT)
                {
                    controller.state = State.MOVEL;
                }
                else if (controller.state == State.LEFT)
                {
                    controller.state = State.MOVER;
                }
            }
        }

        public void getOffTheBoat(int side)
        {
            if (boat[side] != null)
            {
                boat[side].transform.parent = null;
                if (controller.state == State.LEFT)
                {
                    if (boat[side].tag == "Priest")
                    {
                        priests_left.Push(boat[side]);
                    }
                    else if (boat[side].tag == "Devil")
                    {
                        devils_left.Push(boat[side]);
                    }
                }
                else if (controller.state == State.RIGHT)
                {
                    if (boat[side].tag == "Priest")
                    {
                        priests_right.Push(boat[side]);
                    }
                    else if (boat[side].tag == "Devil")
                    {
                        devils_right.Push(boat[side]);
                    }
                }
                boat[side] = null;
            }
        }

        public void priestRightGetOn()
        {
            if (priests_right.Count != 0 && boatCapacity() != 0 && controller.state == State.RIGHT)
                getOn(priests_right.Pop());
        }

        public void priestLeftGetOn()
        {
            if (priests_left.Count != 0 && boatCapacity() != 0 && controller.state == State.LEFT)
                getOn(priests_left.Pop());
        }

        public void devilRightGetOn()
        {
            if (devils_right.Count != 0 && boatCapacity() != 0 && controller.state == State.RIGHT)
                getOn(devils_right.Pop());
        }

        public void devilLeftGetOn()
        {
            if (devils_left.Count != 0 && boatCapacity() != 0 && controller.state == State.LEFT)
                getOn(devils_left.Pop());
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
                controller.state = State.WIN;
                return;
            }

            if (controller.state == State.RIGHT)
            {
                priests = priests_right.Count;
                devils = devils_right.Count;
            }
            else if (controller.state == State.LEFT)
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
                controller.state = State.LOSE;
            }
        }

        void setPositon()
        {
            setCharacterPositions(priests_right, priest_Right_Position);
            setCharacterPositions(priests_left, priest_Left_Position);
            setCharacterPositions(devils_right, devil_Right_Position);
            setCharacterPositions(devils_left, devil_Left_Position);
        }

        bool Move()
        {
            if (controller.state != State.MOVEL && controller.state != State.MOVER) return false;
            if (controller.state == State.MOVEL)
            {
                boat_obj.transform.position = Vector3.MoveTowards(boat_obj.transform.position, boat_Left_Position, speed * Time.deltaTime);
                if (boat_obj.transform.position == boat_Left_Position)
                {
                    controller.state = State.LEFT;
                }
            }
            else
            {
                boat_obj.transform.position = Vector3.MoveTowards(boat_obj.transform.position, boat_Right_Position, speed * Time.deltaTime);
                if (boat_obj.transform.position == boat_Right_Position)
                {
                    controller.state = State.RIGHT;
                }
            }
            return true;
        }

        void Update()
        {
            setPositon();
            if (!Move()) check();
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

  * UserInterface.cs：

    ```c#
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UserInterface : MonoBehaviour
    {

        Controller controller;

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
            controller = Controller.GetInstance();
        }

        void OnGUI()
        {
            GUIStyle myStyle = new GUIStyle
            {
                fontSize = 25,
                fontStyle = FontStyle.Bold
            };

            width = Screen.width / 12;
            height = Screen.height / 12;

            if (controller.state == State.WIN || controller.state == State.LOSE)
            {
                if (controller.state == State.WIN)
                {
                    GUI.Label(new Rect(castw(2f), casth(6f), width, height), "Win!", myStyle);
                }
                else
                {
                    GUI.Label(new Rect(castw(2f), casth(6f), width, height), "Lose!", myStyle);
                }

                if (GUI.Button(new Rect(castw(2f), casth(1f), width, height), "Reset"))
                {
                    controller.reset();
                }
            }
            else
            {
                if (controller.state == State.RIGHT || controller.state == State.LEFT)
                {
                    if (GUI.Button(new Rect(castw(2f), casth(6f), width, height), "Move"))
                    {
                        controller.moveBoat();
                    }
                    if (GUI.Button(new Rect(castw(1.1f), casth(4f), width, height), "On"))
                    {
                        controller.devil_Right_GetOn();
                    }
                    if (GUI.Button(new Rect(castw(1.3f), casth(4f), width, height), "On"))
                    {
                        controller.priest_Right_GetOn();
                    }
                    if (GUI.Button(new Rect(castw(10.5f), casth(4f), width, height), "On"))
                    {
                        controller.devil_Left_GetOn();
                    }
                    if (GUI.Button(new Rect(castw(4.3f), casth(4f), width, height), "On"))
                    {
                        controller.priest_Left_GetOn();
                    }
                    if (GUI.Button(new Rect(castw(1.7f), casth(1.3f), width, height), "Off"))
                    {
                        controller.offBoat_Left();
                    }
                    if (GUI.Button(new Rect(castw(2.5f), casth(1.3f), width, height), "Off"))
                    {
                        controller.offBoat_Right();
                    }
                }
            }
        }
    }
    ```

    ​