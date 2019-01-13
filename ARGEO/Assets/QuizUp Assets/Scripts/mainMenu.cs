using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour {

	public void changeScene(string sceneName){

		StartCoroutine (loadNewScene (sceneName));
	}

	IEnumerator loadNewScene(string sceneName){

		// This is to allow button animations
		yield return new WaitForSeconds (0.4f);
		SceneManager.LoadScene (sceneName);

	}

	public void logOut(){

		PlayerPrefs.SetString ("UID", null);
		PlayerPrefs.Save ();
		StartCoroutine (loadNewScene ("loginScene"));	
	}
}
