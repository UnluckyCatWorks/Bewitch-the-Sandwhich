using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Foco : MonoBehaviour
{
	#region DATA
	public const float speed = 3.0f;

//	public static List<Foco> focos =  new List<Foco> ();
	public static Transform globalTarget;

	public Transform target;
	public bool bypass;
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

		/// In-editor helper
		if (bypass) return;

		/// Find which target to follor
		Transform follow;
		if (globalTarget)	follow = globalTarget;
		else if (target)	follow = target;
		else				follow = null;
		if (!follow) return;

		/// Follow target
		var newRot = Quaternion.LookRotation (follow.position - transform.position);
		var lerp = Quaternion.Slerp (transform.rotation, newRot, speed * Time.deltaTime);
		transform.rotation = lerp;
	}

	private void OnEnable () 
	{
		this.TryGetComponent (out light);
//		focos.Add (this);
	}
}
