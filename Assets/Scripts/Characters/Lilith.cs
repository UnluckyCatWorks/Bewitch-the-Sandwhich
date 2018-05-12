using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lilith : Character 
{
	#region DATA
	public GameObject spellVFX;
	private const float ForceMultiplier = 1.50f;
	private const float StunDuration = 1.25f;
	#endregion

	protected override IEnumerator SpellEffect () 
	{
		// Spawn VFX anyways
		var vfx = Instantiate (spellVFX);
		vfx.transform.position = transform.position + Vector3.up*0.20f;
		Destroy (vfx, 2f);

		// Wait until spell hits
		while (!spellHit) yield return null;

		// Knock hit player
		var dir = other.transform.position - transform.position;
		other.AddCC ("Spell: Bombed", Locks.All, Locks.All, StunDuration);
		other.Knock (dir.normalized * ForceMultiplier, 0.5f);
	}
}
