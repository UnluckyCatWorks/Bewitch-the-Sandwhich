using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceGuy : Character
{
	[Header("Spell Settings")]
	public BoxCollider spellBox;
	public float stunDuration;

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

		// Find all players affected by the spell
		var hits = Physics.OverlapBox (spellBox.center, spellBox.size / 2f);
		var players = from cols in hits
					  where cols.tag == "Player" && cols.name != name
					  select cols.GetComponent<Character> ();

		// Apply spell on them
		foreach (var p in players.Distinct ())
			p.AddCC ("Spell: Frozen", Locks.All, stunDuration);
	}
}
