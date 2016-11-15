using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicWorld : MonoBehaviour {
	DynamicChunk[,] worldChunks;
	
	public int worldSize = 15;
	public int chunkSize = 5;
	public int chunkHeight = 20;

	public int toolRadius = 5;
	public float toolStrength = .01f;

	public Material defaultMaterial;
	
	//When an edge transitions between a positive and negative value, it'll be marked as "crossed"
	public float surfaceCrossValue = 0;
	
	//The sacle of the noise for input into the system
	public float noiseScaleFactor = 20;
	
	public Vector3 worldStartPosition;

	List<DynamicChunk> chunksToUpdate = new List<DynamicChunk>();

	void Start () {
		worldChunks = new DynamicChunk[worldSize, worldSize];
		worldStartPosition = GetPotentialStartPosition();
		for(int i = 0; i < worldSize; i++) {
			for(int j = 0; j < worldSize; j++) {

				float chunkX = worldStartPosition.x + (i*chunkSize);
				float chunkY = 0;
				float chunkZ = worldStartPosition.z + (j*chunkSize);

				worldChunks[i,j] = GenerateNewChunk(chunkX, chunkY, chunkZ);
			}
		}
		for(int i = 0; i < worldSize; i++) {
			for(int j = 0; j < worldSize; j++) {
				worldChunks[i,j].GenerateNewMesh(surfaceCrossValue);
			}
		}
	}

	DynamicChunk GenerateNewChunk(float chunkX, float chunkY, float chunkZ){
		float[,,] chunkData = new float[chunkSize, chunkHeight, chunkSize];

		FillData(ref chunkData, (int)chunkX, (int)chunkY, (int)chunkZ);

		GameObject chunk = new GameObject("Chunk" + chunkX + "," + chunkY + "," + chunkZ,
		                                  typeof(DynamicChunk), typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
		
		chunk.transform.position = new Vector3(chunkX, chunkY, chunkZ);
		chunk.transform.parent = this.transform;
		DynamicChunk dc = chunk.GetComponent<DynamicChunk>();
		dc.Initialize(chunkData, chunkSize, chunkHeight, defaultMaterial, this);
		return dc;
	}

	
	public float GetValue(int x, int y, int z) {
		int relativeX = (int)(x - worldStartPosition.x);
		int relativeZ = (int)(z - worldStartPosition.z);
		
		int chunkX = (int)(relativeX/chunkSize);
		int chunkZ = (int)(relativeZ/chunkSize);
		
		if(chunkX >= 0 && chunkX < worldSize &&
		   y >= 0 && y < chunkHeight && 
		   chunkZ >= 0 && chunkZ < worldSize){
			return worldChunks[chunkX, chunkZ].GetValue(
				(int)(x - worldStartPosition.x)%chunkSize,
				y,
				(int)(z - worldStartPosition.z)%chunkSize);
		} else {
			return GenerateDataValueForPoint(x,y,z);
		}
	}
	
	public void SetValue(int x, int y, int z, float value) {
		int relativeX = (int)(x - worldStartPosition.x);
		int relativeZ = (int)(z - worldStartPosition.z);
		
		int chunkX = (int)(relativeX/chunkSize);
		int chunkZ = (int)(relativeZ/chunkSize);
		if(chunkX >= 0 && chunkX < worldSize &&
		   chunkZ >= 0 && chunkZ < worldSize){
			worldChunks[chunkX, chunkZ].SetValue(
				(int)(x - worldStartPosition.x)%chunkSize,
				y,
				(int)(z - worldStartPosition.z)%chunkSize,
				value);
		}
	}


	void Update() {

		//Add density to a selected radius
		if(Input.GetKey(KeyCode.Q)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast (ray, out hitInfo)) 
			{
				AddDensity(hitInfo.point, toolRadius);
			}
		}

		//Subtract density from a selected radius
		if(Input.GetKey(KeyCode.E)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast (ray, out hitInfo)) 
			{
				SubDensity(hitInfo.point, toolRadius);
			}
		}

		//Apply a crater to the landscape
		if(Input.GetKeyDown(KeyCode.C)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast (ray, out hitInfo)) 
			{
				Crater(hitInfo.point, toolRadius, 10);
			}
		}

		//Apply the flat tool to the landscape
		if(Input.GetKey(KeyCode.F)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast (ray, out hitInfo)) 
			{
				MakeFlat(hitInfo.point, toolRadius, 10);
			}
		}

		//Update the world as we move around
		Vector3 potentialStart = GetPotentialStartPosition();
		if(worldStartPosition.x < potentialStart.x) {
			MoveXPlus();
		}
		else if(worldStartPosition.x > potentialStart.x) {
			MoveXMinus();
		}
		
		if(worldStartPosition.z < potentialStart.z) {
			MoveZPlus();
		}
		else if(worldStartPosition.z > potentialStart.z) {
			MoveZMinus();
		}

		//Rebuild the meshes for any chunks that need it
		if(chunksToUpdate.Count > 0) {
			foreach(DynamicChunk chunk in chunksToUpdate) {
				chunk.GenerateNewMesh(surfaceCrossValue);
			}
			chunksToUpdate.Clear();
		}

		//Spawn a cube to check out the physics
		if(Input.GetKeyDown(KeyCode.Space)) {
			SpawnPrimitive(PrimitiveType.Cube);
		}
	}

	void FillData(ref float[,,] data, int xOffset, int yOffset, int zOffset) {
		for (int x = 0; x < chunkSize; x++) {
			for (int y = 0; y < chunkHeight; y++) {
				for (int z = 0; z < chunkSize; z++)
				{
					data[x,y,z] = GenerateDataValueForPoint(x + xOffset, y + yOffset, z + zOffset);
				}
			}
		}
	}

	//Generates a density value for a given position
	//Also useful for the edges of chunks where we don't have data saved yet, but want to retrieve what
	// the data *will* be, for that edge.
	private float GenerateDataValueForPoint(int x, int y, int z) {

		if(y >= chunkHeight-1) { //put a cap on things outside our bounds
			return -1;
		}
		
		if(y == 0) {//put a floor on the bottom
			return 1;
		}
		
		float dataX = x/noiseScaleFactor;
		float dataY = y/noiseScaleFactor;
		float dataZ = z/noiseScaleFactor;
		
		//Use the built in Perlin noise to generate some passable noise data.
		
		float retValue = Mathf.PerlinNoise(dataY,dataX+dataZ) - Mathf.PerlinNoise(dataX,dataZ);
		
		//Apply a gradient so our values are more likely to be:
		// "air" (less than 0) at the top and "solid" (greater than 0) at the bottom
		retValue -= (((float)y/chunkHeight)-.5f);
		return retValue;
	}

	void MoveXPlus() {
		worldStartPosition.x += chunkSize;
		
		for(int i = 0; i < worldSize; i++) {
			for(int j = 0; j < worldSize; j++) {
				if(i == worldSize - 1) {
					//Build a new column on the right
					worldChunks[i,j] = GenerateNewChunk(worldStartPosition.x + (i*chunkSize), 0, worldStartPosition.z + (j*chunkSize));
					chunksToUpdate.Add(worldChunks[i,j]);
					continue;
				} else if(i == 0) {
					//Free up the chunks at the x minus side
					worldChunks[i,j].Dispose();
				}
				
				//Shift everything else in the arrays to the left
				worldChunks[i,j] = worldChunks[i+1,j];
			}
		}
	}
	
	void MoveZPlus() {
		worldStartPosition.z += chunkSize;
		
		for(int i = 0; i < worldSize; i++) {
			for(int j = 0; j < worldSize; j++) {
				if(j == worldSize - 1) {
					//Build a new column on the top
					worldChunks[i,j] = GenerateNewChunk(worldStartPosition.x + (i*chunkSize), 0, worldStartPosition.z + (j*chunkSize));
					chunksToUpdate.Add(worldChunks[i,j]);
					continue;
				} else if(j == 0) {
					//Free up the chunks at the z minus side
					worldChunks[i,j].Dispose();
				}
				
				//Shift everything else in the arrays down
				worldChunks[i,j] = worldChunks[i,j+1];
			}
		}
	}
	
	void MoveXMinus() {
		worldStartPosition.x -= chunkSize;
		
		for(int i = worldSize - 1; i >= 0; i--) {
			for(int j = 0; j < worldSize; j++) {
				if(i == 0) {
					//Build a new column on the left
					worldChunks[i,j] = GenerateNewChunk(worldStartPosition.x + (i*chunkSize), 0, worldStartPosition.z + (j*chunkSize));
					chunksToUpdate.Add(worldChunks[i,j]);
					continue;
				} else if(i == worldSize - 1) {
					//Free up the chunks at the x plus side
					worldChunks[i,j].Dispose();
				}
				
				//Shift everything else in the arrays to the right
				worldChunks[i,j] = worldChunks[i-1,j];
			}
		}
	}
	
	void MoveZMinus() {
		worldStartPosition.z -= chunkSize;
		
		for(int i = 0; i < worldSize; i++) {
			for(int j = worldSize - 1; j >= 0; j--) {
				if(j == 0) {
					//Build a new column on the bottom
					worldChunks[i,j] = GenerateNewChunk(worldStartPosition.x + (i*chunkSize), 0, worldStartPosition.z + (j*chunkSize));
					chunksToUpdate.Add(worldChunks[i,j]);
					continue;
				} else if(j == worldSize - 1) {
					//Free up the chunks at the z plus side
					worldChunks[i,j].Dispose();
				}
				
				//Shift everything else in the arrays up
				worldChunks[i,j] = worldChunks[i,j-1];
			}
		}
	}


	Vector3 ConvertCameraPosition() {
		Vector3 camPos = Camera.main.transform.position;
		return new Vector3(Mathf.Round(camPos.x/chunkSize)*chunkSize, 0, Mathf.Round(camPos.z/chunkSize)*chunkSize);
	}
	
	Vector3 GetPotentialStartPosition() {
		return new Vector3(ConvertCameraPosition().x-((worldSize*chunkSize)/2), 0, ConvertCameraPosition().z-((worldSize*chunkSize)/2));
	}

	//Create a crater type effect in the landscape
	//This works best on horizontal land. It will primarily reduce the density of the terrain
	// except around the edges it will increase it slightly to provide the illusion of terrain being pushed out.
	void Crater(Vector3 center, float radius, float modAmt) {
		Vector3 currentPosition = new Vector3();
		for(int i = Mathf.FloorToInt(center.x - radius); i < Mathf.CeilToInt(center.x + radius); i++) {
			for(int j = Mathf.FloorToInt(center.y - radius); j < Mathf.CeilToInt(center.y + radius); j++) {
				for(int k = Mathf.FloorToInt(center.z - radius); k < Mathf.CeilToInt(center.z + radius); k++) {
					currentPosition.Set(i,j,k);
					float distance = Vector3.Distance(center, currentPosition);

					if(j < center.y) {
						if(distance < radius) {
							ModDensityAt(i,j,k,modAmt*Mathf.Log((distance/radius)+.2f));
						}
					} else {
						if(distance < radius) {
							ModDensityAt(i,j,k,modAmt*Mathf.Log((distance/radius)));
						}
					}
				}
			}
		}
	}

	//Gradually makes terrain flat. Uses the height of the center as the divide between ground and air.
	void MakeFlat(Vector3 center, float radius, float modAmt) {
		Vector3 currentPosition = new Vector3();
		for(int i = Mathf.FloorToInt(center.x - radius); i < Mathf.CeilToInt(center.x + radius); i++) {
			for(int j = Mathf.FloorToInt(center.y - radius); j < Mathf.CeilToInt(center.y + radius); j++) {
				for(int k = Mathf.FloorToInt(center.z - radius); k < Mathf.CeilToInt(center.z + radius); k++) {
					currentPosition.Set(i,j,k);
					//Everything above the center loses density
					//Everything below gains density
					if(j < center.y) {
						ModDensityAt(i,j,k,.01f);
					} else {
						ModDensityAt(i,j,k,-.01f);
					}
				}
			}
		}
	}

	//A generic function for modifying the density of the terrain within a radius
	void ModifyDensityFunction(Vector3 center, float radius, float modAmt) {
		Vector3 currentPosition = new Vector3();
		for(int i = Mathf.FloorToInt(center.x - radius); i < Mathf.CeilToInt(center.x + radius); i++) {
			for(int j = Mathf.FloorToInt(center.y - radius); j < Mathf.CeilToInt(center.y + radius); j++) {
				for(int k = Mathf.FloorToInt(center.z - radius); k < Mathf.CeilToInt(center.z + radius); k++) {
					currentPosition.Set(i,j,k);
					if(Vector3.Distance(center, currentPosition) < radius) {
						ModDensityAt(i,j,k,modAmt);
					}
				}
			}
		}
	}

	void AddDensity(Vector3 center, float radius) {
		ModifyDensityFunction(center, radius, toolStrength);
	}

	void SubDensity(Vector3 center, float radius) {
		ModifyDensityFunction(center, radius, -toolStrength);
	}

	//Used to change the density value at a specific data point
	void ModDensityAt(int x, int y, int z, float value) {
		int relativeX = (int)(x - worldStartPosition.x);
		int relativeZ = (int)(z - worldStartPosition.z);

		DynamicChunk chunk = GetChunkContaining(x,z);

		if(chunk != null) {
			int chunkOffsetX = (int)(x - worldStartPosition.x)%chunkSize;
			int chunkOffsetZ = (int)(z - worldStartPosition.z)%chunkSize;

			if(chunkOffsetX >= 0 && chunkOffsetX < chunkSize && 
			    y >= 0 && y < chunkHeight &&
			    chunkOffsetZ >= 0 && chunkOffsetZ < chunkSize) {

				float currentValue = chunk.GetValue(chunkOffsetX, y, chunkOffsetZ);
				chunk.SetValue(chunkOffsetX, y, chunkOffsetZ, Mathf.Max(-1, Mathf.Min (currentValue + value, 1)));
				AddChunkToUpdate(chunk);

				//Check surrounding chunks for data changes.
				DynamicChunk neighborChunk;
				if(chunkOffsetX == 0) {
					neighborChunk = GetChunkContaining(x-chunkSize,z);
					AddChunkToUpdate(neighborChunk);
				}
				if(chunkOffsetZ == 0) {
					neighborChunk = GetChunkContaining(x,z-chunkSize);
					AddChunkToUpdate(neighborChunk);
				}

				if(chunkOffsetX == chunkSize) {
					neighborChunk = GetChunkContaining(x+chunkSize,z);
					AddChunkToUpdate(neighborChunk);
				}
				if(chunkOffsetZ == chunkSize) {
					neighborChunk = GetChunkContaining(x,z+chunkSize);
					AddChunkToUpdate(neighborChunk);
				}
			}
		}
	}

	//Keep track of chunks that need their meshes rebuilt
	void AddChunkToUpdate(DynamicChunk chunk) {
		if(chunk != null) {
			if(!chunksToUpdate.Contains(chunk)) {
				chunksToUpdate.Add(chunk);
			}
		}
	}

	//Return the chunk that contains the given coordinates
	DynamicChunk GetChunkContaining(int x, int z) {
		int relativeX = (int)(x - worldStartPosition.x);
		int relativeZ = (int)(z - worldStartPosition.z);

		int chunkX = (int)(relativeX/chunkSize);
		int chunkZ = (int)(relativeZ/chunkSize);

		if(chunkX < worldSize && chunkX >= 0 &&
		   chunkZ < worldSize && chunkZ >= 0) {
		return worldChunks[chunkX, chunkZ];
		} else {
			return null;
		}
	}

	//Spawn a rigid body object, and allow it to live for 10 seconds
	void SpawnPrimitive(PrimitiveType type) {
		Vector3 spawnPosition = MouseUtils.GetMouseWorldPositionAtDepth(10);
		GameObject go = GameObject.CreatePrimitive(type);
		go.transform.position = spawnPosition;
		go.AddComponent<Rigidbody>();
		
		GameObject.Destroy(go, 10);
	}

}
