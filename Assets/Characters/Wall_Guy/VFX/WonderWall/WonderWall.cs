using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WonderWall : MonoBehaviour
{
	Animation anim;
	Transform ext;

	public void Destroy ()
	{
		/// Fade out wall
		anim["Fade"].normalizedTime = 1;
		anim["Fade"].speed = -1;
		anim.Play ();
		Destroy (gameObject, 1f);
	}

	private void Update () 
	{
		ext.Rotate (Vector3.up, 120f * Time.deltaTime);
	}

	private void Start () 
	{
		anim = GetComponent<Animation> ();
		ext = transform.GetChild (1);
	}
}
