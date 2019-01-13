using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using Firebase.Database;
using Firebase.Unity.Editor;



public class signUpscene : MonoBehaviour {



	public DatabaseLinks linksScript;
	Firebase.Auth.FirebaseAuth auth;
	DatabaseReference reference;

	public InputField name_field, email_field, password_field, verifypass_field;
	public Text pgTextField;

	private string username, email, password, verifypass;

	void Awake(){

		auth =  Firebase.Auth.FirebaseAuth.DefaultInstance;
	}

	void Start(){
		
		// Set up the Editor before calling into the realtime database.
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl (linksScript.databaseReferenceLink);

		// Get the root reference location of the database.
		reference = FirebaseDatabase.DefaultInstance.RootReference;
	}


	void Update(){

		if (Input.GetKey (KeyCode.Escape)) {

			SceneManager.LoadScene ("loginScene");
		}
	}

	public void signup(){

		username = name_field.text.ToString ();
		email = email_field.text.ToString ();
		password = password_field.text.ToString ();
		verifypass = verifypass_field.text.ToString ();

		if (username.Equals("") || username.Equals(null)|| email.Equals ("") || email.Equals (null) || 
			password.Equals("") || password.Equals(null) 
			|| verifypass.Equals("") || verifypass.Equals(null)) {

			pgTextField.text = "Information is not correct";

		} else {

			if (!password.Equals (verifypass)) {

				pgTextField.text = "Passwords don't match";

			} else {
				
				auth.CreateUserWithEmailAndPasswordAsync (email, password).ContinueWith (task => {

					if (task.IsCanceled) {
						pgTextField.text = "Canceled";
						Debug.LogError ("CreateUserWithEmailAndPasswordAsync was canceled.");
						return;
					}
					if (task.IsFaulted) {
						pgTextField.text = "Error";
						Debug.LogError ("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
						return;
					}

					// Firebase user has been LoggedIn.
					pgTextField.text = "Account Created Successfully";
					Firebase.Auth.FirebaseUser newUser = task.Result;
					string userId = newUser.UserId;
					Debug.Log (userId);

					// Write the user details to firebase database
					writenewUser(userId, username, email);
				
					PlayerPrefs.SetString("UID",userId);
					PlayerPrefs.Save();

					SceneManager.LoadScene ("loginScene");

				});
			}
		}
	}

	private void writenewUser(string userId, string user_name, string user_email){

		firebaseUserHelper userinfo = new firebaseUserHelper (user_name , user_email);
		string json = JsonUtility.ToJson (userinfo);
		reference.Child ("Users").Child (userId).SetRawJsonValueAsync (json);

		// Another way. This way we don't need helper class
		//reference.Child("Users").Child(userId).Child("name").SetValueAsync(user_name);

	}


	public void tologinScene(){

		SceneManager.LoadScene ("loginScene");
	}
}
