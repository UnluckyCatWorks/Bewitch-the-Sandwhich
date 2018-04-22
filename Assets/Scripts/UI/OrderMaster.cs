using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if FALSE
public static class OrderMaster 
{
	public static Order orderPrefab;

	public static Recipe[] recipes;
	public static List<Order>[] orders;

	#region UTILITIES
	const float offset = 270f;
	const float initialX = 830f;

	public static void SpawnOrder (int player, int recipe = /* RANDOM */ -1) 
	{
		player--;
		if (orders[player].Count == 3) return;

		#region GET DEMANDED RECIPE
		Recipe demand;
		if (recipe == -1)
		{
			var i = Random.Range (0, recipes.Length);
			demand = recipes[i];
		}
		else demand = recipes[recipe]; 
		#endregion

		// Instantiat UI panel & move it as needed
		var order = Object.Instantiate (orderPrefab, Game.ui);
		#region MOVE ORDER
		var t = order.transform as RectTransform;
		var mul = player * 2 - 1;
		var pos = t.anchoredPosition;

		// Spawn on the correct side
		pos.x = initialX * mul;

		pos.x += (offset * -mul) * orders[player].Count;
		t.anchoredPosition = pos; 
		#endregion

		// Add to player's orders
		order.recipe = demand;
		orders[player].Add (order);
	}

	public static void TryCompleteOrder (int player, Recipe recipe) 
	{
		player--;
		if (orders[player].Count == 0) return;

		var idx = -1;
		// Find if given recipe is in the orders list
		for (var i=0; i!=orders[player].Count; i++)
		{
			if (orders[player][i].recipe == recipe)
				idx = i;
		}
		if (idx == -1) return;

		// Move older orders 
		var mul = player * 2 - 1;
		for (var i=idx+1; i<orders[player].Count; i++)
		{
			var o = orders[player][i];
			var delta = offset * mul;
			o.StartCoroutine ( o.Move (delta) );
		}

		// Complete order & remove it
		var order = orders[player][idx];
		order.StartCoroutine (order.Complete ());
		orders[player].RemoveAt (idx);
	}
	#endregion

	#region CALLBACKS
	public static void Update () 
	{

	}

	public static void Initialize () 
	{
		recipes = Resources.LoadAll<Recipe> ("Potions");
		orderPrefab = Resources.Load<Order> ("UI/Order");

		orders = new List<Order>[2];
		orders[0] = new List<Order> (3);
		orders[1] = new List<Order> (3);
	}
	#endregion
}
#endif