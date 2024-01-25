using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PiRadHex.CustomGizmos;
using PiRadHex.Shuffle;
using System;
using ProceduralToolkit;


public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private List<RoomEntity> eternalRooms = new List<RoomEntity>();
    [SerializeField] private List<GameObject> roomPrefabsList = new List<GameObject>();
    [SerializeField] private Transform startPosition;
    [SerializeField] private int minRooms;
    [SerializeField] private int maxRooms;
    [SerializeField] private int gapDistance;

    private List<GameObject> prefabInstances = new List<GameObject>();
    private List<RoomEntity> roomEntities = new List<RoomEntity>();
    private int numOfRandomRooms;

    [SerializeField] private GameObject EternalPortals;

    public List<Path> paths = new List<Path>();

    private void Start()
    {
        EternalPortals.transform.position = new Vector3(8888, 8888, 8888);
        GenerateProceduralPortalityMap();
        if (Application.platform == RuntimePlatform.Android) Application.targetFrameRate = 60;
    }

    public void GenerateProceduralPortalityMap()
    {
        DestroyInstances();
        SetAllUnused();
        SetNumOfRandomRooms();
        AddEternalRooms();
        PlaceRooms();
        GenerateTree();
        PlacePortals();
        SetRoomsNumber();
    }

    private void PlaceRooms()
    {
        for (int i = 0; i < numOfRandomRooms; i++)
        {
            GameObject prefab = roomPrefabsList[UnityEngine.Random.Range(0, roomPrefabsList.Count)];
            GameObject prefabInstance = Instantiate(prefab, transform);
            prefabInstance.transform.position = startPosition.position + Vector3.left * i * gapDistance;

            prefabInstances.Add(prefabInstance);

            RoomEntity roomEntity = prefabInstance.GetComponent<RoomEntity>();
            if (roomEntity != null)
            {
                roomEntities.Add(roomEntity);
            }
        }
    }

    private void DestroyInstances()
    {
        foreach (var room in roomEntities)
        {
            room.ResetPortalCandidates();
        }

        foreach (var prefab in prefabInstances)
        {
            Destroy(prefab);
        }

        roomEntities.Clear();
        prefabInstances.Clear();
    }

    private void SetRoomsNumber()
    {
        int num = 0;
        foreach(var room in roomEntities)
        {
            room.SetRoomNumber(num);
            num++;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        var sceneCamera = SceneView.currentDrawingSceneView == null ? Camera.main : SceneView.currentDrawingSceneView.camera;
        if (Vector3.Distance(sceneCamera.transform.position, transform.position) > 100)
        {
            return;
        }

        if (startPosition != null)
        {
            Gizmos.color = Color.blue;
            CustomGizmos.DrawTriangle(startPosition.position, -startPosition.right);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(startPosition.position, 0.1f);
        }
    }
#endif

    public void SetMinMaxRooms(float min, float max)
    {
        minRooms = (int)min;
        maxRooms = (int)max;
    }

    public int GetNumOfEternalRooms()
    {
        return eternalRooms.Count;
    }

    public int GetNumOfRandomRooms()
    {
        return numOfRandomRooms;
    }

    public int GetUsedPortalPairsCount()
    {
        int count = 0;
        for (int i = 0; i < EternalPortals.transform.childCount; i++)
        {
            if (EternalPortals.transform.GetChild(i).gameObject.activeInHierarchy)
            {
                count++;
            }
            else
            {
                return count;
            }
        }
        return count;
    }

    private void SetNumOfRandomRooms()
    {
        numOfRandomRooms = UnityEngine.Random.Range(minRooms, maxRooms);
    }

    private void AddEternalRooms()
    {
        foreach (var e in eternalRooms)
        {
            roomEntities.Add(e);
        }
    }

    private void PlacePortals()
    {
        int pairIndex = 0;
        int totalPairs = EternalPortals.transform.childCount;
        foreach (var path in paths) 
        {
            int[] sequence = path.GetSequence();
            for (int j = 0; j < sequence.Length - 1; j++)
            {
                int first = sequence[j];
                int second = sequence[j+1];

                Transform portal1 = EternalPortals.transform.GetChild(pairIndex).transform.GetChild(0);
                Transform portal2 = EternalPortals.transform.GetChild(pairIndex).transform.GetChild(1);

                if (roomEntities[first].GetAvailableCandidateCount() > 0 && roomEntities[second].GetAvailableCandidateCount() > 0)
                {
                    Transform t1 = roomEntities[first].GetRandomCandidate();
                    Transform t2 = roomEntities[second].GetRandomCandidate();

                    portal1.position = t1.position;
                    portal1.rotation = t1.rotation;

                    portal2.position = t2.position;
                    portal2.rotation = t2.rotation;

                    portal2.Rotate(0, 180, 0);

                    EternalPortals.transform.GetChild(pairIndex).gameObject.SetActive(true);

                    pairIndex++;
                }
                else Debug.LogWarning("Portal coordinate overflow.");

                if (pairIndex + 1 == totalPairs)
                {
                    Debug.LogWarning("Not enouph portal instances.");
                    return;
                }

            }
        }
    }

    private void SetAllUnused()
    {
        for (int i = 0; i < EternalPortals.transform.childCount; i++)
        {
            EternalPortals.transform.GetChild(i).transform.GetChild(0).localPosition = Vector3.zero;
            EternalPortals.transform.GetChild(i).transform.GetChild(0).localRotation = Quaternion.identity;

            EternalPortals.transform.GetChild(i).transform.GetChild(1).localPosition = Vector3.zero;
            EternalPortals.transform.GetChild(i).transform.GetChild(1).localRotation = Quaternion.identity;

            EternalPortals.transform.GetChild(i).gameObject.SetActive(false);
        }
    }


    private void GenerateTree()
    {
        paths.Clear();
        int[] numOfUsedCandidates = new int[roomEntities.Count];
        Array.Clear(numOfUsedCandidates, 0, numOfUsedCandidates.Length);

        List<int> nodes = new List<int>();
        for (int i = 0; i < roomEntities.Count; i++)
        {
            nodes.Add(i);
        }

        while (nodes.Count > 0)
        {

            int startNode = nodes[UnityEngine.Random.Range(0, nodes.Count)];
            nodes.Remove(startNode);
            numOfUsedCandidates[startNode]++;

            List<int> pathSequence = new List<int> { startNode };
            while (pathSequence.Count < nodes.Count)
            {
                int nextNode = nodes[UnityEngine.Random.Range(0, nodes.Count)];
                if (!pathSequence.Contains(nextNode))
                {
                    pathSequence.Add(nextNode);
                    numOfUsedCandidates[nextNode] += 2;
                }
                else
                {
                    break;
                }
            }

            if (paths.Count > 0)
            {
                int[] firstSeq = paths[0].GetSequence();
                ShuffleList.FisherYates(ref firstSeq);
                for (int i = 0; i < firstSeq.Length; i++)
                {
                    int roomIndex = firstSeq[i];
                    if (roomEntities[roomIndex].GetAvailableCandidateCount() - numOfUsedCandidates[roomIndex] > 0)
                    {
                        pathSequence.Add(roomIndex);
                        numOfUsedCandidates[roomIndex]++;
                        break;
                    }
                }

            }

            Path path = new Path(pathSequence.ToArray());
            paths.Add(path);

            foreach (int node in path.GetSequence())
            {
                nodes.Remove(node);
            }


        }
    }

}

[System.Serializable]
public class Path
{
    [SerializeField]
    private int[] sequence;

    public Path(int[] pathSequence)
    {
        sequence = pathSequence;
    }

    private void ShuffleArray()
    {
        ShuffleList.FisherYates<int>(ref sequence);
    }

    public int[] GetSequence()
    {
        return sequence;
    }

}
