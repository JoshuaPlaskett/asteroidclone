using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent (typeof(AudioSource))]
/// <summary>
/// A basic weapon that looks a set distance in front of it and fires at anything it spots
/// </summary>
public class Weapon : MonoBehaviour 
{
	//Auto-detection of targets - if true, will fire automatically
	public bool autoDetect = true;
	//Distance to check for targets
	public float rangeCheck = 10f;
	//The delay between shots (if not through user input)
	public float shootDelay = 1f;
	//The bullet prefab to spawn
	public GameObject bullet;
	private List<GameObject> bulletList = new List<GameObject>(); //List to pool our bullets into
	//Number of bullet objects to spawn into our object pool
	public int numberOfBullets = 40;
	//Our current weapon level
	public int weaponLevel = 1;
	//Can the weapon fire
	protected bool canFire = true;
	
	//Array of gun sounds - random one chosen to play when fired
	public AudioClip[] gunSounds;

	//Implement the Enumerator interface so that we can Instantiate lots of bullets without impacting gameplay
	virtual protected IEnumerator Start () 
	{
		//Instantiate all the bullets we need
		for(int i = 0; i < numberOfBullets; i++)
		{
			GameObject newBullet; //Create a new game object variable for a new bullet holder
			//Set it the an instance of the bullet prefab we have
			newBullet = Instantiate(bullet,new Vector3(transform.position.x, transform.position.y, -0.1f),transform.rotation) as GameObject;
			//Parent it to this weapon to keep our inspector clean
			newBullet.transform.parent = transform;
			//Give it the same tag as the weapon so we don't end up shooting ourselves
			newBullet.tag = this.tag;
			//Add it to our bullet object pool
			bulletList.Add (newBullet);

			yield return null;
		}
		yield return null;
	}

	virtual protected void Update()
	{
		//If the weapon is automatic
		if(autoDetect)
		{
			//Draw a debug ray so we can see where we're aiming
			Debug.DrawRay(transform.position,-transform.up*rangeCheck);
			//Set up an array to hold all the objects our ray will hit
			RaycastHit2D[] hits;
			//Cast a ray out forwards a predefined distance
			hits = Physics2D.RaycastAll(transform.position, -transform.up, rangeCheck);
			//Loop through all the hit objects
			for(int hitIndex = 0; hitIndex < hits.Length; hitIndex++) 
			{
				//Get our hit at the given index
				RaycastHit2D hit = hits[hitIndex];
				//Check the tag isn't the same as our weapon's
				if(hit.transform.tag != this.tag)
				{
					//If we can shoot...
					if(canFire)
					{
						//Then shoot
						Fire ();
						//We need to "reload", so we cannot shoot
						canFire = false;
						//Invoke the function that enables us to shoot again after the given delay
						Invoke ("EnableFire", shootDelay);
					}
					//Break out of the loop, we only want to shoot at the first thing we hit that wasn't of the same tag as us
					break;
				}
			}
		}
	}

	/// <summary>
	/// Basic fire function
	/// </summary>
	virtual public void Fire()
	{
		//If we're not muted, play the fire sound at half-volume
		if(!GameManager.instance.soundManager.getMuteStatus())
			GetComponent<AudioSource>().PlayOneShot(gunSounds[0],GameManager.instance.soundManager.volume/2);

		//Check we have enough bullets to fire
		if(bulletList.Count > weaponLevel)
		{
			//Check the weapon level and perform the respective fire mode
			switch(weaponLevel)
			{
			case 3:
				//Basic fire mode stage 3: Activate the bullet with this transform
				bulletList[0].GetComponent<Bullet>().Activate(this, transform);
				//Remove the first bullet in the list so it's not accessed until it's added back in to the pool
				bulletList.RemoveAt(0);
				//Now for the horrible bit, case fallthrough is not allowed and is a compile time error.
				//To resolve this, we use the dreaded goto to goto case 2
				goto case 2; //One of the vary few rare cases where a goto is "allowed"
			case 2:
				//Basic fire mode stage 2: Activate the bullet with this transform and 10 degrees added
				bulletList[0].GetComponent<Bullet>().Activate(this, transform.position, transform.rotation * Quaternion.Euler(0,0,10));
				//Remove the first bullet in the list so it's not accessed until it's added back in to the pool
				bulletList.RemoveAt(0);
				//Basic fire mode stage 2: Activate the bullet with this transform and 10 degrees subtracted
				bulletList[0].GetComponent<Bullet>().Activate(this, transform.position, transform.rotation * Quaternion.Euler(0,0,-10));
				//Remove the first bullet in the list so it's not accessed until it's added back in to the pool
				bulletList.RemoveAt(0);
				break;
			case 1:
				//Basic fire mode stage 1: Activate the bullet with this transform
				bulletList[0].GetComponent<Bullet>().Activate(this, transform);
				//Remove the first bullet in the list so it's not accessed until it's added back in to the pool
				bulletList.RemoveAt(0);
				break;
			}
			//If we have an animator to do a shoot animation, trigger isFiring in the animator
			if(GetComponent<Animator>())
				GetComponent<Animator>().SetTrigger("isFiring");
		}
	}

	/// <summary>
	/// Allows the weapon to fire again
	/// </summary>
	protected void EnableFire()
	{
		canFire = true;
	}

	/// <summary>
	/// Adds the given <paramref name="newBullet"/> game object to the bullet list. Tends to be called by the bullet itself.
	/// </summary>
	/// <param name="newBullet">New bullet to add.</param>
	virtual public void AddBulletToList(GameObject newBullet)
	{
		bulletList.Add (newBullet);

	}
}
