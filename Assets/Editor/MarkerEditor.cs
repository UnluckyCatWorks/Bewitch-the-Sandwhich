using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Marker))]
public class MarkerEditor : Editor
{
	Marker marker;
	Color color;
	bool icon;

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		if (EditorApplication.isPlaying) return;

		// Helpers
		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("Helper controls", EditorStyles.boldLabel);

		EditorGUI.BeginChangeCheck ();
		color = EditorGUILayout.ColorField ("Color", color);
		icon = EditorGUILayout.ToggleLeft ("Show icon", icon);

		if (EditorGUI.EndChangeCheck ()) 
		{
			Undo.RecordObject (marker, "Marker changed");
			marker.Set (color, icon? 1 : 0);
		}

		if (GUILayout.Button ("Orientate icon"))
			marker.MakeIconFaceCamera ();
	}

	private void OnEnable () 
	{
		marker = target as Marker;
		marker.SetUp ();
		Marker.Initialize ();

		color = marker.GetCurrentColor ();
	}
}
