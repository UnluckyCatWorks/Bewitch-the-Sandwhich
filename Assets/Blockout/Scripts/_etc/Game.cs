using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Game : MonoBehaviour
{
	public static Game manager;

	public Potion[] potions;

	private void Awake () 
	{
		manager = this;
	}
}
