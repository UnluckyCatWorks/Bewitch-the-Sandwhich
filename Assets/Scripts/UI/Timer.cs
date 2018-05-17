using System;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Timer : MonoBehaviour
{
	#region DATA
	[Header ("References")]
	public Image bg;
	public Image sldr;
	public Image deco;

	[Header ("Color stages")]
	[Range (0f, 1f)]
	public float value;
	public Gradient slider;
	public Gradient background;
	#endregion

	#region CALLBACKS
	private void Update ()
	{
		sldr.fillAmount = value;
		bg.color = background.Evaluate (value);
		sldr.color = slider.Evaluate (value);
	}

	public void Awake () 
	{
		// Reset
		sldr.fillAmount = 0f;
		bg.color = background.Evaluate (0f);
		sldr.color = slider.Evaluate (0f);
	} 
	#endregion

	[Serializable]
	public struct Colors 
	{
		public Color background;
		public Color slider;
	}
}
