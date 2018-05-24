using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
	#region DATA
	public int maxWeapons;
	public float spawnRate;

	internal List<WeaponSupply> spawns;
	#endregion

	#region CALLBACKS
	private IEnumerator Start () 
	{
		while (true) 
		{
			if (Grabbable.globalCount <= maxWeapons)
			{
				// Spawn random ingredient

				yield return new WaitForSeconds (spawnRate);
			}
			yield return null;
		}
	}

	private void Awake ()  
	{
		spawns = GetComponentsInChildren<WeaponSupply> ().ToList ();
	}
	#endregion
}
