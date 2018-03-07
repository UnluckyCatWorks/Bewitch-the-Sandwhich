using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BInteractable : MonoBehaviour
{
	#region MARKING
	[Header ("Marker Settings")]
	public List<MeshFilter> markables;
	private static Material markerMat;

	public static readonly Color32[] markerColors =
	{
		new Color32 ( 000, 000, 000, 000 ),		// Action
		new Color32 ( 000, 000, 000, 000 ),		// Special
		new Color32 ( 000, 000, 000, 000 ),		// Both
	};

	public void Mark ()
	{
		foreach (var o in markables)
		{
			Graphics.DrawMesh(o.sharedMesh, o.transform.localToWorldMatrix, markerMat, 0);
		}
	}

	private void InitializeMarker () 
	{
		if (markerMat == null)
			markerMat = Resources.Load<Material>("Marker");

		foreach (var m in GetComponentsInChildren<MeshFilter>())
		{
			if (!m.name.Contains("NoMark_"))
			{
				markables.Add(m);
				//				m.sharedMesh.SoftNormalsAsColors();
			}
		}
	}
	#endregion

	#region INTERACTION
	public abstract PlayerIsAbleTo CheckInteraction (BCharacter player);

	public abstract void Action (BCharacter player);                     // Pressing Action key
	public virtual void Special (BCharacter player) { /* optional */ }   // Pressing Special key 

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
