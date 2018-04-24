using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alby : Character
{
	[Header ("Spell Settings")]
	public GameObject vfx;
	public float stunDuration;
	public Marker areaOfEffect;

	protected override IEnumerator CastSpell () 
	{
		/// Show area
		areaOfEffect.On (5, bypass: true);

		/// Allow spell aiming while self-stunned
		while (effects.ContainsKey("Spell Casting"))
			yield return null;

		/// Hide area
		areaOfEffect.Off (0, bypass: true);

		/// Find all players affected by the spell (except self)
		bool hitPlayer = false;
		var col = areaOfEffect.GetComponent<SphereCollider> ();
		var hits = Physics.OverlapSphere (areaOfEffect.transform.position, col.radius);
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
			if (TutorialGame.Checks != null &&
				TutorialGame.Checks.ContainsKey ("Spell") &&
				(TutorialGame.Checks["Spell"] == 0 || TutorialGame.Checks["Spell"] == 2))

				TutorialGame.Checks["Spell"] += 1;

			/// Spawn VFX
			var vfx = Instantiate (this.vfx);
			vfx.transform.position = other.transform.position + (Vector3.up * 0.75f);
			Destroy (vfx.gameObject, 2f);
		}

		/// If hit, CC ohter player
		other.AddCC ("Spell: Stoned", Locks.All);

		var anim = other.GetComponent<Animator> ();
		var mat = other.GetComponentInChildren<Renderer> ().sharedMaterial;
		int _StoneLevel = Shader.PropertyToID ("_StoneLevel");

		/// Turn him stone
		float factor = 0f;
		while (factor <= 1.1f)
		{
			float value = Mathf.Clamp01 (Mathf.Pow (factor, 2f));
			mat.SetFloat (_StoneLevel, value);
			anim.speed = 1 - value;

			yield return null;
			factor += Time.deltaTime / /*duration*/ 0.20f;
		}

		/// Wait until stun is over
		yield return new WaitForSeconds (stunDuration);

		/// Turn back to normal
		factor = 0f;
		while (factor <= 1.1f)
		{
			float value = Mathf.Clamp01 (Mathf.Pow (factor, 0.4f));
			mat.SetFloat (_StoneLevel, 1 - value);
			anim.speed = value;

			yield return null;
			factor += Time.deltaTime / /*duration*/ 0.20f;
		}

		// Remove CC
		other.RemoveCC ("Spell: Stoned");
	}
}
