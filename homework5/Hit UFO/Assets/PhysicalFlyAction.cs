using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalFlyAction : SSAction {

	bool enableThrow = true;
	Vector3 force;

	// Use this for initialization
	public override void Start () {
		enable = true;
		gameobject.GetComponent<Rigidbody>().useGravity = true;
		gameobject.GetComponent<Rigidbody>().isKinematic = false;
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
				gameobject.GetComponent<Rigidbody>().velocity = Vector3.zero;  
				gameobject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
                enableThrow = false;
			}
		}
		if (this.gameobject.transform.position.y < -4)
        {
            this.destroy = true;
            this.enable = false;
            this.callback.SSActionEvent(this);
        }
	}

}
