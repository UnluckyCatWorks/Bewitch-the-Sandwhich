using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Host : MonoBehaviour 
{
	#region DATA
	[Header ("Hat reference")]
	public Transform hat;
	public Transform headSupport;
	public Transform handSupport;
	public bool startOnHand;
	#endregion

	#region CALLBACKS
	private void Awake () 
	{
		if (startOnHand)
		{
			// Snap to hand
			hat.SetParent (handSupport);
			hat.localPosition = Vector3.zero;
			hat.localRotation = Quaternion.identity;
		}
	}
	#endregion

	#region ANIMATOR HELPERS
	public void HatToHead () 
    {
		StartCoroutine (MoveTo (headSupport));
    }
	private IEnumerator MoveTo (Transform target) 
	{
		hat.SetParent (null, true);
		var iPos = hat.position;
		var iRot = hat.rotation;

		float factor = 0f;
		float duration = 0.1f;
		while (true)
		{
			var value = Mathf.Pow (factor, 0.6f);
			hat.position = Vector3.Lerp (iPos, target.position, value);
			hat.rotation = Quaternion.Slerp (iRot, target.rotation, value);

			if (factor > 1f) 
			{
				hat.SetParent (target, true);
				yield break;
			}

			yield return null;
			factor += Time.deltaTime / duration;
		}
	}
    #endregion
}
