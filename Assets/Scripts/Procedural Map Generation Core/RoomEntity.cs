using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PiRadHex.CustomGizmos;
using PiRadHex.Shuffle;

public class RoomEntity : MonoBehaviour
{
    [System.Serializable]
    public class PortalCandidate
    {
        public Transform transform;
        public bool isEmpty;
    }

    [SerializeField] private List<PortalCandidate> portalCandidates = new List<PortalCandidate>();

    private void Awake()
    {
        ResetPortalCandidates();
    }

    public void ResetPortalCandidates()
    {
        foreach (var candidate in portalCandidates)
        {
            candidate.isEmpty = true;
        }
    }

    public Transform GetRandomCandidate()
    {
        ShuffleList.FisherYates(ref portalCandidates);
        foreach (var candidate in portalCandidates)
        {
            if (candidate.isEmpty == true)
            {
                candidate.isEmpty = false;
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

}
