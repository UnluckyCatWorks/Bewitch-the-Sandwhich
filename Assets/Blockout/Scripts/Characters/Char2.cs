using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char2 : BCharacter
{
	[Header ("Wall guy Settings")]
	public GameObject wall;
	public float wallDuration;
	public float wallDistance;
	public float maxWallDistance;

	protected override IEnumerator CastSpell () 
	{
		// Root player while casting
		var selfStun = 0.5f;
		var locks = (Locks.Movement | Locks.Interaction | Locks.Spells);
		AddCC ("Spell Casting", locks, selfStun);

		// Wait casting time
		var startTime = Time.time;
		while (Time.time < startTime + selfStun-0.01f)
			yield return null;

		// Get all 'grounds'
		var grounds = Physics.OverlapSphere ( Vector3.zero, 30f, 1<<9 );

		// Iterate through and find closest available location to casting point
		var castPoint = transform.position + movingDir*wallDistance;
		Vector3 closestCastPoint = Vector3.zero;
		float closestDistance = 100f;
		foreach (var c in grounds)
		{
			var closestPoint = c.ClosestPoint (castPoint);
			var dist = Vector3.Distance (castPoint, closestPoint);
			if (dist <= maxWallDistance && dist < closestDistance)
			{
				closestCastPoint = closestPoint;
				closestDistance = dist;
			}
		}

		if (closestCastPoint != Vector3.zero)
		{
			// Build wall
			var go = Instantiate (wall);
			go.transform.position = closestCastPoint;
			go.transform.rotation = transform.rotation;
		}
		else print ("lol");
	}
}
