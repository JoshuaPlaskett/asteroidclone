using UnityEngine;
using System.Collections;

/// <summary>
/// Player - The script that controls the player object - handles input and movement / shooting etc
/// </summary>
[RequireComponent (typeof(Rigidbody2D))] //Always requires a rigid body to perform movement on.
public class Player : ShipActor 
{
	private float angularDrag = 5f; //Default angular drag

	public SpriteRenderer cockpit, leftWing, rightWing, engine; //Varibale for each of the sprites the user can adjust
	public Weapon[] myWeapons; //Array of the player weapons - set in the inspector
	private int weaponLevel = 1; //Our current weapon level

	//Variables to store regularly accessed components e.g. rigidbody2D
	private Rigidbody2D _rigidbody2D;

	void Start () 
	{
		//Cache our rigidbody2D
		_rigidbody2D = GetComponent<Rigidbody2D>();
		//cache our angular drag so that we can easily adjust it later
		angularDrag = _rigidbody2D.angularDrag;
	}
	
	void Update () 
	{
		HandleInput();
	}

	/// <summary>
	/// Handles the user input.
	/// </summary>
	private void HandleInput()
	{
		//If we're rotating (which we want to use force for) lower our angular drag temporarily
		if(Input.GetAxis("Horizontal") != 0)
		{
			_rigidbody2D.angularDrag = 0;
		}
		else
		{
			_rigidbody2D.angularDrag = angularDrag;
		}
		//Horizontal axis affects torque
		_rigidbody2D.AddTorque(Input.GetAxis("Horizontal")*-rotateSpeed); //Apply negative rotation speed to rotate in the "correct" perceived direction
		//Vertical axis affects velocity through relative force - relative forced used to add velocity in the direction we're facing
		_rigidbody2D.AddRelativeForce(new Vector2(0,Input.GetAxis("Vertical")*speed)); //Add relative force to take facing direction into consideration
		if(Input.GetButtonDown ("Fire1"))
		{
			//Tell all of our predefined weapons that we want to shoot
			foreach(Weapon weapon in myWeapons)
			{
				weapon.Fire ();
			}
		}
	}

	/// <summary>
	/// Upgrades the players weapons.
	/// </summary>
	public void UpgradeWeapon()
	{
		if(weaponLevel >= 3)
		{
			GameManager.instance.score += 100;
			return;
		}
		weaponLevel++;
		//Tell all our weapons our new level
		foreach(Weapon weapon in myWeapons)
		{
			weapon.weaponLevel = weaponLevel;
		}
	}

	/// <summary>
	/// Kills the player and tells the game manager to change state to game over
	/// </summary>
	override protected void Kill()
	{
		GameManager.instance.CallGameOver(2f); //Wait 2 seconds before showing game over
		//Destroy the player - we will instantiate a new one if we restart
		Destroy(this.gameObject);
	}
}
