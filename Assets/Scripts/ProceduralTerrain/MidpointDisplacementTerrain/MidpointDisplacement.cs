using UnityEngine;
using System.Collections;

public class MidpointDisplacement {
	public float[,] data;
	int size;
	float rough = 1f;
	float roughFactor;
	float maxRange;
	float minRange;
	float baseValue = 1;

	public MidpointDisplacement(int size, float roughFactor, float minRange, float maxRange, float baseValue) {
		data = new float[size,size];
		this.size = size;
		this.roughFactor = roughFactor;
		this.baseValue = baseValue;
		this.minRange = minRange;
		this.maxRange = maxRange;
	}

	public void Generate(int iterations) {
		for (int i = 0; i < iterations; i++) {
			MidPointSmooth();
		}
	}

	public void Generate(params float[] roughValues) {
		for(int i = 0; i < roughValues.Length; i++) {
			rough = roughValues[i];
			MidPointSmooth();
		}
	}

	public void SetBaseValue() {
		for (int i = 0; i < size; i++) {
			for (int k = 0; k < size; k++) {
				data[i,k] = baseValue;
			}
		}
	}

	public void SetRandomValue() {
		for (int i = 0; i < size; i++) {
			for (int k = 0; k < size; k++) {
				data[i,k] = Mathf.Min(maxRange, Mathf.Max(minRange, Random.value*baseValue));
			}
		}
	}

	private void MidPointDisplace(int Xstep, int Ystep, float roughness) {
		for (int i = Xstep - 1; i < size; i += Xstep) {
			for (int j = Ystep - 1; j < size; j += Ystep) {
				float xmym = GetNeighbor(i, j, -Xstep, -Ystep);
				float xmyp = GetNeighbor(i, j, -Xstep, Ystep);
				float xpym = GetNeighbor(i, j, Xstep, -Ystep);
				float xpyp = GetNeighbor(i, j, Xstep, Ystep);
				data[i,j] = (xmym + xmyp + xpym + xpyp) / 4;
				float rv = (Random.value - Random.value) * roughness;
				data[i,j] = Mathf.Min(maxRange, Mathf.Max(minRange, data[i,j] + rv));
			}
		}
		for (int i = Xstep - 1; i < size; i += Xstep) {
			for (int j = Ystep - 1; j < size; j += Ystep) {
				SquareStep((i + size - Xstep) % size, j, Xstep, Ystep, roughness);
				SquareStep(i, (j + Ystep) % size, Xstep, Ystep, roughness);
				SquareStep(i, (j + size - Ystep) % size, Xstep, Ystep, roughness);
				SquareStep((i + Xstep) % size, j, Xstep, Ystep, roughness);
			}
		}
	}
	
	private void SquareStep(int i, int j, int Xstep, int Ystep, float roughness) {
		float xm = GetNeighbor(i, j, -Xstep, 0);
		float yp = GetNeighbor(i, j, 0, Ystep);
		float ym = GetNeighbor(i, j, 0, -Ystep);
		float xp = GetNeighbor(i, j, Xstep, 0);
		data[i,j] = (xm + yp + ym + xp) / 4;
		float randomValue = (Random.value - Random.value) * roughness;
		data[i,j] = Mathf.Min(maxRange, Mathf.Max(minRange, data[i,j] + randomValue));
	}
	
	private void MidPointSmooth() {
		for (int i = size; i >= 1; i /= 2) {
			MidPointDisplace(i, i, rough);
			rough *= Mathf.Pow(2, -roughFactor);
		}
	}

	float GetNeighbor(int x, int y, int xOffset, int yOffset) {
		if (xOffset > 0) {
			if (yOffset > 0) {
				return data[(x + xOffset) % size,(y + yOffset) % size];
			} else if (yOffset < 0) {
				return data[(x + xOffset) % size,(y + size + yOffset) % size];
			} else { //zOffset == 0
				return data[(x + xOffset) % size, y];
			}
		} else if (xOffset < 0) {
			if (yOffset > 0) {
				return data[(x + size + xOffset) % size,(y + yOffset) % size];
			} else if (yOffset < 0) {
				return data[(x + size + xOffset) % size,(y + size + yOffset) % size];
			} else {
				return data[(x + size + xOffset) % size,y];
			}
		} else {
			if (yOffset > 0) {
				return data[x,(y + yOffset) % size];
			} else if (yOffset < 0) {
				return data[x,(y + size + yOffset) % size];
			} else {
				return data[x,y];
			}
		}
	}
}
