using UnityEngine;
using System.Collections;

public class WeaponUpgrade : Collectible {

	//If we've collided with the player
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.GetComponent<Player>() != null)
		{
			//Call the OnCollected method
			OnCollected ();
		}
	}
	
	/// <summary>
	/// Upgrades the player's weapon once collected
	/// </summary>
	protected override void OnCollected ()
	{
		//Get's the player through the game manager and calls its upgrade weapon method
		GameManager.instance._player().GetComponent<Player>().UpgradeWeapon();
		//If we've set an explosion, instantiate it
		if(collectionExplosion != null)
		{
			Instantiate(collectionExplosion, transform.position, transform.rotation);
		}
		//Destroy the collectible so we can't collect it again
		Destroy (this.gameObject);
	}
}
