using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loopAudio : MonoBehaviour {

	void Awake(){

		DontDestroyOnLoad (this);
		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
		}
	}
}
