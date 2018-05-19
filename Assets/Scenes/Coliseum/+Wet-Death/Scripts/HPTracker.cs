using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPTracker : MonoBehaviour 
{
	#region DATA
	public int playerID;
	public int hp;

	public List<Image> hearts;
	internal static List<HPTracker> trackers = new List<HPTracker> (2);
	#endregion

	#region UTILS
	public static void Kill (int player) 
	{
		int id = player - 1;
		var tracker = trackers.Find (t => t.playerID == id);

		// Remove a heart from player
		tracker.hp--;
		tracker.hearts[tracker.hp].CrossFadeAlpha (0f, 0.5f, false);

		if (tracker.hp == 0)
		{
			// Declare winner if someone lost all hearts
			print ("LOL, perdio " + Player.all[id].playingAs);
		}

		// Accelerate rotator
		WetDeath.rotatorSpeed += (Game.manager as WetDeath).rotationIncrement;
	}
	#endregion

	#region CALLBACKS
	private IEnumerator Start () 
	{
		// Hide hearts
		hearts.ForEach (i => i.SetAlpha (0f));

		// Wait until ~= round display is over
		yield return new WaitForSeconds (2f);

		// Make hearts appear in cannon
		foreach (var i in hearts)
		{
			i.color = Character.Get (Player.all[playerID].playingAs).focusColor;
			i.CrossShowAlpha (1f, 0.5f, false);
			yield return new WaitForSeconds (0.3f);
		}
		// Register tracker
		trackers.Add (this);
	}
	#endregion
}
