using UnityEngine;
using System.Collections;

[RequireComponent (typeof(SpriteRenderer))]
/// <summary>
/// Asteroid class - handles breaking asteroids into pieces and giving them an initial velocity and rotation.
/// </summary>
public class Asteroid : Actor 
{
	//Our explosion gameobject
	public GameObject explosion;
	//The smaller asteroids that this one will break into on destruction
	public GameObject smallerAsteroid;
	//How many pieces this asteroid should break into
	public int pieces = 3;
	//The maximum starting force this asteroid will have
	public float startForce = 50f;
	//The maximum starting torque this asteroid will have
	public float startTorque = 50f;
	//The sprites that this asteroid could use
	public Sprite[] sprites;
	//The points this asteroid will award when destroyed
	public int points = 10;
	//The time it takes to fade into play, this is so new asteroids spawned won't instantly kill the player
	//This gives them a small chance to escape
	public float fadeInTime = 1f;
	//Variable to cache the sprite renderer for later use
	private SpriteRenderer spriteRenderer;

	void Start () 
	{
		//Get our components that we access
		spriteRenderer = GetComponent<SpriteRenderer>();
		//Set our sprite to a random one in our array of sprites that should be set in the inspector
		spriteRenderer.sprite = sprites[Random.Range (0,sprites.Length)];
		//Add a random force and torque
		GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(Random.Range (-startForce,startForce),Random.Range (-startForce,startForce)));
		GetComponent<Rigidbody2D>().AddTorque(Random.Range(-50f,50f));
		//Add this asteroid to our GameManager's list to keep track of
		GameManager.instance.asteroidsInPlay.Add (this.gameObject);
		//Start to fade it in
		StartCoroutine("FadeIn",fadeInTime);
	}

	/// <summary>
	/// Fades the asteroid in. Uses IEnumerator so that it doesn't halt play
	/// </summary>
	/// <param name="fadeTime">Fade in time.</param>
	IEnumerator FadeIn(float fadeTime)
	{
		//Grab our current sprite alpha
		float alpha = spriteRenderer.color.a;
		//For each run through of this loop up until the given fade in time do...
		for (float time = 0.0f; time < 1.0f; time += Time.deltaTime / fadeTime)
		{
			//Create new colour with alpha based off it's current alpha lerped by the time that has passed
			Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha,1f,time));
			//Set the colour of the sprite using this new colour
			spriteRenderer.color = newColor;
			//Yield until next frame
			yield return null;
		}
		//Once it's faded in, activate it's collider
		GetComponent<Collider2D>().enabled = true;
	}
	
	void OnTriggerEnter2D(Collider2D collider)
	{
		//If we've collided with a ShipActor AKA a player or enemy
		if(collider.GetComponent<ShipActor>())
		{
			//Send a message to it that it's been hit. We don't need it to have to be received
			collider.SendMessage ("OnHit",100f,SendMessageOptions.DontRequireReceiver);
		}
		//Check that we've not collided with another asteroid, the range collider of a weapon or a collectible, then break apart
		if(!collider.GetComponent<Asteroid>() && collider.GetComponent<Weapon>() == null && collider.GetComponent<Collectible>() == null)
			BreakApart();
	}

	/// <summary>
	/// Breaks the asteroid apart into it's set amount of pieces.
	/// </summary>
	private void BreakApart()
	{
		//Create our explosion
		Instantiate (explosion, transform.position, transform.rotation);
		//Create the predefined amount of smaller asteroid objects
		for(int i = 0; i < pieces; i++)
		{
			Instantiate (smallerAsteroid, new Vector3(transform.position.x + Random.Range (-1,1),transform.position.y + Random.Range (-1,1)),transform.rotation);
		}
		//Remove this asteroid from the managed list
		GameManager.instance.asteroidsInPlay.Remove(this.gameObject);
		//Award the player the points
		GameManager.instance.score += points;
		//Destroy this
		Destroy (this.gameObject);
	}
	
	protected override void OnHit(float damage = 10f)
	{
		BreakApart();
	}
}
