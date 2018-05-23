using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameStats 
{
	public int roundsWon;
	public float[] scores;

	public GameStats (int scoreAmount = 3) 
	{
		roundsWon = 0;
		scores = new float[scoreAmount];
	}
}
