using UnityEngine;
using System.Collections;
using System.IO;

public class SimpleSubObject : MonoBehaviour {

	public string subString = "this is a sub string";
	public int valueX = 43;
	public byte valueB = 2;
	
	public void WriteObjectState(BinaryWriter binaryWriter) {
		//write our own state
		binaryWriter.Write(subString);
		binaryWriter.Write(valueX);
		binaryWriter.Write(valueB);

		binaryWriter.Write(this.gameObject.name);
	}

	public void ReadObjectState(BinaryReader binaryReader) {

		this.subString = binaryReader.ReadString();
		this.valueX = binaryReader.ReadInt32();
		this.valueB = binaryReader.ReadByte();
		
		this.gameObject.name = binaryReader.ReadString();
		
	}
	
	public void WriteObjectState_Web(string prependKey) {
		PlayerPrefs.SetString(prependKey+"subString",subString);
		PlayerPrefs.SetInt(prependKey+"valueX",valueX);
		PlayerPrefs.SetInt(prependKey+"valueB",valueB);
		PlayerPrefs.SetString(prependKey+"objectName", this.gameObject.name);
	}

	public void ReadObjectState_Web(string prependKey) {
		subString = PlayerPrefs.GetString(prependKey+"subString");
		valueX = PlayerPrefs.GetInt(prependKey+"valueX");
		valueB = (byte)PlayerPrefs.GetInt(prependKey+"valueB");
		this.gameObject.name = PlayerPrefs.GetString(prependKey+"objectName");
	}

}
