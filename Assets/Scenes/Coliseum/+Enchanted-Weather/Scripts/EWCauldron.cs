using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EWCauldron : MonoBehaviour 
{
	#region DATA
	[Header ("References")]
	public ParticleSystem splash;
	public Renderer surface;

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
		surface.material.SetColor ("_EmissionColor", owner.areaColor);
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
			scores[owner.ownerID - 1]++;
			ingredient.DestroySilenty ();
			splash.Play (true);
		}
	} 
	#endregion
}
