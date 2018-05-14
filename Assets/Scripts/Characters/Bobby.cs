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

	private ParticleSystem effectInstance;
	#endregion

	protected override IEnumerator SpellEffect () 
	{
		// Wait until spell hits
		while (!spellHit) yield return null;

		// Make other player go crazy
		other.AddCC ("Spell: Burned", Locks.Burning | Locks.Abilities | Locks.Dash, Locks.Spells, BurnDurtion);

		// Show Impact VFX
		spellVFX.transform.position = other.transform.position + Vector3.up * 0.75f;
		spellVFX.SetActive (true);

		// Show persistent VFX (spawn new because lol)
		effectInstance = Instantiate (fireVFX, other.transform);
		effectInstance.gameObject.SetActive (true);

		// Wait until effect is over
		yield return new WaitForSeconds (BurnDurtion);
		effectInstance.Stop (true, ParticleSystemStopBehavior.StopEmitting);
	}

	private void OnDestroy () 
	{
		if (effectInstance != null)
			effectInstance.Stop (true, ParticleSystemStopBehavior.StopEmitting);
	}
}
