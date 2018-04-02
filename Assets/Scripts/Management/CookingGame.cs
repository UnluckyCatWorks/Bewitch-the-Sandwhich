using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingGame : Game
{
	public Boat boat;
	public float boatSpawnTime;

	private float clock;
	protected override IEnumerator Logic () 
	{
		/// Auto-initialize first boat (prefab)
		boat.supply = boat.GetComponent<Supply> ();

		while (true)
		{
			/// Wait spawn time
			while (clock <= boatSpawnTime)
			{
				if (!paused) clock += Time.deltaTime;
				yield return null;
			}

			/// Get ingredient of next boat
			IngredientID next;
			var ingredient = boat.supply.ingredient;
			if ((int)ingredient == 4) next = IngredientID.Cristal;
			else next = ingredient + 1;

			// Spawn the boat
			boat = Instantiate (boat);
			boat.supply.ingredient = next;
			boat.UpdateBoatType ();
			clock = 0f;
			yield return null;
		}
	}
}
