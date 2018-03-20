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

#region TIMER
[Serializable]
public struct TimerColors 
{
	public Color basis;
	public Color from;
	public Color target;
}
#endregion

#region PLAYER
public enum PlayerIsAbleTo
{
	None,
	Action,
	Special,
	Both
}

public enum ControllerType
{
	Half_Keyboard_1,
	Half_Keyboard_2,

	Full_Gamepad
}

[System.Flags]
public enum Locks
{
	// Locomotion
	Movement = 1 << 0,
	Rotation = 1 << 2,
	Dash = 1 << 3,

	// Abilities
	Interaction = 1 << 4,
	Spells = 1 << 5,
	
	// Specials
	Locomotion = (Dash | Movement),
	Abilities = (Interaction | Spells),
	All = (Locomotion | Abilities | Rotation),
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
	NADA,
	Cristal,
	Seta,
	Escama_Dragon,
	Gusano
}

public enum IngredientType 
{
	TALCUAL,
	Molido,
	Destilado,
	Transmutado
}

[Serializable]
public struct IngredientInfo 
{
	public IngredientID id;
	public IngredientType type;
}
#endregion
