using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PiRadHex.CustomGizmos;
using PiRadHex.Shuffle;
using System;


public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> roomPrefabsList = new List<GameObject>();
    [SerializeField] private Transform startPosition;
    [SerializeField] private int minRooms;
    [SerializeField] private int maxRooms;
    [SerializeField][Range(0f, 1f)] private float pairRatio = 0f;
    [SerializeField] private int gapDistance;

    private List<GameObject> prefabInstances = new List<GameObject>();
    private List<RoomEntity> roomEntities = new List<RoomEntity>();
    private int numOfRooms;
    private int numOfPairPortals;

    [SerializeField] private GameObject EternalPortals;

    public List<Path> paths = new List<Path>();
    //private HashSet<string> connections = new HashSet<string>();
    //public bool allowLoops = false;


    private void Start()
    {
        EternalPortals.transform.position = new Vector3(8888, 8888, 8888);
        GenerateProceduralPortalityMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SeedGenerator.Instance.GenerateNewSeed();
            GenerateProceduralPortalityMap();

        }
    }

    public void GenerateProceduralPortalityMap()
    {
        DestroyInstances();
        SetAllUnused();
        SetNumOfRooms();
        GenerateTree();
        PlaceRooms();
        SetNumOfPairs();
        MakePairs();

    }

    void PlaceRooms()
    {
        for (int i = 0; i < numOfRooms; i++)
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

    void DestroyInstances()
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

    private void OnDrawGizmosSelected()
    {/*
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
        }*/
    }

    public void SetNumOfRooms()
    {
        numOfRooms = UnityEngine.Random.Range(minRooms, maxRooms);
    }

    public void SetNumOfPairs()
    {
        numOfPairPortals = UnityEngine.Random.Range(roomEntities.Count * Mathf.FloorToInt(pairRatio + 1) - 1,
                                        Mathf.Min(roomEntities.Count * (Mathf.FloorToInt(MinTotalCandidate() / 2 * pairRatio) + 1),
                                                  roomEntities.Count * Mathf.FloorToInt(MinTotalCandidate() / 2)));
    }

    public void MakePairs()
    {
        int i = 0;
        foreach (var path in paths) 
        {
            int[] sequence = path.GetSequence();
            for (int j = 0; j < sequence.Length - 1; j++)
            {
                int first = sequence[j];
                int second = sequence[j+1];

                Transform portal1 = EternalPortals.transform.GetChild(i).transform.GetChild(0);
                Transform portal2 = EternalPortals.transform.GetChild(i).transform.GetChild(1);

                Transform t1 = roomEntities[first].GetRandomCandidate();
                Transform t2 = roomEntities[second].GetRandomCandidate();

                portal1.position = t1.position;
                portal1.rotation = t1.rotation;

                portal2.position = t2.position;
                portal2.rotation = t2.rotation;

                portal2.Rotate(0, 180, 0);

                i++;

            }
        }
    }

    private int FindFirstEmptyRoomIndex(int index)
    {
        for (int i = index; i < roomEntities.Count; i++)
        {
            if (roomEntities[i].GetAvailableCandidateCount() == roomEntities[i].GetTotalCandidateCount())
            {
                return i;
            }
        }
        return -1;
    }

    private int FindFirstNotFilledRoomIndexExcept(int index)
    {
        for (int i = 0; i < roomEntities.Count; i++)
        {
            if (i == index) continue;
            if (roomEntities[i].GetAvailableCandidateCount() > 0)
            {
                return i;
            }
        }
        return -1;
    }

    private int MinTotalCandidate()
    {
        int min = Mathf.RoundToInt(Mathf.Infinity);
        foreach (var room in roomEntities)
        {
            int total = room.GetTotalCandidateCount();
            if (min < total)
            {
                min = total;
            }
        }
        return min;
    }

    public void SetAllUnused()
    {
        for (int i = 0; i < EternalPortals.transform.childCount; i++)
        {
            EternalPortals.transform.GetChild(i).transform.GetChild(0).localPosition = Vector3.zero;
            EternalPortals.transform.GetChild(i).transform.GetChild(0).localRotation = Quaternion.identity;

            EternalPortals.transform.GetChild(i).transform.GetChild(1).localPosition = Vector3.zero;
            EternalPortals.transform.GetChild(i).transform.GetChild(1).localRotation = Quaternion.identity;

        }
    }


    void GenerateTree()
    {
        paths.Clear();
        List<int> nodes = new List<int>();
        for (int i = 0; i < numOfRooms; i++)
        {
            nodes.Add(i);
        }

        while (nodes.Count > 0)
        {

            int startNode = nodes[UnityEngine.Random.Range(0, nodes.Count)];
            nodes.Remove(startNode);

            List<int> pathSequence = new List<int> { startNode };
            while (pathSequence.Count < nodes.Count)
            {
                int nextNode = nodes[UnityEngine.Random.Range(0, nodes.Count)];
                if (!pathSequence.Contains(nextNode))
                {
                    pathSequence.Add(nextNode);
                }
                else
                {
                    break;
                }
            }

            if (paths.Count > 0)
            {
                pathSequence.Add (paths[0].GetSequence()[UnityEngine.Random.Range(0, paths[0].GetSequence().Length)]);
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
