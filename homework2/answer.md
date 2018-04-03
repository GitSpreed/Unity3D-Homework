### 一、简答并用程序验证

* 游戏对象运动的本质是什么？

  游戏对象运动的本质是Transform组件中各项（Position、Rotation、Scale）值的改变。

  ```c#
  void Update() {
      this.transform.position += Vector3.left * Time.deltaTime;
  }
  ```

* 请用三种方法以上方法，实现物体的抛物线运动。

  * 方法一：

    ```c#
    public class move : MonoBehaviour {  
    	
      	public float speed_horizontal = 1;
    	public float speed_vertical = 1; 
    	public float accelerated_speed = 1;

     	// Use this for initialization  
    	void Start () {  
          
    	}  
      
    	// Update is called once per frame  
    	void Update () {  
    		transform.position += Vector3.right * Time.deltaTime * speed_horizontal;  
    		transform.position += Vector3.down * Time.deltaTime * speed_vertical;  
        	speed_vertical += accelerated_speed;  
       	}  
    }  
    ```

  * 方法二：

    ```c#
    public class move : MonoBehaviour {  
    	
      	public float speed_horizontal = 1;
    	public float speed_vertical = 1; 
    	public float accelerated_speed = 1;

     	// Use this for initialization  
    	void Start () {  
          
    	}  
      
    	// Update is called once per frame  
    	void Update () {  
            Vector3 vec = new Vector3(Vector3.right * Time.deltaTime * speed_horizontal, Vector3.down * Time.deltaTime * speed_vertical, 0);
    		transform.position += vec;  
        	speed_vertical += accelerated_speed;  
       	}  
    } 
    ```

  * 方法三：

    ```c#
    public class move : MonoBehaviour {  
    	
      	public float speed_horizontal = 1;
    	public float speed_vertical = 1; 
    	public float accelerated_speed = 1;

     	// Use this for initialization  
    	void Start () {  
          
    	}  
      
    	// Update is called once per frame  
    	void Update () {  
            Vector3 vec = new Vector3(Vector3.right * Time.deltaTime * speed_horizontal, Vector3.down * Time.deltaTime * speed_vertical, 0);
    		this.transform.Translate(vec);  
        	speed_vertical += accelerated_speed;  
       	}  
    }
    ```

* 写一个程序，实现一个完整的太阳系， 其他星球围绕太阳的转速必须不一样，且不在一个法平面上。

  ```c#
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  public class Rotate : MonoBehaviour {

      private Transform point;				//公转围绕的点
      public float speed_GZ, speed_ZZ;		//公转速度与自转速度
      private float rx, ry;

  	// Use this for initialization
  	void Start () {
          speed_GZ = 10;
          speed_ZZ = 1;
          point = this.transform.parent;
          rx = Random.Range(1, 100);
          ry = Random.Range(1, 100);
  	}

      // Update is called once per frame
       void Update () {
          Revolution();
          Rotation_self();
  	}
  	
      //公转
      private void Revolution()
      {
          this.transform.RotateAround(point.position, new Vector3(rx, ry, 0), speed_GZ * Time.deltaTime);
      }

      //自转
      private void Rotation_self()
      {
          this.transform.RotateAround(this.transform.position, Vector3.right, speed_ZZ * Time.deltaTime);
      }
  }

  ```

  ​