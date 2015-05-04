using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Leaderboard class to perform all leaderboard calculations.
/// Extends ScriptableObject rather than Monobehaviour so does not need to be attached to a GameObject.
/// Used to store large amounts of data - in this case all scores so that we can get a top ten and a position within all scores
/// </summary>
public class Leaderboard : ScriptableObject 
{
	//List to store scores in, position in list is the leaderboard position (-1 because it starts at 0), the value is the score and name
	private List<string> scoreboard = new List<string>();
	//Constant for PlayerPrefs String key value
	const string LEADERBOARD_STRING = "leaderboard";
	
	/// <summary>
	/// Loads the leaderboard from PlayerPrefs.
	/// </summary>
	public void LoadLeaderboard()
	{
		//Parse the PlayerPrefs leaderboard string into a list of scores
		scoreboard = parseStringToLeaderboard(PlayerPrefs.GetString (LEADERBOARD_STRING));
		//Make sure it's sorted by highest score first
		SortScores ();
	}

	/// <summary>
	/// Saves the leaderboard to PlayerPrefs as a long string
	/// </summary>
	public void SaveLeaderboard()
	{
		PlayerPrefs.SetString (LEADERBOARD_STRING, parseLeaderboardToString(scoreboard));
		PlayerPrefs.Save ();
	}

	/// <summary>
	/// Parses the string to a list for the leaderboard.
	/// </summary>
	/// <returns>The leaderboard list.</returns>
	/// <param name="theString">The leaderboard as a string.</param>
	private List<string> parseStringToLeaderboard(string theString)
	{
		//Create a list to return at the end
		List<string> finalScoreboard = new List<string>();
		//Split the string by '/' and loop through the array of strings
		foreach(string score in theString.Split ('/'))
		{
			//Add each one to the temporary list of scores
			finalScoreboard.Add (score);
		}
		//Sort the temporary list so lowest score is first
		finalScoreboard.Sort();
		//Then reverse is so the highest score is first
		scoreboard.Reverse();
		//Return this temporary list
		return finalScoreboard;
	}

	/// <summary>
	/// Parses the leaderboard into a string.
	/// </summary>
	/// <returns>The leaderboard as a string.</returns>
	/// <param name="theList">The leaderboard as a list.</param>
	private string parseLeaderboardToString(List<string> theList)
	{
		//Create an empty string
		string scores = "";
		//For each score in the given list
		for(int scoreIndex = 0; scoreIndex < theList.Count; scoreIndex++)
		{
			//Check if our scores contains any scores yet and if not, set it to this first score
			if(scores == "")
				scores = theList[scoreIndex];
			//Otherwise, append this score to the end of the string
			else
				scores = scores + theList[scoreIndex];
			//If it's not the last score in the list, add a "/" to be used as a delimiter when parsing this string later
			if(scoreIndex < theList.Count - 1)
				scores = scores + "/";
		}
		//Return the string of scores
		return scores;
	}

	/// <summary>
	/// Evaluates the score and returns the position in the leaderboard where it fits
	/// </summary>
	/// <returns>The position in the leaderboard.</returns>
	/// <param name="score">The score to evaluate.</param>
	public int evaluateScore(int score)
	{
		//Add this score in the correct format (score first then the player's name, separated by a ";")
		scoreboard.Add (score.ToString("000000")+";"+GameManager.instance.playerName);
		//Sort the scores into highest first
		SortScores();
		//Return the position of the score just provided by getting its new index in the list
		return scoreboard.IndexOf(score.ToString("000000")+";"+GameManager.instance.playerName);
	}

	/// <summary>
	/// Sorts the scores.
	/// </summary>
	private void SortScores()
	{
		//Sort the list using the built in sort function for lists, puts lowest score first
		scoreboard.Sort ();
		//Reverse it to get highest score first
		scoreboard.Reverse ();
	}

	/// <summary>
	/// Gets the score of a given position. Returns null if there's not a score at the given position yet
	/// </summary>
	/// <returns>The score string in format "name;score".</returns>
	/// <param name="position">The position to return.</param>
	public string getPosition(int position)
	{
		if(scoreboard.Count > position)
			return scoreboard[position];
		else
			return null;
	}
}
