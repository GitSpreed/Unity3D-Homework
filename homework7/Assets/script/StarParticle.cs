using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        float defAngle = Mathf.Abs(angle - (int)(angle / Mathf.PI * 3.0f) * Mathf.PI / 3.0f - Mathf.PI / 6.0f);
        float len = radius * Mathf.Sin(defAngle) * Mathf.Cos(Mathf.PI * 2 / 3 - defAngle) / Mathf.Sin(Mathf.PI * 2 / 3 - defAngle);
        return new Vector3(len * Mathf.Cos(angle), 0, len * Mathf.Sin(angle));
    }

}
