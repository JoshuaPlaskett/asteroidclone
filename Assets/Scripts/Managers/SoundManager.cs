using UnityEngine;
using System.Collections;

/// <summary>
/// Sound manager - Handles all things sound though doesn't necessarily play the sounds. Objects can request volume, mute status etc. to play their own sounds
/// </summary>
public class SoundManager : ScriptableObject 
{
	private float _volume = 1; // The current volume of the game

	private bool isMuted = false; // Whether the game has been muted or not

	public float volume
	{
		get {return _volume;}
		set
		{
			_volume = value;
			foreach(AudioSource audio in GameObject.FindObjectsOfType<AudioSource>())
			{
				audio.volume = _volume;
			}
		}
	}

	/// <summary>
	/// Gets the mute status.
	/// </summary>
	/// <returns><c>true</c>, if the game is muted, <c>false</c> otherwise.</returns>
	public bool getMuteStatus()
	{
		return isMuted;
	}

	/// <summary>
	/// Sets the mute status.
	/// </summary>
	/// <param name="value">The value to set isMuted to.</param>
	public void setMuteStatus(bool value)
	{
		isMuted = value;
		foreach(AudioSource audio in GameObject.FindObjectsOfType<AudioSource>())
		{
			audio.mute = isMuted;
		}
	}
}
