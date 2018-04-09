using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Foco : MonoBehaviour
{
	#region DATA
	public const float speed = 3.0f;

	public static List<Foco> focos =  new List<Foco> ();
	public static Transform globalTarget;

	public Transform target;
	public bool bypass;
	#endregion

	private void Update () 
	{
		/// In-editor helper
		if (bypass) return;

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

	private void Awake () 
	{
		focos.Add (this);
	}
}
