using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline_Original_Code : MonoBehaviour
{
	public static void SoftNormalsInTangents (Mesh mesh) 
	{
		var mVerts = mesh.vertices;
		var mTangents = new Vector4[mVerts.Length];
		var mTris = mesh.triangles;

		Dictionary<Vector3, SoftNormal> vertDict = new Dictionary<Vector3, SoftNormal>();

		for (int i = 0; i < mTris.Length; i += 3)
		{
			int i0 = mTris[i];
			int i1 = mTris[i + 1];
			int i2 = mTris[i + 2];

			var v0 = mVerts[i0];
			var v1 = mVerts[i1];
			var v2 = mVerts[i2];

			Vector3 dir = v1 - v0;
			Vector3 tan = v2 - v0;

			if (dir == Vector3.zero) continue;
			if (tan == Vector3.zero) continue;

			var normal = Vector3.Cross(tan, dir).normalized;

			float dirLength = dir.magnitude;

			float height = Vector3.Dot(tan, Vector3.Cross(dir / dirLength, normal));

			float area = (dirLength * height) / 2f;

			normal *= area;

			Vector4 tangent = new Vector4(normal.x, normal.y, normal.z, 0);

			for (int j = 0; j < 3; j++)
			{
				SoftNormal softNormal;
				int vertIndex = mTris[i + j];
				Vector3 vert = mVerts[vertIndex];
				if (!vertDict.TryGetValue(mVerts[mTris[i + j]], out softNormal))
				{
					softNormal = new SoftNormal();
					vertDict.Add(vert, softNormal);
				}
				if (!softNormal.indexs.Contains(vertIndex)) softNormal.indexs.Add(vertIndex);
				softNormal.normal += tangent;
			}
		}

		var softNormals = vertDict.Values;

		foreach (var softNormal in softNormals)
		{
			for (int i = 0; i < softNormal.indexs.Count; i++)
			{
				mTangents[softNormal.indexs[i]] += softNormal.normal;
			}
		}

		for (int i = 0; i < mTangents.Length; i++)
		{
			mTangents[i] = -mTangents[i].normalized;
		}
		mesh.tangents = mTangents;
	}

	private void Awake () 
	{
		GetComponent<MeshFilter> ().mesh.SoftNormalsAsColors ();
	}
}
