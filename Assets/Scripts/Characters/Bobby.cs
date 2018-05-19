using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobby : Character
{
	#region DATA
	public GameObject spellVFX;
	public ParticleSystem fireVFX;

	public const float BurnDurtion = 3f;
	public const float SpeedMultiplier = 2f;

	internal ParticleSystem effectInstance;
	#endregion

	protected override IEnumerator SpellEffect () 
	{
		// Wait until spell hits
		while (!spellHit) yield return null;

		// Make other player go crazy
		other.AddCC ("Spell: Burnt", (Locks.Burning | Locks.Dash), (Locks.Spells | Locks.Dash), BurnDurtion);

		// Show Impact VFX
		spellVFX.transform.parent = null;
		spellVFX.transform.position = other.transform.position + Vector3.up * 0.75f;
		spellVFX.SetActive (true);

		// Show persistent VFX (spawn new because lol)
		effectInstance = Instantiate (fireVFX, other.transform);
		effectInstance.gameObject.SetActive (true);

		// Wait until effect is over
		yield return new WaitForSeconds (BurnDurtion);
		spellVFX.transform.parent = transform;
		if (effectInstance != null)
			effectInstance.Stop (true, ParticleSystemStopBehavior.StopEmitting);
	}

	private void OnDestroy () 
	{
		if (effectInstance != null)
			effectInstance.Stop (true, ParticleSystemStopBehavior.StopEmitting);
	}
}
