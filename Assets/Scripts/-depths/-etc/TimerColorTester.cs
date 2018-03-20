using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TimerColorTester : MonoBehaviour
{
	[Range (0f,1f)] public float slider;
	[Range (0, 4)] public int theme;
	public bool visualizeTarget;

	private Timer timer;
	private void Update ()
	{
		if (timer == null || timer.themes.Length <= theme) return;

		timer.slider.fillAmount = slider;
		var t = timer.themes[theme];
		timer.bg.color = t.basis;
		timer.slider.color = (visualizeTarget) ? t.target : t.from;
	}

	private void OnEnable ()
	{
		timer = GetComponent<Timer> ();
		timer.Awake ();
	}
}
