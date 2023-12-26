using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PiRadHex.CustomGizmos;

public class RoomEntity : MonoBehaviour
{
    [System.Serializable]
    public class PortalCandidate
    {
        public Transform transform;
        public bool isEmpty;
    }

    [SerializeField] private List<PortalCandidate> portalCandidates = new List<PortalCandidate>();

    public List<PortalCandidate> GetPortalCandidates() { return portalCandidates; }

    public void ResetPortalCandidates()
    {
        foreach (var candidate in portalCandidates)
        {
            candidate.isEmpty = true;
        }
    }

    private void Awake()
    {
        ResetPortalCandidates();
    }


    private void OnDrawGizmosSelected()
    {
        var sceneCamera = SceneView.currentDrawingSceneView == null ? Camera.main : SceneView.currentDrawingSceneView.camera;
        if (Vector3.Distance(sceneCamera.transform.position, transform.position) > 20f) { return; }

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
