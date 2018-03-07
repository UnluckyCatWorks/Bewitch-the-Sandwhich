using System;
using System.Collections.Generic;
using UnityEngine;

// All the states a machine can be in
public enum MachineState
{
	Waiting,		// Machine is resting
	Working,		// The machine is locked, doing its process
	Completed,		// The machine is done processing, player can pick up result
	Overheating,	// Machine is starting to overload, player can still pick it up
	Overloading		// Machine is locked again, overloading; player can't pick up, result is lost
}

internal class SoftNormal
{
	public List<int> indexs = new List<int>();
	public Vector4 normal;
}

#region PLAYER
public enum PlayerIsAbleTo
{
	None,
	Action,
	Special,
	Both
}

public enum PlayerController
{
	Keyboard,
	Gamepad
}

[System.Flags]
public enum Locks
{
	// Locomotion
	Movement = 1 << 0,
	Rotation = 1 << 2,
	Sprint = 1 << 3,

	// Abilities
	Interaction = 1 << 4,
	Spells = 1 << 5,
	
	// Specials
	Locomotion = (Sprint | Rotation | Movement),
	Abilities = (Interaction | Spells),
	All = (Locomotion | Abilities),
	NONE = 0
}

public struct Effect
{
	public Locks cc;
}
#endregion

#region INGREDIENTS
// List of all (raw) ingredients
public enum IngredientID
{
	NONE,
	// TO DO
	Eye
}

public enum IngredientType
{
	Raw,
	Ground,
	Destiled,
	Transmuted
}

[System.Serializable]
public struct IngredientInfo
{
	public IngredientID id;
	public IngredientType type;
}
#endregion

#region POTIONS
public enum PotionID
{
	NONE
	// TODO
}

public enum PotionType
{
	Original
	// TODO
}

[System.Serializable]
public struct PotionInfo
{
	public PotionID id;
	public PotionType type;
}
#endregion
