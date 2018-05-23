using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ranking : MonoBehaviour
{
	#region DATA
	[Header ("References")]
	public Animator anim;
	public Text title;
	[Space]
	public Image p1Pic;
	public Text p1Name;
	public Text p1Rounds;
	[Space]
	public Image p2Pic;
	public Text p2Name;
	public Text p2Rounds;
	[Space]
	public List<RankingScore> scores;
	#endregion

	#region CALLBACKS
	private IEnumerator Start () 
	{
		#region TITLES
		switch (Game.mode)
		{
			case Game.Modes.WetDeath:
				title.text = "WET DEATH";
				scores[0].title.text = "Kills";
				scores[1].title.text = "Spells landed";
				scores[2].title.text = "Dash collisions";
				break;
			case Game.Modes.MeltingRace:
				title.text = "MELTING RACE";
				scores[0].title.text = "Ingredients cooked";
				scores[1].title.text = "Spells landed";
				scores[2].title.text = "Baths";
				break;
			case Game.Modes.WizardWeather:
				title.text = "WIZARD WEATHER";
				scores[0].title.text = "Objects recollected";
				scores[1].title.text = "Dash collisions";
				scores[2].title.text = "Baths";
				break;
		}
		#endregion

		#region PLAYERS INFO
		// Player 1
		var p1 = Player.all[0];
		p1Name.text = p1.name;
		p1Pic.sprite = p1.character.avatar;
		p1Rounds.color = p1.character.focusColor;
		p1Rounds.text = "0";

		// Player 2
		var p2 = Player.all[1];
		p2Name.text = p2.name;
		p2Pic.sprite = p2.character.avatar;
		p2Rounds.text = "0";
		p2Rounds.color = p2.character.focusColor;

		// Bar colors
		foreach (var s in scores)
		{
			s.p1Bar.color = p1.character.focusColor;
			s.p2Bar.color = p2.character.focusColor;
		}
		#endregion

		// Wait a bit
		yield return new WaitForSeconds (1f);

		#region ROUNDS
		float factor = 0f;
		while (factor <= 1.1f)
		{
			// Smoothe curve
			float value = Mathf.Pow (factor, 2f);
			// Update text
			p1Rounds.text = Mathf.Lerp (0f, p1.ranking.roundsWon, value).ToString ("N0");
			p2Rounds.text = Mathf.Lerp (0f, p2.ranking.roundsWon, value).ToString ("N0");
			// Continue
			yield return null;
			factor += Time.deltaTime / /*duration*/ 2f;
		}
		#endregion

		#region SLIDERS
		// Play sliders
		StartCoroutine (scores[0].Yeah (0));
		yield return new WaitForSeconds (0.2f);
		StartCoroutine (scores[1].Yeah (1));
		yield return new WaitForSeconds (0.2f);
		yield return scores[2].Yeah (2);
		#endregion

		// Show return button
		yield return new WaitForSeconds (0.5f);
		anim.SetTrigger ("End");
	}
	#endregion
}
