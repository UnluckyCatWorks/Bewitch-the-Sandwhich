using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor; 
#endif

[ExecuteInEditMode]
public class Marker : MonoBehaviour
{
	#region DATA
	public Sprite icon;
	public bool updateIcon;

	/// Transition
	private int watchers;
	private bool inTransition;
	private const float duration = 0.5f;
	private float factor;
	private Color iColor;
	private float iAlpha;

	/// References
	private MeshRenderer marker;
	private SpriteRenderer infoSign;

	/// Static
	private static bool init;
	private static MaterialPropertyBlock block;
	private static int ColorID;
	#endregion

	#region UTILS
	public void On (int id) 
	{
		watchers += id;
		iColor = block.GetColor (ColorID);
		iAlpha = infoSign.color.a;
		inTransition = true;
	}
	public void Off (int id) 
	{
		watchers -= id;
		iColor = block.GetColor (ColorID);
		iAlpha = infoSign.color.a;
		inTransition = true;
	}

	private static void Initialize () 
	{
		if (init) return;
		ColorID = Shader.PropertyToID ("_Color");
		block = new MaterialPropertyBlock ();
		block.SetColor (ColorID, new Color (0, 0, 0, 0));
		init = true;
	}
	#endregion

	#region CALLBACKS
	private void Update ()
	{
		/// Make icon face camera
		if (updateIcon && icon != null)
		{
			var t = infoSign.transform;
			var dir = Camera.main.transform.position - t.position;
			t.rotation = Quaternion.LookRotation (dir.normalized);
		}

		if (inTransition) 
		{
			/// Break
			if (factor > 1.1f)
			{
				inTransition = false;
				factor = 0f;
				return;
			}

			/// Do
			float value = Mathf.Pow (factor, 0.6f);
			var color = Color.Lerp (iColor, Game.teamColors[watchers], value);
			var alpha = Mathf.Lerp (iAlpha, Mathf.Clamp01 (watchers), value);
			block.SetColor (ColorID, color);
			marker.SetPropertyBlock (block);
			infoSign.SetAlpha (alpha);

			/// Continue
			factor += Time.smoothDeltaTime / duration;
		}
	}

	private void OnEnable () 
	{
		Initialize ();

		/// Set up references
		marker = GetComponentInChildren<MeshRenderer> ();
		infoSign = GetComponentInChildren<SpriteRenderer> ();

		/// Set up ready-state
		marker.SetPropertyBlock (block);
		infoSign.sprite = icon;
		factor = 0f;
	}
	#endregion

	#region EDITOR TESTING
	[ContextMenu("Switch Player 1")]
	public void SwitchPlayerOne () 
	{
		if (watchers == 0 || watchers == 2) On (1);
		else Off (1);
	}
	[ContextMenu ("Switch Player 2")]
	public void SwitchPlayerTwo () 
	{
		if (watchers == 0 || watchers == 1) On (2);
		else Off (2);
	}
	#endregion
}
