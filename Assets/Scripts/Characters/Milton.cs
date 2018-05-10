using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Milton : Character
{
	#region DATA
	public const float MadnessDuration = 2f;
	#endregion

	protected override void SpellEffect () 
	{
		// Spawn VFX

		// Make other player go crazy
		other.AddCC ("Spell: Crazy", Locks.Crazy, Locks.Spells, MadnessDuration);
	}
}
