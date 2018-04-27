using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineInterface : Interactable
{
	#region DATA
	[Header ("Basic parameters")]
	public Transform holder;                // The Transform that holds the ingredient
	public IngredientType resultType;       // How the ingredient is processed
	public float duration;                  // Time until work is completed
	public float safeTime;					// Time until start overheating
	public float overheatTime;              // Time until full overload

	internal Grabbable obj;					// The object being processed
	protected MachineController machine;    // State-Machine logic
	protected Coroutine cooking;			// The coroutine that controls the cooking process
	#endregion

	#region INTERACTION
	public override void Action (Character player) 
	{
		// If machine is waiting input
		if (machine.state == MachineState.Waiting)
		{
			// Cook ingredient
			obj = player.toy;
			player.toy = null;
			machine.anim.SetTrigger ("Start_Working");
			cooking = StartCoroutine (KeepWithHolder ());
		}
		else
		// If machine has finished processing
		if (machine.state == MachineState.Completed
		|| machine.state == MachineState.Overheating)
		{
			player.toy = obj;
			machine.anim.SetTrigger ("Pickup");
			obj.transform.SetParent (null, true);
			obj = null;
			StopCoroutine (cooking);
		}
	}

	public sealed override bool CheckInteraction (Character player) 
	{
		// If machine is waiting input
		if (machine.state == MachineState.Waiting)
		{
			// Check that player is actually carrying something
			if (player.toy == null) return false;

			// Check that object is valid
			var ingredient = player.toy.GetComponent<Ingredient> ();
			if (ingredient == null || ingredient.type != IngredientType.TALCUAL)
				return false;

			// If everything is okay
			return true;
		}
		else
		// If machine has finished processing
		if (machine.state == MachineState.Completed
		|| machine.state == MachineState.Overheating)
		{
			// Check that player isn't already carrying something
			if (player.toy != null) return false;

			// If everything is okay
			return true;
		}
		else return false;
	}
	#endregion

	#region UTILS
	// Triggered when the machine has finished its work
	public virtual void ProcessObject () 
	{
		var ingredient = obj.GetComponent<Ingredient> ();
		ingredient.Process (resultType);
	}

	// Triggered when machine overloads
	public virtual void Overload (float UpForce) 
	{
		var force = transform.forward + Vector3.up*UpForce;
		obj.Throw (force * 15f, null);
		obj.Destroy (1.5f);
		obj = null;
	} 
	#endregion

	#region CALLBACKS
	protected override void Awake () 
	{
		base.Awake ();
		// Get reference
		machine = GetComponent<Animator>().GetBehaviour<MachineController>();
	}
	#endregion

	#region HELPERS
	public IEnumerator KeepWithHolder () 
	{
		var factor = 0f;
		var duration = 0.5f;
		var iPos = obj.transform.position;
		while (factor <= 1f) 
		{
			/// Move to Holder position
			var newPos = Vector3.Lerp (iPos, holder.position, factor);
			obj.transform.position = newPos;
			/// Scale it down
			obj.transform.localScale = Vector3.one * (1-factor);

			factor += Time.deltaTime / duration;
			yield return null;
		}
		/// Parent to Holder
		obj.transform.SetParent (holder);
		obj.transform.localPosition = Vector3.zero;
		obj.transform.rotation = Quaternion.identity;

		/// Wait until machine is done
		factor = 0f;
		while (machine.state != MachineState.Completed) yield return null;
		while (factor <= 1f) 
		{
			/// Scale up again & recolocate
			obj.transform.localScale = Vector3.one * factor;
			obj.transform.localPosition = Vector3.zero;

			factor += Time.deltaTime / duration;
			yield return null;
		}
	}

	private int playersNear;
	public void PlayerIsNear (bool near) 
	{
		playersNear += near? +1 : -1;
		if (playersNear == 0) machine.anim.SetBool ("PlayersNear", false);
		else machine.anim.SetBool ("PlayersNear", true);
	}
	#endregion
}
