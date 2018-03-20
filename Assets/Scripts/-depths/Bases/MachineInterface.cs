using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineInterface : Interactable
{
	[Header ("References")]
	public Timer timer;						// The timer of the machine
	public Transform holder;				// The Transform that holds the ingredient
	protected Rigidbody obj;				// The object being processed
	protected MachineController machine;    // State-Machine logic

	[Header ("Basic parameters")]
	public IngredientType resultType;		// How the ingredient is processed
	public float duration;					// Time until work is completed
	public float overheatTime;				// Time until start overheating
	public float overloadTime;				// Time until full overload

	#region INTERACTION
	public override void Action (Character player) 
	{
		// If machine is waiting input
		if (machine.state == MachineState.Waiting)
		{
			// Cook ingredient
			obj = player.grab.body;
			player.grab = null;
			machine.anim.SetTrigger (MachineController.Start_Working);
		}
		else
		// If machine has finished processing
		if (machine.state == MachineState.Completed
		|| machine.state == MachineState.Overheating)
		{
			player.grab = obj.GetComponent<Grabbable> ();
			machine.anim.SetTrigger (MachineController.Pickup);
			obj = null;
		}
	}

	public sealed override PlayerIsAbleTo CheckInteraction (Character player) 
	{
		// If machine is waiting input
		if (machine.state == MachineState.Waiting)
		{
			// Check that player is actually carrying something
			if (player.grab == null) return PlayerIsAbleTo.None;

			// Check that object is valid
			var ingredient = player.grab.GetComponent<Ingredient> ();
			if (ingredient == null || ingredient.type != IngredientType.TALCUAL)
				return PlayerIsAbleTo.None;

			// If everything is okay
			return PlayerIsAbleTo.Action;
		}
		else
		// If machine has finished processing
		if (machine.state == MachineState.Completed
		|| machine.state == MachineState.Overheating)
		{
			// Check that player isn't already carrying something
			if (player.grab != null) return PlayerIsAbleTo.None;

			// If everything is okay
			return PlayerIsAbleTo.Action;
		}
		else return PlayerIsAbleTo.None;
	} 
	#endregion

	// Triggered when the machine has finished its work
	public virtual void ProcessObject () 
	{
		var ingredient = obj.GetComponent<Ingredient> ();
		ingredient.Process(resultType);
	}
	// Triggered when machine overloads
	public virtual void Overload () 
	{
		Destroy(obj.gameObject);
		obj = null;
	}

	#region CALLBACKS
	protected virtual void FixedUpdate ()
	{
		if (obj == null) return;
		// Make object follow Holder
		var newPos = Vector3.Lerp(obj.position, holder.position, Time.fixedDeltaTime * 7f);
		obj.MovePosition(newPos);
	}

	protected override void Awake () 
	{
		base.Awake ();
		// Get references
		machine = GetComponent<Animator>().GetBehaviour<MachineController>();
	}
	#endregion

	#region HELPERS
	public void SetTrigger (int trigger) 
	{
		machine.anim.SetTrigger (trigger);
	}
	#endregion
}
