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

public enum SpellResult 
{
	Undefined,
	Missed,
	Hit
}

#region PLAYER
public enum Characters 
{
	NONE,
	Bobby,
	Lilith,
	Amy,
	Milton
}

public enum ControllerType 
{
	UNESPECIFIED,

	Keyboard_Left,
	Keyboard_Right,

	Gamepad_1,
	Gamepad_2
}

[Flags]
public enum Locks
{
	// Locomotion
	Movement = 1 << 0,
	Rotation = 1 << 2,
	Dash = 1 << 3,

	// Capabilities
	Interaction = 1 << 4,
	Spells = 1 << 5,
	
	// Curses
	Crazy = 1 << 6,
	Burning = 1 << 7,

	// Groups
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
	Gusano,
	Count
}

public enum IngredientType 
{
	TALCUAL,
	Molido,
	Destilado,
	Transmutado,
	Count
}

[Serializable]
public struct IngredientInfo 
{
	public IngredientID id;
	public IngredientType type;
}
#endregion
