using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class reset : MonoBehaviour {

	public void resetGame(){

		PlayerPrefs.DeleteAll();
		SceneManager.LoadScene ("loginScene");
	}
}
