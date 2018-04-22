using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGuy : Character
{
	[Header ("Spell Settings")]
	public float stunDuration;
	public float aoeRadius;

	protected override IEnumerator CastSpell () 
	{
		var pos = Vector3.zero;
		/// Allow spell aiming while self-stunned
		while (effects.ContainsKey("Spell Casting"))
		{
			pos = transform.position + transform.forward * (aoeRadius + 0.15f);
			yield return null;
		}

		/// Find all players affected by the spell (except self)
		bool hitPlayer = false;
		var hits = Physics.OverlapSphere (pos, aoeRadius);
		foreach (var h in hits)
		{
			/// Find out if any hit was the other player
			if (h.name == other.name)
				hitPlayer = true;
		}
		/// In case there's not hit,
		/// abort spell casting
		if (!hitPlayer) yield break;

		/// If hit, CC ohter player, and turn him stone
		other.AddCC ("Spell: Stoned", Locks.All);

		var anim = other.GetComponent<Animator> ();
		var mat = other.GetComponentInChildren<Renderer> ().sharedMaterial;
		int _StoneLevel = Shader.PropertyToID ("_StoneLevel");

		float factor = 0f;
		while (factor <= 1.1f) 
		{
			float value = Mathf.Clamp01 (Mathf.Pow (factor, 0.8f));
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
			float value = Mathf.Clamp01 (Mathf.Pow (factor, 1.3f));
			mat.SetFloat (_StoneLevel, 1 - value);
			anim.speed = value;

			yield return null;
			factor += Time.deltaTime / /*duration*/ 0.20f;
		}

		// Remove CC
		other.RemoveCC ("Spell: Stoned");
	}
}
