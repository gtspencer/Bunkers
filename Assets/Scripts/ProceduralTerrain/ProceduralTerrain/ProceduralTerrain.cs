using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (MeshFilter))]
[RequireComponent (typeof (MeshRenderer))]
public class ProceduralTerrain : MonoBehaviour {

	int size = 25;

	float[,,] data;

	//When an edge transitions between a positive and negative value, it'll be marked as "crossed"
	public float surfaceCrossValue = 0;

	//The sacle of the noise for input into the system
	public float noiseScaleFactor = 20;

	Mesh localMesh;

	MeshFilter meshFilter;

	// Use this for initialization
	void Start () {
		localMesh = new Mesh();
		meshFilter = GetComponent<MeshFilter>();
		data = new float[size,size,size];
		FillData(transform.position.x, transform.position.y, transform.position.z);
		ApplyDataToMesh();
	}
	
	// Update is called once per frame
	void Update () {
		bool changed = false;
		bool changedMeshOnly = false;

		if(Input.GetKey(KeyCode.Q)) {
			surfaceCrossValue += .01f;
			changedMeshOnly = true;
		}

		if(Input.GetKey(KeyCode.E)) {
			surfaceCrossValue -= .01f;
			changedMeshOnly = true;
		}

		if(Input.GetKey(KeyCode.A)) {
			Camera.main.transform.Translate(-.5f, 0, 0, Space.World);
			this.transform.Translate(-.5f, 0, 0, Space.World);
			changed = true;
		}

		if(Input.GetKey(KeyCode.D)) {
			Camera.main.transform.Translate(.5f, 0, 0, Space.World);
			this.transform.Translate(.5f, 0, 0, Space.World);
			changed = true;
		}

		if(Input.GetKey(KeyCode.S)) {
			Camera.main.transform.Translate(0, 0, -.5f, Space.World);
			this.transform.Translate(0, 0, -.5f, Space.World);
			changed = true;
		}
		
		if(Input.GetKey(KeyCode.W)) {
			Camera.main.transform.Translate(0, 0, .5f, Space.World);
			this.transform.Translate(0, 0, .5f, Space.World);
			changed = true;
		}

		if(Input.GetKey(KeyCode.R)) {
			noiseScaleFactor += .1f;
			changed = true;
		}
		
		if(Input.GetKey(KeyCode.F)) {
			noiseScaleFactor -= .1f;
			changed = true;
		}

		if(changed || changedMeshOnly){
			if(changed)
				FillData(transform.position.x, transform.position.y, transform.position.z);

			ApplyDataToMesh();
		}
	}

	void ApplyDataToMesh() {
		TerrainMeshGenerator.FillMesh(ref localMesh, data, size, size, surfaceCrossValue);
		meshFilter.mesh = localMesh;
	}

	void FillData(float xOrigin, float yOrigin, float zOrigin) {
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				for (int z = 0; z < size; z++)
				{
					//Make all the outside edges solid, by wrapping the solids in -1s
					if(x == 0 || x == size-1) {
						data[x,y,z] = -1;
						continue;
					}
					if(y == 0 || y == size-1) {
						data[x,y,z] = -1;
						continue;
					}
					if(z == 0 || z == size-1) {
						data[x,y,z] = -1;
						continue;
					}

					float dataX = (xOrigin + x)/noiseScaleFactor;
					float dataY = (yOrigin + y)/noiseScaleFactor;
					float dataZ = (zOrigin + z)/noiseScaleFactor;

					//Use the built in Perlin noise to generate some passable noise data.

					data[x,y,z] = Mathf.PerlinNoise(dataY,dataX+dataZ) - Mathf.PerlinNoise(dataX,dataZ);

					//Apply a gradient so our values are more likely to be:
					// "air" (less than 0) at the top and "solid" (greater than 0) at the bottom
					data[x,y,z] += -(((float)y/size)-.5f);

				}
			}
		}

		//Set some data points manually just to see them displayed.
//		data[12,20,12] = .2f;
//		data[13,20,12] = 1;
//		data[14,20,12] = .2f;
	}
	
}
