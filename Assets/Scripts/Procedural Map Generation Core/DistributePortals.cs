using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistributePortals : MonoBehaviour
{
    [SerializeField] private GameObject EternalPortals;
    [SerializeField] GameObject doorPrefab;

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
    private List<GameObject> doorInstances = new List<GameObject>();

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

        int baseIndex = GetFirstUnusedPortalPairIndex();

        for (int i = 0; i < pair1.Count; i++)
        {
            portalPairs[i + baseIndex].portal1.position = pair1[i].position;
            portalPairs[i + baseIndex].portal1.rotation = pair1[i].rotation;
            InstantiateDoor(pair1[i], portalPairs[i].portal1.transform);

            portalPairs[i + baseIndex].portal2.position = pair2[i].position;
            portalPairs[i + baseIndex].portal2.rotation = pair2[i].rotation;
            InstantiateDoor(pair2[i], portalPairs[i].portal2.transform);

            portalPairs[i + baseIndex].portal2.Rotate(0, 180, 0);

            portalPairs[i + baseIndex].isUsed = true;

            //portalPairs[i].portal1.parent.gameObject.SetActive(true);

        }
    }

    private void InstantiateDoor(Transform _transform, Transform _parent)
    {
        GameObject prefabInstance = Instantiate(doorPrefab, transform);
        prefabInstance.transform.position = _transform.position + new Vector3(0, 1.5f, 0f);
        prefabInstance.transform.rotation = _transform.rotation;

        prefabInstance.transform.SetParent(_parent);
        prefabInstance.transform.localPosition += Vector3.forward * 0.5f;
        prefabInstance.transform.SetParent(transform);

        doorInstances.Add(prefabInstance);
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

            //portalPairs[i].portal1.parent.gameObject.SetActive(false);
        }
    }

    public void DestroyDoors()
    {
        foreach (var door in doorInstances)
        {
            Destroy(door);
        }

        doorInstances.Clear();
    }

    public int GetFirstUnusedPortalPairIndex()
    {
        for (int i = 0; i < portalPairs.Count; i++)
        {
            if (portalPairs[i].isUsed) continue;
            return i;
        }
        return 0;
    }

}