using System;
using UnityEngine;
using System.Collections.Generic;

public class WorldMeshGenerator
{
	public static void FillMesh(ref Mesh meshToUpdate, int chunkX, int chunkY, int chunkZ, DynamicWorld world, int size, int height, float surfaceCrossValue) {
		
		int vertexIndex = 0;
		Vector3[] interpolatedValues = new Vector3[12];
		
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangleIndices = new List<int>();
		
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < height-1; y++) {
				for (int z = 0; z < size; z++)
				{
					if(vertices.Count > 64000) {
						//Maximum vertex count for a mesh is 65k
						//If reaching this limit we should be making smaller or less complex meshes
						break;
					}
					
					Vector3 basePoint = new Vector3(x,y,z);
					
					//Get the 8 corners of this cube
					float p0 = world.GetValue(chunkX + x,  chunkY + y,  chunkZ + z);
					float p1 = world.GetValue(chunkX + x+1,chunkY + y,  chunkZ + z);
					float p2 = world.GetValue(chunkX + x,  chunkY + y+1,chunkZ + z);
					float p3 = world.GetValue(chunkX + x+1,chunkY + y+1,chunkZ + z);
					float p4 = world.GetValue(chunkX + x,  chunkY + y,  chunkZ + z+1);
					float p5 = world.GetValue(chunkX + x+1,chunkY + y,  chunkZ + z+1);
					float p6 = world.GetValue(chunkX + x,  chunkY + y+1,chunkZ + z+1);
					float p7 = world.GetValue(chunkX + x+1,chunkY + y+1,chunkZ + z+1);
					
					//A bitmap indicating which edges the surface of the volume crosses
					int crossBitMap = 0;
					
					
					if ( p0 < surfaceCrossValue ) crossBitMap |= 1;
					if ( p1 < surfaceCrossValue ) crossBitMap |= 2;
					
					if ( p2 < surfaceCrossValue ) crossBitMap |= 8;
					if ( p3 < surfaceCrossValue ) crossBitMap |= 4;
					
					if ( p4 < surfaceCrossValue ) crossBitMap |= 16;
					if ( p5 < surfaceCrossValue ) crossBitMap |= 32;
					
					if ( p6 < surfaceCrossValue ) crossBitMap |= 128;
					if ( p7 < surfaceCrossValue ) crossBitMap |= 64;
					
					//Use the edge look up table to determin the configuration of edges
					int edgeBits = Contouring3D.EdgeTableLookup[crossBitMap];
					
					//The surface did not cross any edges, this cube is either complelety inside, or completely outside the volume
					if(edgeBits == 0)
						continue;
					
					float interpolatedCrossingPoint = 0f;
					
					//Calculate the interpolated positions for each edge that has a crossing value
					
					//Bottom four edges
					if ((edgeBits & 1) > 0 )
					{		
						interpolatedCrossingPoint = ( surfaceCrossValue - p0 ) / ( p1 - p0 );
						interpolatedValues[0] = Vector3.Lerp(new Vector3(x,y,z), new Vector3(x+1,y,z), interpolatedCrossingPoint);
					}
					if ((edgeBits & 2) > 0)
					{
						interpolatedCrossingPoint = ( surfaceCrossValue - p1 ) / ( p3 - p1 );
						interpolatedValues[1] = Vector3.Lerp(new Vector3(x+1,y,z), new Vector3(x+1,y+1,z), interpolatedCrossingPoint);
					}
					if ((edgeBits & 4) > 0)
					{
						interpolatedCrossingPoint = ( surfaceCrossValue - p2 ) / ( p3 - p2 );
						interpolatedValues[2] = Vector3.Lerp(new Vector3(x,y+1,z), new Vector3(x+1,y+1,z), interpolatedCrossingPoint);
					}
					if ((edgeBits & 8) > 0)
					{
						interpolatedCrossingPoint = ( surfaceCrossValue - p0 ) / ( p2 - p0 );
						interpolatedValues[3] = Vector3.Lerp(new Vector3(x,y,z), new Vector3(x,y+1,z), interpolatedCrossingPoint);
					}
					
					//Top four edges
					if ((edgeBits & 16) > 0)
					{
						interpolatedCrossingPoint = ( surfaceCrossValue - p4 ) / ( p5 - p4 );
						interpolatedValues[4] = Vector3.Lerp(new Vector3(x,y,z+1), new Vector3(x+1,y,z+1), interpolatedCrossingPoint);
					}
					if ((edgeBits & 32) > 0)
					{
						interpolatedCrossingPoint = ( surfaceCrossValue - p5 ) / ( p7 - p5 );
						interpolatedValues[5] = Vector3.Lerp(new Vector3(x+1,y,z+1), new Vector3(x+1,y+1,z+1), interpolatedCrossingPoint);
					}
					if ((edgeBits & 64) > 0)
					{
						interpolatedCrossingPoint = ( surfaceCrossValue - p6 ) / ( p7 - p6 );
						interpolatedValues[6] = Vector3.Lerp(new Vector3(x,y+1,z+1), new Vector3(x+1,y+1,z+1), interpolatedCrossingPoint);
					}
					if ((edgeBits & 128) > 0)
					{
						interpolatedCrossingPoint = ( surfaceCrossValue - p4 ) / ( p6 - p4 );
						interpolatedValues[7] = Vector3.Lerp(new Vector3(x,y,z+1), new Vector3(x,y+1,z+1), interpolatedCrossingPoint);
					}
					
					//Side four edges
					if ((edgeBits & 256) > 0)
					{
						interpolatedCrossingPoint = ( surfaceCrossValue - p0 ) / ( p4 - p0 );
						interpolatedValues[8] = Vector3.Lerp(new Vector3(x,y,z), new Vector3(x,y,z+1), interpolatedCrossingPoint);
					}
					if ((edgeBits & 512) > 0)
					{
						interpolatedCrossingPoint = ( surfaceCrossValue - p1 ) / ( p5 - p1 );
						interpolatedValues[9] = Vector3.Lerp(new Vector3(x+1,y,z), new Vector3(x+1,y,z+1), interpolatedCrossingPoint);
					}
					if ((edgeBits & 1024) > 0)
					{
						interpolatedCrossingPoint = ( surfaceCrossValue - p3 ) / ( p7 - p3 );
						interpolatedValues[10] = Vector3.Lerp(new Vector3(x+1,y+1,z), new Vector3(x+1,y+1,z+1), interpolatedCrossingPoint);
					}
					if ((edgeBits & 2048) > 0)
					{
						interpolatedCrossingPoint = ( surfaceCrossValue - p2 ) / ( p6 - p2 );
						interpolatedValues[11] = Vector3.Lerp(new Vector3(x,y+1,z), new Vector3(x,y+1,z+1), interpolatedCrossingPoint);
					}
					
					//Shift the cross bit map to use as an index into the triangle look up table
					crossBitMap <<= 4;
					
					int triangleIndex = 0;
					while ( Contouring3D.TriangleLookupTable[ crossBitMap + triangleIndex ] != -1 ) 
					{
						//For each triangle in the look up table, create a triangle and add it to the list.
						int index1 = Contouring3D.TriangleLookupTable[crossBitMap + triangleIndex];
						int index2 = Contouring3D.TriangleLookupTable[crossBitMap + triangleIndex + 1];
						int index3 = Contouring3D.TriangleLookupTable[crossBitMap + triangleIndex + 2];
						
						vertices.Add(new Vector3(interpolatedValues[index1].x, interpolatedValues[index1].y, interpolatedValues[index1].z));
						vertices.Add(new Vector3(interpolatedValues[index2].x, interpolatedValues[index2].y, interpolatedValues[index2].z));
						vertices.Add(new Vector3(interpolatedValues[index3].x, interpolatedValues[index3].y, interpolatedValues[index3].z));
						
						triangleIndices.Add(vertexIndex);
						triangleIndices.Add(vertexIndex+1);
						triangleIndices.Add(vertexIndex+2);
						vertexIndex += 3;
						triangleIndex += 3;
					}
				}
			}
		}
		
		//Create texture coordinates for all the vertices
		List<Vector2> texCoords = new List<Vector2>();
		Vector2 emptyTexCoords0 = new Vector2(0,0);
		Vector2 emptyTexCoords1 = new Vector2(0,1);
		Vector2 emptyTexCoords2 = new Vector2(1,1);
		
		for(int texturePointer = 0; texturePointer < vertices.Count; texturePointer+=3) {
			//There should be as many texture coordinates as vertices.
			//This example does not support textures, so fill with zeros
			texCoords.Add(emptyTexCoords1);
			texCoords.Add(emptyTexCoords2);
			texCoords.Add(emptyTexCoords0);
		}
		
		//Generate the mesh using the vertices and triangle indices we just created
		meshToUpdate.Clear();
		meshToUpdate.vertices = vertices.ToArray();
		meshToUpdate.triangles = triangleIndices.ToArray();
		meshToUpdate.uv = texCoords.ToArray();
		meshToUpdate.RecalculateNormals();
		meshToUpdate.RecalculateBounds();
	}
}

