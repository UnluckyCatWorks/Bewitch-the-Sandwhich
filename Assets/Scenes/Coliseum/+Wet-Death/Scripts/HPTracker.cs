using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPTracker : MonoBehaviour 
{
	#region DATA
	public int playerID;
	public List<SpriteRenderer> hearts;

	internal int hp;
	private bool init;

	internal static List<HPTracker> trackers;
	#endregion

	#region UTILS
	public static void Kill (int player) 
	{
		int id = player - 1;
		var tracker = trackers.Find (t => t.playerID == id);

		// Remove a heart from player
		tracker.hp--; 
		tracker.hearts[tracker.hp].SetAlpha (0f);

		if (tracker.hp == 0)
			Game.DeclareWinner (player==1? 2 : 1);

		// Accelerate rotator
		WetDeath.rotatorSpeed += (Game.manager as WetDeath).rotationIncrement;
	}
	#endregion

	#region CALLBACKS
	public IEnumerator Start () 
	{
		// Reset tracker
		Initialization ();
		hp = hearts.Count;

		// Wait until ~= round display is over
		if (!init)
		{
			yield return new WaitForSeconds (2.5f);
			init = true;
		}

		float factor = 0f;
		float duration = 1f;
		while (factor <= 1f + (hp * 0.3f) + 0.1f) 
		{
			// Fade in in cannon
			for (int i=0; i!=hp; i++)
			{
				if (hearts[i].color.a >= 1f) continue;
				hearts[i].SetAlpha (factor - (i * 0.3f));
			}

			yield return null;
			factor += Time.deltaTime / duration;
		}
	}

	private void Initialization () 
	{
		if (init) return;
		// Initialize hearts
		hearts.ForEach (s =>
		{
			s.color = Player.all[playerID].character.focusColor;
			s.SetAlpha (0f);
		});
	}
	#endregion
}
