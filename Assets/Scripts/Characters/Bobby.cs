using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobby : Character
{
	#region DATA
	public GameObject spellVFX;
	public ParticleSystem fireVFX;

	public const float BurnDurtion = 4f;
	public const float SpeedMultiplier = 2f;

	private ParticleSystem effect;
	#endregion

	protected override IEnumerator SpellEffect () 
	{
		// Wait until spell hits
		while (!spellHit) yield return null;

		// Make other player go crazy
		other.AddCC ("Spell: Burned", Locks.Burning | Locks.Abilities | Locks.Dash, Locks.Spells, BurnDurtion);

		// Spawn Impact VFX
		var spell = Instantiate (spellVFX);
		spell.transform.position = other.transform.position + Vector3.up * 0.75f;
		Destroy (spell, 2f);

		// Spawn persistent VFX
		effect = Instantiate (fireVFX, other.transform);
		effect.transform.localPosition = new Vector3 (0f, 0.25f, -0.25f);

		yield return new WaitForSeconds (BurnDurtion);
		effect.Stop (true, ParticleSystemStopBehavior.StopEmitting);
	}

	private void OnDestroy () 
	{
		if (effect)
			effect.Stop (true, ParticleSystemStopBehavior.StopEmitting);
	}
}
