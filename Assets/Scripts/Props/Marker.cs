using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
	#region DATA
	// External
	public Sprite icon;
	public bool isBillboard;

	// Transition
	private const float Duration = 0.25f;
	private bool inTransition;
	private float factor;

	private float iAlpha;
	private float fAlpha;
	private Color iColor;
	private List<Color> fColors;

	// References
	internal MeshRenderer mesh;
	internal SpriteRenderer sign;

	// Static
	public static MaterialPropertyBlock block;
	public static int ColorID;
	private static bool init;
	#endregion

	#region UTILS
	public void On (Color color) 
	{
		iColor = block.GetColor (ColorID);
		fColors.Add (color);

		iAlpha = sign.color.a;
		fAlpha = 1.0f;

		factor = 0f;
		inTransition = true;
	}
	public void Off (Color color)
	{
		iColor = block.GetColor (ColorID);
		fColors.Remove (color);

		iAlpha = sign.color.a;
		fAlpha = 0f;

		factor = 0f;
		inTransition = true;
	}
	public void Off () 
	{
		iColor = block.GetColor (ColorID);
		fColors.RemoveAt (fColors.Count - 1);

		iAlpha = sign.color.a;
		fAlpha = 0f;

		factor = 0f;
		inTransition = true;
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
		var t = sign.transform;
		var dir = Camera.main.transform.position - t.position;
		t.rotation = Quaternion.LookRotation (dir.normalized);
	}

	// Helper for the custom editor
	public void Set (Color color, float icon) 
	{
		sign.SetAlpha (icon);
		block.SetColor (ColorID, color);
		mesh.SetPropertyBlock (block);
	}

	// Helper for setting internal references
	public void SetUp () 
	{
		// Set up references
		if (!mesh) mesh = GetComponentInChildren<MeshRenderer> ();
		if (!sign) sign = GetComponentInChildren<SpriteRenderer> ();
		if (fColors == null) 
		{
			fColors = new List<Color> (2);
			fColors.Add (new Color (0, 0, 0, 0));
		}
	}

	// Helper that retrieves current color
	public Color GetCurrentColor ()
	{
		mesh.GetPropertyBlock (block);
		return block.GetColor (ColorID);
	}
	#endregion

	#region CALLBACKS
	private void Update () 
	{
		if (isBillboard && icon != null)
			MakeIconFaceCamera ();

		if (inTransition) 
		{
			// Break
			if (factor > 1.1f)
			{
				inTransition = false;
				factor = 0f;
				return;
			}

			// Do
			float value = Mathf.Pow (factor, 0.6f);
			var color = Color.Lerp (iColor, fColors[fColors.Count - 1], value);
			var alpha = Mathf.Lerp (iAlpha, fAlpha, value);
			block.SetColor (ColorID, color);

			mesh.SetPropertyBlock (block);
			sign.SetAlpha (alpha);

			// Continue
			factor += Time.deltaTime / Duration;
		}
	}

	public void Awake () 
	{
		Initialize ();
		SetUp ();

		// Restore initial state
		Set (new Color (0, 0, 0, 0), 0);
		sign.sprite = icon;
		factor = 0f;
	}
	#endregion
}
