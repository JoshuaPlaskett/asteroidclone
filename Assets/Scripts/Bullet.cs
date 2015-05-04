using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class Bullet : Actor 
{
	//The force applied to the bullet on creation
	public float startingForce = 300f;
	//Lifespan of bullet before it destroys itself
	public float lifeSpan = 3f;
	//The bullet's current life
	private float currentLife = 0f;
	//The weapon that created the bullet
	public Weapon myParent;
	//The damage this bullet deals
	public float damage = 10f;

	void Start () 
	{
		//On start we deactivate it because it has been added to a pool of bullets
		Deactivate();
		//If myParent has been set, set it as the transform's parent
		if(myParent != null)
			transform.parent = myParent.transform;
	}

	protected override void FixedUpdate () 
	{
		base.FixedUpdate();
		//If the bullet is alive
		if(gameObject.activeSelf)
		{
			//Detract from it's life
			currentLife -= lifeSpan * Time.deltaTime;
			//If it's out of life, deactivate it
			if(currentLife <= 0)
				Deactivate();
		}
	}

	/// <summary>
	/// Activate the bullet from the given <paramref name="Weapon"/> with the given <paramref name="Transform"/>.
	/// </summary>
	/// <param name="weapon">The weapon that's activating the bullet</param>
	/// <param name="startTransform">Start transform.</param>
	public void Activate(Weapon weapon, Transform startTransform)
	{
		transform.position = startTransform.position;
		transform.rotation = startTransform.rotation;
		myParent = weapon;
		//Detach from parent
		transform.parent = null;
		//Enable the game object
		gameObject.SetActive(true);
		//Add the bullet's startingForce
		GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0,-startingForce));
		//Set the lifespan of the bullet
		currentLife = lifeSpan;
	}

	/// <summary>
	/// Activate the bullet from the given <paramref name="Weapon"/> with the given <paramref name="position"/> and <paramref name="rotation"/>.
	/// </summary>
	/// <param name="weapon">The weapon that's activating the bullet</param>
	/// <param name="startPosition">Starting position</param>
	/// <param name="startRotation">Starting rotation</param>
	public void Activate(Weapon weapon, Vector3 startPosition, Quaternion startRotation)
	{
		transform.position = startPosition;
		transform.rotation = startRotation;
		myParent = weapon;
		//Detach from parent
		transform.parent = null;
		//Enable the game object
		gameObject.SetActive(true);
		//Add the bullet's startingForce
		GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0,-startingForce));
		//Set the lifespan of the bullet
		currentLife = lifeSpan;
	}

	/// <summary>
	/// Deactivate the bullet, reset its position to zero, remove velocity, set its transform parent back, disable the game object
	/// and add it back to its pool
	/// </summary>
	public void Deactivate()
	{
		//Set velocity to zero
		GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		//Set transform to zero
		transform.position = Vector3.zero;
		//Set rotation to zero
		transform.rotation = Quaternion.Euler(Vector3.zero);
		//Reattatch to parent if any
		if(myParent)
			transform.parent = myParent.transform;
		//Disable the game object
		gameObject.SetActive(false);
		//If it has a parent, add it back to it's pool
		if(myParent != null)
			myParent.AddBulletToList(this.gameObject);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		//If the object we've hit does not have the same tag, is not a weapon range collider
		//And is not a collectible, then we send the hit message and deactivate this bullet
		if(other.tag != this.tag && other.GetComponent<Weapon>() == null
		   && other.GetComponent<Collectible>() == null)
		{
			other.SendMessage("OnHit",damage,SendMessageOptions.DontRequireReceiver);
			Deactivate();
		}
	}

	protected override void OnHit(float damage = 10f)
	{
		//Do Nothing
	}
}
