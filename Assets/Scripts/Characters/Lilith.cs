using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lilith : Character 
{
	#region DATA
	public ParticleSystem spellVFX;
	private const float ForceMultiplier = 1.20f;
	private const float StunDuration = 1.25f;
	#endregion

	protected override IEnumerator SpellEffect () 
	{
		// Show VFX anyways
		spellVFX.transform.parent = null;
		spellVFX.GetComponent<Animation> ().Play ();
		spellVFX.Play (true);
		StartCoroutine (Reparent ());

		// Wait until spell hits
		while (spellResult == SpellResult.Undefined) yield return null;
		if (spellResult == SpellResult.Missed) yield break;

		// Knock hit player
		var dir = other.transform.position - transform.position;
		other.AddCC ("Spell: Bombed", Locks.All, Locks.All, StunDuration);
		other.Knock (dir.normalized * ForceMultiplier, 0.5f);
	}

	private IEnumerator Reparent () 
	{
		yield return new WaitForSeconds (2.2f);
		spellVFX.transform.parent = transform;
		spellVFX.transform.localPosition = Vector3.up * 0.402f;
	}
}
