using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobby : Character
{
	#region DATA
	public ParticleSystem spellVFX;
	public ParticleSystem fireVFX;

	public const float BurnDurtion = 3f;
	public const float SpeedMultiplier = 2f;

	private ParticleSystem effectInstance;
	#endregion

	protected override IEnumerator SpellEffect () 
	{
		// Wait until spell hits
		while (spellResult == SpellResult.Undefined) yield return null;
		if (spellResult == SpellResult.Missed) yield break;

		// Make other player go crazy
		other.AddCC ("Spell: Burnt", (Locks.Burning | Locks.Dash), (Locks.Spells | Locks.Dash), BurnDurtion);

		// Show Impact VFX
		spellVFX.transform.parent = null;
		spellVFX.transform.position = other.transform.position + Vector3.up * 0.75f;
		spellVFX.Play (true);

		// Show persistent VFX
		effectInstance = Instantiate (fireVFX, other.transform);
		// Make it auto-destroy
		var main = effectInstance.main;
		main.stopAction = ParticleSystemStopAction.Destroy;
		main.duration = BurnDurtion;
		effectInstance.Play (true);

		// Re-parent
		yield return new WaitForSeconds (1.5f);
		spellVFX.transform.parent = transform;
		spellVFX.transform.localPosition = Vector3.zero;
	}
}
