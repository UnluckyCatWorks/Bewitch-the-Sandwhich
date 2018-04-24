using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
	#region DATA
	public Sprite icon;
	public bool updateIcon;

	/// Transition
	private int watchers;
	private bool inTransition;
	private const float duration = 0.25f;
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
	public void On (int id, bool bypass=false) 
	{
		watchers += id;
		if (bypass) watchers = id;
		iColor = block.GetColor (ColorID);
		iAlpha = infoSign.color.a;
		inTransition = true;
		factor = 0f;
	}
	public void Off (int id, bool bypass=false) 
	{
		watchers -= id;
		if (bypass) watchers = id;
		iColor = block.GetColor (ColorID);
		iAlpha = infoSign.color.a;
		inTransition = true;
		factor = 0f;
	}

	public void Set (Color color, bool icon) 
	{
		infoSign.SetAlpha (icon? 1 : 0);
		block.SetColor (ColorID, color);
		marker.SetPropertyBlock (block);
	}

	public static void Initialize () 
	{
		if (init) return;
		ColorID = Shader.PropertyToID ("_Color");
		block = new MaterialPropertyBlock ();
		block.SetColor (ColorID, new Color (0, 0, 0, 0));
		init = true;
	}

	public void MakeIconFaceCamera () 
	{
		var t = infoSign.transform;
		var dir = Camera.main.transform.position - t.position;
		t.rotation = Quaternion.LookRotation (dir.normalized);
	}
	#endregion

	#region CALLBACKS
	private void Update ()
	{
		if (updateIcon && icon != null)
			MakeIconFaceCamera ();

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

	public void OnEnable () 
	{
		Initialize ();

		/// Set up references
		marker = GetComponentInChildren<MeshRenderer> ();
		infoSign = GetComponentInChildren<SpriteRenderer> ();

		/// Set up ready-state
		block.SetColor (ColorID, new Color (0, 0, 0, 0));
		marker.SetPropertyBlock (block);
		infoSign.sprite = icon;
		infoSign.SetAlpha (0);
		factor = 0f;
	}
	#endregion
}
