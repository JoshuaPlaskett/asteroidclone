using UnityEngine;
using System.Collections;

/// <summary>
/// Actor - Base class for all game objects e.g. asteroids, players, enemies
/// Handles things like screen wrapping. MUST OVERRIDE OnHit(float damage = 10f);
/// </summary>
public abstract class Actor : MonoBehaviour 
{

	// Update is called once per frame
	protected virtual void FixedUpdate () 
	{
		//All actors screen wrap:
		if(Camera.main.WorldToViewportPoint(transform.position).x > 1f)
			transform.position = new Vector3(Camera.main.ViewportToWorldPoint(new Vector3(0,0,0)).x,transform.position.y);
		if(Camera.main.WorldToViewportPoint(transform.position).x < 0)
			transform.position = new Vector3(Camera.main.ViewportToWorldPoint(new Vector3(1f,0,0)).x,transform.position.y);
		if(Camera.main.WorldToViewportPoint(transform.position).y > 1f)
			transform.position = new Vector3(transform.position.x,Camera.main.ViewportToWorldPoint(new Vector3(0,0,0)).y);
		if(Camera.main.WorldToViewportPoint(transform.position).y < 0)
			transform.position = new Vector3(transform.position.x,Camera.main.ViewportToWorldPoint(new Vector3(0,1f,0)).y);
	}

	/// <summary>
	/// Raises the hit event - must be overriden
	/// </summary>
	/// <param name="damage">The amount of damage to be dealt </param>
	protected abstract void OnHit(float damage = 10f);
}
