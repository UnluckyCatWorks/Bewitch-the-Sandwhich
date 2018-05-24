using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CortinillaEditor : MonoBehaviour
{
	[Range (0f, 1f)]
	public float scale;
	internal Image cortinilla;

	private void Update () 
	{
		cortinilla.materialForRendering.SetFloat ("_Scale", scale);
	}

	private void Awake () 
	{
		cortinilla = GetComponent<Image> ();
	}
}
