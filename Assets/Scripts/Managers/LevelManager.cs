using UnityEngine;
using System.Collections;

/// <summary>
/// Level manager - Handles the creation and destruction of levels
/// </summary>
public class LevelManager : MonoBehaviour 
{
	public GameObject asteroid; //Variable to store asteroid prefab (set in inspector)
	public GameObject[] enemies; //Array to store enemy prefabs (set in inspector)

	/// <summary>
	/// Creates a level
	/// </summary>
	/// <param name="asteroids">The amount of asteroids in the level</param>
	/// <param name="enemies">The amount of enemies in the level</param>
	public void CreateLevel(int asteroids, int enemyCount)
	{
		int index = 0;
		//While we still have asteroids or enemies to create
		while(index < asteroids || index < enemyCount)
		{
			//Check if we need more asteroids and then instantiate one in a random position if we do
			if(index < asteroids)
			{
				Instantiate (asteroid,new Vector3(Random.Range (-15f,15f),Random.Range (-15f,15f)),Quaternion.Euler(Vector3.zero));
			}
			//Check if we need more enemies and then instantiate one in a random position if we do
			//Note that we only spawn enemies from our array that are in a position lower than our current level
			//This is to "ease" the player in just a little bit
			if(index < enemyCount)
			{
				Instantiate (enemies[Random.Range (0, GameManager.instance.currentLevel < enemies.Length ? GameManager.instance.currentLevel : enemies.Length)]
					             , new Vector3(Random.Range (-15,15f),Random.Range (-15f,15f)),Quaternion.Euler(Vector3.zero));
			}
			//Remember to increment index to help us break out of the while loop
			index++;
		}
	}
}
