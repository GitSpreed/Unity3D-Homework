using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarenEvent : MonoBehaviour {

	public delegate void GarenSubject (GameObject self, string message);
	public static event GarenSubject OnGarenSubjectNotify;

	void JumpTopPoint() {
		Debug.Log ("Jump to the top point!!!");
		if (OnGarenSubjectNotify != null)
			OnGarenSubjectNotify (this.gameObject, "AtTop");
	}
}
