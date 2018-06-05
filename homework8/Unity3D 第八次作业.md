## Unity3D 第八次作业

### 一、UGUI的实现方法

1. 添加一个Panel 3D对象，用来承载人物。

2. 添加一个预制人物，命名为Ethan，效果如下图所示：

   ![TIM截图20180605222342](E:\Study\3D游戏编程\homework\homework8\picture\TIM截图20180605222342.png)

3. 给Ethan游戏对象添加画布（Canvas）子对象。

4. 给Canvas添加Slider子对象，对象层次如下：

   ![TIM截图20180605222846](E:\Study\3D游戏编程\homework\homework8\picture\TIM截图20180605222846.png)

5. 将Slider 子对象Handle Slider Area设置为不可用（disable）：

   ![TIM截图20180605222907](E:\Study\3D游戏编程\homework\homework8\picture\TIM截图20180605222907.png)

6. 将Background的Color属性设置为黑色：

   ![TIM截图20180605223018](E:\Study\3D游戏编程\homework\homework8\picture\TIM截图20180605223018.png)

7. 将Fill的Color属性设置为红色：

   ![TIM截图20180605223125](E:\Study\3D游戏编程\homework\homework8\picture\TIM截图20180605223125.png)

   这时，运行发现血条的右边总有一段是黑色的（因为禁用了Handle Slider Area属性，而原本这里是滑动Slider的圆点）

8. 为解决上述问题，设置Fill的Right的属性：

   ![TIM截图20180605223424](E:\Study\3D游戏编程\homework\homework8\picture\TIM截图20180605223424.png)

   此时，血条会随着人物的转动而转动：

   ![TIM截图20180605223711](E:\Study\3D游戏编程\homework\homework8\picture\TIM截图20180605223711.png)

9. 添加一个脚本，使得血条总是正对屏幕：

   ```c#
   using UnityEngine;
   
   public class LookAtCamera : MonoBehaviour
   {
   
       void Update()
       {
           this.transform.LookAt(Camera.main.transform.position);
       }
   }
   ```



### 二、IMGUI的是实现方法

1. 编辑以下脚本：

   ```c#
   using System.Collections;
   using System.Collections.Generic;
   using UnityEngine;
   
   public class HealthPanel : MonoBehaviour {
   
       public float healthPanelOffset = 0.35f;
       public GUISkin mySkin;
   
   
       void OnGUI()
       {
           //设置血条样式
           GUI.skin = mySkin;
           Vector3 worldPos = new Vector3(transform.position.x, transform.position.y + healthPanelOffset, transform.position.z);
           Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
           //设置血条随人物移动
           GUI.HorizontalSlider(new Rect(screenPos.x - 50, screenPos.y - 30, 100, 100), 50, 0, 100);
       }
   }
   ```

2. 将上述脚本加载到人物对象上

3. Assets-->Create-->GUI Skin 新建一个自己的UI皮肤，命名为mySkin

4. 对mySkin进行设置，将Horizontal Slider Thumb所有状态下的Background设置为none，将Horizontal Slider所有状态下的Background设置为一张红色的图片。

   ![TIM截图20180605234238](E:\Study\3D游戏编程\homework\homework8\picture\TIM截图20180605234238.png)

   ![TIM截图20180605234218](E:\Study\3D游戏编程\homework\homework8\picture\TIM截图20180605234218.png)

5. 将mySkin拖到脚本中

   ![TIM截图20180605234423](E:\Study\3D游戏编程\homework\homework8\picture\TIM截图20180605234423.png)





### 三、两种实现方式的比较

1. UGUI的实现更加直观（所见即所得），操作更加方便，所有的设置都可以直接通过改变其属性实现
2. IMGUI的实现更加符合程序员的编程习惯，且理论上可以实现更加多样的样式（通过设置GUI Skin），但是实现过程不够直观，效果需要不断的运行来观察。



### 四、效果演示

[演示链接]()



### 五、预制使用方法

两个预制人物分别是两种不同的实现方法，直接拖到场景中即可使用，WASD可以控制人物的移动。