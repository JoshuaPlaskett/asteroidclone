using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent (typeof(AudioSource))]
[RequireComponent (typeof(Leaderboard))]
[RequireComponent (typeof(SoundManager))]
[RequireComponent (typeof(LevelManager))]
/// <summary>
/// Game manager - Handles the overall game behaviours.
/// Singleton so that any class may speak with the game manager.
/// Holds reference to other managers so that an object may make requests of them through the game manager
/// </summary>
public class GameManager : MonoBehaviour 
{
	//List of the asteroids that are currently in play
	public List<GameObject> asteroidsInPlay;
	//List of enemies that are currently in play
	public List<GameObject> enemiesInPlay;

	public GameObject playerPrefab; //Player prefab for spawning new player objects from when we restart

	//Reference to the player for enemies etc to access
	public GameObject player;
	/// <summary>
	/// Sound Manager - use to check mute status or volume level
	/// </summary>
	/// <value>The sound manager.</value>
	public SoundManager soundManager;

	/// <summary>
	/// Leaderboard used to evaluate and return scores
	/// </summary>
	/// <value>The leaderboard.</value>
	public Leaderboard leaderboard;

	public int tablePosition;
	/// <summary>
	/// The level manager - used to create levels
	/// </summary>
	public LevelManager levelManager;

	//Singleton
	//private reference
	private static GameManager _instance;
	//public reference
	public static GameManager instance
	{
		get
		{
			//Check if private reference exists, if not, find this and refer to it
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<GameManager>();
			return _instance;
		}
	}
	//Game states
	public enum State
	{
		Start,
		Game,
		GameOver
	};
	//The current state of the game
	[HideInInspector]
	public State currentState;
	//The current level we are on
	[HideInInspector]
	public int currentLevel = 0;
	//The player's name - by default, "Player 1"
	[HideInInspector]
	public string playerName = "Player 1";
	//The player's score
	[HideInInspector]
	public int score = 0;
	//An array of music clips to randomly play through as the background music
	public AudioClip[] musicClips;
	//Our audio source
	[HideInInspector]
	public AudioSource audioSource;

	void Start () 
	{
		//Check to see which scene we're in - if we're on the logo scene, load the game scene after a short period
		if(Application.loadedLevelName == "Logo")
		{
			Invoke("LoadGameScene",3f);
		}
		//On start set state to Start for menu management
		currentState = State.Start;

		//Make sure this doesn't get destroyed
		DontDestroyOnLoad(this.gameObject);
		if(leaderboard == null)
			leaderboard = Leaderboard.CreateInstance<Leaderboard>();
		//Load up the scores from PlayerPrefs
		leaderboard.LoadLeaderboard();
		if(soundManager == null)
			soundManager = SoundManager.CreateInstance<SoundManager>();
		audioSource = GetComponent<AudioSource>();
	}

	/// <summary>
	/// Loads the "Game" scene.
	/// </summary>
	void LoadGameScene()
	{
		Application.LoadLevel("Game");
	}

	/// <summary>
	/// Raises the level was loaded event.
	/// </summary>
	void OnLevelWasLoaded()
	{
		//If we're on the "Game" level
		if(Application.loadedLevelName == "Game")
		{
			//Instantiate an instance of our player prefab
			player = Instantiate(playerPrefab, Vector3.zero, transform.rotation) as GameObject;
			//Disable the player class of our player gameobject so that we can't move it about in the game menu
			player.GetComponent<Player>().enabled = false;
		}
	}

	void Update () 
	{
		//If we're in the game state and there are no asteroids and no enemies in play, go to the next level
		if(currentState == State.Game && asteroidsInPlay.Count == 0 && enemiesInPlay.Count == 0)
		{
			NextLevel();
		}
		//If our music has stopped, play a random track from our array of music
		if(!audioSource.isPlaying)
		{
			audioSource.clip = musicClips[Random.Range (0,musicClips.Length)];
			//Check if the game isn't muted first
			if(!soundManager.getMuteStatus())
			{
				audioSource.Play ();
			}
		}
	}
	
	/// <summary>
	/// Increments the level number and spawns more asteroids and enemies.
	/// </summary>
	void NextLevel()
	{
		//Increase the level we're on
		currentLevel++;
		//Limit the amount of asteroids we create per level so we don't get a screen swamped full of asteroids!
		int asteroidsToCreate = 2*currentLevel;
		if(asteroidsToCreate > 5)
			asteroidsToCreate = 5;
		//Tell our level manager to create a new level with the given amount of asteroids and enemies
		levelManager.CreateLevel(asteroidsToCreate,2*currentLevel);
	}

	/// <summary>
	/// Countdown to show GameOver - creates a small delay between dying and seeing the game over screen
	/// </summary>
	/// <param name="delay">Delay before game over state is set and therefore menu shown</param>
	public void CallGameOver(float delay)
	{
		Invoke ("GameOver",delay);
	}

	/// <summary>
	/// Turns the state to game over and does a high score check
	/// </summary>
	void GameOver()
	{
		currentState = State.GameOver;
		//Pass the player's score to the leaderboard to check
		tablePosition = leaderboard.evaluateScore(score);
		print ("Table position: " + tablePosition);
	}

	/// <summary>
	/// Resets the game - clears asteroids and enemies list, resets the level back to 0, sets the state back to start and reloads the scene
	/// </summary>
	public void Restart()
	{
		//Clear all asteroids
		while(asteroidsInPlay.Count > 0)
		{
			asteroidsInPlay.RemoveAt(0);
		}
		//Clear all enemies
		while(enemiesInPlay.Count > 0)
		{
			enemiesInPlay.RemoveAt (0);
		}
		//Reset the current level to zero
		currentLevel = 0;
		//Set the state back to start
		currentState = State.Start;
		//Reload the level to clear the scene
		Application.LoadLevel(Application.loadedLevel);
	}

	/// <summary>
	/// Returns the GameObject of the player
	/// </summary>
	public GameObject _player()
	{
		return player;
	}
}
