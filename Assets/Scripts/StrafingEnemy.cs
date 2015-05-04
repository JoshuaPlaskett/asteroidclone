using UnityEngine;
using System.Collections;

public class StrafingEnemy : Enemy 
{
	
	public Weapon myWeapon;
	
	//On creation check if the target has been set
	override protected void Start () 
	{
		base.Start ();
		if(myWeapon.GetType() == typeof(WeaponFollow))
		{
			WeaponFollow followWeapon = myWeapon as WeaponFollow;
			followWeapon.target = GameManager.instance._player().transform;
			myWeapon = followWeapon;
		}
		GetComponent<Rigidbody2D>().AddRelativeForce(transform.right*(Random.Range (speed*0.5f,speed*2f)), ForceMode2D.Impulse);
	}
}
