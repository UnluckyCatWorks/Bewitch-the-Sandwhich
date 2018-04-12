using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(IntroLogos))]
public class IntroLogosEditor : Editor 
{
	public bool isCortinilla;
	private float slider;

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		slider = EditorGUILayout.Slider ("Fade value", slider, 0f, 1f);
		(target as IntroLogos).GetComponent<Image> ().materialForRendering.SetFloat ("_Scale", 1-slider);
	}
}