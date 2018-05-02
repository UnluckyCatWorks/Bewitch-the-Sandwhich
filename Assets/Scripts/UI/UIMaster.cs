using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMaster : MonoBehaviour 
{
	public static UIMaster manager;

	private void Awake () 
	{
		DontDestroyOnLoad (gameObject);
		manager = this;
	}

	#region SCENE LOADING
	public static void LoadScene (string scene) 
	{
		manager.StartCoroutine (manager.CortinillaToScene (scene));
	}

	IEnumerator CortinillaToScene (string scene) 
	{
		var cortinilla = Instantiate (Resources.Load<Image> ("Prefabs/Cortinilla"), transform);
		int id = Shader.PropertyToID ("_Scale");

		/// Close cortinilla
		var factor = 0f;
		var duration = 2f;
		while (factor <= 1.1f) 
		{
			cortinilla.materialForRendering.SetFloat (id, factor);
			factor += Time.deltaTime / duration;
			yield return null;
		}

		/// Load scene
		yield return SceneManager.LoadSceneAsync (scene);
		/// Wait extra time always
		yield return new WaitForSeconds (1f);

		/// Open Cortinilla
		while (factor >= 0f)
		{
			cortinilla.materialForRendering.SetFloat (id, factor);
			factor -= Time.deltaTime / duration;
			yield return null;
		}
		/// Get rid of it
		Destroy (cortinilla.gameObject);

		/// Enable game
		Game.manager.enabled = true;
	}
	#endregion
}
