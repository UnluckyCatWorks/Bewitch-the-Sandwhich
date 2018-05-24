using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingScore : MonoBehaviour
{
	#region DATA
	[Header ("References")]
	public Text title;
	[Space]
	public Text p1Score;
	public Image p1Bar;
	public Image p1Trophy;
	[Space]
	public Text p2Score;
	public Image p2Bar;
	public Image p2Trophy;

	private const float Duration = 3f;
	private const float BarMaxWidth = 294;
	#endregion

	public IEnumerator Yeah (int id) 
	{
		#region INTERNAL DATA
		var p1 = Player.Get (1).ranking;
		var p2 = Player.Get (2).ranking;

		// Avoid dividing by 0
		float total = Mathf.Max (p1[id] + p2[id], 0.0001f);
		float maxW1 = BarMaxWidth * p1[id] / total;
		float maxW2 = BarMaxWidth * p2[id] / total;

		float factor = 0f; 
		#endregion

		while (factor <= 1.1f) 
		{
			// Smooth curve
			float value = Mathf.Pow (Mathf.Clamp01(factor), 2f);

			// Update numbers
			p1Score.text = Mathf.Lerp (0, p1[id], value).ToString ("N0");
			p2Score.text = Mathf.Lerp (0, p2[id], value).ToString ("N0");

			// Update bars
			p1Bar.rectTransform.sizeDelta = new Vector2 (maxW1 * value, p1Bar.rectTransform.sizeDelta.y);
			p2Bar.rectTransform.sizeDelta = new Vector2 (maxW2 * value, p2Bar.rectTransform.sizeDelta.y);

			// Continue
			yield return null;
			factor += Time.deltaTime / Duration;
		}
	}
}