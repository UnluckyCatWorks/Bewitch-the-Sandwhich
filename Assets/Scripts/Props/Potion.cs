using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Grabbable
{
	public PotionID id;
	public PotionType type;
	public PotionInfo info 
	{
		get { return new PotionInfo { id = id, type = type }; }
	}
	public IngredientInfo[] receipt;
	public Sprite icon;
}
