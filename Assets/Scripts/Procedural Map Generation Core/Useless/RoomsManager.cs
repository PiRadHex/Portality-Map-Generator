using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PiRadHex.CustomGizmos;
using PiRadHex.Shuffle;
using JetBrains.Annotations;

[RequireComponent(typeof(DistributePortals))]
public class RoomsManager : MonoBehaviour
{
    public List<Loop> loops = new List<Loop>();
    private DistributePortals portalPlacer;

    private void Awake()
    {
        portalPlacer = GetComponent<DistributePortals>();
    }

    private void Start()
    {
        GenerateProceduralPortalityMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GenerateProceduralPortalityMap();
        }
    }

    public void GenerateProceduralPortalityMap()
    {
        DestroyInstances();
        portalPlacer.SetAllUnused();
        portalPlacer.DestroyDoors();

        foreach (Loop loop in loops)
        {
            loop.Clear();
            loop.SetNumOfRooms();
            loop.AddEternalRooms();
            PlaceRoomsIn(loop);
            loop.SetNumOfPairs();
            loop.MakePairs();
            if (loop.pair1.Count == 0 || loop.pair2.Count == 0) continue;
            portalPlacer.PlaceLinkedPortals(loop.pair1, loop.pair2);

        }
    }

    void PlaceRoomsIn(Loop loop)
    {
        for (int i = 0; i < loop.numOfRooms; i++)
        {
            GameObject prefab = loop.roomPrefabsList[Random.Range(0, loop.roomPrefabsList.Count)];

            GameObject prefabInstance = Instantiate(prefab, transform);
            prefabInstance.transform.position = loop.startPosition.position + Vector3.left * i * loop.gapDistance;

            loop.prefabInstances.Add(prefabInstance);

            RoomEntity roomEntity = prefabInstance.GetComponent<RoomEntity>();
            if (roomEntity != null)
            {
                loop.roomEntities.Add(roomEntity);
            }
            
        }
    }

    void DestroyInstances()
    {
        foreach (var loop in loops)
        {
            foreach (var room in loop.roomEntities)
            {
                room.ResetPortalCandidates();
            }
            foreach (var prefab in loop.prefabInstances)
            {
                Destroy(prefab);
            }

            loop.roomEntities.Clear();
            loop.prefabInstances.Clear();
        }

    }

    private void OnDrawGizmosSelected()
    {/*
        var sceneCamera = SceneView.currentDrawingSceneView == null ? Camera.main : SceneView.currentDrawingSceneView.camera;
        if (Vector3.Distance(sceneCamera.transform.position, transform.position) > 100) { return; }

        foreach (var loop in loops)
        {
            if (loop.startPosition != null)
            {
                Gizmos.color = Color.blue;
                CustomGizmos.DrawTriangle(loop.startPosition.position, - loop.startPosition.right);

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(loop.startPosition.position, 0.1f);
            }
        }*/
    }

}



[System.Serializable]
public class Loop
{
    [SerializeField] private List<RoomEntity> eternalRooms = new List<RoomEntity>();
    public List<GameObject> roomPrefabsList = new List<GameObject>();
    public Transform startPosition;
    [SerializeField] private int minRooms;
    [SerializeField] private int maxRooms;
    public int gapDistance;
    [SerializeField][Range(0f, 1f)] private float pairRatio = 0f;

    [HideInInspector] public List<GameObject> prefabInstances = new List<GameObject>();
    [HideInInspector] public List<RoomEntity> roomEntities = new List<RoomEntity>();
    [HideInInspector] public List<Transform> pair1 = new List<Transform>();
    [HideInInspector] public List<Transform> pair2 = new List<Transform>();
    [HideInInspector] public int numOfRooms;
    [HideInInspector] public int numOfPairPortals;

    public void SetNumOfRooms()
    {
        numOfRooms = Random.Range(minRooms, maxRooms);
    }

    public void SetNumOfPairs()
    {
        numOfPairPortals = Random.Range(roomEntities.Count * Mathf.FloorToInt(pairRatio + 1) - 1,
                                        Mathf.Min(roomEntities.Count * (Mathf.FloorToInt(MinTotalCandidate() / 2 * pairRatio) + 1),
                                                  roomEntities.Count * Mathf.FloorToInt(MinTotalCandidate() / 2)));
    }

    public void AddEternalRooms()
    {
        foreach (var e in eternalRooms)
        {
            roomEntities.Add(e);
        }
    }

    public void Clear()
    {
        foreach (var room in roomEntities)
        {
            room.ResetPortalCandidates();
        }
    }

    public void MakePairs()
    {
        pair1.Clear();
        pair2.Clear();
        ShuffleList.FisherYates(ref roomEntities);
        Debug.Log("=-=-=-=-=-= makePairs() =-=-=-=-=-=");
        Debug.Log("numOfRooms= " + numOfRooms);
        Debug.Log("roomEntities.Count= " + roomEntities.Count);
        Debug.Log("numOfPairPortals= " + numOfPairPortals);

        for (int i = 0; i < numOfPairPortals; i++)
        {
            int index = i > roomEntities.Count - 1 ? i % roomEntities.Count : i;
            if (roomEntities[index].GetAvailableCandidateCount() > 0)
            {
                pair1.Add(roomEntities[index].GetRandomCandidate());

                int linkedRoomIndex = FindFirstEmptyRoomIndex(index + 1);
                if (linkedRoomIndex != -1)
                {
                    pair2.Add(roomEntities[linkedRoomIndex].GetRandomCandidate());
                }
                else
                {
                    linkedRoomIndex = FindFirstNotFilledRoomIndexExcept(index);
                    if (linkedRoomIndex != -1)
                    {
                        pair2.Add(roomEntities[linkedRoomIndex].GetRandomCandidate());
                    }
                    else
                    {
                        pair1.Remove(pair1[pair1.Count - 1]);
                        Debug.Log("=-=-=-=-=-= The End? =-=-=-=-=-=");
                        return;
                    }
                }
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

}