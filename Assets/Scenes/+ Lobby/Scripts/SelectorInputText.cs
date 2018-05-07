using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectorInputText : MonoBehaviour, ISelectHandler
{
	public void OnSelect (BaseEventData eventData) 
	{
		Selector.SetAllFrozen (frozen: true);
	}
}
