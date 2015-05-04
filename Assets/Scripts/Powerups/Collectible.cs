using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
/// <summary>
/// Collectible item.
/// </summary>
public abstract class Collectible : Actor 
{
	public GameObject collectionExplosion;
	
	void Start () 
	{
		//Add random force - no torque needed
		GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(Random.Range (-1f,1f),Random.Range (-1f,1f)),ForceMode2D.Impulse);
	}

	protected override void OnHit (float damage = 10f)
	{
		//Do Nothing
	}

	/// <summary>
	/// Raises the collected event.
	/// </summary>
	protected abstract void OnCollected();
}
