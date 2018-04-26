using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alby : Character
{
	#region DATA
	// External
	[Header ("Spell settings")]
	public Color areaColor;
	public GameObject spellVFX;

	// Internal
	private const float StunDuration = 2f;
	private Marker areaOfEffect;
	#endregion

	protected override IEnumerator CastSpell () 
	{
		areaOfEffect.On (5, bypass: true);              // Show area
		yield return new WaitForSeconds (0.50f);        // Allow spell aiming while self-stunned
		areaOfEffect.Off (0, bypass: true);             // Hide area


		bool hitPlayer = false;
		// Find all players affected by the spell
		var col = areaOfEffect.GetComponent<SphereCollider> ();
		var hits = Physics.OverlapSphere (areaOfEffect.transform.position, col.radius);
		foreach (var h in hits)
		{
			// Find out if any hit was the other player
			if (h.name == other.name)
				hitPlayer = true;
		}
		/// In case there's no hit, abort spell casting
		if (!hitPlayer) yield break;
		else
		{
			/// If in the tutorial
			if (TutorialGame.IsChecking ("Spell"))
				TutorialGame.Checks["Spell"].Set ("Alby", true);

			/// Spawn VFX
			var vfx = Instantiate (spellVFX);
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
			float value = Mathf.Clamp01 (Mathf.Pow (factor, 1.2f));
			mat.SetFloat (_StoneLevel, value);
			anim.speed = 1 - value;

			yield return null;
			factor += Time.deltaTime / /*duration*/ 0.3f;
		}
		factor = 0f;

		/// Wait until stun is over
		yield return new WaitForSeconds (StunDuration);

		/// Turn back to normal
		while (factor <= 1.1f)
		{
			float value = Mathf.Clamp01 (Mathf.Pow (factor, 0.8f));
			mat.SetFloat (_StoneLevel, 1 - value);
			anim.speed = value;

			yield return null;
			factor += Time.deltaTime / /*duration*/ 0.3f;
		}

		/// Remove CC
		other.RemoveCC ("Spell: Stoned");
	}



	protected override void Awake () 
	{
		base.Awake ();
		areaOfEffect = GetComponentInChildren<Marker> ();
	}
}
