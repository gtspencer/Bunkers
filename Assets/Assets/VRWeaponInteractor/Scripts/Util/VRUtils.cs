using UnityEngine;
using System.Collections;

public class VRUtils : MonoBehaviour 
{
	public static Vector3 ClosestPointOnLine(Vector3 startPoint, Vector3 endPoint, Vector3 tPoint)
	{
		Vector3 startPointTotPointVector = tPoint - startPoint;
		Vector3 startPointToEndPointVector = (endPoint - startPoint).normalized;

		float d = Vector3.Distance(startPoint, endPoint);
		float t = Vector3.Dot(startPointToEndPointVector, startPointTotPointVector);

		if (t <= 0) 
			return startPoint;

		if (t >= d) 
			return endPoint;

		Vector3 distanceAlongVector = startPointToEndPointVector * t;

		Vector3 closestPoint = startPoint + distanceAlongVector;

		return closestPoint;
	}

	public static T CopyComponent<T>(T original, GameObject destination) where T : Component
	{
		System.Type type = original.GetType();
		Component copy = destination.AddComponent(type);
		System.Reflection.FieldInfo[] fields = type.GetFields();
		foreach (System.Reflection.FieldInfo field in fields)
		{
			field.SetValue(copy, field.GetValue(original));
		}
		return copy as T;
	}

	public static Vector3 GetConeDirection(Vector3 originalDirection, float coneSize)
	{
		// random tilt around the up axis
		Quaternion randomTilt = Quaternion.AngleAxis(Random.Range(0f, coneSize), Vector3.up);
		// random spin around the forward axis
		Quaternion randomSpin = Quaternion.AngleAxis(Random.Range(0f, 360f), originalDirection);
		// tilt then spin
		Quaternion tiltSpin = (randomSpin * randomTilt);
		// fire in direction with offset
		Vector3 coneDirection = (tiltSpin * originalDirection).normalized;
		return coneDirection;
	}
	
	public static Vector2 GetPlayAreaSize()
	{
		Valve.VR.HmdQuad_t sizeQuad = new Valve.VR.HmdQuad_t();
		SteamVR_PlayArea.GetBounds(SteamVR_PlayArea.Size.Calibrated, ref sizeQuad);
		Vector3 corner1 = new Vector3(sizeQuad.vCorners0.v0, sizeQuad.vCorners0.v1, sizeQuad.vCorners0.v2);
		Vector3 corner2 = new Vector3(sizeQuad.vCorners1.v0, sizeQuad.vCorners1.v1, sizeQuad.vCorners1.v2);
		Vector3 corner3 = new Vector3(sizeQuad.vCorners2.v0, sizeQuad.vCorners2.v1, sizeQuad.vCorners2.v2);
		float distWidth = Vector3.Distance(corner1, corner2);
		float distHeight = Vector3.Distance(corner2, corner3);
		return new Vector2(distWidth, distHeight);
	}
}
