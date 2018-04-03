using UnityEngine;
using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine.UI;

public static class HExtensions
{
	#region MESH
	/// Credit to:
	/// · https://stackoverflow.com/questions/45477806/general-method-for-calculating-smooth-vertex-normals-with-100-smoothness
	/// · Oskar Stålberg (@OskSta)
	public static void SoftNormalsAsColors (this Mesh mesh)
	{
		var tris = mesh.triangles;
		var verts = mesh.vertices;
		var tans = new Vector4[verts.Length];

		var dicVerts = new Dictionary<Vector3, SoftNormal> ();
		for (var v=0; v<tris.Length; v+=3)
		{
			int i1 = tris[v + 0];
			int i2 = tris[v + 1];
			int i3 = tris[v + 2];

			// p1, p2 and p3 are the points in the face
			var p1 = verts[i1];
			var p2 = verts[i2];
			var p3 = verts[i3];

			// Calculate normal of the surface using edges
			var edge1 = p2 - p1;
			var edge2 = p3 - p1;

			if (edge1 == Vector3.zero) continue;
			if (edge2 == Vector3.zero) continue;
			var n = Vector3.Cross(edge1, edge2).normalized;

			// Get the angle between the two other points for each point
			var angles = new float[3];
			angles[0] = Vector3.Angle(p2 - p1, p3 - p1);   // p1 is the 'base' here
			angles[1] = Vector3.Angle(p3 - p2, p1 - p2);   // p2 is the 'base' here
			angles[2] = Vector3.Angle(p1 - p3, p2 - p3);   // p3 is the 'base' here

			// Store the weighted normal in an structured array
			var tangent = new Vector4(n.x, n.y, n.z, 0f);
			for (var i=0; i!=3; i++)
			{
				SoftNormal softNormal;
				int vertIndex = tris[v + i];
				var vert = verts[vertIndex];

				if (!dicVerts.TryGetValue(vert, out softNormal))
				{
					softNormal = new SoftNormal ();
					dicVerts.Add (vert, softNormal);
				}
				if (!softNormal.indexs.Contains(vertIndex))
					softNormal.indexs.Add(vertIndex);

				softNormal.normal += tangent * angles[i];
			}
		}

		//okay
		foreach (var softNormal in dicVerts.Values)
		foreach (var index in softNormal.indexs)
				tans[index] += softNormal.normal;

		var colors = new Color32[tans.Length];
		// Normalize sum
		for (int i=0; i!=tans.Length; i++)
		{
			tans[i] = tans[i].normalized;
			// Get corresponding color
			var color =
				new Color32
				(
					// Tranform to range [0,1] and then [0,255]
					(byte) ( ( (tans[i].x + 1) / 2 ) * 255 ),
					(byte) ( ( (tans[i].y + 1) / 2 ) * 255 ),
					(byte) ( ( (tans[i].z + 1) / 2 ) * 255 ),
					1);
			colors[i] = color;
		}
		// Save to mesh
		mesh.colors32 = colors;
	}
	#endregion

	#region FLAG ENUM
	/// <summary>
	/// Usage: "if ( someEnum.HasFlag (someEnumFlag) ) {..}"
	/// </summary>
	public static bool HasFlag<T> ( this T e, T flag ) where T : struct, IConvertible
	{
		var value = e.ToInt32(CultureInfo.InvariantCulture);
		var target = flag.ToInt32(CultureInfo.InvariantCulture);

		return ((value & target) == target);
	}

	/// <summary>
	/// Usage: "someEnum = someEnum.SetFlag (someEnumFlag);"
	/// </summary>
	public static T SetFlag<T> (this T e, T flag) where T : struct, IConvertible
	{
		var value = e.ToInt32(CultureInfo.InvariantCulture);
		var newFlag = flag.ToInt32(CultureInfo.InvariantCulture);

		return (T)(object)(value | newFlag);
	}

	/// <summary>
	/// Usage: "someEnum = someEnum.UnsetFlag (someEnumFlag);"
	/// </summary>
	public static T UnsetFlag<T> (this T en, T flag) where T : struct, IConvertible
	{
		int value = en.ToInt32(CultureInfo.InvariantCulture);
		int newFlag = flag.ToInt32(CultureInfo.InvariantCulture);

		return (T)(object)(value & ~newFlag);
	}
	#endregion

	#region UI
	public static void CrossFadeAlphaFixed (this Graphic g, float alpha, float duration, bool ignoreTimeScale)
	{
		//Make the alpha 1
		Color fixedColor = g.color;
		fixedColor.a = 1;
		g.color = fixedColor;

		//Set the 0 to zero then duration to 0
		g.CrossFadeAlpha (0f, 0f, true);

		//Finally perform CrossFadeAlpha
		g.CrossFadeAlpha (alpha, duration, ignoreTimeScale);
	} 

	public static void SetAlpha (this SpriteRenderer r, float alpha) 
	{
		var c = r.color;
		c.a = alpha;
		r.color = c;
	}
	#endregion
}
