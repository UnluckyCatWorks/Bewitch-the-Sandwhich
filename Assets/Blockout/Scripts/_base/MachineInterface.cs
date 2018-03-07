using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineInterface : BInteractable
{
	[Header("Basic parameters")]
	public IngredientType resultType;		// How the ingredient is processed
	public Transform holder;				// The Transform that holds the ingredient
	protected Rigidbody obj;				// The object being processed

	public float duration;					// Time until work is completed
	public float overheatTime;				// Time until start overheating
	public float overloadTime;				// Time until full overload
	protected MachineController machine;    // State-Machine logic

	public override void Action (BCharacter player)
	{
		// If machine is waiting input
		if (machine.state == MachineState.Waiting)
		{
			// Cook ingredient
			obj = player.gobj.body;
			player.gobj = null;
			machine.anim.SetTrigger(MachineController.Start_Working);
		}
		else
		// If machine has finished processing
		if (machine.state == MachineState.Completed
		|| machine.state == MachineState.Overheating)
		{
			player.gobj = obj.GetComponent<BGrabbableObject> ();
			machine.anim.SetTrigger(MachineController.Pickup);
			obj = null;
		}
	}

	public override PlayerIsAbleTo CheckInteraction (BCharacter player) 
	{
		// If machine is waiting input
		if (machine.state == MachineState.Waiting)
		{
			// Check that player is actually carrying something
			if (player.gobj == null) return PlayerIsAbleTo.None;

			// Check that object is valid
			var ingredient = player.gobj.GetComponent<Ingredient>();
			if (ingredient == null || ingredient.type != IngredientType.Raw)
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
			if (player.gobj != null) return PlayerIsAbleTo.None;

			// If everything is okay
			return PlayerIsAbleTo.Action;
		}
		else return PlayerIsAbleTo.None;
	}

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
}
