using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Boat : MonoBehaviour
{
	public Transform track;

	private void Update () 
	{
		if (!track) return;

		// Position
		var speed = Time.deltaTime * 7f;
		var newPos = Vector3.Lerp (transform.position, track.position, speed);
		// Rotation
		speed = Time.deltaTime * 120f;
		var newRot = Quaternion.Slerp (transform.rotation, track.rotation, speed);
		// Update
		transform.position = newPos;
		transform.rotation = newRot;
	}
}
