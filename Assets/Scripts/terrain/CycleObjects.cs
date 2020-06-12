using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleObjects : MonoBehaviour
{
    public GameObject[] objects;
    private int ClickCounter;

    // Start is called before the first frame update
    void Start()
    {
        ClickCounter = Random.Range(0, objects.Length);

        objects[ClickCounter].SetActive(true);
    }

    public void NextObject ()
    { 
            objects[ClickCounter].SetActive(false);

            if (ClickCounter + 2 > objects.Length)
            {
                ClickCounter = 0;
            }
            else
            {
                ClickCounter++;
            }

            objects[ClickCounter].SetActive(true);

    }

}
