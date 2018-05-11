using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour {

	public delegate void EscapeEvent();
	public static event EscapeEvent escape;

	public delegate void CatchEvent();
	public static event CatchEvent becatch;

	public delegate void AreaEvent();
	public static event AreaEvent area;

	public void PlayerEscape() {
		if (escape != null) {
			escape();
		}
	}

	public void CatchPlayer() {
		if (becatch != null) {
			becatch();
		}
	}

	public void AreaChange(int param) {
		if (area != null) {
			area(param);
		}
	}
}
