using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomEntity : MonoBehaviour
{
    [System.Serializable]
    public class PortalCandidate
    {
        public Transform transform;
        public bool isEmpty;
    }

    private List<PortalCandidate> portalCandidates = new List<PortalCandidate>();

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


    private void OnDrawGizmos()
    {
        //var sceneCamera = Application.isPlaying? Camera.main : SceneView.currentDrawingSceneView.camera;
        if (Vector3.Distance(SceneView.currentDrawingSceneView.camera.transform.position, transform.position) > 20f) return;

        foreach (var candidate in portalCandidates)
        {
            if (candidate.transform != null)
            {
                DrawTriangle(candidate.transform.position, candidate.transform.forward, Color.blue);
            }
        }
    }

    private void DrawTriangle(Vector3 position, Vector3 direction, Color color, float size = 1f)
    {
        Gizmos.color = color;

        int fillerLines = Mathf.CeilToInt(Mathf.Pow(2, 6));
        for (int i = 0; i < fillerLines; i++)
        {
            Gizmos.DrawCube(position + i * direction * size / fillerLines, new Vector3(0.02f, 0.001f, 0.4f - i / (fillerLines / 4f * 10f)));
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(position, 0.1f);
        
    }

}
