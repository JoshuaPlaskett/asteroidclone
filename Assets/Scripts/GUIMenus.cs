using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class GUIMenus : MonoBehaviour 
{
	public Player player; //Reference to our player
	private int shipColor = 0; //0 = Blue, 1 = Green, 2 = Yellow, 3 = Red
	private string[] colorStrings = new string[] {"Blue", "Green", "Yellow", "Red"}; //Available ship colours
	private int cockPitPart = 0; //Number of our equipped cockpit part
	private int enginePart = 1; //Number of our equipped engine part
	private int wingPart = 0; //Number of our equipped wing parts
	private string myColor = "Blue"; //Our currently selected colour, Blue by default

	public GUISkin guiSkin; //The skin the GUI will use - set in inspector

	const float NATIVE_WIDTH = 1980f; //Constant native width defined for GUI. Using this with GUI.matrix allows for easier scaling
	const float NATIVE_HEIGHT = 1080f; //Same as constant width but for height

	public AudioClip guiSound; // GUI sound to play when clicking buttons

	public Leaderboard leaderboard; //Variable for our leaderboard for easier access when displaying it in the game over state

	private bool showOptions = false; //Flag used to decide whether to show the options GUI or not, overrides the state

	void Start () 
	{
		//Grab the player from the game manager - used so we can modify it in the starting menu
		player = GameManager.instance._player().GetComponent<Player>();
		//Grab the leaderboard so that we can display it in the game over menu
		leaderboard = GameManager.instance.leaderboard;
	}

	void OnGUI()
	{
		//Unity GUI pre-Unity 5 used here, even though this was made in Unity 5.
		//Set the GUI skin
		GUI.skin = guiSkin;
		//Do GUI scaling using the GUI.matrix. This allows us to use values based off of the constant native values for positioning
		//and size etc and the matrix will sort out scaling it to the screen size
		//PRINCIPLE FOUND ONLINE AT: http://answers.unity3d.com/questions/169056/bulletproof-way-to-do-resolution-independant-gui-s.html
		//Credits to SilverTabby//
		float guiX = Screen.width / NATIVE_WIDTH;
		float guiY = Screen.height / NATIVE_HEIGHT;
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3(guiX, guiY, 1));
		//End credits//

		//If we have set the options flag to true, show the options menu, otherwise show the respective menu based on game state
		if(showOptions)
		{
			GUI.Window(0, new Rect(NATIVE_WIDTH/2-NATIVE_WIDTH/4,50,NATIVE_WIDTH/2,NATIVE_HEIGHT-100), OptionsWindow, "");
		}
		else
		{
			//Check which state we are in and call the respective function to handle the GUI in that state
			switch(GameManager.instance.currentState)
			{
				case GameManager.State.Start :
				{
					StartMenu();
					break;
				}
				case GameManager.State.Game :
				{
					GameMenu ();
					break;
				}
				case GameManager.State.GameOver :
				{
					GameOverMenu();
					break;
				}
			}
		}
	}

	/// <summary>
	/// Plays the GUI sound that was set in the inspector.
	/// </summary>
	void PlayGUISound()
	{
		//Check if the game is muted
		if(!GameManager.instance.soundManager.getMuteStatus())
		{
			//If not, play the sound using the volume from Sound Manager
			GetComponent<AudioSource>().PlayOneShot(guiSound,GameManager.instance.soundManager.volume);
		}
	}

	/// <summary>
	/// The options window for GUI - contains credits, mute and volume.
	/// </summary>
	void OptionsWindow(int windowID)
	{
		Color tempColor = GUI.contentColor;
		GUI.contentColor = Color.black;
		GUILayout.Label ("OPTIONS");
		GUILayout.FlexibleSpace();
		GUI.contentColor = tempColor;
		if(GUILayout.Button (GameManager.instance.soundManager.getMuteStatus() ? "Unmute" : "Mute"))
		{
			GameManager.instance.soundManager.setMuteStatus(!GameManager.instance.soundManager.getMuteStatus());
		}

		GUI.contentColor = Color.black;
		GUILayout.BeginHorizontal();
		{
			GUILayout.Label ("Volume:");
			GUI.contentColor = Color.black;
			GameManager.instance.soundManager.volume = GUILayout.HorizontalSlider(GameManager.instance.soundManager.volume,
					                                                                0f,1f);
			GUI.contentColor = tempColor;
		}
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		GUI.contentColor = Color.black;
		GUILayout.Label ("Credits");
		GUILayout.Label ("Art & Sound Effects");
		GUILayout.Label ("Kenney - www.kenney.nl");
		GUILayout.Label ("Music");
		GUILayout.Label ("Ashok Prema - Unicorn");
		GUILayout.Label ("Steamtech Mayhem - SoundImage");
		GUILayout.FlexibleSpace();
		GUI.contentColor = tempColor;
		if(GUILayout.Button ("Close",GUILayout.MinHeight(100f)))
			showOptions = false;
	}

	/// <summary>
	/// The game's start menu. Allows the user to adjust their ship's appearance and colour and start the game
	/// </summary>
	void StartMenu()
	{
		//Set ourselves up a nice little area to draw our menu
		GUILayout.BeginArea(new Rect(NATIVE_WIDTH/2-NATIVE_WIDTH/4,10,NATIVE_WIDTH/2,NATIVE_HEIGHT-10));
		//Start the game button
		if(GUILayout.Button ("Start",GUILayout.MinHeight(100)))
		{
			//Play the GUI sound
			PlayGUISound ();
			//Enable player control
			player.enabled = true;
			//Set it to active
			player.gameObject.SetActive(true);
			//Zoom the camera out
			Camera.main.orthographicSize = 10;
			//Reset the score
			GameManager.instance.score = 0;
			//Set the game state to "Game"
			GameManager.instance.currentState = GameManager.State.Game;
		}
		//----------------------------------------------//
		//---------------- SHIP CUSTOMISATION ----------//
		//----------------------------------------------//
		GUILayout.BeginHorizontal();
		{
			shipColor = GUILayout.SelectionGrid(shipColor,colorStrings,4,GUILayout.MinHeight(100)); //Selection grid of preset ship colours
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		{
			cockPitPart = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)cockPitPart,0f,7f,GUILayout.MinHeight(100)));
			GUILayout.Label ("Cockpit Part: " + cockPitPart);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		{
			wingPart = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)wingPart,0f,7f,GUILayout.MinHeight(100)));
			GUILayout.Label ("Wing Part: " + cockPitPart);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		{
			enginePart = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)enginePart,1f,5f,GUILayout.MinHeight(100)));
			GUILayout.Label ("Engine Part: " + cockPitPart);
		}
		GUILayout.EndHorizontal();
		GameManager.instance.playerName = GUILayout.TextField(GameManager.instance.playerName); //Allow the player to change their name
		GUILayout.FlexibleSpace();
		//Options menu
		if(GUILayout.Button ("Options",GUILayout.MinHeight(100f)))
		{
			PlayGUISound ();
			showOptions = true;
		}
		GUILayout.EndArea();
		//If there's been a change to the gui, update the player's ship
		if(GUI.changed)
		{
			switch(shipColor)
			{
			case 0:
				myColor = "Blue";
				break;
			case 1:
				myColor = "Green";
				break;
			case 2:
				myColor = "Yellow";
				break;
			case 3:
				myColor = "Red";
				break;
			}
			player.cockpit.sprite = Resources.Load ("Parts/cockpit"+myColor+"_"+cockPitPart, typeof(Sprite)) as Sprite;
			player.leftWing.sprite = Resources.Load ("Parts/wing"+myColor+"_"+wingPart, typeof(Sprite)) as Sprite;
			player.rightWing.sprite = Resources.Load ("Parts/wing"+myColor+"_"+wingPart, typeof(Sprite)) as Sprite;
			player.engine.sprite = Resources.Load ("Parts/engine"+enginePart, typeof(Sprite)) as Sprite;
		}

	}

	void GameMenu()
	{
		//For this section we want to align our labels' text to the left rather than in the centre
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		GUILayout.Label ("Score: " + GameManager.instance.score.ToString("000000"),GUILayout.MinWidth(500));
		GUILayout.Label ("Asteroids Left: " + GameManager.instance.asteroidsInPlay.Count,GUILayout.MinWidth(500));
		GUILayout.Label ("Enemies Left: " + GameManager.instance.enemiesInPlay.Count,GUILayout.MinWidth(500));
		//Reset the skin back to how it's supposed to be
		GUI.skin.label.alignment = guiSkin.label.alignment;
	}

	void GameOverMenu()
	{
		//Make sure that the label alignment has been reset after the game menu
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		//Use the full screen for this section
		GUILayout.BeginArea(new Rect(0,0,NATIVE_WIDTH,NATIVE_HEIGHT));
		{
			GUILayout.BeginVertical();
			{
				GUILayout.Label ("Game Over");
				GUILayout.FlexibleSpace();
				//Our scoreboard area
				GUILayout.BeginVertical();
				{
					//Loop through the top ten positions
					for(int position = 0; position < 10; position++)
					{
						//Check that the value in this position isn't empty or null
						if(leaderboard.getPosition (position) != null && leaderboard.getPosition (position).Contains (";"))
						{
							//If this is our score, highlight it
							if(position == GameManager.instance.tablePosition)
							{
								GUILayout.Label ((position+1) + ": " + leaderboard.getPosition(position).Split (';')[1] + "  -  "
								                 + leaderboard.getPosition(position).Split (';')[0],guiSkin.FindStyle("YourScoreLabel"));
							}
							//Otherwise add it in as normal
							else
							{
								GUILayout.Label ((position+1) + ": " + leaderboard.getPosition(position).Split (';')[1] + "  -  "
							                 + leaderboard.getPosition(position).Split (';')[0]);
							}
						}
					}
					//If the position we came in is not in the top ten, append it to the bottom of the list and highlight it
					if(GameManager.instance.tablePosition >= 10)
					{
						GUILayout.Label ((GameManager.instance.tablePosition+1) + ": " 
						                 + leaderboard.getPosition (GameManager.instance.tablePosition).Split (';')[1] + "  -  " 
						                 + leaderboard.getPosition (GameManager.instance.tablePosition).Split (';')[0],guiSkin.FindStyle("YourScoreLabel"));
					}
				}
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				//Our Game Over menu options
				GUILayout.BeginHorizontal();
				{
					//This button saves the leaderboard then tells the game manager to restart the game
					if(GUILayout.Button ("Restart",GUILayout.MinHeight(100f)))
					{
						PlayGUISound ();
						leaderboard.SaveLeaderboard();
						PlayerPrefs.Save ();
						GameManager.instance.Restart();
					}
					//Options menu
					if(GUILayout.Button ("Options",GUILayout.MinHeight(100f)))
					{
						PlayGUISound ();
						showOptions = true;
					}
					//Save the leaderboard and player prefs in general then quit
					if(GUILayout.Button ("Quit",GUILayout.MinHeight(100f)))
					{
						PlayGUISound ();
						leaderboard.SaveLeaderboard();
						PlayerPrefs.Save ();
						Application.Quit();
					}
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndArea ();
	}
}
