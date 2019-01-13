using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Firebase;
using UnityEngine.SceneManagement;
using Firebase.Database;
using Firebase.Unity.Editor;


public class trueFalseManager : MonoBehaviour {

	[Tooltip("This string must be same as that of the child node name you have used in firebase" +
		"database. See documentation for more")]
	public string categoryNameInDatabse ;

	[Tooltip("The total number of questions that you have in Firebase Database. See documentation for more")]
	public int totalQuestionsinDatabase = 5;

	[Tooltip("The total number of questions a player should answer. It should always be equal or less than" +
		"total questions in database")]
	public int totalQuestionstoAsk = 3;

	[Tooltip("Time for each question. If player fails to answer within this time, the game ends")]
	public float timeForeachQuestion;

	[Tooltip("Create a New Gameobject. Attach DatabaseLinks.cs script to it" +
		"and then insert that gameobject here. See documentation for more")]
	public DatabaseLinks linksScript;
	DatabaseReference reference;

	[Tooltip("TextFields to show strings")]
	public Text questionfield, showanswerfield, timeText;

	[Tooltip("TextFields to show strings")]
	public GameObject gameEndPanel;

	[Tooltip("TextFields to show strings")]
	public Text gameEndscore, highScoreText, questionNumberTf;

	[Tooltip("True and False clickable buttons")]
	public Button truebtn, falsebtn;

	// Random index is an integer that is generated at the start of game and before showing new question.
	// Using this randomindex, we identify which question we have to ask from player
	private int randomIndex;

	// A boolean that changes its state depending on whether the current question is true or false
	private bool isTrue;

	// This boolean is used to stop the countdown while we are waiting for next question
	private bool waitingForNextQuestion = false;

	// A dynamic list which holds the random Indexes that are already asked. It helps us 
	// in creating a workflow where no question is ever repeated
	private static List<int> alreadyAnswered = new List<int> ();

	// An integer variable which increases its value at the start of each question.
	// It is used to determine if all the questions as defined by totalQuestionstoAsk variable 
	// have been asked from the user.
	private static int answeredCount;

	// Score
	private static int score;

	void Awake(){

		// Set up the Editor before calling into the realtime database.
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl (linksScript.databaseReferenceLink);
		// Get the root reference location of the database.
		reference = FirebaseDatabase.DefaultInstance.RootReference;

		// First of all we will generate a random variable which will act as a child node for question.
		// This random variable should be different everytime and shall never repeat so that no 
		// question is repeated throughout the game
		generateRandomQuestion ();

	}

	// Use this for initialization
	void Start () {
		
		answeredCount++;
		gameEndPanel.SetActive (false);
		questionfield.text = "Looking for question...";
		questionNumberTf.text = "Question : " + answeredCount.ToString() + " / " + totalQuestionstoAsk.ToString ();

		truebtn.interactable = false;
		falsebtn.interactable = false;

		// Setting this boolean to true would stop countdown timer. The timer will start 
		// only after the question has been retrieved from firebase database. The timer 
		// starts in retrieveQuestionfromDatabase function where the boolean is set to
		// false
		waitingForNextQuestion = true;

		// In Second step, we will retrieve the question String from database
		retrieveQuestionfromDatabase ();
	


	}
		

	void Update(){

		if (!waitingForNextQuestion) {
			if (timeForeachQuestion > 0f) {
				timeForeachQuestion = timeForeachQuestion - Time.deltaTime;
				timeText.text = timeForeachQuestion.ToString ("F1");
			} else {
				gameEnd ();
				return;
			}
		}
	}

	void generateRandomQuestion()
	{
		randomIndex = Random.Range(1, totalQuestionsinDatabase);
		while(alreadyAnswered.Contains(randomIndex))
		{
			randomIndex = Random.Range(1, totalQuestionsinDatabase);
		}

		alreadyAnswered.Add (randomIndex);
	}

	void retrieveQuestionfromDatabase(){

		reference.Child("TrueFalse").Child(categoryNameInDatabse).Child(randomIndex.ToString()).ValueChanged += HandleValueChanged;

	}

	void HandleValueChanged(object sender, ValueChangedEventArgs args) {
		if (args.DatabaseError != null) {
			Debug.LogError(args.DatabaseError.Message);
			return;
		}
		string question = args.Snapshot.Child ("Question").Value.ToString ();
		string answer = args.Snapshot.Child ("Answer").Value.ToString ();

		if (answer.Equals ("True"))
			isTrue = true;
		else if (answer.Equals ("False"))
			isTrue = false;

		questionfield.text = question;
	
		waitingForNextQuestion = false;

		// make the buttons clickable
		truebtn.interactable = true;
		falsebtn.interactable = true;
	}
		
	public void trueSelected(){

		if (isTrue) {
			showanswerfield.text = "Right";
			score++;
		}
			else
			showanswerfield.text = "Wrong";

		StartCoroutine (nextQuestion ());
	}

	public void falseSelected(){
	
		if (!isTrue) {
			score++;
			showanswerfield.text = "Right";
		}
			else
			showanswerfield.text = "Wrong";

		StartCoroutine (nextQuestion ());

	}

	IEnumerator nextQuestion(){

		// make the buttons not interactable so that player doesn't choose any other choice
		// while we are waiting for next question
		truebtn.interactable = false;
		falsebtn.interactable = false;

		waitingForNextQuestion = true;
		yield return new WaitForSeconds (1f);
	
		if (answeredCount == totalQuestionstoAsk) {
			showanswerfield.text = "All Questions Asked";
			gameEnd ();
		} else {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}
	}

	private void gameEnd(){

		gameEndPanel.SetActive (true);
		gameEndscore.text = score.ToString ();

		// Calculating and storing highscores
		int highscore = PlayerPrefs.GetInt (categoryNameInDatabse, 0);
		if (highscore.Equals (null) || highscore.Equals ("") || highscore.Equals(0)) {

			highscore = score;
			highScoreText.text = highscore.ToString();
			PlayerPrefs.SetInt (categoryNameInDatabse, highscore);
			PlayerPrefs.Save ();
		} else {

			if (score > highscore) {
				highscore = score;
				highScoreText.text = highscore.ToString ();
				PlayerPrefs.SetInt (categoryNameInDatabse, highscore);
				PlayerPrefs.Save ();
			}

			if (score < highscore) {

				highScoreText.text = highscore.ToString ();
			}
		}
		
	highScoreText.text = highscore.ToString ();

		//Reset variables
		alreadyAnswered.Clear();
		answeredCount = 0;
		score = 0;
	}

	public void retry(){

		string sceneName = SceneManager.GetActiveScene ().name.ToString ();
		StartCoroutine (loadNewScene (sceneName));
	}

	public void tomenu(){

		StartCoroutine (loadNewScene ("mainMenu"));
	}

	IEnumerator loadNewScene(string sceneName){

		// This is to allow button animations
		yield return new WaitForSeconds (0.4f);
		SceneManager.LoadScene (sceneName);

	}
}
