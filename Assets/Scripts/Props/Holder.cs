using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holder : Interactable
{
	private Collider col;
	public bool locked { get; private set; }

	[FlagEnum]
	public ObjectTypes validObjects;
	public Rigidbody obj;

	#region INTERACTION
	public override void Action (Character player) 
	{
		/// If player is dropping
		if (player.grab)
		{
			/// Drop object
			obj = player.grab.body;
			player.grab = null;
		}
		/// If player is grabbing
		else
		{
			/// Grab object
			obj.isKinematic = true;
			player.grab = obj.GetComponent<Grabbable>();
			obj = null;
		}
	}

	public override bool CheckInteraction (Character player) 
	{
		/// If player is dropping
		if (player.grab)
		{
			/// Can't drop if another object is already in
			if (obj != null || !IsValidObject(player.grab)) return false;
			/// If everything's fine
			return true;
		}
		else
		{
			/// Can't grab air
			if (obj == null) return false;
			/// If everything's ok
			return true;
		}
	}
	#endregion

	#region CALLBACKS
	private void FixedUpdate ()
	{
		if (obj == null) return;
		// Make object follow Holder
		var newPos = Vector3.Lerp (obj.position, transform.position, Time.fixedDeltaTime * 7f);
		obj.MovePosition (newPos);
	}
	protected override void Awake ()
	{
		base.Awake ();
		col = GetComponent<Collider> ();
	} 
	#endregion

	#region HELPERS
	public void Lock (){ locked = true; col.enabled = false; }
	public void Unlock () { locked = false; col.enabled = true; }

	[Flags]
	public enum ObjectTypes 
	{
		RawIngredient = 1 << 0,
		ProcessedIngredient = 1 << 1,
		Potion = 1 << 2
	}

	public bool IsValidObject (Grabbable obj) 
	{
		/// If it's a raw ingredient
		if (validObjects.HasFlag (ObjectTypes.RawIngredient))
		{
			var ingredient = obj as Ingredient;
			if (ingredient != null && ingredient.type == IngredientType.TALCUAL)
				return true;
		}
		/// If it's NOT a raw ingredient
		if (validObjects.HasFlag(ObjectTypes.ProcessedIngredient))
		{
			var ingredient = obj as Ingredient;
			if (ingredient != null && ingredient.type != IngredientType.TALCUAL)
				return true;
		}
		/// If none of above is valid
		return false;
	}
	#endregion
}
