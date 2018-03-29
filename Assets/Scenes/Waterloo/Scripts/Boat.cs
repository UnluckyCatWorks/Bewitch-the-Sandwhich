using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[ExecuteInEditMode]
public class Boat : MonoBehaviour
{
	#region DATA
	public MeshFilter pila;
	public SpriteRenderer info;

	private Animation[] bridges;
	private Supply supply; 
	#endregion

	#region UTILS
	public void BridgeUp (int bridge) 
	{
		bridges[bridge - 1].Play ("BridgeUp");
	}
	public void BridgeDown (int bridge) 
	{
		bridges[bridge - 1].Play ("BridgeDown");
	}

	private void UpdateBoatType ()
	{
		var id = supply.ingredient.ToString ();
		info.sprite = Resources.Load<Sprite> ("UI/" + id);
		pila.mesh = Resources.Load<Mesh> ("3D/Pila_" + id);
	} 
	#endregion

	private void OnEnable ()
	{
		/// Get all scene bridges
		bridges = new Animation[3];
		bridges[0] = GameObject.Find ("Bridge_L").GetComponent<Animation> ();
		bridges[1] = GameObject.Find ("Bridge_M").GetComponent<Animation> ();
		bridges[2] = GameObject.Find ("Bridge_R").GetComponent<Animation> ();
		/// Change boat type
		supply = GetComponent<Supply> ();
		UpdateBoatType ();
	}
}
