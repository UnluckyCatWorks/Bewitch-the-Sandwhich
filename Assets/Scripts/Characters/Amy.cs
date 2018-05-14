using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amy : Character
{
	#region DATA
	public GameObject spellVFX;
	public ParticleSystem madnessVFX;

	public const float MadnessDuration = 3f;
	#endregion

	protected override IEnumerator SpellEffect () 
	{
		// Wait until spell hits
		while (!spellHit) yield return null;

		// Make other player go crazy
		other.AddCC ("Spell: Crazy", Locks.Crazy, Locks.Spells, MadnessDuration);

		// Spawn Impact VFX
		spellVFX.transform.position = other.transform.position + (Vector3.up * 0.75f);
		spellVFX.SetActive (true);

		// Spawn persistent VFX
		madnessVFX.transform.SetParent (other.transform, false);
		madnessVFX.gameObject.SetActive (true);

		// Wait until effect is over
		yield return new WaitForSeconds (MadnessDuration);
		madnessVFX.Stop (true, ParticleSystemStopBehavior.StopEmitting);
		yield return new WaitForSeconds (0.7f);
		madnessVFX.transform.SetParent (transform, false);
	}

	private void OnDestroy () 
	{
		if (madnessVFX != null)
			madnessVFX.Stop (true, ParticleSystemStopBehavior.StopEmitting);
	}
}
