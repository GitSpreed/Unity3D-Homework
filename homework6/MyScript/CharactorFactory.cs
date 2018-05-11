using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorFactory : MonoBehaviour {

	private List<GameObject> used = new List<GameObject>();
	private List<GameObject> free = new List<GameObject>();

	List<GameObject> getPatrols() {
		int[] pos_x = {8, 12, 10, -5, -9, -4, -10};
		int[] pos_y = {19, 10, -15, 16, 10, -1, -14};
		int[] number = {1, 2, 3, 4, 4, 5, 6};

		for (int i = 0; i < 9; i++) {

		}

	}
}
