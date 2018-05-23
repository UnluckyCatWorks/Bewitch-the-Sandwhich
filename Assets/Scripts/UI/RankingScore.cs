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
	private const float BarMinWidth = 69;
	private const float BarMaxWidth = 279;
	#endregion

	public IEnumerator Yeah (int id) 
	{
		#region INTERNAL DATA
		var p1 = Player.all[0].ranking;
		var p2 = Player.all[1].ranking;

		// Avoid dividing by 0
		float total = Mathf.Max (p1.scores[id] + p2.scores[id], 0.0001f);
		float[] temp = new float[2];

		float maxW1 = Mathf.Lerp (BarMinWidth, BarMaxWidth, p1.scores[id] / total);
		float maxW2 = Mathf.Lerp (BarMinWidth, BarMaxWidth, p2.scores[id] / total);

		float factor = 0f; 
		#endregion

		while (factor <= 1.1f) 
		{
			// Smooth curve
			float value = Mathf.Pow (factor, 2f);

			// Update numbers
			p1Score.text = Mathf.Lerp (0, p1.scores[id], value).ToString ("N0");
			p2Score.text = Mathf.Lerp (0, p2.scores[id], value).ToString ("N0");

			// Update bars
			float w1 = Mathf.Lerp (BarMinWidth, maxW1, value);
			p1Bar.rectTransform.sizeDelta = new Vector2 (w1, p1Bar.rectTransform.sizeDelta.y);
			float w2 = Mathf.Lerp (BarMinWidth, maxW2, value);
			p2Bar.rectTransform.sizeDelta = new Vector2 (w2, p2Bar.rectTransform.sizeDelta.y);

			// Continue
			yield return null;
			factor += Time.deltaTime / Duration;
		}
	}
}