using UnityEngine;
using System.Collections;

public class ShieldPowerUp : Collectible 
{
	public float shieldAmount = 50f; //The amount to increase our shield by when collected

	//Check if we've collided with a player, if so, call our OnCollected method
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.GetComponent<Player>() != null)
		{
			OnCollected ();
		}
	}

	/// <summary>
	/// Increases the player's shield by <paramref name="shieldAmount"/>
	/// </summary>
	protected override void OnCollected ()
	{
		GameManager.instance._player().GetComponent<Player>().increaseShield(shieldAmount);
		//If we have an explosion, instantiate it
		if(collectionExplosion != null)
		{
			Instantiate(collectionExplosion, transform.position, transform.rotation);
		}
		Destroy (this.gameObject);
	}
}
