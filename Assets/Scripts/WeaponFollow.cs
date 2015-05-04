using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This weapon looks at a given target and fires at it
/// </summary>
public class WeaponFollow : Weapon 
{
	//Our target we want to look at and shoot at
	public Transform target;
	//A list of available targets to choose from
	private List<Transform> targets = new List<Transform>();
	//How quickly we rotate to look at our target
	public float rotationSpeed = 500f;

	//This objects collider is set to IgnoreRaycast layer so that it doesn't interrupt with bullets etc.
	//If an object enters our weapon's "Range" then we add it to our list of targets to aim at
	private void OnTriggerEnter2D(Collider2D collider)
	{
		//Make sure the tag is not the same as this objects tag (i.e. enemy doesn't target other enemies
		if(collider.tag != this.tag)
		{
			//Make sure we're not about to target a bullet or power up
			if(collider.GetComponent<Bullet>() == null && collider.GetComponent<Collectible>() == null)
			{
				targets.Add (collider.transform);
			}
		}
	}

	//If an object leaves our weapon's "Range" then we remove it from our list of targets to aim at
	private void OnTriggerExit2D(Collider2D collider)
	{
		//If it leaves the trigger area, then remove it from our list of targets
		targets.Remove (collider.transform);
	}

	override protected void Update () 
	{
		//Check that we have some targets to aim at first
		if(targets.Count > 0)
		{
			//Check to see what target is nearest to us
			foreach(Transform newTarget in targets)
			{
				//Make sure it exists
				if(newTarget != null)
				{
					//Get it's distance from us
					float distance = Vector2.Distance(transform.position,newTarget.position);
					//Check if we already have a target
					if(target != null)
					{
						//If the distance to the new target is smaller than the distance to our current target
						if(distance > Vector2.Distance (transform.position, target.position))
						{
							//Assign the new target as our target
							target = newTarget;
						}
					}
					//If we don't have a target already, then this new target will be our target
					else
						target = newTarget;
				}
			}
		}
		//Once we've determined which target is closest to us - aim at it and fire away
		if(target && transform && autoDetect)
		{
			//Get the rotation we need to be looking at the target
			Quaternion rot = Quaternion.LookRotation( target.position - transform.position, Vector3.forward );
			//We only want to focus on the z axis
			rot.x = 0;
			rot.y = 0;
			transform.rotation = rot;
			//Draw a debug ray to see where we're aiming
			Debug.DrawRay(transform.position,-transform.up*rangeCheck);
			//Prepare an array to store all our ray's hits
			RaycastHit2D[] hits;
			//Do our raycast
			hits = Physics2D.RaycastAll(transform.position, -transform.up,rangeCheck);
			//For each hit
			foreach(RaycastHit2D hit2D in hits)
			{
				//Check it doesn't have our tag
				if(hit2D.transform.tag != this.tag && canFire)
				{
					//Fire away
					Fire ();
					//Set ourselves up to reload
					canFire = false;
					//Call our reload after a given time
					Invoke ("EnableFire",shootDelay);
					//We only need to shoot once, break out of the loop now
					break;
				}
			}
		}
	}
}
