using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mary : Character
{
	#region DATA
	// External
	public Color areaColor;
	public GameObject spellVFX;

	// Internal
	private const float ForceMultiplier = 1.50f;
	private const float StunDuration = 1.25f;
	private Marker areaOfEffect;
	#endregion

	protected override IEnumerator CastSpell () 
	{
		areaOfEffect.On (areaColor);				// Show area
		yield return new WaitForSeconds (0.50f);		// Allow spell aiming while self-stunned
		areaOfEffect.Off ();						// Hide area

		/// Spawn VFX always
		var vfx = Instantiate (spellVFX);
		vfx.transform.position = transform.position + (Vector3.up * 0.2f);
		Destroy (vfx.gameObject, 2f);

		// Find out if any hit was the other player
		var hits = Physics.OverlapSphere (transform.position, areaOfEffect.GetComponent<SphereCollider> ().radius);
		foreach (var h in hits)
		{	
			if (h.name == other.name)
			{
				// If inside tutorial
				var check = TutorialGame.TutorialPhases.Casting_Spells;
				if (TutorialGame.IsChecking (check))
					TutorialGame.Checks[check].Set (Characters.Mary, true);

				// Knock hit player
				var dir = other.transform.position - transform.position;
				other.AddCC ("Spell: Bombed", Locks.All, StunDuration);
				other.Knock (dir.normalized * ForceMultiplier, 0.5f);
			}
		}
	}

	protected override void Awake () 
	{
		base.Awake ();
		areaOfEffect = GetComponentInChildren<Marker> ();
	}
}
