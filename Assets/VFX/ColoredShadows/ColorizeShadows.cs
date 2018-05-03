using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[ExecuteInEditMode]
[ImageEffectOpaque]
[ImageEffectAllowedInSceneView]
public class ColorizeShadows : MonoBehaviour
{
	public Color color;
	public Material mat;

	private void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if (!mat) return;
		UnityEngine.Graphics.Blit (source, destination, mat);
	}

	private void OnEnable () 
	{
		if (!mat) return;
		mat.SetColor ("_Color", color);
	}

	#if UNITY_EDITOR
	private void Update () 
	{
		if (!mat) return;
		mat.SetColor ("_Color", color);
	}
	#endif
}
