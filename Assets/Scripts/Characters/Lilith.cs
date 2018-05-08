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

	protected override void SpellEffect () 
	{
		// Knock hit player
		var dir = other.transform.position - transform.position;
		other.AddCC ("Spell: Bombed", Locks.All, StunDuration);
		other.Knock (dir.normalized * ForceMultiplier, 0.5f);
	}
}
