using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
	[NonSerialized] public RectTransform t;
	[NonSerialized] public Recipe recipe;

	IEnumerator Start () 
	{
		t = transform as RectTransform;
		var start = t.anchoredPosition;
		var target = new Vector2 (start.x, 0f);

		var factor = 0f;
		var duration = 0.3f;
		while (factor < 1f)
		{
			// Move down
			var newPos = Vector2.Lerp (start, target, factor);
			t.anchoredPosition = newPos;

			factor += Time.deltaTime / duration;
			yield return null;
		}
	}

	public IEnumerator Complete () 
	{
		var start = t.anchoredPosition;
		var target = new Vector2 (start.x, 170f);

		var factor = 0f;
		var duration = 0.15f;
		while (factor < 1f)
		{
			// Move down
			var newPos = Vector2.Lerp (start, target, factor);
			t.anchoredPosition = newPos;

			factor += Time.deltaTime / duration;
			yield return null;
		}
		Destroy (gameObject);
	}

	public IEnumerator Move ( float offset ) 
	{
		var start = t.anchoredPosition;
		var target = new Vector2 (start.x + offset, start.y);

		var factor = 0f;
		var duration = 0.3f;
		while (factor < 1f)
		{
			// Move down
			var newPos = Vector2.Lerp (start, target, factor);
			t.anchoredPosition = newPos;

			factor += Time.deltaTime / duration;
			yield return null;
		}
	}
}
