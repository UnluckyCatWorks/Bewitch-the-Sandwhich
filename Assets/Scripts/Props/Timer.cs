using System;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
	#region DATA
	public TimerColors[] themes;

	[NonSerialized] public Image bg;
	[NonSerialized] public Image slider;
	[NonSerialized] public Image deco;

	private Color fromColor;
	private Color targetColor;
	private float sliderValue;
	#endregion

	#region UTILITIES
	public void ChangeTo (Enum theme, float value = 0f)
	{
		// Get the next theme
		var id = (theme as IConvertible).ToInt32 (CultureInfo.InvariantCulture);
		targetTheme = themes[id];

		// Cache current values
		ogBG = bg.color;
		ogSlider = slider.color;
		ogValue = slider.fillAmount;

		// Start transition
		inTransition = true;
		SetSlider (value);
		time = 0;
	}

	public void SetSlider (float value) 
	{
		sliderValue = value;
		if (!inTransition)
		{
			slider.fillAmount = value;
			slider.color = Color.Lerp (fromColor, targetColor, value);
		}
	}
	#endregion

	private bool inTransition;
	TimerColors targetTheme;
	Color ogBG, ogSlider;
	float ogValue;
	float time;
	private void Update () 
	{
		if (!inTransition) return;
		var duration = 0.6f * ogValue;

		if (time <= duration)
		{
			var factor = time / duration;
			bg.color = Color.Lerp (ogBG, targetTheme.basis, factor);
			slider.color = Color.Lerp (ogSlider, targetTheme.from, factor);
			slider.fillAmount = Mathf.Lerp (ogValue, sliderValue, factor);

			time += Time.deltaTime;
		}
		else
		{
			fromColor = targetTheme.from;
			targetColor = targetTheme.target;

			inTransition = false;
		}
	}

	public void Awake () 
	{
		bg = transform.GetChild (0).GetComponent<Image> ();
		slider = transform.GetChild (1).GetComponent<Image> ();
		deco = transform.GetChild (2).GetComponent<Image> ();

		// Reset
		slider.fillAmount = 0f;
		bg.color = themes[0].basis;
		targetColor = themes[0].target;
		slider.color = fromColor = themes[0].from;
	}
}
