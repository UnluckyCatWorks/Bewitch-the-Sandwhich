using UnityEngine;
using UnityEditor;

/// <summary>
/// Helper attribute class for Unity that makes a field
/// only be editable on the prefab, as if it was a static
/// and shared between their instances.
/// </summary>
public class SharedFieldAttribute : PropertyAttribute { }

[CustomPropertyDrawer(typeof(SharedFieldAttribute))]
public class SharedFieldDrawer : PropertyDrawer
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.isInstantiatedPrefab) return;

		// Generic drawer
		EditorGUI.BeginProperty (position, label, property);

		EditorGUI.PropertyField (position, property, label);
		property.serializedObject.ApplyModifiedProperties ();

		EditorGUI.EndProperty ();
	}
}
