# 粒子 Flower

### 效果示意图

![](https://github.com/GitSpreed/Unity3D-Homework/blob/master/homework7/picture/TIM%E6%88%AA%E5%9B%BE20180530002541.png?raw=true)

### 实现代码

* 每个粒子的属性代码 
**StarParticle.cs**
```c#
public class StarParticle {
    public float radius;
    public float angle;
	
    public StarParticle(float r, float a)
    {
        radius = r;
        angle = a;
    }

    //根据角度和花朵半径，计算粒子的坐标
    public Vector3 getPosition()
    {
        //计算粒子在每朵花瓣中的偏转角
        float defAngle = Mathf.Abs(angle - (int)(angle / Mathf.PI * 3.0f) * Mathf.PI / 3.0f - Mathf.PI / 6.0f);
        //计算粒子距离中心的距离
        float len = radius * Mathf.Sin(defAngle) * Mathf.Cos(Mathf.PI * 2 / 3 - defAngle) / Mathf.Sin(Mathf.PI * 2 / 3 - defAngle);
        //返回粒子坐标
        return new Vector3(len * Mathf.Cos(angle), 0, len * Mathf.Sin(angle));
    }

}
```



* 为了花朵可以有一个清晰的边界，所以在最外围创建一个对象outer，outer的例子在固定半径上做旋转运动，构成了花朵的边界。

  **Outer.cs**
```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outer : MonoBehaviour {

    private ParticleSystem _particle_system;
    private ParticleSystem.Particle[] _particle_array;

	//粒子个数
    public int particleNum = 3000;
    //花朵半径
    public float baseRadius = 2f;
    //旋转速度
    public float speed = 0.3f;

    private StarParticle[] property;

    // 初始化各属性
    void Start () {
        _particle_system = GetComponent<ParticleSystem>();
        _particle_array = new ParticleSystem.Particle[particleNum];
        property = new StarParticle[particleNum];
        _particle_system.Emit(particleNum);
        _particle_system.GetParticles(_particle_array);

        for (int i = 0; i < particleNum; i++)
        {
            float angle = Random.Range(0, Mathf.PI * 2);
            float radius = baseRadius;
            property[i] = new StarParticle(radius, angle);
            _particle_array[i].position = property[i].getPosition();
        }
        _particle_system.SetParticles(_particle_array, _particle_array.Length);

    }
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < particleNum; i++)
        {
            //旋转角度增加
            property[i].angle += Random.Range(0, 1.0f / 360) * speed;
            if (property[i].angle > Mathf.PI * 2)
                property[i].angle -= Mathf.PI * 2;
            //得到当前的位置
            _particle_array[i].position = property[i].getPosition();

        }
        _particle_system.SetParticles(_particle_array, _particle_array.Length);
        
    }
}
```



* 花朵内部需要有填充才更加好看，所以另外新建一个对象inner，填充花朵的内部

  **inner.cs**

  ```c#
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  public class inner : MonoBehaviour
  {

      private ParticleSystem _particle_system;
      private ParticleSystem.Particle[] _particle_array;

      public int particleNum = 3000;
      public float baseRadius = 2f;
      public float speed = 1.0f;
      
      private StarParticle[] property;
      
      // Use this for initialization
      void Start()
      {
          _particle_system = GetComponent<ParticleSystem>();
          _particle_array = new ParticleSystem.Particle[particleNum];
          property = new StarParticle[particleNum];
          _particle_system.Emit(particleNum);
          _particle_system.GetParticles(_particle_array);

          for (int i = 0; i < particleNum; i++)
          {
              float angle = Random.Range(0, Mathf.PI * 2);
              //该处初始化与Outer不一样，Outer为固定边界，而inner为随机范围，使得粒子分布在一个环形的范围内
              float radius = Random.Range(baseRadius / 4 * 3, baseRadius);
              property[i] = new StarParticle(radius, angle);
              _particle_array[i].position = property[i].getPosition();
          }
          _particle_system.SetParticles(_particle_array, _particle_array.Length);
      }

      // Update is called once per frame
      void Update()
      {
          for (int i = 0; i < particleNum; i++)
          {
              property[i].angle += Random.Range(0, 1.0f / 360) * speed;
              if (property[i].angle > Mathf.PI * 2)
                  property[i].angle -= Mathf.PI * 2；
              _particle_array[i].position = property[i].getPosition();
          }
          _particle_system.SetParticles(_particle_array, _particle_array.Length);
      }
  }
  ```

  inner只在初始化的注释处和Outer不一样，inner的粒子分布在一个范围内

* 为了使得花朵更加的动态，添加一下代码，使得花朵内部的半径是变化的，这样花朵便会由大变小，然后由小变大。

  ```c#
  	//添加属性
  	public float floating = 0.1f;	//花朵变化速率
      private bool flag = true; 		//用于判断此时花朵是变大还是变小

  	//添加在Update里的代码
  	if (flag)
      {
          //花朵变小
          property[i].radius -= floating;
          if (property[i].radius <= 0.0f) flag = false;
      }
      else
      {
          //花朵变大
          property[i].radius += floating;
          if (property[i].radius >= baseRadius) flag = true;
      }
  ```



### 演示视频

[点击这里查看演示视频]()
