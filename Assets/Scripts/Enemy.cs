using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : ShipActor 
{
	protected const float Threshold = 1e-3f; //Threshold for minumum amount of turn

	protected bool canFire = true;

	public GameObject myTarget;
	public int points = 100;

	public GameObject[] drops; //Array of game objects that this enemy can drop on death
	//The percentage chance of dropping an item
	public int chanceOfDrop = 10;

	virtual protected void Start()
	{
		myTarget = GameManager.instance._player();
		GameManager.instance.enemiesInPlay.Add (this.gameObject);
	}

	protected override void Kill ()
	{
		GameManager.instance.enemiesInPlay.Remove(this.gameObject);
		GameManager.instance.score += points;
		if(drops.Length > 0)
		{
			if(Random.Range (0,100) < chanceOfDrop)
			{
				Instantiate (drops[Random.Range (0,drops.Length)],transform.position,Quaternion.Euler(0,0,0));
			}
		}
		base.Kill ();
	}
}
