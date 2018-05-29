using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inner : MonoBehaviour
{

    private ParticleSystem _particle_system;
    private ParticleSystem.Particle[] _particle_array;

    public int particleNum = 3000;
    public float baseRadius = 2f;
    public float floating = 0.1f;
    public float speed = 1.0f;
    private bool flag = true; 

    private StarParticle[] property;


    // Use this for initialization
    void Start()
    {
        _particle_system = GetComponent<ParticleSystem>();
        _particle_array = new ParticleSystem.Particle[particleNum];
        property = new StarParticle[particleNum];

        //_particle_system.maxParticles = particleNum;
        _particle_system.Emit(particleNum);
        _particle_system.GetParticles(_particle_array);

        for (int i = 0; i < particleNum; i++)
        {
            float angle = Random.Range(0, Mathf.PI * 2);
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
                property[i].angle -= Mathf.PI * 2;
            if (flag)
            {
                property[i].radius -= floating;
                if (property[i].radius <= 0.0f) flag = false;
            }
            else
            {
                property[i].radius += floating;
                if (property[i].radius >= baseRadius) flag = true;
            }
            
            _particle_array[i].position = property[i].getPosition();
        }
        _particle_system.SetParticles(_particle_array, _particle_array.Length);
    }
}
