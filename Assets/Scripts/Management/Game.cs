using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Game : MonoBehaviour
{
	#region DATA
	public bool initDialog;

	public static Game manager;			/// Self-reference
	public static bool paused;			/// Whether the game paused
	public static RectTransform ui;     /// Scene UI parent
	public static Color[] teamColors =	/// The representative color of each player
	{
		new Color32 (247, 133, 19, 200),	/// Orange (Wall Guy)
		new Color32 (59, 129, 249, 230),	/// Blue (Ice Guy)
		new Color32 (248, 71, 255, 190)		/// Purple? (Both)
	};

	#endregion

	protected virtual void Start () 
	{
		manager = this;

		/// Initialize game
		Marker.Initialize ();
		if (initDialog) DialogMaster.Initialize ();
		StartCoroutine (Logic ());
	}

	protected abstract IEnumerator Logic ();
}
