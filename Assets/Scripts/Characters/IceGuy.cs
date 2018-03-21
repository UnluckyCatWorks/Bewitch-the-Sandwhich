using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceGuy : Character
{
	[Header("Spell Settings")]
	public float stunDuration;

	protected override void CastSpell () 
	{
		var radius = 2.2f;
		var pos = transform.position + transform.forward * (radius + 0.15f);

		// Find all players affected by the spell
		var hits = Physics.OverlapSphere (pos, radius);
		var players = from cols in hits
					  where cols.tag == "Player" && cols.name != name
					  select cols.GetComponent<Character> ();

		// Apply spell on them
		foreach (var p in players.Distinct ())
			p.AddCC ("Spell: Frozen", Locks.All, stunDuration);
	}
}
