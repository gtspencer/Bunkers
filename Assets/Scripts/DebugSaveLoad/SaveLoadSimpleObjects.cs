using UnityEngine;
using System.Collections;
using System.IO;

public class SaveLoadSimpleObjects : MonoBehaviour {
	public string saveFile = @"SaveFile.save";

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.L)) {
			ReadSimpleObjects();
		}

		if(Input.GetKeyDown(KeyCode.S)) {
			WriteSimpleObjects();
		}
	}

	void ReadSimpleObjects() {
		if(File.Exists(saveFile)) {
			using(FileStream fs = File.OpenRead(saveFile)) {
				BinaryReader fileReader = new BinaryReader(fs);
				int simpleObjectCount = fileReader.ReadInt32();
				for(int simpleCount = 0; simpleCount < simpleObjectCount; simpleCount++) {
					GameObject simpleObject = new GameObject();
					SimpleObject simpleScript = simpleObject.AddComponent<SimpleObject>();
					simpleScript.ReadObjectState(fileReader);
				}
			}
		}
	}

	void WriteSimpleObjects() {
		//using statement will dispose of the object inside when we're done using it.
		//This is important for objects like files, that we don't want to leave open.
		using(FileStream fs = File.OpenWrite(saveFile)) {
			SimpleObject[] simpleObjects = UnityEngine.Object.FindObjectsOfType<SimpleObject>();
			BinaryWriter fileWriter = new BinaryWriter(fs);
			fileWriter.Write(simpleObjects.Length);
			foreach (SimpleObject simpleObject in simpleObjects) {
				simpleObject.WriteObjectState(fileWriter);
			}
		}
	}
}
