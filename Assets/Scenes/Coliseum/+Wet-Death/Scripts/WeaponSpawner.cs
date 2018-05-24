using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
	#region DATA

	#endregion

	#region CALLBACKS
	private IEnumerator Start () 
	{
		while (true) 
		{

			yield return null;
		}
	}

	private void Awake ()  
	{
		
	}
	#endregion
}
