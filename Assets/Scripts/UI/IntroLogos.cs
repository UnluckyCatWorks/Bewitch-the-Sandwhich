using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroLogos : MonoBehaviour 
{
	// 0 is self
	// the rest are logos
	private Graphic[] images;
	private const float duration = 2f;

	private void Start () 
	{
		// Get all references
		images = GetComponentsInChildren<Graphic> ();

		// Make "cortinilla" full opaque
		images[0].materialForRendering.SetFloat ("_Scale", 1f);
		// Set alpha of all graphics (except self) to 0
		for (int i=1; i!=images.Length; i++) images[i].color = new Color(1,1,1, 0);

		StartCoroutine (FadeAlpha ());
		StartCoroutine (FadeScale ());
	}

	#region FADING COROUTINES
	IEnumerator FadeAlpha () 
	{
		// Fade in
		float factor = 0f;
		while (factor <= 1f)
		{
			for (int i=1; i!=images.Length; i++)
			{
				Color color = new Color (1, 1, 1, factor);
				images[i].color = color;
			}

			yield return null;
			factor += Time.deltaTime / (1f);
		}

		// Wait a bit
		yield return new WaitForSecondsRealtime (duration);

		// Fade out
		factor = 0f;
		while (factor <= 1f)
		{
			for (int i=1; i!=images.Length; i++)
			{
				Color color = new Color (1, 1, 1, 1f - factor);
				images[i].color = color;
			}

			yield return null;
			factor += Time.deltaTime / (1f);
		}
	}

	IEnumerator FadeScale () 
	{
		var minScale = Vector3.one * 0.95f;
		var maxScale = Vector3.one * 1.0f;

		/// Scale up graphics
		float factor = 0f;
		while (factor <= 1f)
		{
			for (int i=1; i!=images.Length; i++)
			{
				var scale = Vector3.Lerp (minScale, maxScale, factor);
				images[i].transform.localScale = scale;
			}

			yield return null;
			factor += Time.deltaTime / (2f + duration);
		}
	}
	#endregion
}
