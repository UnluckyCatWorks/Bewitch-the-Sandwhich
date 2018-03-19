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
		transform.GetChild(old).gameObject.SetActive(false);
		transform.GetChild(neew).gameObject.SetActive(true);
		type = newType;
	}
}
