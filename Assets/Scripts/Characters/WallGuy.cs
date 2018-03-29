using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGuy : Character
{
	[Header ("Spell Settings")]
	public WonderWall wall;
	public float wallDuration;
	public float wallDistance;

	protected override IEnumerator CastSpell () 
	{
		var clock = 0f;
		var go = Instantiate (wall);
		while (clock <= spellSelfStun-0.15f)
		{
			var newPos = transform.position;
			newPos.y = -0.095f;
			go.transform.position = newPos + movingDir * wallDistance;
			clock += Time.deltaTime;
			yield return null;
		}
		yield return new WaitForSeconds (wallDuration);
		go.Destroy ();
	}
}
