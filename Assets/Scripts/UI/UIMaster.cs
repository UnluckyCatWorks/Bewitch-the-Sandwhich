using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMaster : MonoBehaviour 
{
	public static UIMaster manager;

	#region CALLBACKS
	[RuntimeInitializeOnLoadMethod]
	private static void Initialize () 
	{
		if (manager == null)
			Instantiate (Resources.Load<UIMaster> ("Prefabs/UI/UI-Master"));
	}

	private void Awake ()
	{
		DontDestroyOnLoad (gameObject);
		manager = this;
	} 
	#endregion

	#region SCENE LOADING
	public static void LoadScene (Game.Modes scene) 
	{
//		string sceneToLoad = (scene == Game.Modes.Tutorial? "Lobby" : scene.ToString ());
		manager.StartCoroutine (manager.CortinillaToScene ((int)scene - 1));
		Game.stopped = true;
	}

	IEnumerator CortinillaToScene (int scene) 
	{
		var cortinilla = Instantiate (Resources.Load<Image> ("Prefabs/UI/Cortinilla"), transform);
		int id = Shader.PropertyToID ("_Scale");

		// Close cortinilla
		float duration = 2f;
		float factor = 0f;
		while (factor <= 1.1f) 
		{
			cortinilla.materialForRendering.SetFloat (id, factor);
			yield return null;
			factor += Time.deltaTime / duration;
		}
//		factor = 0f;

		// Start loading scene
		var loading = SceneManager.LoadSceneAsync (scene);

		#warning Mode help not implemented yet
		// Show mode help
//		var help = cortinilla.transform.GetChild ((int)Game.mode-2).GetComponent<Image> ();
//		while (factor <= 1.1f)
//		{
//			help = 
//			yield return null;
//			factor += Time.deltaTime / /*duration*/ 0.25f;
//		}
//
//		while

		yield return loading;

		// Open Cortinilla
		while (factor >= -0.1f) 
		{
			cortinilla.materialForRendering.SetFloat (id, factor);
			factor -= Time.deltaTime / duration;
			yield return null;
		}

		// Enable game mode logic
		Game.manager.enabled = true;

		// Get rid of this
		Destroy (cortinilla.gameObject);
	}
	#endregion
}
