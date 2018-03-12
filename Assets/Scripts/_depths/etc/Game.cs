using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
	#region DATA
	public static Game manager;
	public static int[] scores;
	
	public Potion[] potions;
	#endregion

	#region UTILITIES
	public static void SpawnOrder ( PotionID order ) 
	{

	}
	#endregion

	private void Awake () 
	{
		scores = new int[3];
		manager = this;
	}
}
