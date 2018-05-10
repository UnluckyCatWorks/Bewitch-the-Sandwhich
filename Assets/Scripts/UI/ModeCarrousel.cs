using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeCarrousel : MonoBehaviour
{
	#region DARA
	SmartAnimator anim;
	#endregion

	#region CALLBACKS
	private void Awake () 
	{
		anim = new SmartAnimator (GetComponent<Animator> ());
	}
	#endregion
}
