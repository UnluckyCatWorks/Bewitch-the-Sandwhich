using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Milton : Character
{
	#region DATA
	public GameObject spellVFX;

	[Header ("Medusa Setting")]
	public Vector3 triplanarScale;
	public float triplanarSharpness;
	public Texture2D texture;
	public Texture2D normalTexture;
	public float normalScale;
	public Texture2D emissionTexture;
	[ColorUsage (false, true, 0, 8, 0.125f, 3)]
	public Color emissionColor;

	[Header ("Extrude settings")]
	[Range (0f, 1f)]
	public float level;
	public float extrudeAmount;
	public Vector2 minMaxHeight;

	private const float StunDuration = 2f;
	#endregion

	private IEnumerator TurnIntoStone () 
	{
		var anim = other.GetComponent<Animator> ();
		int _StoneLevel = Shader.PropertyToID ("_StoneLevel");

		// Apply CC
		other.AddCC ("Spell: Stoned", Locks.All);

		// Turn into stone
		float factor = 0f;
		while (factor <= 1.1f)
		{
			float value = Mathf.Clamp01 (Mathf.Pow (factor, 2f));
			other.mat.SetFloat (_StoneLevel, value);
			// Slow down animator
			anim.speed = 1 - value;

			yield return null;
			factor += Time.deltaTime / /*duration*/ 0.30f;
		}

		// Wait until stun is over
		yield return new WaitForSeconds (StunDuration);

		// Turn back to normal
		while (factor >= -0.1f)
		{
			float value = Mathf.Clamp01 (Mathf.Pow (factor, 2f));
			other.mat.SetFloat (_StoneLevel, value);
			// Restore animator
			anim.speed = 1 - value;

			yield return null;
			factor -= Time.deltaTime / /*duration*/ 0.30f;
		}

		// Remove CC
		other.RemoveCC ("Spell: Stoned");
	}

	public void SetMedusaSettings () 
	{
		Shader.SetGlobalVector 
		("_StoneScale",
			new Vector4
			(
				triplanarScale.x,
				triplanarScale.y,
				triplanarScale.z,
				triplanarSharpness
			)
		);
		Shader.SetGlobalTexture ("_StoneTex", texture);
		Shader.SetGlobalTexture ("_StoneNormal", normalTexture);
		Shader.SetGlobalFloat ("_StoneNormalScale", normalScale);
		Shader.SetGlobalTexture ("_StoneEmission", emissionTexture);
		Shader.SetGlobalColor ("_StoneEmissionColor", emissionColor);
		Shader.SetGlobalFloat ("_StoneLevel", level);
		Shader.SetGlobalFloat ("_StoneExtrude", extrudeAmount);
		Shader.SetGlobalFloat ("_StoneMin", minMaxHeight.x);
		Shader.SetGlobalFloat ("_StoneMax", minMaxHeight.y);
	}

	#region CALLBACKS
	protected override IEnumerator SpellEffect ()
	{
		// Wait until spell hits
		while (!spellHit) yield return null;

		// Show impact VFX
		spellVFX.transform.position = other.transform.position + (Vector3.up * 0.3f);
		spellVFX.SetActive (true);

		// Turn other player into stone
		StartCoroutine (TurnIntoStone ());
	}

	protected override void Awake ()
	{
		base.Awake ();
		SetMedusaSettings ();
	}

	private void OnDestroy () 
	{
		// Reset material, in case it's destroyed mid-spell
		other.mat.SetFloat ("_StoneLevel", 0f);
	} 
	#endregion
}