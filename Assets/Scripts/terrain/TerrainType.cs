using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainType : MonoBehaviour
{
    public enum Terrain {Mountain, Forest, Building}

    //public Terrain terrainType { get; set; }

    public Terrain terrainType;

    public void SetTerrainType(Terrain value)
    {
        terrainType = value;
    }

    public Terrain GetTerrainType()
    {
        return terrainType;
    }

    internal void SetActive(bool v)
    {
        throw new NotImplementedException();
    }
}
