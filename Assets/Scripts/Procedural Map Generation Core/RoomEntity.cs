using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PiRadHex.CustomGizmos;
using PiRadHex.Shuffle;
using UnityEditor.ShaderGraph.Drawing;
using Unity.VisualScripting;
using UnityEngine.UIElements;

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
    [SerializeField] List<Color> colors;

    private List<GameObject> doorInstances = new List<GameObject>();

    private Color randomColor;
    private List<Material> doorMaterials = new List<Material>();

    private void Awake()
    {
        foreach (var candidate in portalCandidates)
        {
            portalCandidatesCopy.Add(candidate);
        }
        ResetPortalCandidates();
    }

    private void SetColor()
    {
        Material material = new Material(Shader.Find("Standard"));
        material.color = randomColor;
        foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = material;
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

            SetEnableDoor(candidate.transform, false);
        }

        int i = 0;
        foreach (var renderer in doorInstances[doorInstances.Count - 1].GetComponentsInChildren<MeshRenderer>())
        {
            if (doorMaterials.Count != 0) doorMaterials[i] = renderer.material;
            else doorMaterials.Add(renderer.material);
            i++;
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
        
        randomColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
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
                SetEnableDoor(candidate.transform);
                return candidate.transform;
            }
        }
        return null;
    }

    private void SetEnableDoor(Transform _transform, bool _mode = true)
    {
        foreach (var door in _transform.GetComponentsInChildren<Door>())
        {
            int i = 0;
            foreach (var renderer in _transform.GetChild(0).GetComponentsInChildren<MeshRenderer>())
            {
                if (_mode) renderer.material = doorMaterials[i];
                i++;
                Debug.Log(i);
            }
            door.transform.GetComponent<Collider>().enabled = _mode;
        }
        
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

    private void OnDrawGizmosSelected()
    {/*
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
        }*/
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

}
