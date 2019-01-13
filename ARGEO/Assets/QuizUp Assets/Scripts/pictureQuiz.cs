using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Firebase;
using UnityEngine.SceneManagement;
using Firebase.Database;
using Firebase.Unity.Editor;

public class pictureQuiz : MonoBehaviour {

	[Tooltip("This string must be same as that of the child node name you have used in firebase" +
		"database. See documentation for more")]
	public string categoryNameInDatabse ;

	[Tooltip("The total number of questions that you have in Firebase Database. See documentation for more")]
	public int totalQuestionsinDatabase = 5;

	[Tooltip("The total number of questions a player should answer. It should always be less than" +
		"total questions in database")]
	public int totalQuestionstoAsk = 3;

	[Tooltip("Time for each question. If player fails to answer within this time, the game ends")]
	public float timeForeachQuestion;

	[Tooltip("Create a New Gameobject. Attach DatabaseLinks.cs script to it" +
		"and then insert that gameobject here. See documentation for more")]
	public DatabaseLinks linksScript;
	DatabaseReference reference;

	[Tooltip("UI Image component that shows the downloaded image")]
	public RawImage img;

	[Tooltip("TextFields to show strings")]
	public Text questionfield, showanswerfield,questionNumbertf, timeText, option1Text, option2Text, option3Text, option4Text;

	[Tooltip("TextFields to show strings")]
	public GameObject gameEndPanel;

	[Tooltip("TextFields to show strings")]
	public Text gameEndscore, highScoreText;

	[Tooltip("All buttons")]
	public Button option1Btn, option2Btn, option3Btn, option4Btn;

	// Random index is an integer that is generated at the start of game and before showing new question.
	// Using this randomindex, we identify which question we have to ask from player
	private int randomIndex;

	// This string stores which option is correct
	private string correctAnswerOption;


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

	// Strings to store values retrieved from database
	private string question, option1, option2, option3, option4;

	void Awake(){

		showanswerfield.text = "";

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
		questionNumbertf.text = "Question : " + answeredCount.ToString() + " / " + totalQuestionstoAsk.ToString ();

		// Make the buttons not clickable so that player can't click any option unless
		// everything has been loaded
		option1Btn.interactable = false;
		option2Btn.interactable = false;
		option3Btn.interactable = false;
		option4Btn.interactable = false;


		// Setting this boolean to true would stop countdown timer. The timer will start 
		// only after the question has been retrieved from firebase database. The timer 
		// starts in retrieveQuestionfromDatabase function where the boolean is set to
		// false
		waitingForNextQuestion = true;


		// In Second step, we will retrieve the picture and question String from database
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

		reference.Child(categoryNameInDatabse).Child(randomIndex.ToString()).ValueChanged += HandleValueChanged;
	}

	void HandleValueChanged(object sender, ValueChangedEventArgs args) {
		if (args.DatabaseError != null) {
			Debug.LogError(args.DatabaseError.Message);
			return;
		}
		question = args.Snapshot.Child ("Question").Value.ToString ();
		option1 = args.Snapshot.Child ("a").Value.ToString ();
		option2 = args.Snapshot.Child ("b").Value.ToString ();
		option3 = args.Snapshot.Child ("c").Value.ToString ();
		option4 = args.Snapshot.Child ("d").Value.ToString ();
		correctAnswerOption = args.Snapshot.Child ("Correct").Value.ToString ();
		string picUrl = args.Snapshot.Child ("PictureUrl").Value.ToString ();

		StartCoroutine(downloadimage(picUrl));
	}

	IEnumerator downloadimage(string picurl) {
		WWW www = new WWW (picurl);
		yield return www ;
		if (www.isDone) {

			img.texture = www.texture;

			// setting the textfields
			questionfield.text = question;
			option1Text.text = "A : "+ option1;
			option2Text.text = "B : "+ option2;
			option3Text.text = "C : "+ option3;
			option4Text.text = "D : "+ option4;

			// Make the buttons clickable
			option1Btn.interactable = true;
			option2Btn.interactable = true;
			option3Btn.interactable = true;
			option4Btn.interactable = true;

			// by turning the boolean to false, the timer will Start
			waitingForNextQuestion = false;
		}

	}
		

	public void option1Selected(){

		if (correctAnswerOption.Equals("a")) {
			showanswerfield.text = "Right";
			score++;
		}
		else
			showanswerfield.text = "Wrong";

		StartCoroutine (nextQuestion ());
	}

	public void option2Selected(){

		if (correctAnswerOption.Equals("b")) {
			showanswerfield.text = "Right";
			score++;
		}
		else
			showanswerfield.text = "Wrong";

		StartCoroutine (nextQuestion ());

	}

	public void option3Selected(){

		if (correctAnswerOption.Equals("c")) {
			showanswerfield.text = "Right";
			score++;
		}
		else
			showanswerfield.text = "Wrong";

		StartCoroutine (nextQuestion ());

	}

	public void option4Selected(){

		if (correctAnswerOption.Equals("d")) {
			showanswerfield.text = "Right";
			score++;
		}
		else
			showanswerfield.text = "Wrong";

		StartCoroutine (nextQuestion ());

	}
	IEnumerator nextQuestion(){

		waitingForNextQuestion = true;

		yield return new WaitForSeconds (0.4f);
		// make buttons hide uninteractable so that player don't click any button during 
		// the waiting time. We wait for some time to make allow button animations
		option1Btn.interactable = false;
		option2Btn.interactable = false;
		option3Btn.interactable = false;
		option4Btn.interactable = false;


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
