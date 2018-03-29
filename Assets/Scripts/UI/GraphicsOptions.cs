using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using PostFx = UnityEngine.Rendering.PostProcessing.PostProcessProfile;

public class GraphicsOptions : MonoBehaviour
{
	#region UI
	private static PostFx profile;
	private static bool initialCheck;

	public Dropdown resolution;
	public Toggle fullscreen;
	public Toggle vsync;

	public Dropdown textures;
	public Dropdown shadows;

	public Toggle ao;
	public Toggle aa;
	#endregion

	#region UTILS
	public void LoadOptions () 
	{
		/// Read values from disk to UI
		resolution.value	= PlayerPrefs.GetInt ("Options:Resolution", 0);
		fullscreen.isOn		= PlayerPrefs.GetInt ("Options:Fullscreen", 1) == 1? true : false;
		vsync.isOn			= PlayerPrefs.GetInt ("Options:V-Sync", 1) == 1? true : false;
		textures.value		= PlayerPrefs.GetInt ("Options:Textures", 3);
		shadows.value		= PlayerPrefs.GetInt ("Options:Shadows", 3);
		ao.isOn				= PlayerPrefs.GetInt ("Options:AO", 1) == 1? true : false;
		aa.isOn				= PlayerPrefs.GetInt ("Options:AA", 1) == 1? true : false;
	}
	public void SaveOptions () 
	{
		/// Save UI values to disk
		PlayerPrefs.SetInt ("Options:Resolution", resolution.value);
		PlayerPrefs.SetInt ("Options:Fullscreen", fullscreen.isOn? 1 : 0);
		PlayerPrefs.SetInt ("Options:V-Sync", vsync.isOn? 1 : 0);
		PlayerPrefs.SetInt ("Options:Textures", textures.value);
		PlayerPrefs.SetInt ("Options:Shadows", shadows.value);
		PlayerPrefs.SetInt ("Options:AO", ao.isOn? 1 : 0);
		PlayerPrefs.SetInt ("Options:AA", aa.isOn? 1 : 0);
		PlayerPrefs.Save ();
	}

	public void ApplyValues () 
	{
		/// Camera layer
		var layer = Camera.main.GetComponent<PostProcessLayer> ();

		/// Resolution & Screen mode
		string[] literal = resolution.options[resolution.value].text.Split ('x');
		int w = int.Parse ( literal[0] );
		int h = int.Parse ( literal[1] );
		Screen.SetResolution (w, h, fullscreen.isOn);

		/// V-Sync
		QualitySettings.vSyncCount = vsync.isOn? 1 : 0;
		/// Textures
		QualitySettings.masterTextureLimit = (3 - textures.value);
		/// Shadows
		QualitySettings.shadowResolution = (ShadowResolution)shadows.value;

		/// AO
		AmbientOcclusion ao;
		profile.TryGetSettings (out ao);
		ao.active = this.ao.isOn;

		/// AA
		if (aa.isOn)	layer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
		else			layer.antialiasingMode = PostProcessLayer.Antialiasing.None;
	} 

	[ContextMenu ("Hola")]
	private void PopulateResolution () 
	{
		/// Populates resolution drop-down
		/// with all valid resolutions
		var all = Screen.resolutions;
		var list = new List<Dropdown.OptionData> ();
		foreach (var r in all)
		{
			/// Format: 1920x1080
			var text = r.width + "x" + r.height;
			var option = new Dropdown.OptionData (text);
			list.Add (option);
		}
		resolution.options = list.Distinct ().ToList ();
	}
	#endregion

	private void Awake () 
	{
		PopulateResolution ();
		/// When application started (first time)
		if (!initialCheck)
		{
			profile = Resources.Load<PostFx> ("VFX/Post-Profile");
			LoadOptions ();
			ApplyValues ();
//			SaveOptions ();
			initialCheck = true;
		}
	}
}
