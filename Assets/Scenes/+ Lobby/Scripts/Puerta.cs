using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puerta : MonoBehaviour
{
	public Transform portalTarget;
	public ParticleSystem vortex;

	private void OnTriggerEnter (Collider other) 
	{
		var p = other.GetComponent<Character> ();
		if (!p) return;

		/// Make player enter portal
		p.AddCC ("Entered portal", Locks.All);
		StartCoroutine (EnterPortal (p.transform));
	}

	private IEnumerator EnterPortal (Transform t) 
	{
		var iPos = t.position;
		var iRot = t.rotation;

		float factor = 0f;
		float duration = 2f;
		while (factor <= 1.1f)
		{
			float value = Mathf.Pow (factor, 4);
			var cPos = Vector3.Lerp (iPos, portalTarget.position, value);
			var cRot = Quaternion.Slerp (iRot, portalTarget.rotation, value);
			t.SetPositionAndRotation (cPos, cRot);

			yield return null;
			factor += Time.deltaTime / duration;
		}
		/// Notify
		TutorialGame.Checks["Portal"]++;
		t.gameObject.SetActive (false);
	}

	public void VortexOn () 
	{
		/// Play particle system
		vortex.Play ();
	}
}
