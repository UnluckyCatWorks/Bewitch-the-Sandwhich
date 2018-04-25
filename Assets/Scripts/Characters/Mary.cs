using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mary : Character
{
	[Header ("Spell Settings")]
	public GameObject vfx;
	public float forceMultiplier;
	public float stunDuration;
	public Marker areaOfEffect;

	protected override IEnumerator CastSpell () 
	{
		/// Show area
		areaOfEffect.On (3, bypass: true);

		/// Allow spell aiming while self-stunned
		while (effects.ContainsKey ("Spell Casting"))
			yield return null;

		/// Hide area
		areaOfEffect.On (0, bypass: true);

		/// Spawn VFX always
		var vfx = Instantiate (this.vfx);
		vfx.transform.position = transform.position + (Vector3.up * 0.2f);
		Destroy (vfx.gameObject, 2f);

		/// Find all players affected by the spell (except self)
		bool hitPlayer = false;
		var hits = Physics.OverlapSphere (transform.position, areaOfEffect.GetComponent<SphereCollider> ().radius);
		foreach (var h in hits)
		{
			/// Find out if any hit was the other player
			if (h.name == other.name)
				hitPlayer = true;
		}
		/// In case there's no hit, abort spell casting
		if (!hitPlayer) yield break;
		else
		{
			/// If hit, and maybe in tutorial
			if (TutorialGame.IsChecking("Spell"))
				TutorialGame.Checks["Spell"].Set ("Mary", true);
		}

		/// Knock hit player
		var dir = other.transform.position - transform.position;
		other.Knock (dir.normalized * forceMultiplier);
		other.AddCC ("Spell: Bombed", Locks.All, stunDuration);
	}
}
