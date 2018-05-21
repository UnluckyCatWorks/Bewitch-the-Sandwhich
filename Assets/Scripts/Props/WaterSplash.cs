using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplash : MonoBehaviour
{
	public ParticleSystem[] VFX;

	private void OnTriggerEnter (Collider other) 
	{
		int id = Random.Range (0, VFX.Length);

		var splash = Instantiate (VFX[id]);
		splash.transform.position = other.transform.position;
		Destroy (splash, 1.5f);
	}
}
