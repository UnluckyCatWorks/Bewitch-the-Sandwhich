using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Boat : MonoBehaviour
{
	#region DATA
	// External
	public Transform driver;
	public MeshFilter pila;
	public SpriteRenderer info;
	public ParticleSystem puff;
	public Supply supply;

	// Internal
	public static List<Boat> boats = new List<Boat>(3);
	#endregion

	#region UTILS
	public static void SwitchAll (IngredientID target) 
	{
		var chosen = Random.Range (0, boats.Count);
		for (int i=0; i!=boats.Count; i++)
		{
			if (i != chosen) 
			{
				// Select a random ingredient
				IngredientID random = target;
				for (int a=0; a!=i+1; a++) 
				{
					random++;
					if (random == IngredientID.Count) 
						random = IngredientID.Seta;
				}
				boats[i].supply.ingredient = random;

				// Save selection
				boats[i].supply.ingredient = random;
			}
			// Chosen boat carries cauldron target
			else boats[i].supply.ingredient = target;

			// Update boat
			boats[i].puff.Play (true);
			boats[i].UpdateBoatType ();
		}
	}

	public void UpdateBoatType () 
	{
		var id = supply.ingredient.ToString ();
		info.sprite = Resources.Load<Sprite> ("UI/" + id);
		pila.mesh = Resources.Load<Mesh> ("3D/Pila_" + id);
		puff.Play (true);
	}
	#endregion

	#region CALLBACKS
	private void Update () 
	{
		transform.position = driver.position;
		transform.rotation = driver.rotation;
	}

	private void Awake () 
	{
		boats.Add (this);
	} 
	#endregion
}
