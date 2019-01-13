using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.SceneManagement;
using Firebase.Unity.Editor;

public class getUserInfoSettingsScene : MonoBehaviour {

	FirebaseDatabase database;
	DatabaseReference reference;

	public DatabaseLinks linksScript;
	public Text usernameField;
	public InputField changeNameField;

	string username, uid;

	// Use this for initialization
	void Start () {

		InitializeFirebase ();
		getUserInfo ();
	}

	void InitializeFirebase(){
		
		#region settingUp Database Link
		// Set up the Editor before calling into the realtime database.
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl (linksScript.databaseReferenceLink);
		// Get the root reference location of the database.
		reference = FirebaseDatabase.DefaultInstance.RootReference;
		#endregion

	}
	
	void getUserInfo(){

		uid = PlayerPrefs.GetString ("UID", null);
		if (uid.Equals(null) || uid.Equals("")) {
			Debug.Log ("Cannot Get User Name");
		}
		else
			reference.Child ("Users").Child (uid).ValueChanged += HandleValueChanged;
	}
		
	void HandleValueChanged(object sender, ValueChangedEventArgs args) {
		if (args.DatabaseError != null) {
			Debug.LogError(args.DatabaseError.Message);
			return;
		}
		string username = args.Snapshot.Child ("username").Value.ToString ();
		usernameField.text = username.ToUpper();
		Debug.Log (username);
	}

	public void changeName(){

		string newName = changeNameField.text.ToString ();
		reference.Child ("Users").Child (uid).Child ("username").SetValueAsync (newName);
		usernameField.text = newName.ToUpper();
		changeNameField.text = "";
	}
}
