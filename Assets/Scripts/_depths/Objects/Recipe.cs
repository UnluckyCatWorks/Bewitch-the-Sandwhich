#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Recipe", menuName="New recipe", order=1000)]
public class Recipe : ScriptableObject
{
	public IngredientInfo ingredient1;
	public IngredientInfo ingredient2;
	public IngredientInfo ingredient3;
	public IngredientInfo ingredient4;

	// Getter
	public IngredientInfo[] info 
	{
		get { return new IngredientInfo[] 
		{
			ingredient1,
			ingredient2,
			ingredient3,
			ingredient4,
		};}
	}

	#region OPERATORS
	public static bool operator == (Recipe a, Recipe b)
	{
		return a.name == b.name;
	}

	public static bool operator != (Recipe a, Recipe b)
	{
		return a.name != b.name;
	} 
	#endregion
}
