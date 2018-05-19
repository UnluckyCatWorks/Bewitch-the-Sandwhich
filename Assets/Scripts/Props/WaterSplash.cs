using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplash : MonoBehaviour
{
	public ParticleSystem VFX;

	private void OnTriggerEnter (Collider other) 
	{
		var splash = Instantiate (VFX);
		splash.transform.position = other.transform.position;
		Destroy (splash, 1.5f);
	}
}
