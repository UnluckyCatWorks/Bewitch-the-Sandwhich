using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : Grabbable
{
	public IngredientID id;
	public IngredientType type;
	public IngredientInfo info 
	{
		get { return new IngredientInfo { id = id, type = type }; }
	}
	public Sprite icon;

	public void Process ( IngredientType newType ) 
	{
		var old  = (int) type;
		var neew = (int) newType;

		transform.GetChild (old).GetComponent<Renderer> ().enabled = false;
		transform.GetChild (old).GetComponent<Collider> ().enabled = false;

		transform.GetChild (neew).GetComponent<Renderer> ().enabled = true;
		transform.GetChild (neew).GetComponent<Collider> ().enabled = true;

		type = newType;
	}

	protected override void Awake () 
	{
		base.Awake ();
		int id = (int) type;
		for (int i=0; i!=transform.childCount-1; i++) 
		{
			if (i == id) continue;
			transform.GetChild (i).GetComponent<Renderer> ().enabled = false;
			transform.GetChild (i).GetComponent<Collider> ().enabled = false;
		}
	}
}
