using UnityEngine;
using System.Collections;
[RequireComponent (typeof (AudioSource))]
/// <summary>
/// A class to destroy its game object after a given time. Mostly used for particle effect objects
/// </summary>
public class Explosion : MonoBehaviour 
{
	public float destroyAfter = 5f; //Seconds until object is destroyed
	private AudioSource audioSource;

	//On start invoke the kill method with a predefined delay
	void Start () 
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.volume = GameManager.instance.soundManager.volume/2;
		audioSource.mute = GameManager.instance.soundManager.getMuteStatus();
		audioSource.Play();
		Invoke ("Kill", destroyAfter);
	}
	//Destroy this game object
	void Kill()
	{
		Destroy (this.gameObject);
	}
}
