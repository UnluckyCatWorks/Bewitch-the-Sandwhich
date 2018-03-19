using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SapoHelper : MonoBehaviour
{
	private MachineInterface sapo;

	// Animator IDs
	private static bool hashd;
	public static int Lengua_Fuera;
	public static int Lengua_Dentro;

	private void OnTriggerEnter (Collider other) 
	{
		if (other.tag == "Player"
		&& sapo.CheckInteraction (other.GetComponent<Character> ()) != PlayerIsAbleTo.None)
		{
			sapo.SetTrigger (Lengua_Fuera);
		}
	}
	private void OnTriggerExit (Collider other) 
	{
		if (other.tag == "Player")
		{
			sapo.SetTrigger (Lengua_Dentro);
		}
	}

	private void Awake () 
	{
		if (!hashd)
		{
			Lengua_Fuera  = Animator.StringToHash ("Lengua_Fuera");
			Lengua_Dentro = Animator.StringToHash ("Lengua_Dentro");
			hashd = true;
		}
	}
}
