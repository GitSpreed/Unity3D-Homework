# Homework1

### 简答题

1. 解释游戏对象（GameObjects）和资源（Assets）的区别与联系。

   * 区别

     * 游戏对象：游戏中的人物、道具和场景都是游戏对象，游戏对象就像一个容器，它本身并不能单独完成什么动作。
     * 资源：资源包括脚本、图片、音频等，资源通常用来完成一个任务或者动作，但资源本身不能单独存在于游戏中。

   * 联系

     游戏对象需要加载相应的资源来完成某个任务或者动作，而资源依赖于游戏对象而存在于游戏中。

2. 编写一个代码，使用 debug 语句来验证MonoBehaviour基本行为或事件触发的条件

   * 基本行为包括 Awake() Start() Update() FixedUpdate() LateUpdate()
   * 常用事件包括 OnGUI() OnDisable() OnEnable()

   代码如下：

   ```c++
   using System.Collections;
   using System.Collections.Generic;
   using UnityEngine;

   public class InitBeh : MonoBehaviour {

       public Transform table;

       //First run in project
       private void Awake()
       {
           Debug.Log("Init Awake");
       }

       // Use this for initialization
       void Start () {
           Debug.Log("Init Start");
       }

       // Update is called once per frame
       void Update () {
           Debug.Log("Init Update");
       }

       private void OnGUI()
       {
           Debug.Log("onGUI");
       }

       private void OnDisable()
       {
           Debug.Log("onDisable");
       }

       private void OnEnable()
       {
           Debug.Log("onEnale");
       }
   }
   ```

   ​

3. 查找脚本手册，了解 GameObject，Transform，Component 对象

   * 分别翻译官方对三个对象的描述（Description）
     * GameObject
       * GameObjects are the fundamental objects in Unity that represent characters, props and scenery. They do not accomplish much in themselves but they act as containers for Components, which implement the real functionality.
       * 游戏对象是Unity中表示人物、道具、场景的基本对象。它本身并不能完成很多工作，但是它可以充当组件的容器，来实现真正的功能。
     * Transform
       * The **Transform** component determines the **Position**, **Rotation**, and **Scale** of each object in the scene. Every GameObject has a Transform.
       * 变换组件决定场景中每个对象的位置、旋转和缩放。每个游戏对象都有一个变换。
     * Component
       * Components are the nuts and bolts of objects and behaviors in a game.
       * 组件是游戏中对象的行为和具体细节。


   * 描述下图中 table 对象（实体）的属性、table 的 Transform 的属性、 table 的部件

     ![picture](https://github.com/GitSpreed/Unity3D-Homework/blob/master/homework1/picture/ch02-homework.png?raw=true)

     * table对象的属性：

       >tag: Untagged
       >
       >Layer: Default

     * Transform的属性

       > Position x = 0  y = 0  z = 0
       >
       > Rotation x = 0  y = 0  z = 0
       >
       > Scale x = 1  y = 1  z = 1

     * 部件

       > Transform
       >
       > Cube(Mesh Filter)
       >
       > Box Collider
       >
       > Mesh Renderer

   * 用 UML 图描述 三者的关系（请使用 UMLet 14.1.1 stand-alone版本出图）

     ![picture](https://github.com/GitSpreed/Unity3D-Homework/blob/master/homework1/picture/new.jpg?raw=true)

4. 整理相关学习资料，编写简单代码验证以下技术的实现：

   * 查找对象

     ```c++
     GameObject.Find("Name");
     ```

   * 添加子对象

     ```c++
     Gameobject.CreatePrimitive(PrimitiveType);
     ```

   * 遍历对象树

     ```c++
     foreach(Transform child in transform) {}
     ```

   * 清除所有子对象

     ```c++
     foreach(Transform child in transform) {
        Destroy(child.gameObject);
     }
     ```

5. 资源预设（Prefabs）与 对象克隆 (clone)

   * 预设（Prefabs）有什么好处？

     应用预设我们可以快速的创建很多有相同或类似属性的对象实例，同时也可以方便的一起修改这些对象的属性。

   * 预设与对象克隆 (clone or copy or Instantiate of Unity Object) 关系？

     预设和克隆都可以快速的创建很多有相同属性的对象实例、

   * 制作 table 预制，写一段代码将 table 预制资源实例化成游戏对象

     ```c++
     public class NewBehaviourScript : MonoBehaviour {
         private string prePath = "prefabs/table";
         void Start () {
             GameObject Table = Instantiate(Resource.Load(prePath), new Vector(4,0,0),Quaternion.identity) as GameObject;
         }
     }
     ```

6. 尝试解释组合模式（Composite Pattern / 一种设计模式）。使用 BroadcastMessage() 方法

   * 向子对象发送消息

     ```c++
     BroadcastMessage("Test");
     ```

     ​