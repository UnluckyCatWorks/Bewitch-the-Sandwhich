using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Foco : MonoBehaviour
{
	#region DATA
	public const float speed = 3.0f;

	public Color32 color;
	public float intensity;
	public bool shadowing;

	new private Light light;
	#endregion

	private void Update () 
	{
		/// Adjust light
		light.color = color;
		light.intensity = intensity;
		light.shadows = shadowing? LightShadows.Soft : LightShadows.None;
	}

	private void OnEnable () 
	{
		this.TryGetComponent (out light);
//		focos.Add (this);
	}
}
