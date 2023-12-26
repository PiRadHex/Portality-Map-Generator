using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PiRadHex.CustomGizmos;
using PiRadHex.Shuffle;

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
        DestroyInstances(loops);
        DestroyItems();

        portalPlacer.SetAllUnused();

        foreach (Loop loop in loops)
        {
            loop.setNumsOfRoomsAndPairs();
            loop.AddEternalRooms();
            PlaceRoomsIn(loop);
            loop.MakePairs();
            if (loop.pair1.Count == 0 | loop.pair2.Count == 0) continue;     
            portalPlacer.PlaceLinkedPortals(loop.pair1, loop.pair2);
   
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DestroyInstances(loops);
            portalPlacer.SetAllUnused();

            foreach (Loop loop in loops)
            {
                loop.setNumsOfRoomsAndPairs();
                loop.AddEternalRooms();
                PlaceRoomsIn(loop);
                loop.MakePairs();
                if (loop.pair1.Count == 0 | loop.pair2.Count == 0) continue;
                portalPlacer.PlaceLinkedPortals(loop.pair1, loop.pair2);

            }
        }
    }

    

    void PlaceRoomsIn(Loop loop)
    {
        for (int i = 0; i < loop.numOfRooms; i++)
        {
            GameObject prefab = loop.roomPrefabsList[Random.Range(0, loop.roomPrefabsList.Count)];

            GameObject prefabInstance = Instantiate(prefab, transform);
            prefabInstance.transform.position = loop.startPosition.position + Vector3.left * i * loop.gapDistance;

            //prefabInstance.name = prefab.name + " - " + prefabInstance.GetInstanceID();

            loop.prefabInstances.Add(prefabInstance);

            // place random items in the room
            //itemPlacers[0].PlacePrefabsAtPosotions(new List<Transform>(prefabInstance.transform.GetChild(1).transform.GetChild(1).GetComponentsInChildren<Transform>()));
        }
    }



    void DestroyInstances(List<Loop> loops)
    {
        foreach (Loop loop in loops)
        {
            //prefabInstances.Remove(startRoom);
            //prefabInstances.Remove(endRoom);

            for (int i = 0; i < loop.prefabInstances.Count; i++)
            {
                Destroy(loop.prefabInstances[i]);
            }
            loop.prefabInstances.Clear();
            //Debug.Log("DestroyInstances() is done.");
            //Debug.Log(prefabInstances.Count);
        }

    }



    void DestroyItems()
    {
        //itemPlacers[0].DestroyInstances();
    }

    private void OnDrawGizmosSelected()
    {
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
        }
    }



}



[System.Serializable]
public class Loop
{
    public List<GameObject> eternalRooms = new List<GameObject>();
    public List<GameObject> roomPrefabsList = new List<GameObject>();
    public Transform startPosition;
    public int minRooms;
    public int maxRooms;
    public int gapDistance;
    [Range(1f, 2f)]
    public float pairRatio = 1f;

    [HideInInspector] public List<GameObject> prefabInstances = new List<GameObject>();
    [HideInInspector] public List<Transform> pair1 = new List<Transform>();
    [HideInInspector] public List<Transform> pair2 = new List<Transform>();
    [HideInInspector] public int numOfRooms;
    [HideInInspector] public int numOfPairPortals;

    public void setNumsOfRoomsAndPairs()
    {
        
        numOfRooms = Random.Range(minRooms, maxRooms);
        numOfPairPortals = Random.Range((numOfRooms + eternalRooms.Count) * Mathf.RoundToInt(pairRatio) - 1, (numOfRooms + eternalRooms.Count) * Mathf.RoundToInt(pairRatio));
    }

    public void AddEternalRooms()
    {
        foreach (GameObject e in eternalRooms)
        {
            prefabInstances.Add(e.transform.GetChild(0).gameObject);
        }
    }

    public void MakePairs()
    {
        pair1.Clear();
        pair2.Clear();
        ShuffleList.FisherYates(ref prefabInstances);
        Debug.Log("=-=-=-=-=-= makePairs() =-=-=-=-=-=");
        Debug.Log("numOfRooms= " + numOfRooms);
        Debug.Log("prefabInstances.Count= " + prefabInstances.Count);
        Debug.Log("numOfPairPortals= " + numOfPairPortals);

        for (int i = 0; i < numOfPairPortals; i++)
        {
            int index = i > prefabInstances.Count - 1 ? i - prefabInstances.Count : i;
            Transform pair1NextCandidate = SelectPositionCandidate(index);
            if (pair1NextCandidate == null) continue;
            pair1.Add(pair1NextCandidate);
            int linkedRoomIndex = FindFirstEmptyRoomIndex(index + 1);
            if (linkedRoomIndex != -1)
            {
                pair2.Add(SelectPositionCandidate(linkedRoomIndex));
            }
            else
            {
                linkedRoomIndex = FindFirstNotFilledRoomIndex();
                if (linkedRoomIndex != -1)
                {
                    pair2.Add(SelectPositionCandidate(linkedRoomIndex));
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

    private Transform SelectPositionCandidate(int index)
    {
        List<Transform> positionCandidates = new List<Transform>(prefabInstances[index].transform.GetChild(1).transform.GetChild(0).GetComponentsInChildren<Transform>());
        positionCandidates.Remove(positionCandidates[0]);
        ShuffleList.FisherYates(ref positionCandidates);
        for (int i = 0; i < positionCandidates.Count; i++)
        {
            if (positionCandidates[i].name == "used") continue;
            positionCandidates[i].name = "used";
            return positionCandidates[i];
        }
        return null;
    }

    private int FindFirstEmptyRoomIndex(int index)
    {
        for (int i = index; i < prefabInstances.Count; i++)
        {
            bool isEmpty = false;
            List<Transform> positionCandidates = new List<Transform>(prefabInstances[i].transform.GetChild(1).transform.GetChild(0).GetComponentsInChildren<Transform>());
            for (int j = 0; j < positionCandidates.Count; j++)
            {
                if (positionCandidates[j].name == "used") break;
                isEmpty = true;
            }
            if (isEmpty)
            {
                return i;
            }
        }
        return -1;
    }

    private int FindFirstNotFilledRoomIndex()
    {
        for (int i = 0; i < prefabInstances.Count; i++)
        {
            List<Transform> positionCandidates = new List<Transform>(prefabInstances[i].transform.GetChild(1).transform.GetChild(0).GetComponentsInChildren<Transform>());
            for (int j = 0; j < positionCandidates.Count; j++)
            {
                if (positionCandidates[j].name != "used")
                {
                    return i;
                }
            }
        }
        return -1;
    }



}