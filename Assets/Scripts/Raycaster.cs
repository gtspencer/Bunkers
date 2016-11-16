using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Raycaster : MonoBehaviour {

	public float maxDistance = 10;
	public int layerMask = 0;
	
	public List<Vector3> rayHitPositions = new List<Vector3>();
	public List<Vector3> sphereHitPositions = new List<Vector3>();
    /**
	ComboBox layerChooser;

	// Use this for initialization
	void Start () {
		layerChooser = new ComboBox();
     	layerChooser.SetAllowMultiple(true);

		//There's no built in way to get layers, so LayerUtils was created to get the currently set layers.
		Dictionary<string, int> layers = LayerUtils.GetLayers();
		foreach(string layerName in layers.Keys) {
			layerChooser.AddItem(layerName, layers[layerName], OnLayerSelected, OnLayerUnselected);
		}
	}
	
	void OnLayerSelected(ComboBox.ComboItem item) {
		layerMask = layerMask | (1<<(int)item.value);
	}

	void OnLayerUnselected(ComboBox.ComboItem item) {
		layerMask = layerMask & ~(1<<(int)item.value);
	}
    */
    // Update is called once per frame
    void Update () {

		//Allow us to rotate the caster at 20 degrees per second
		if(Input.GetKey(KeyCode.A)) {
			this.transform.Rotate(Vector3.up, 20f * Time.deltaTime);
		}

		if(Input.GetKey(KeyCode.D)) {
			this.transform.Rotate(Vector3.up, -20f * Time.deltaTime);
		}

		rayHitPositions.Clear();
		sphereHitPositions.Clear();

		DrawRayHit();

		//DrawRayHitAll();

		//DrawSphereHit();

		//DrawSphereHitAll();
	}

	/// <summary>
	/// Int to a string of 1's and 0's representing the bits of the integer.
	/// </summary>
	/// <returns>A string representing the bits of an integer</returns>
	/// <param name="toConvert">Integer to convert</param>
	string IntToBits(int toConvert) {
		StringBuilder sb = new StringBuilder();
		for(int index = 0; index < 32; index++) {
			if((1<<index & toConvert) > 0)
				sb.Insert(0, "1");
			else
				sb.Insert(0, "0");
		}

		return sb.ToString();
	}


	/// <summary>
	/// Draws a ray cast, displaying hit information
	/// </summary>
	void DrawRayHit() {
		RaycastHit hitInfo;
		if(Physics.Raycast (this.transform.position, this.transform.forward, out hitInfo, maxDistance, layerMask)) {
			//We have a hit!
			Debug.DrawLine(this.transform.position, hitInfo.point, Color.red, .01f, true);
			rayHitPositions.Add(hitInfo.point);
		} else {
			Debug.DrawLine(this.transform.position, this.transform.forward * maxDistance, Color.green, .01f, true);
		}
	}

	/// <summary>
	/// Draws a ray cast, displaying multiple hit information
	/// </summary>
	void DrawRayHitAll() {
		Debug.DrawLine(this.transform.position, this.transform.forward * maxDistance, Color.green, .01f, true);
		RaycastHit[] hits = Physics.RaycastAll (this.transform.position, this.transform.forward, maxDistance, layerMask);
		if(hits.Length > 0) {
			foreach(RaycastHit hit in hits) {
				//We have a hit!
				Debug.DrawLine(this.transform.position, hit.point, Color.red, .01f, true);
				rayHitPositions.Add(hit.point);
			}
		}
	}

	/// <summary>
	/// Draws the sphere hit
	/// </summary>
	void DrawSphereHit() {
		RaycastHit hitInfo;
		if(Physics.SphereCast (this.transform.position, .5f, this.transform.forward, out hitInfo, maxDistance, layerMask)) {
			//We have a hit!
			Debug.DrawLine(this.transform.position, hitInfo.point, Color.red, .01f, true);
			sphereHitPositions.Add(hitInfo.point);
		} else {
			Debug.DrawLine(this.transform.position, this.transform.forward * maxDistance, Color.green, .01f, true);
		}
	}

	/// <summary>
	/// Draws the sphere hit for multiple spheres
	/// </summary>
	void DrawSphereHitAll() {
		Debug.DrawLine(this.transform.position, this.transform.forward * maxDistance, Color.green, .01f, true);
		RaycastHit[] hits = Physics.SphereCastAll (this.transform.position, .5f, this.transform.forward, maxDistance, layerMask);
		if(hits.Length > 0) {
			foreach(RaycastHit hit in hits) {
				//We have a hit!
				Debug.DrawLine(this.transform.position, hit.point, Color.red, .01f, true);
				sphereHitPositions.Add(hit.point);
			}
		}
	}

	/// <summary>
	/// Raises the draw gizmos event.
	/// </summary>
	void OnDrawGizmos() {
		//Draw the hit locations
		foreach(Vector3 rayHitPosition in rayHitPositions) {
			Gizmos.DrawWireCube(rayHitPosition, new Vector3(1f, .1f, .1f));
		}
		foreach(Vector3 sphereHitPosition in sphereHitPositions) {
			Gizmos.DrawWireSphere(sphereHitPosition, .5f);
		}
	}

	/// <summary>
	/// Raises the GUI event.
	/// </summary>
	void OnGUI() {
		//layerChooser.Draw(GUI.skin.box, new Rect(0,0, 110, 250));
		GUI.Label(new Rect(130, 0, 230, 40), "LayerMask: " + IntToBits(layerMask));
	}
}
