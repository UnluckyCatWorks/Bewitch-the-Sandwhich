using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Milton))] 
public class MiltonEditor : Editor 
{
	Milton milton;

	public override void OnInspectorGUI () 
	{
		EditorGUI.BeginChangeCheck ();
		base.OnInspectorGUI ();

		if (EditorGUI.EndChangeCheck ())
			milton.SetMedusaSettings ();
	}

	private void OnEnable () 
	{
		milton = target as Milton;
	}
}
