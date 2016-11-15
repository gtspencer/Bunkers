using UnityEngine;
using System.Collections;

[RequireComponent (typeof (MeshFilter))]
[RequireComponent (typeof (MeshRenderer))]
[RequireComponent (typeof (MeshCollider))]
public class MidpointDisplaceLandscape : MonoBehaviour {

	MidpointDisplacement mpd;
	public int size;
	public int worldHeight;
	public float[] roughValues;
	public float roughFactor;
	public Material defaultMaterial;
	public bool smooth;
	bool altered = false;
	float[,,] worldData;

	public int toolRadius = 5;
	public float toolStrength = .01f;

	// Use this for initialization
	void Start () {
		Generate();
	}

	void Generate() {
		mpd = new MidpointDisplacement(size, roughFactor, 0, worldHeight, worldHeight/2f);
		mpd.SetBaseValue();
		mpd.SetRandomValue();
		mpd.Generate(roughValues);
		worldData = new float[size,worldHeight,size];
		
		for(int i = 0; i < size*size; i++) {
			float height = Mathf.Min(mpd.data[(int)i/size,i%size], worldHeight-1);

			for(int h = 0; h < (int)(height); h++)
				worldData[(int)i/size, h, i%size] = 1;

			if(smooth) {
				if(height >= 0)
					worldData[(int)i/size, (int)(height), i%size] = height - (int)height;
			}
		}
		RegenerateMesh();
	}

	void RegenerateMesh() {
		Mesh m = new Mesh();
		TerrainMeshGenerator.FillMesh(ref m, worldData, size, worldHeight, .5f);
		this.GetComponent<MeshFilter>().mesh = m;
		this.GetComponent<MeshCollider>().sharedMesh = m;
		this.GetComponent<MeshRenderer>().material = defaultMaterial;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)) {
			Generate();
		}

		//Add density to a selected radius
		if(Input.GetKey(KeyCode.Q)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast (ray, out hitInfo)) 
			{
				AddDensity(hitInfo.point, toolRadius);
				altered = true;
			}
		}
		
		//Subtract density from a selected radius
		if(Input.GetKey(KeyCode.E)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast (ray, out hitInfo)) 
			{
				SubDensity(hitInfo.point, toolRadius);
				altered = true;
			}
		}

		if(altered) {
			RegenerateMesh();
			altered = false;
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
		if(x >= 0 && x < size &&
		   y >= 0 && y < worldHeight &&
		   z >= 0 && z < size) {

			worldData[x,y,z] = Mathf.Max(0, Mathf.Min (worldData[x,y,z] + value, 1));
		}
	}
}
