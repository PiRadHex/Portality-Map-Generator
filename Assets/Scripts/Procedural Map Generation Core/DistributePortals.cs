using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistributePortals : MonoBehaviour
{
    public GameObject EternalPortals;
    private List<GameObject> portalPairs = new List<GameObject>();

    void Awake()
    {
        for (int i = 0; i < EternalPortals.transform.childCount; i++)
        {
            portalPairs.Add(EternalPortals.transform.GetChild(i).gameObject);
        }
    }

    public void PlaceLinkedPortals(List<Transform> pair1, List<Transform> pair2)
    {
        Debug.Log("pair1.Count= " + pair1.Count + ", pair2.Count= " + pair2.Count);
        for (int i = 0; i < pair1.Count; i++)
        {
            //Debug.Log("i= " + i);
            if (portalPairs[i].name == "used") continue;

            portalPairs[i].transform.GetChild(0).position = pair1[i].position;
            portalPairs[i].transform.GetChild(0).rotation = pair1[i].rotation;

            portalPairs[i].transform.GetChild(1).position = pair2[i].position;
            portalPairs[i].transform.GetChild(1).rotation = pair2[i].rotation;

            portalPairs[i].transform.GetChild(1).Rotate(0, 180, 0);

            portalPairs[i].name = "used";
        }
    }

    public void SetAllUnused()
    {
        for (int i = 0; i < portalPairs.Count; i++)
        {
            portalPairs[i].name = "unused";
        }
    }

}