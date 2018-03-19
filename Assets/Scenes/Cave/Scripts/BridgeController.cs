using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
	Animation[] bridges;
	public enum Bridge 
	{
		UP,
		TOP_R,
		MID_R,
		BOT_R,
		TOP_L,
		MID_L,
		BOT_L
	}

	public void Up (Bridge bridge) 
	{
		var i = (int) bridge;
		bridges[i].Play ("BridgeUp");
	} 

	public void Down (Bridge bridge) 
	{
		var i = (int) bridge;
		bridges[i].Play ("BridgeDown");
	}

	private void Awake ()
	{
		// Cache references
		bridges = new Animation[7];
		bridges[0] = GameObject.Find ("Bridge_UP").GetComponent<Animation> ();

		bridges[1] = GameObject.Find ("Bridge_TOP_R").GetComponent<Animation> ();
		bridges[2] = GameObject.Find ("Bridge_MID_R").GetComponent<Animation> ();
		bridges[3] = GameObject.Find ("Bridge_BOT_R").GetComponent<Animation> ();

		bridges[4] = GameObject.Find ("Bridge_TOP_L").GetComponent<Animation> ();
		bridges[5] = GameObject.Find ("Bridge_MID_L").GetComponent<Animation> ();
		bridges[6] = GameObject.Find ("Bridge_BOT_L").GetComponent<Animation> ();
	}
}
