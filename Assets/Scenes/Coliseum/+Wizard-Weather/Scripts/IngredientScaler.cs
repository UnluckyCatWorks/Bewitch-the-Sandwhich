using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientScaler : MonoBehaviour
{
	const float InitialScale = 0.1f;
	const float Increment = 0.5f;

	private void Update () 
	{
		transform.localScale += new Vector3 (Increment, Increment, Increment) * Time.deltaTime;
	}

	private void OnCollisionEnter () 
	{
		GetComponent<Ingredient> ().Destroy (0.2f);
	}

	private void Start () 
	{
		transform.localScale = new Vector3 (InitialScale, InitialScale, InitialScale);
	}
}
