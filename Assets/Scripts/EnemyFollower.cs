using UnityEngine;
using System.Collections;

public class EnemyFollower : Enemy 
{

	protected void Update() 
	{
		if(myTarget)
		{
			Vector2 dir = transform.InverseTransformPoint(myTarget.transform.position);
			
			float angle = Vector2.Angle(Vector2.right, dir);
			
			angle = dir.y < 0 ? -angle : angle;
			
			if (Mathf.Abs(angle) > Threshold)
			{
				transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime * Mathf.Sign(angle));
			}
			
			GetComponent<Rigidbody2D>().AddForce (transform.right * speed*8 * Time.deltaTime, ForceMode2D.Impulse);
			
			if(GetComponent<Rigidbody2D>().velocity.magnitude > speed)
			{
				GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity.normalized * speed;
			}
		}
	}


}
