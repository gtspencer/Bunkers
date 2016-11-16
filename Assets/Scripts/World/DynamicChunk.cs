using UnityEngine;
using System.Collections;

public class DynamicChunk : MonoBehaviour {

	int size;
	int height;

	MeshFilter meshFilter;
	MeshCollider meshCollider;
	DynamicWorld parentWorld;

	float[,,] data;

	public void Initialize(float[,,] data, int XZSize, int YSize, Material material, DynamicWorld world) {
		this.meshFilter = GetComponent<MeshFilter>();
		this.meshCollider = GetComponent<MeshCollider>();
		this.GetComponent<MeshRenderer>().material = material;
		this.data = data;
		this.size = XZSize;
		this.height = YSize;
		this.parentWorld = world;
	}

	public void GenerateNewMesh(float surfaceCrossValue) {
		Mesh m = new Mesh();
		WorldMeshGenerator.FillMesh(ref m, (int)transform.position.x, (int)transform.position.y, (int)transform.position.z, parentWorld, size, height, surfaceCrossValue);
		this.meshFilter.mesh = m;
		this.meshCollider.sharedMesh = m;
	}

	public float GetValue(int x, int y, int z) {
		return data[x,y,z];
	}

	public void SetValue(int x, int y, int z, float value) {
		data[x,y,z] = value;
	}

	public void Dispose(){
		GameObject.Destroy(this.gameObject);
	}
}
