using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMaster : MonoBehaviour 
{
	#region DATA
	public static UIMaster manager; 

	internal static List<AudioSource> musics;
	private static Musics currentlyPlayingMusic;
	#endregion

	#region CALLBACKS
	[RuntimeInitializeOnLoadMethod]
	private static void Initialize () 
	{
		if (manager == null)
			Instantiate (Resources.Load<UIMaster> ("Prefabs/UI/UI-Master"));
	}

	private void Awake () 
	{
		manager = this;
		DontDestroyOnLoad (gameObject);

		musics = GetComponentsInChildren<AudioSource> ().ToList ();
		PlayMusic (Musics.Menu);
	} 
	#endregion

	#region SCENE LOADING
	public static void LoadScene (Game.Modes scene) 
	{
		manager.StartCoroutine (manager.CortinillaToScene ((int)scene - 1));
		Game.stopped = true;
	}

	IEnumerator CortinillaToScene (int scene) 
	{
		var cortinilla = Instantiate (Resources.Load<Image> ("Prefabs/UI/Cortinilla"), transform);
		int id = Shader.PropertyToID ("_Scale");

		#region CLOSE CORTINILLA
		float duration = 2f;
		float factor = 0f;
		while (factor <= 1.1f)
		{
			cortinilla.materialForRendering.SetFloat (id, Mathf.Clamp01 (factor));
			yield return null;
			factor += Time.deltaTime / duration;
		} 
		#endregion

		// If loading a Game Mode
		if (scene != 0) 
		{
			// Start loading scene (additively)
			var loading = SceneManager.LoadSceneAsync (scene, LoadSceneMode.Additive);
			yield return loading;

			// De-activate WHOLE Lobby
			foreach (var g in SceneManager.GetSceneByBuildIndex (0).GetRootGameObjects ()) 
			{
				if (g.name == "Camera_Rig")
				{
					// Don't de-activate camera animator
					g.transform.GetChild (0).gameObject.SetActive (false);
					continue;
				}
				else
				if (g.name == "Focos")
				{
					// Don't de-activate the Focos animator
					g.GetComponentsInChildren <MeshRenderer> ().ToList ()
						.ForEach (m=> m.enabled = false);
					continue;
				}

				// De-activate anything else
				g.SetActive (false);
			}

			// Spawn player Characters on new scene
			SceneManager.SetActiveScene (SceneManager.GetSceneByBuildIndex (scene));
			Character.SpawnPack ();
			Game.manager.OnAwake ();

			PlayMusic (Musics.Game);
			Cursor.visible = false;
		}
		// If returning to Lobby
		else
		{
			// Un-load current scene
			var unloading = SceneManager.UnloadSceneAsync (SceneManager.GetActiveScene ());
			SceneManager.SetActiveScene (SceneManager.GetSceneByBuildIndex (0));
			yield return unloading;

			PlayMusic (Musics.Menu);
			Cursor.visible = true;

			// Activate Lobby
			foreach (var g in SceneManager.GetSceneByBuildIndex (0).GetRootGameObjects ()) 
			{
				if (g.name == "Camera_Rig")
				{
					// Reactivate camera
					g.transform.GetChild (0).gameObject.SetActive (true);
					continue;
				}
				else
				if (g.name == "Focos")
				{
					// Re-activate each Foco
					g.GetComponentsInChildren <MeshRenderer> ().ToList ()
						.ForEach (m=> m.enabled = true);
					continue;
				}
				// Skip some already-disabled objects
				else if (g.name == "Host") continue;
				else if (g.name == "Puerta_Wrapper") continue;
				else if (g.name.Contains ("Supply")) continue;

				// Re-activate anything else
				g.SetActive (true);
			}
		}

		#region OPEN CORTINILLA
		while (factor >= -0.2f)
		{
			cortinilla.materialForRendering.SetFloat (id, Mathf.Clamp01(factor));
			factor -= Time.deltaTime / duration;
			yield return null;
		}
		// Get rid of this
		Destroy (cortinilla.gameObject);
		#endregion

		// Enable game mode logic
		if (scene!=0) Game.manager.enabled = true;
	}
	#endregion

	#region RANKING
	public static void ShowRanking () 
	{
		var prefab = Resources.Load<Ranking> ("Prefabs/UI/UI-Ranking");
		var ranking = Instantiate (prefab);
	}
	#endregion

	#region AUDIO
	public enum Musics 
	{
		None,
		Menu,
		Game,
		Count
	}
	public enum SFx 
	{
		Whistle
	}

	public static void PlayMusic (Musics music) 
	{
		// Stop if already playing any music
		if (currentlyPlayingMusic != Musics.None)
			musics[(int) currentlyPlayingMusic-1].Stop ();

		// Start playing new music
		int id = (int) music - 1;
		musics[id].Play ();
		currentlyPlayingMusic = music;
	}

	public static void PlayEffect (SFx sfx) 
	{
		int id = ((int)Musics.Count - 1) + (int)sfx;
		musics[id].Play ();
	}
	#endregion
}
