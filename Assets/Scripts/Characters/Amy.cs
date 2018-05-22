using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amy : Character
{
	#region DATA
	public ParticleSystem spellVFX;
	public ParticleSystem madnessVFX;

	public const float MadnessDuration = 3f;

	private ParticleSystem effectInstance;
	#endregion

	protected override IEnumerator SpellEffect () 
	{
		// Wait until spell hits
		while (spellResult == SpellResult.Undefined) yield return null;
		if (spellResult == SpellResult.Missed) yield break;

		// Make other player go crazy
		other.AddCC ("Spell: Crazy", Locks.Crazy, Locks.Spells, MadnessDuration);

		// Show Impact VFX
		spellVFX.transform.parent = null;
		spellVFX.transform.position = other.transform.position + Vector3.up * 0.75f;
		spellVFX.Play (true);

		// Show persistent VFX
		effectInstance = Instantiate (madnessVFX, other.transform);
		// Make it auto-destroy
		var main = effectInstance.main;
		main.stopAction = ParticleSystemStopAction.Destroy;
		main.duration = MadnessDuration;
		effectInstance.Play (true);

		// Re-parent
		yield return new WaitForSeconds (1.5f);
		spellVFX.transform.parent = transform;
		spellVFX.transform.localPosition = Vector3.zero;
	}
}
