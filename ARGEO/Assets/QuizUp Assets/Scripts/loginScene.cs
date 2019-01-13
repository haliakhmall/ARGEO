using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement;

public class loginScene : MonoBehaviour {

	Firebase.Auth.FirebaseAuth auth;
	Firebase.Auth.FirebaseUser user;

	public InputField email_field, password_field;
	public Text pgTextField;

	private string email, password;

	void Start () {

		InitializeFirebase ();
		getCurrentUser ();
	}
		
	#region Authentication Initialization
	// Handle initialization of the necessary firebase modules:
	void InitializeFirebase() {
		Debug.Log("Setting up Firebase Auth");
		auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
	}
	#endregion

	void getCurrentUser()
	{
		string userId = PlayerPrefs.GetString("UID","");
		if (userId.Equals (null) || userId.Equals (""))
			Debug.Log ("Not Signed In");
		else {
		
			Debug.Log ("Already Signed In");
			SceneManager.LoadScene ("mainMenu");
		}


	}
	public void login(){

		email = email_field.text.ToString ();
		password = password_field.text.ToString ();

		if (email.Equals ("") || email.Equals (null) || password.Equals("") || password.Equals(null)) {

			pgTextField.text = "Username and Password Cannot be Empty";
		} else {
			auth.SignInWithEmailAndPasswordAsync (email, password).ContinueWith (task => {
				
				if (task.IsCanceled) {
					Debug.LogError ("SignInWithEmailAndPasswordAsync was canceled.");
					pgTextField.text = "Cancelled";
					return;
				}
				if (task.IsFaulted) {
					Debug.LogError ("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
					pgTextField.text = "Error";
					return;
				}

				// Firebase user has been LoggedIn.
				pgTextField.text = "Signedin";
				Firebase.Auth.FirebaseUser newUser = task.Result;

				string uid = newUser.UserId;
				PlayerPrefs.SetString("UID",uid);
				PlayerPrefs.Save();

				SceneManager.LoadScene("mainMenu");
						
			});
		}
	}


	public void signup(){
		
		SceneManager.LoadScene ("signUp");
		}
	}
