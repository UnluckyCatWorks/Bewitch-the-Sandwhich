using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGuy : Character
{
	[Header ("Spell Settings")]
	public GameObject wall;
	public float wallDuration;
	public float wallDistance;

	protected override void CastSpell () 
	{
		var go = Instantiate (wall);
		go.transform.position = transform.position + transform.forward * wallDistance;
		go.transform.rotation = transform.rotation;
	}
}
