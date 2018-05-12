using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amy : Character
{
	#region DATA
	public GameObject spellVFX;
	public ParticleSystem madnessVFX;

	public const float MadnessDuration = 3f;

	private ParticleSystem effect;
	#endregion

	protected override IEnumerator SpellEffect () 
	{
		// Wait until spell hits
		while (!spellHit) yield return null;

		// Make other player go crazy
		other.AddCC ("Spell: Crazy", Locks.Crazy, Locks.Spells, MadnessDuration);

		// Spawn Impact VFX
		var spell = Instantiate (spellVFX);
		spell.transform.position = other.transform.position + Vector3.up*0.75f;
		Destroy (spell, 2f);

		// Spawn persistent VFX
		effect = Instantiate (madnessVFX, other.transform);
		effect.transform.localPosition = Vector3.up*1f;

		yield return new WaitForSeconds (MadnessDuration);
		effect.Stop (true, ParticleSystemStopBehavior.StopEmitting);
	}

	private void OnDestroy () 
	{
		if (effect)
			effect.Stop (true, ParticleSystemStopBehavior.StopEmitting);
	}
}
