using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightFollow : MonoBehaviour
{
	public Transform target;

	private void Update ()
	{
		var newRot = Quaternion.LookRotation (target.position - transform.position);
		var lerp = Quaternion.Slerp (transform.rotation, newRot, Time.deltaTime * 4f);
		transform.rotation = lerp;
	}
}
