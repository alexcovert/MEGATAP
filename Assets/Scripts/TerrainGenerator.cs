// Peter Tieu
﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {
	[Range(1.0f, 50.0f)]
	public int heightScale = 1;
	public int edgeSize = 256;
	[Range(1,6)]
	public int octaves = 1;
	[Range(0f, 100f)]
	public float persistence = 0.25f;
	private float[,] heightField;

	[Range(0.0f, 256.0f)]
	public float noiseXstart = 0f;
	[Range(0.0f, 256.0f)]
	public float noiseXspan = 0f;

	[Range(0.0f, 256.0f)]
	public float noiseYstart = 0f;
	[Range(0.0f, 256.0f)]
	public float noiseYspan = 0f;

	[Range(0, 100)]
	public int numTrees = 12;
	public float treeColorAdjustment = 0.4f;

	private Terrain terrain;

	public Texture2D goalTexture;

	[Range(0.01f, 1.0f)]
	public float mixFraction = 0.7f;

	// Use this for initialization
	void Start ()
	{
		heightField = new float[edgeSize, edgeSize];
		terrain = GetComponent<Terrain>();
		GenerateTerrainGuidanceTexture(terrain.terrainData, goalTexture, mixFraction);
		AssignSplatMap(terrain.terrainData);
		//PlaceTreeAcrossTerrains(terrain.terrainData, numTrees);

	}

	// Update is called once per frame
	void Update ()
	{

	}

	TerrainData GenerateTerrainGuidanceTexture( TerrainData terrainData, Texture2D guideTexture, float mixFraction)
	{
		terrainData.heightmapResolution = edgeSize;
		terrainData.size = new Vector3 ( edgeSize, heightScale, edgeSize );
		GenerateHeightGuidanceTexture(guideTexture, mixFraction);
		terrainData.SetHeights(0, 0, heightField);

		return terrainData;

	}

	void GenerateHeightGuidanceTexture(Texture2D guideTexture, float mixFraction)
	{
		for(int y = 0; y < edgeSize; y++)
		{
			for(int x = 0; x < edgeSize; x++)
			{
				heightField[y, x] = CalculateHeightGuidanceTexture(guideTexture, y, x, mixFraction);
			}
		}
	}

	float CalculateHeightGuidanceTexture(Texture2D guideTex, int y, int x, float mixFraction)
	{
		float xfrac, yfrac;
		float greyScaleVal;
		float noiseVal = 0.0f;
		xfrac = (float)x / (float)edgeSize;
		yfrac = (float)y / (float)edgeSize;
		greyScaleVal = guideTex.GetPixelBilinear(xfrac, yfrac).grayscale;

		noiseVal = CalculateHeightOctaves(y,x);

		return (greyScaleVal * mixFraction) + noiseVal * (1 - mixFraction);

	}

	float CalculateHeightOctaves(int y, int x)
	{
		float noiseVal = 0.0f;
		float frequency = 1.0f;
		float amplitude = 1.0f;
		float maxValue = 0.0f;

		for(int i = 0; i< octaves; i++)
		{
			float perlinX = noiseXstart + ((float)x / (float)edgeSize) * noiseXspan * frequency;
			float perlinY = noiseYstart + ((float)y / (float)edgeSize) * noiseYspan * frequency;

			noiseVal += Mathf.PerlinNoise(perlinX*frequency, perlinY*frequency) * amplitude;

			maxValue += amplitude;
			amplitude *= persistence;
			frequency *= 2;
		}
		return noiseVal/maxValue;
	}

	public void PlaceTreeAcrossTerrains(TerrainData terrainData, int numTrees)
	{
		float treeX = 0;
		float treeZ = 0;

		for(int i = 0; i < numTrees; i++)
		{
			treeX = Random.Range(0f, 1f);
			treeZ = Random.Range(0f, 1f);
			PlaceTree(treeX, treeZ);
		}

	}

	public void PlaceTree(float treeX, float treeZ)
	{
		TreeInstance myTreeInstance = new TreeInstance();
		Vector3 position = new Vector3(treeX, 0, treeZ);
		int numPrototypes = terrain.terrainData.treePrototypes.Length;
		int selectedPrototype = AssignTree(terrain.terrainData, position);

		if( numPrototypes == 0) return; // if no tree presets, doesn't place anything
		if( selectedPrototype == -1) return;

		myTreeInstance.position = position;
		myTreeInstance.color = Color.white * Random.Range(1f, 1f - treeColorAdjustment); //GetTreeColor not working
		myTreeInstance.lightmapColor = Color.white;
		myTreeInstance.prototypeIndex = selectedPrototype;
		myTreeInstance.heightScale = 1.0f;
		myTreeInstance.widthScale = 1.0f;
		myTreeInstance.rotation = Random.Range(0.0f, 6.283185f);
		terrain.AddTreeInstance(myTreeInstance);
	}

	public int AssignTree(TerrainData terrainData, Vector3 position)
	{
		int prototype = -1; // no tree by default, if on a slope
		//Debug.Log(position.x);
		float height = terrainData.GetInterpolatedHeight(position.x, position.z);
		Vector3 normal = terrainData.GetInterpolatedNormal(position.x, position.z);
		float angle = terrainData.GetSteepness(position.x, position.z);
		float frac = angle / 90.0f;




		if(height <= (heightScale/2.0f) && (height != 0)) // at lower elevations, place tree 9
		{
			prototype = 1;
		}

		if((height >= heightScale * 0.6f) && normal.y > 0.8 ) // at higher elevations, place tree 0
		{
			prototype = 0;
		}

		if(frac > 0.3f)
		{
			prototype = -1;
		}


		return prototype;
	}

	void AssignSplatMap(TerrainData terrainData)
	{
		float[,,] splatmapData;
		splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];
		float[] splatWeights = new float[terrainData.alphamapLayers];

		for(int y = 0; y < terrainData.alphamapHeight; y++)
		{
			for (int x = 0; x < terrainData.alphamapWidth; x++)
			{
				float y_alpha = (float)y / (float)terrainData.alphamapHeight;
		    float x_alpha = (float)x / (float)terrainData.alphamapWidth;
				float angle = terrainData.GetSteepness(y_alpha, x_alpha);
				float height = terrainData.GetHeight(Mathf.RoundToInt(y_alpha * terrainData.heightmapHeight), Mathf.RoundToInt(x_alpha * terrainData.heightmapWidth));
				Vector3 normal = terrainData.GetInterpolatedNormal(y_alpha, x_alpha);

				float frac = angle / 90.0f;

				if(height <= (heightScale/2.0f)){ // at lower elevations, more grass texture
					splatWeights[2] = (float)(1 - frac);
				}
				// At steep angles, more texture 2 (dirt)
				splatWeights[1] = frac;

				// At high elevations, more ground influence
				if((height >= heightScale * 0.6f) && normal.y > 0.8 )
				{
					splatWeights[0] = normal.y;
					splatWeights[1] = 0.0f;
					splatWeights[2] = 0.0f;
				}

				// Calculates normalization factor from weights
				float z = splatWeights.Sum();

				for (int i = 0; i < terrainData.alphamapLayers; i++)
				{
							 // Normalize so that sum of all texture weights = 1
							splatWeights[i] /= z;

							// Assign this point to the splatmap array
							splatmapData[x, y, i] = splatWeights[i];
				}
			}
		}
		terrainData.SetAlphamaps(0, 0, splatmapData);
	}
}
