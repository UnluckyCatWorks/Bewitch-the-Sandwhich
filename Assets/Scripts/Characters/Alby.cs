using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alby : Character
{
	#region DATA
	// External
	public Color areaColor;
	public GameObject spellVFX;

	// Internal
	private const float StunDuration = 2f;
	private Marker areaOfEffect;
	#endregion

	protected override IEnumerator CastSpell () 
	{
		areaOfEffect.On (areaColor);					// Show area
		yield return new WaitForSeconds (0.50f);		// Allow spell aiming while self-stunned
		areaOfEffect.Off ();							// Hide area

		bool hitPlayer = false;
		var col = areaOfEffect.GetComponent<SphereCollider> ();
		var hits = Physics.OverlapSphere (areaOfEffect.transform.position, col.radius, 1<<14);
		foreach (var h in hits)
		{
			// Find out if any hit was the other player
			if (h.name == other.name)
				hitPlayer = true;
		}
		if (hitPlayer)
		{
			// If inside tutorial 
			Tutorial.SetCheckFor (Characters.Alby, Tutorial.Phases.Casting_Spells, true);

			// Spawn VFX
			var vfx = Instantiate (spellVFX);
			vfx.transform.position = other.transform.position + (Vector3.up * 0.75f);
			Destroy (vfx.gameObject, 2f);

			StartCoroutine (TurnIntoStone ());
		}
	}

	private IEnumerator TurnIntoStone () 
	{
		var anim = other.GetComponent<Animator> ();
		var mat = other.GetComponentInChildren<Renderer> ().sharedMaterial;
		int _StoneLevel = Shader.PropertyToID ("_StoneLevel");

		// Apply CC
		other.AddCC ("Spell: Stoned", Locks.All);

		// Turn into stone
		float factor = 0f;
		while (factor <= 1.1f)
		{
			float value = Mathf.Clamp01 (Mathf.Pow (factor, 2f));
			mat.SetFloat (_StoneLevel, value);
			// Slow down animator
			anim.speed = 1 - value;

			yield return null;
			factor += Time.deltaTime / /*duration*/ 0.30f;
		}

		/// Wait until stun is over
		yield return new WaitForSeconds (StunDuration);

		/// Turn back to normal
		while (factor >= -0.1f)
		{
			float value = Mathf.Clamp01 (Mathf.Pow (factor, 2f));
			mat.SetFloat (_StoneLevel, value);
			// Restore animator
			anim.speed = 1 - value;

			yield return null;
			factor -= Time.deltaTime / /*duration*/ 0.30f;
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
