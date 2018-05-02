using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
	[CreateAssetMenu]
	public class LocalStats : ScriptableObject 
	{
		public List<PlayerData> registeredPlayers;
	}

	[Serializable]
	public class PlayerData 
	{
		public string name;
		public List<GameModeStats> stats;
	}

	[Serializable]
	public struct GameModeStats 
	{
		public Game.Modes mode;

		public int kills;
		public int deaths;

		public int roundsWon;
		public int roundsPlayed;

		// more stuff
	}
}
