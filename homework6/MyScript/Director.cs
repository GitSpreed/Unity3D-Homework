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
