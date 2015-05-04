using UnityEngine;
using System.Collections;

/// <summary>
/// Builds upon the <paramref name="Actor"/> class. Customised for minimal space ship actions
/// </summary>
public class ShipActor : Actor 
{
	//The explosion object for when the ship is destroyed
	public GameObject explosion;
	//The speed of the ship
	public float speed = 5f;
	//Its rotation speed
	public float rotateSpeed = 5f;
	public float health = 100f;
	public float shield = 0f; //By default, no shield
	public SpriteRenderer shieldSpriteRenderer; //The sprite renderer of the shield
	public AudioClip shieldDownSound;
	public AudioClip shieldUpSound;

	/// <summary>
	/// Fires a bullet - empty for now
	/// </summary>
	virtual public void Fire()
	{
	}

	/// <summary>
	/// Adds the bullet to the list of bullets.
	/// </summary>
	/// <param name="newBullet">New bullet.</param>
	virtual public void AddBulletToList(GameObject newBullet)
	{
	}

	/// <summary>
	/// Raises the hit event.
	/// </summary>
	/// <param name="damage">Amount of damage to take</param>
	protected override void OnHit(float damage = 10f)
	{
		//Check to see if our shield can absorb all of the damage
		if(shield >= damage)
		{
			shield -= damage;
			//Adjust the alpha of the shield sprite based on its remaining strength
			if(shieldSpriteRenderer)
				shieldSpriteRenderer.color = new Color(shieldSpriteRenderer.color.r,shieldSpriteRenderer.color.g,shieldSpriteRenderer.color.b,shield/100f);
		}
		//If not, use what remaining shield we have to absorb some damage, if any, and then take the remaining damage from health
		else
		{
			//Apply whatever damage is left after using up the shield's strength to the health
			health -= damage - shield;
			//If there was some shield to use up, play the shield down sound
			if(shield > 0)
			{
				GetComponent<AudioSource>().PlayOneShot(shieldDownSound, GameManager.instance.soundManager.volume);
			}
			//Set the shield to zero 
			shield = 0f;
			//If there is a shield sprite, set it's alpha to zero
			if(shieldSpriteRenderer)
			{
				shieldSpriteRenderer.color = new Color(shieldSpriteRenderer.color.r,shieldSpriteRenderer.color.g,shieldSpriteRenderer.color.b,0);
			}
		}
		//Check to see if we're now out of health
		if(health <= 0)
		{
			//If so, create our explosion
			Instantiate (explosion, transform.position, transform.rotation);
			Kill();
		}
	}

	/// <summary>
	/// Increases the shield value.
	/// </summary>
	/// <param name="value">Value.</param>
	public void increaseShield(float value)
	{
		if(shield + value > 100)
			shield = 100f;
		else
			shield += value;
		if(shieldSpriteRenderer)
		{
			shieldSpriteRenderer.color = new Color(shieldSpriteRenderer.color.r,shieldSpriteRenderer.color.g,shieldSpriteRenderer.color.b,shield/100f);
			GetComponent<AudioSource>().PlayOneShot(shieldUpSound, GameManager.instance.soundManager.volume);
		}
	}

	/// <summary>
	/// Rudimentary kill function to be overriden
	/// </summary>
	virtual protected void Kill()
	{
		Destroy (this.gameObject);
	}
}
