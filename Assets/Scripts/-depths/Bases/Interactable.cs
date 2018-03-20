using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
	#region MARKING
	private static Material markerMat;
	private List<MeshFilter> markables;

	public static readonly Color32[] markerColors =
	{
		new Color32 ( 000, 000, 000, 000 ),		// Action
		new Color32 ( 000, 000, 000, 000 ),		// Special
		new Color32 ( 000, 000, 000, 000 ),		// Both
	};

	public void Mark ()
	{
		// Well, yeah
	}

	private void InitializeMarker () 
	{
		if (markerMat == null)
		{
			markerMat = new Material (Shader.Find ("Hidden/Marker"));
			markerMat.hideFlags = HideFlags.HideAndDontSave;
			markerMat.SetColor ( "_Color", new Color32 (255, 169, 0, 255) );
		}
	}
	#endregion

	#region INTERACTION
	public abstract PlayerIsAbleTo CheckInteraction (Character player);

	public abstract void Action (Character player);                     // Pressing Action key
	public virtual void Special (Character player) { /* optional */ }   // Pressing Special key 

	// Helper
	public static PlayerIsAbleTo Result (bool action, bool special) 
	{
		if (action && special) return PlayerIsAbleTo.Both;

		else if (action) return PlayerIsAbleTo.Action;
		else if (special) return PlayerIsAbleTo.Special;

		else return PlayerIsAbleTo.None;
	}
	#endregion

	protected virtual void Awake () 
	{
		InitializeMarker ();
	}
}
