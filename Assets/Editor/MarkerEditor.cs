using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Marker))]
public class MarkerEditor : Editor
{
	List<Marker> markers;
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
			foreach (var m in markers)
			{
				Undo.RecordObject (m, "Marker changed");
				m.Set (color, icon? 1 : 0);
			}
		}

		if (GUILayout.Button ("Orientate icon"))
			markers.ForEach (m=> m.MakeIconFaceCamera ());
	}

	private void OnEnable () 
	{
		markers = targets.Select (x => x as Marker).ToList ();
		foreach (var m in markers) m.SetUp ();
		Marker.Initialize ();
	}
}
