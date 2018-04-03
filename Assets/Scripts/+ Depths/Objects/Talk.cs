using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu (fileName = "New Dialog", menuName = "New dialog", order = 1000)]
public class Talk : ScriptableObject 
{
	public Dialog[] dialog;
}

[Serializable]
public struct Dialog
{
	[TextArea]
	public string message;
	public Sprite image;
	public float speed;
}
