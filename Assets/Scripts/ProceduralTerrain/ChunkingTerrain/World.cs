using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
	
	Chunk[,] worldChunks;

	public int worldSize = 15;
	public int chunkSize = 5;
	public int chunkHeight = 20;

	public Material defaultMaterial;

	//When an edge transitions between a positive and negative value, it'll be marked as "crossed"
	public float surfaceCrossValue = 0;
	
	//The sacle of the noise for input into the system
	public float noiseScaleFactor = 20;

	public Vector3 worldStartPosition;


	// Use this for initialization
	void Start () {
		worldChunks = new Chunk[worldSize, worldSize];
		worldStartPosition = GetPotentialStartPosition();
		for(int i = 0; i < worldSize; i++) {
			for(int j = 0; j < worldSize; j++) {
				worldChunks[i,j] = new Chunk(worldStartPosition.x + (i*chunkSize), 0, worldStartPosition.z + (j*chunkSize),
				                             chunkSize, chunkHeight, noiseScaleFactor, surfaceCrossValue, defaultMaterial, this.transform);
			}
		}
	}

	void MoveXPlus() {
		worldStartPosition.x += chunkSize;

		for(int i = 0; i < worldSize; i++) {
			for(int j = 0; j < worldSize; j++) {
				if(i == worldSize - 1) {
					//Build a new column on the right
					worldChunks[i,j] = new Chunk(worldStartPosition.x + (i*chunkSize), 0, worldStartPosition.z + (j*chunkSize),
					                             chunkSize, chunkHeight, noiseScaleFactor, surfaceCrossValue, defaultMaterial, this.transform);
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
					worldChunks[i,j] = new Chunk(worldStartPosition.x + (i*chunkSize), 0, worldStartPosition.z + (j*chunkSize),
					                             chunkSize, chunkHeight, noiseScaleFactor, surfaceCrossValue, defaultMaterial, this.transform);
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
					worldChunks[i,j] = new Chunk(worldStartPosition.x + (i*chunkSize), 0, worldStartPosition.z + (j*chunkSize),
					                             chunkSize, chunkHeight, noiseScaleFactor, surfaceCrossValue, defaultMaterial, this.transform);
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
					worldChunks[i,j] = new Chunk(worldStartPosition.x + (i*chunkSize), 0, worldStartPosition.z + (j*chunkSize),
					                             chunkSize, chunkHeight, noiseScaleFactor, surfaceCrossValue, defaultMaterial, this.transform);
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
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.D)) {
			MoveXPlus();
		}
		if(Input.GetKeyDown(KeyCode.A)) {
			MoveXMinus();
		}
		if(Input.GetKeyDown(KeyCode.R)) {
			MoveZPlus();
		}
		if(Input.GetKeyDown(KeyCode.F)) {
			MoveZMinus();
		}

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
	}


}
