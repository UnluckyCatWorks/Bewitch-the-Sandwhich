using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameStats 
{
	public float this[Enum index] 
	{
		get
		{
			// Null check
			if (scores == null)
				scores = new float[ScoreAmount];

			// Return value
			int id = Convert.ToInt32 (index);
			return scores[id];
		}
		set
		{
			// Null check
			if (scores == null)
				scores = new float[ScoreAmount];
			 
			// Set value
			int id = Convert.ToInt32 (index);
			scores[id] = value;
		}
	}
	public float this[int index] 
	{
		get
		{
			// Null check
			if (scores == null)
				scores = new float[ScoreAmount];

			// Return value
			return scores[index];
		}
		set
		{
			// Null check
			if (scores == null)
				scores = new float[ScoreAmount];

			// Set value
			scores[index] = value;
		}
	}

	public int roundsWon;
	private float[] scores;

	private const int ScoreAmount = 3;
}
