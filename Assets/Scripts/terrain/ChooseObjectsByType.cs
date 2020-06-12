using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using System.Linq;

public class ChooseObjectsByType : MonoBehaviour
{
    public TrackableBehaviour theTrackable;
    private int ClickCounter;

    List<TerrainType> displayList = new List<TerrainType>();
    public List<TerrainType> terrainObjects = new List<TerrainType>();

    // Start is called before the first frame update
    void Start()
    {
        ClickCounter = 0;
        terrainObjects[ClickCounter].gameObject.SetActive(true);

        if (theTrackable == null)
        {
            Debug.Log("Warning: Trackable not set !!");
        }
    }

    public List<TerrainType> Filter (TerrainType.Terrain type)
    {

        Debug.Log("List size " + terrainObjects.Count);

        displayList = (from typeOfObject in terrainObjects
                       where typeOfObject.terrainType == type
                       select typeOfObject).ToList();
        return displayList;

    }

    public void Forest()
    {
        terrainObjects[ClickCounter].gameObject.SetActive(false);
        Filter(TerrainType.Terrain.Forest);

        TerrainType[] filteredArray = displayList.ToArray();
        Debug.Log(filteredArray.Length);

        filteredArray[ClickCounter].gameObject.SetActive(false);

        if (ClickCounter + 2 > filteredArray.Length)
        {
            ClickCounter = 0;
        }
        else
        {
            ClickCounter++;
        }

        filteredArray[ClickCounter].gameObject.SetActive(true);
    }

    public void Mountain()
    {
        terrainObjects[ClickCounter].gameObject.SetActive(false);
        Filter(TerrainType.Terrain.Mountain);

        TerrainType[] filteredArray = displayList.ToArray();
        Debug.Log(filteredArray.Length);

        filteredArray[ClickCounter].gameObject.SetActive(false);

        if (ClickCounter + 2 > filteredArray.Length)
        {
            ClickCounter = 0;
        }
        else
        {
            ClickCounter++;
        }

        filteredArray[ClickCounter].gameObject.SetActive(true);
    }

    public void Building()
    {
        terrainObjects[ClickCounter].gameObject.SetActive(false);
        Filter(TerrainType.Terrain.Building);

        TerrainType[] filteredArray = displayList.ToArray();
        Debug.Log(filteredArray.Length);

        filteredArray[ClickCounter].gameObject.SetActive(false);

        if (ClickCounter + 2 > filteredArray.Length)
        {
            ClickCounter = 0;
        }
        else
        {
            ClickCounter++;
        }

        filteredArray[ClickCounter].gameObject.SetActive(true);
    }

}
