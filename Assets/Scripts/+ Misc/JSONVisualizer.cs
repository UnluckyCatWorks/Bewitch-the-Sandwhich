using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class JSONVisualizer : MonoBehaviour
{
	public Object target;
	private Object _target;

	[TextArea (10, 20)]
	public string result;

	private void Update ()
	{
		if (target && _target != target)
		{
			_target = target;
			result = JsonUtility.ToJson (_target, prettyPrint: true);
		}
		else
		if (!target) _target = null;
	}
}
