using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
	#region DATA
	public Image mask;
	public Text text;

	internal string content;
	internal float duration;
	#endregion

	#region CALLBACKS
	public IEnumerator FadeTitle () 
	{
		text.text = content;
		// Fade-in
		float factor = 0f;
		while (factor <= 1.1f)
		{
			text.SetAlpha (factor);
			mask.fillAmount = factor;

			yield return null;
			factor += Time.deltaTime / /*fade duration*/ 0.5f;
		}

		// Wait
		yield return new WaitForSeconds (duration);

		// Fade-out
		while (factor >= -0.1f)
		{
			text.SetAlpha (factor);
			mask.fillAmount = factor;

			yield return null;
			factor -= Time.deltaTime / /*fade duration*/ 0.5f;
		}

		// Destroy title
		Destroy (gameObject);
	}
	#endregion

	#region HELPERS
	public static Coroutine Show (string text, float duration) 
	{
		var prefab = Resources.Load<Title> ("Prefabs/UI/Title");
		var title = Instantiate (prefab, UIMaster.manager.transform);

		title.content = text;
		title.duration = duration;

		return title.StartCoroutine (title.FadeTitle ());
	}
	#endregion
}
