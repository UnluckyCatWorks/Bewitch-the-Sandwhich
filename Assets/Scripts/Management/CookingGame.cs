using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingGame : Game
{
	public Boat boat;
	public float boatSpawnTime;

	[Header ("madremia")]
	public Mesh sapoMal;
	public Mesh sapoBien;

	private void OnApplicationQuit () 
	{
		if (!Application.isEditor)
			System.Diagnostics.Process.GetCurrentProcess ().Kill ();
	}

	private float clock;
	protected override IEnumerator Logic () 
	{
		yield break;

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

			/// Spawn the new boat
			boat = Instantiate (boat);
			boat.supply.ingredient = next;
			boat.UpdateBoatType ();

			clock = 0f;
			yield return null;
		}
	}

	protected override void Awake () 
	{
		base.Awake ();
		sapoMal.uv = sapoBien.uv;
	}
}
