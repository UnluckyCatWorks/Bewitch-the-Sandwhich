using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Cauldron : Interactable 
{
	#region DATA
	public ParticleSystem splash;
	#endregion

	#region CALLBAKCS
	IEnumerator ThrowInto (Grabbable toy, Character owner) 
	{
		var iPos = toy.transform.position;
		var tPos = transform.position + (Vector3.up);
		bool splashed = false;

		float factor = 0f;
		while (factor <= 1.1f) 
		{
			var newPos = Vector3.Lerp (iPos, tPos, factor);
			toy.body.MovePosition (newPos);

			if (!splashed && factor < 0.5f)
			{
				splash.Play ();
				splashed = true;
			}

			yield return new WaitForFixedUpdate ();
			factor += Time.fixedDeltaTime / /*duration*/ 0.15f;
		}
		Destroy (toy.gameObject);
		OnDrop (owner);
	}

	protected abstract void OnDrop (Character owner);

	protected virtual bool OptionalCheck (Character player) { return true; }
	#endregion

	#region OVERRIDES
	public sealed override void Action (Character player)
	{
		StartCoroutine (ThrowInto (player.toy, player));
		player.toy = null;
	}

	public sealed override bool CheckInteraction (Character player)
	{
		// Can't drop if holding nothing
		if (!player.toy) return false;
		// Per-cauldron additional check
		if (!OptionalCheck (player)) return false;

		// If everything's ok
		return true;
	}
	#endregion
}
