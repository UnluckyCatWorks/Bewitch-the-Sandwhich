using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
	public Animation[] bridges;

	public void Up (int bridge) 
	{
		bridges[bridge].Play ("BridgeUp");
	} 

	public void Down (int bridge) 
	{
		bridges[bridge].Play ("BridgeDown");
	}
}
