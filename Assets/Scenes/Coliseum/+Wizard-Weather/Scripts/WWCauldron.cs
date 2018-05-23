using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WWCauldron : MonoBehaviour 
{
	#region DATA
	[Header ("References")]
	public ParticleSystem splash;
	public Renderer surface;
	public AudioSource correctSound;

	internal Character owner;

	public static int[] scores = new int[2];
	#endregion

	#region UTILS
	public static int GetWinner ()  
	{
		// Player 1 wins
		if (scores[0] > scores[1]) return 1;
		else
		// Player 2 wins
		if (scores[1] > scores[0]) return 2;
		// Empate
		else return 0;
	}

	public void SetUpFor (Character owner) 
	{
		this.owner = owner;
		transform.position = owner.grabHandle.position;
		surface.material.SetColor ("_Color", owner.focusColor);
		surface.material.SetColor ("_EmissionColor", owner.areaColor * 0.5f);
	}
	#endregion

	#region CALLBACKS
	private void Update () 
	{
		transform.position = owner.grabHandle.position;
		transform.rotation = owner.grabHandle.rotation;
	}

	private void OnTriggerEnter (Collider other) 
	{
		var ingredient = other.GetComponentInParent<Ingredient> ();
		if (ingredient) 
		{
			if (!Game.stopped) 
			{
				// Update score
				int id = owner.ownerID - 1;
				scores[id]++;
				owner.Owner.ranking.scores[0]++;
				(Game.manager as WizardWeather).playerScores[id].text = scores[id].ToString ("00");
				// Play sound
				correctSound.Play (); 
			}
			splash.Play (true);
			ingredient.DestroySilenty ();
		}
	} 
	#endregion
}
