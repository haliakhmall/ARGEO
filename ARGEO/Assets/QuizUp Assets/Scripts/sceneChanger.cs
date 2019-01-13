using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneChanger : MonoBehaviour {

	[Tooltip("The name of scene when user hits back button on device")]
	public string sceneNameOnPressingBack;

	void Update(){

		if (Input.GetKey (KeyCode.Escape)) {
		
			SceneManager.LoadScene (sceneNameOnPressingBack);
		}
	}

	public void changeScene(string sceneName){

		StartCoroutine (loadNewScene (sceneName));
	}

	IEnumerator loadNewScene(string sceneName){

		// This is to allow button animations
		yield return new WaitForSeconds (0.4f);
		SceneManager.LoadScene (sceneName);

	}
}
