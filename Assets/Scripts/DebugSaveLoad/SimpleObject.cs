using UnityEngine;
using System.Collections;
using System.IO;

public class SimpleObject : MonoBehaviour {

	public string aStringObject = "test";
	public float aFloatValue = 43.2f;
	
	public void WriteObjectState(BinaryWriter binaryWriter) {
		//Get all the subObjects that are children of this object.
		SimpleSubObject[] subObjects = this.transform.GetComponentsInChildren<SimpleSubObject>();
		//Write out how many objects there are, so we know how many to read in later
		binaryWriter.Write(subObjects.Length);

		//Each object is responsible for writing its own state.
		foreach(SimpleSubObject subObject in subObjects) {
			subObject.WriteObjectState(binaryWriter);
		}

		//Now write our own state
		binaryWriter.Write(aStringObject);
		binaryWriter.Write(aFloatValue);

		binaryWriter.Write(this.gameObject.name);
	}

	public void ReadObjectState(BinaryReader binaryReader) {
		//Get the subObjects count
		int simpleSubCount = binaryReader.ReadInt32();
		for(int subCount = 0; subCount < simpleSubCount; subCount++) {
			GameObject simpleSub = new GameObject();
			SimpleSubObject simpleSubScript = simpleSub.AddComponent<SimpleSubObject>();
			simpleSubScript.ReadObjectState(binaryReader);
			simpleSub.transform.parent = this.transform;
		}

		this.aStringObject = binaryReader.ReadString();
		this.aFloatValue = binaryReader.ReadSingle();

		this.gameObject.name = binaryReader.ReadString();

	}

	public void WriteObjectState_Web(string prependKey) {
		//Get all the subObjects that are children of this object.
		SimpleSubObject[] subObjects = this.transform.GetComponentsInChildren<SimpleSubObject>();
		//Write out how many objects there are, so we know how many to read in later
		PlayerPrefs.SetInt(prependKey+"subObjectCount", subObjects.Length);

		//Each object is responsible for writing its own state.
		//Maintain a subCount variable to add distinction between objects
		int subCount = 0;
		foreach(SimpleSubObject subObject in subObjects) {
			subObject.WriteObjectState_Web(prependKey + subCount++);
		}

		PlayerPrefs.SetString(prependKey+"aStringObject",aStringObject);
		PlayerPrefs.SetFloat(prependKey+"aFloatValue",aFloatValue);

		PlayerPrefs.SetString(prependKey+"objectName", this.gameObject.name);
	}
	
	public void ReadObjectState_Web(string prependKey) {
		//Get the subObjects count
		int simpleSubCount = PlayerPrefs.GetInt(prependKey+"subObjectCount");
		for(int subCount = 0; subCount < simpleSubCount; subCount++) {
			GameObject simpleSub = new GameObject();
			SimpleSubObject simpleSubScript = simpleSub.AddComponent<SimpleSubObject>();
			simpleSubScript.ReadObjectState_Web(prependKey + subCount);
			simpleSub.transform.parent = this.transform;
		}
		
		this.aStringObject = PlayerPrefs.GetString(prependKey+"aStringObject");
		this.aFloatValue = PlayerPrefs.GetFloat(prependKey+"aFloatValue");

		this.gameObject.name = PlayerPrefs.GetString(prependKey+"objectName");
	}
}
