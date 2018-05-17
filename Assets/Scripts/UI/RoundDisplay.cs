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

	public Text p1Name;
	public Text p1Score;

	public Text p2Name;
	public Text p2Score;
	#endregion

	#region UTILS

	public static IEnumerator Show (int round, Characters winner) 
	{
		// Spawn new prefab
		var prefab = Resources.Load<RoundDisplay> ("Prefabs/UI/Round_Display");
		var display = Instantiate (prefab, UIMaster.manager.transform);

		// Configure the display
		display.roundTitle.text = "Round " + round;
		if (round != 1) display.flags.ForEach (f => f.color = Character.Get (winner).focusColor);

		display.p1Name.text = Player.all[0].name;
		display.p1Score.text = Player.all[0].currentStats.roundScore.ToString ();
		display.p1Score.color = Character.Get (Player.all[0].playingAs).focusColor;

		display.p2Name.text = Player.all[1].name;
		display.p2Score.text = Player.all[1].currentStats.roundScore.ToString ();
		display.p2Score.color = Character.Get (Player.all[1].playingAs).focusColor;

		// Fade-in animation
		var anim = display.GetComponent<Animation> ();
		yield return new WaitForSeconds (anim["In"].clip.averageDuration + 1f);

		// Wait until stage is cleared
		yield return Game.manager.ResetStage ();

		// Fade-out animation
		anim.Play ("Out");
		yield return new WaitForSeconds (anim["Out"].clip.averageDuration);
	}
	#endregion
}