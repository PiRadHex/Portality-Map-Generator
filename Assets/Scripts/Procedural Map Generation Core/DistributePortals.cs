using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistributePortals : MonoBehaviour
{
    [SerializeField] private GameObject EternalPortals;

    private class PortalPair
    {
        public PortalPair(Transform _portal1, Transform _portal2)
        {
            portal1 = _portal1;
            portal2 = _portal2;
            isUsed = false;
        }

        public Transform portal1;
        public Transform portal2;
        public bool isUsed;

        
    }

    private List<PortalPair> portalPairs = new List<PortalPair>();

    void Awake()
    {
        EternalPortals.transform.position = new Vector3 (8888, 8888, 8888);
        for (int i = 0; i < EternalPortals.transform.childCount; i++)
        {
            Transform portal1 = EternalPortals.transform.GetChild(i).transform.GetChild(0);
            Transform portal2 = EternalPortals.transform.GetChild(i).transform.GetChild(1);
            portalPairs.Add(new PortalPair(portal1, portal2));
        }
    }

    public void PlaceLinkedPortals(List<Transform> pair1, List<Transform> pair2)
    {
        Debug.Log("pair1.Count= " + pair1.Count + ", pair2.Count= " + pair2.Count);
        for (int i = 0; i < pair1.Count; i++)
        {
            //Debug.Log("i= " + i);
            if (portalPairs[i].isUsed == true) continue;

            portalPairs[i].portal1.position = pair1[i].position;
            portalPairs[i].portal1.rotation = pair1[i].rotation;

            portalPairs[i].portal2.position = pair2[i].position;
            portalPairs[i].portal2.rotation = pair2[i].rotation;

            portalPairs[i].portal2.Rotate(0, 180, 0);

            portalPairs[i].isUsed = true;

            portalPairs[i].portal1.parent.gameObject.SetActive(true);
        }
    }

    public void SetAllUnused()
    {
        for (int i = 0; i < portalPairs.Count; i++)
        {
            portalPairs[i].portal1.localPosition = Vector3.zero;
            portalPairs[i].portal1.localRotation = Quaternion.identity;

            portalPairs[i].portal2.localPosition = Vector3.zero;
            portalPairs[i].portal2.localRotation = Quaternion.identity;

            portalPairs[i].isUsed = false;

            portalPairs[i].portal1.parent.gameObject.SetActive(false);
        }
    }

}