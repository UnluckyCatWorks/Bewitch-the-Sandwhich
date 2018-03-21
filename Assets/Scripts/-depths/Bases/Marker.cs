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
	[Range (0f, 1f)]
	public float alpha;
	private float _alpha;

	// Interal vars
	private MeshRenderer marker;
	private SpriteRenderer infoSign;

	// Static vars
	private static MaterialPropertyBlock block;
	private static int _ColorID;
	private static int _AlphaID;
	private static bool init;
	#endregion

	#region UTILS
	private int simultaneousMarks;
	public void On (int id) 
	{
		infoSign.SetAlpha (0.9f);
		block.SetFloat (_AlphaID, 0.9f);

		simultaneousMarks += id;
		block.SetColor (_ColorID, Game.manager.teamColors[simultaneousMarks-1]);
		marker.SetPropertyBlock (block);
	}
	public void Off (int id) 
	{
		simultaneousMarks -= id;
		if (simultaneousMarks == 0)
		{
			infoSign.SetAlpha (0.0f);
			block.SetFloat (_AlphaID, 0.0f);
		}
		else block.SetColor (_ColorID, Game.manager.teamColors[simultaneousMarks-1]);

		marker.SetPropertyBlock (block);
	}

	public static void Initialize () 
	{
		block = new MaterialPropertyBlock ();
		_ColorID = Shader.PropertyToID ("_Color");
		_AlphaID = Shader.PropertyToID ("_Alpha");
		init = true;
	}
	#endregion

	#region CALLBACKS
	private void Update ()
	{
		#if UNITY_EDITOR
		// Change alpha value in inspector
		if (alpha != _alpha && !EditorApplication.isPlaying)
		{
			infoSign.SetAlpha (alpha);
			block.SetFloat (_AlphaID, alpha);
			marker.SetPropertyBlock (block);
			_alpha = alpha;
		} 
		#endif

		// Make icon face camera
		if (updateIcon)
		{
			var t = infoSign.transform;
			var dir = Camera.main.transform.position - t.position;
			t.rotation = Quaternion.LookRotation (dir.normalized);
		}
	}

	private void OnEnable () 
	{
		#if UNITY_EDITOR
		if (!init)
		{
			block = new MaterialPropertyBlock ();
			_AlphaID = Shader.PropertyToID ("_Alpha");
			init = true;
		}
		#endif

		marker = GetComponentInChildren<MeshRenderer> ();
		infoSign = GetComponentInChildren<SpriteRenderer> ();
		infoSign.sprite = icon;

		Off (0);
	} 
	#endregion
}
