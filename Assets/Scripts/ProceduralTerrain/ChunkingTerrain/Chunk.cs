using System;
using UnityEngine;

public class Chunk
{
	float xOrigin;
	float yOrigin;
	float zOrigin;
	
	float noiseScaleFactor;
	
	int size;
	int height;

	
	Mesh chunkMesh;
	GameObject meshGameObject;
	
	public Chunk(float x, float y, float z, int size, int height, float noiseScaleFactor, float surfaceCrossValue, Material meshMaterial, Transform parent) {
		//Add one to the size to account for the marching cubes rendering missing the final edge (this causes a slight overlap in data, 
		// we can either do this, or store the data at a world level and reference it when building chunks).
		size++;
		this.size = size;
		this.height = height;
		float[,,] data = new float[size,height,size];
		this.xOrigin = x;
		this.yOrigin = y;
		this.zOrigin = z;
		this.noiseScaleFactor = noiseScaleFactor;

		meshGameObject = new GameObject("Chunk" + x + "," + y + "," + z, typeof(MeshFilter), typeof(MeshRenderer));

		meshGameObject.transform.position = new Vector3(x, y, z);
		chunkMesh = new Mesh();
		FillData(data);
		TerrainMeshGenerator.FillMesh(ref chunkMesh, data, size, height, surfaceCrossValue);
		data = null;

		meshGameObject.GetComponent<MeshFilter>().mesh = chunkMesh;
		meshGameObject.GetComponent<MeshRenderer>().material = meshMaterial;

		meshGameObject.transform.parent = parent;
	}
	
	private void FillData(float[,,] data) {
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < height; y++) {
				for (int z = 0; z < size; z++)
				{
					if(y == height-1) { //put a cap on things outside our bounds
						data[x,y,z] = -1;
						continue;
					}
					
					if(y == 0) {//put a floor on the bottom
						data[x,y,z] = 1;
						continue;
					}
					
					float dataX = (xOrigin + x)/noiseScaleFactor;
					float dataY = (yOrigin + y)/noiseScaleFactor;
					float dataZ = (zOrigin + z)/noiseScaleFactor;
					
					//Use the built in Perlin noise to generate some passable noise data.
					
					data[x,y,z] = Mathf.PerlinNoise(dataY,dataX+dataZ) - Mathf.PerlinNoise(dataX,dataZ);
					
					//Apply a gradient so our values are more likely to be:
					// "air" (less than 0) at the top and "solid" (greater than 0) at the bottom
					data[x,y,z] -= (((float)y/height)-.5f);
					
				}
			}
		}
	}

	public void Dispose ()
	{
		chunkMesh = null;
		GameObject.Destroy(meshGameObject);
	}
}


