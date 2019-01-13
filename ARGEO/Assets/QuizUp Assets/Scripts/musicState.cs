using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class musicState : MonoBehaviour {

	public Sprite musicOff, musicOn;

	private string musicStateString;
	private RawImage icon;
	private Button button;
	private GameObject backgroundAudio;
	private AudioSource audiosource;

	// Use this for initialization
	void Start () {

		button = this.gameObject.GetComponent<Button> ();
		icon = this.gameObject.GetComponentInChildren<RawImage> ();
		backgroundAudio = GameObject.FindGameObjectWithTag ("BackgroundAudio");
		audiosource = backgroundAudio.GetComponent<AudioSource> ();

		musicStateString = PlayerPrefs.GetString ("MusicState", null);


		// For first time Users
		if (musicStateString.Equals (null) || musicStateString.Equals ("")) {

			icon.texture = musicOn.texture;

			// for first time users, we set it to Yes so its easier to handle later
			PlayerPrefs.SetString ("MusicState", "Yes");
			PlayerPrefs.Save ();
			audiosource.volume = 0.4f;
		}

		if (musicStateString.Equals ("Yes")) {
			icon.texture = musicOn.texture;
			audiosource.volume = 0.4f;
		}

		if (musicStateString.Equals ("No")) {
			icon.texture = musicOff.texture;
			audiosource.volume = 0f;
		}

		button.onClick.AddListener (buttonClicked);


	}
	
	void buttonClicked(){

		musicStateString = PlayerPrefs.GetString ("MusicState", null);

		if (musicStateString.Equals ("Yes")) {
			icon.texture = musicOff.texture;
			PlayerPrefs.SetString ("MusicState", "No");
			PlayerPrefs.Save ();
			Debug.Log ("Music turned Off");
			audiosource.volume = 0f;
		}

		if (musicStateString.Equals ("No")) {
			icon.texture = musicOn.texture;
			PlayerPrefs.SetString ("MusicState", "Yes");
			PlayerPrefs.Save ();
			Debug.Log ("Music turned On");
			audiosource.volume = 0.4f;
		}
	}
}
