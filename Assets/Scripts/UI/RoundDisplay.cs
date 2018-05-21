using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundDisplay : MonoBehaviour 
{
	#region DATA
	public Text roundTitle;
	public List<Image> flags;
	public GameObject draw;

	[Header ("Players")]
	public Text p1Name;
	public Text p1Score;

	public Text p2Name;
	public Text p2Score;
	#endregion

	#region UTILS

	public static IEnumerator Show (int round, int winner) 
	{
		// Spawn new prefab
		var prefab = Resources.Load<RoundDisplay> ("Prefabs/UI/Round_Display");
		var display = Instantiate (prefab, UIMaster.manager.transform);

		#region TARGET CONFIGURATION
		// Configure the display
		display.roundTitle.text = "Round " + round;
		if (round != 1) 
		{
			// Empate! :o
			if (winner == 0)
			{
				display.draw.SetActive (true);
				display.flags[0].color = Player.all[0].character.focusColor;
				display.flags[1].color = Player.all[1].character.focusColor;
			}
			else 
			{
				display.draw.SetActive (false);
				display.flags.ForEach (f=> f.color = Player.all[winner-1].character.focusColor);
			}
		}
		else display.draw.SetActive (false);

		display.p1Name.text = Player.all[0].name;
		display.p1Score.text = Player.all[0].currentStats.roundScore.ToString ();
		display.p1Score.color = Character.Get (Player.all[0].playingAs).focusColor;

		display.p2Name.text = Player.all[1].name;
		display.p2Score.text = Player.all[1].currentStats.roundScore.ToString ();
		display.p2Score.color = Character.Get (Player.all[1].playingAs).focusColor; 
		#endregion

		// Fade-in animation
		var anim = display.GetComponent<Animation> ();
		yield return new WaitForSeconds (anim["In"].clip.averageDuration + 1f);

		// Start scene reset
		Coroutine reset = null;
		if (round != 1)
			reset = display.StartCoroutine (Game.manager.ResetStage ());

		// Fade-out animation
		anim.Play ("Out");
		yield return new WaitForSeconds (anim["Out"].clip.averageDuration);

		// Wait until stage is cleared
		if (reset != null) yield return reset;

		// Destroy display after
		Destroy (display.gameObject);
	}
	#endregion
}