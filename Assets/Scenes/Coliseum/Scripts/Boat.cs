using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Boat : MonoBehaviour
{
	#region DATA
	// External
	public MeshFilter pila;
	public SpriteRenderer info;

	// Internal
	internal Supply supply;
	#endregion

	#region UTILS
	public void UpdateBoatType ()
	{
		var id = supply.ingredient.ToString ();
		info.sprite = Resources.Load<Sprite> ("UI/" + id);
		pila.mesh = Resources.Load<Mesh> ("3D/Pila_" + id);
	} 

	public void AutoDestroy () 
	{
		Destroy (transform.parent.gameObject);
	}
	#endregion

	private void Awake () 
	{
		supply = GetComponent<Supply> ();
		UpdateBoatType ();
	}
}
