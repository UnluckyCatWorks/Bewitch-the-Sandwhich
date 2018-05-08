using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobby : Character 
{
	#region DATA
	public GameObject spellVFX;
	private const float StunDuration = 2f;
	#endregion

	protected override void SpellEffect () 
	{
		// Spawn VFX
		var vfx = Instantiate (spellVFX);
		vfx.transform.position = other.transform.position + (Vector3.up * 0.75f);
		Destroy (vfx.gameObject, 2f);

		// Turn other player into stone
		StartCoroutine (TurnIntoStone ());
	}

	private IEnumerator TurnIntoStone () 
	{
		var anim = other.GetComponent<Animator> ();
		int _StoneLevel = Shader.PropertyToID ("_StoneLevel");

		// Apply CC
		other.AddCC ("Spell: Stoned", Locks.All);

		// Turn into stone
		float factor = 0f;
		while (factor <= 1.1f)
		{
			float value = Mathf.Clamp01 (Mathf.Pow (factor, 2f));
			other.mat.SetFloat (_StoneLevel, value);
			// Slow down animator
			anim.speed = 1 - value;

			yield return null;
			factor += Time.deltaTime / /*duration*/ 0.30f;
		}

		// Wait until stun is over
		yield return new WaitForSeconds (StunDuration);

		// Turn back to normal
		while (factor >= -0.1f)
		{
			float value = Mathf.Clamp01 (Mathf.Pow (factor, 2f));
			other.mat.SetFloat (_StoneLevel, value);
			// Restore animator
			anim.speed = 1 - value;

			yield return null;
			factor -= Time.deltaTime / /*duration*/ 0.30f;
		}

		// Remove CC
		other.RemoveCC ("Spell: Stoned");
	}
}
