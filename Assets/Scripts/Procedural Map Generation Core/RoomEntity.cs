using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PiRadHex.CustomGizmos;
using PiRadHex.Shuffle;
using TMPro;

public class RoomEntity : MonoBehaviour
{
    [System.Serializable]
    public class PortalCandidate
    {
        public Transform transform;
        public bool isEmpty;
    }

    [SerializeField] private List<PortalCandidate> portalCandidates = new List<PortalCandidate>();
    private List<PortalCandidate> portalCandidatesCopy = new List<PortalCandidate>();

    [SerializeField] GameObject doorPrefab;
    public bool setRandomColor = true;
    [SerializeField] List<TextMeshPro> textNumbers = new List<TextMeshPro>(); 
    private List<GameObject> doorInstances = new List<GameObject>();
    private List<Material> doorMaterials = new List<Material>();

    private void Awake()
    {
        foreach (var candidate in portalCandidates)
        {
            portalCandidatesCopy.Add(candidate);
        }
        ResetPortalCandidates();
        CacheDoorMaterials();
    }

    public void ResetPortalCandidates()
    {
        foreach (var candidate in portalCandidates)
        {
            candidate.isEmpty = true;
        }
        for (int i = 0; i < portalCandidatesCopy.Count; i++)
        {
            portalCandidates[i] = portalCandidatesCopy[i];
        }

        DestroyDoors();
        PlaceDoors();
        SetColor();
    }

    public Transform GetRandomCandidate()
    {
        ShuffleList.FisherYates(ref portalCandidates);
        foreach (var candidate in portalCandidates)
        {
            if (candidate.isEmpty == true)
            {
                candidate.isEmpty = false;
                SetDoorEnable(candidate.transform, true);
                return candidate.transform;
            }
        }
        return null;
    }

    public int GetAvailableCandidateCount()
    {
        int count = 0;
        foreach (var candidate in portalCandidates)
        {
            if (candidate.isEmpty == true)
            {
                count++;
            }
        }
        return count;
    }

    public int GetTotalCandidateCount()
    {
        return portalCandidates.Count;
    }

    public void SetRoomNumber(int num)
    {
        foreach (var textMesh in textNumbers)
        {
            textMesh.text = num.ToString();
        }
    }

    private void SetColor()
    {
        Material material = new Material(Shader.Find("Standard"));
        material.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        if (setRandomColor)
        {
            foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
            {
                if (renderer.GetComponent<TextMeshPro>() == null) renderer.material = material;
            }
        }
        
    }

    private void PlaceDoors()
    {
        foreach (var candidate in portalCandidates)
        {
            GameObject prefabInstance = Instantiate(doorPrefab, candidate.transform);
            prefabInstance.transform.position = candidate.transform.position;
            prefabInstance.transform.rotation = candidate.transform.rotation;
            prefabInstance.transform.localPosition += Vector3.forward * 0.5f;

            doorInstances.Add(prefabInstance);

            SetDoorEnable(candidate.transform, false);
        }
    }

    private void DestroyDoors()
    {
        foreach (var door in doorInstances)
        {
            Destroy(door);
        }

        doorInstances.Clear();
    }

    private void SetDoorEnable(Transform _transform, bool _mode = true)
    {
        if (_mode)
        {
            int i = 0;
            foreach (var renderer in _transform.GetChild(0).GetComponentsInChildren<MeshRenderer>())
            {
                renderer.material = doorMaterials[i]; // I don't know why, but after generating a new seed, the doors to the eternal rooms don't update as expected.
                //if(_transform.parent.name == "Room [A]") Debug.Log(renderer.material + ":::::" + doorMaterials[i]);
                i++;
            }
        }
        foreach (var door in _transform.GetComponentsInChildren<Door>())
        {
            door.transform.GetComponent<Collider>().enabled = _mode;
        }
        _transform.GetChild(0).GetComponent<Animator>().enabled = _mode;
    }

    private void CacheDoorMaterials()
    {
        doorMaterials.Clear();
        GameObject prefabInstance = Instantiate(doorPrefab, transform);
        foreach (var renderer in prefabInstance.GetComponentsInChildren<MeshRenderer>())
        {
            doorMaterials.Add(renderer.material);
        }
        Destroy(prefabInstance);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        var sceneCamera = SceneView.currentDrawingSceneView == null ? Camera.main : SceneView.currentDrawingSceneView.camera;
        if (Vector3.Distance(sceneCamera.transform.position, transform.position) > 50f) { return; }

        foreach (var candidate in portalCandidates)
        {
            if (candidate.transform != null)
            {
                if (candidate.isEmpty)
                {
                    Gizmos.color = Color.blue;
                }
                else
                {
                    Gizmos.color = Color.red;
                }
                
                CustomGizmos.DrawTriangle(candidate.transform.position, candidate.transform.forward);

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(candidate.transform.position, 0.1f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) { return; }
        var sceneCamera = SceneView.currentDrawingSceneView == null ? Camera.main : SceneView.currentDrawingSceneView.camera;
        if (Vector3.Distance(sceneCamera.transform.position, transform.position) > 50f) { return; }

        foreach (var candidate in portalCandidates)
        {
            if (candidate.transform != null)
            {
                if (candidate.isEmpty)
                {
                    Gizmos.color = Color.blue;
                }
                else
                {
                    Gizmos.color = Color.red;
                }

                CustomGizmos.DrawTriangle(candidate.transform.position, candidate.transform.forward);

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(candidate.transform.position, 0.1f);
            }
        }
    }
#endif

}
