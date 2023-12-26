using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;


    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    public int xSize = 20;
    public int zSize = 20;


    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = GenerateNoiseMapping(x, z);
                vertices[i] = new Vector3(x,y,z);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for(int z = 0; z < zSize;z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;

                //yield return new WaitForSeconds(0.01f);
            }
            vert++;
        }

        uvs = new Vector2[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                uvs[i] = new Vector2((float)x/xSize,(float)z/zSize);
                i++;
            }
        }

    }


    private void OnGUI()
    {
        CreateShape();
        UpdateMesh();
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null) return;

        for(int i = 0; i < vertices.Length; i++) {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }

    private float GenerateNoiseMapping(float x, float z)
    {
        int octaves = 4;
        float[] frequencies = new float[] { 0.25f, 1f, 4f, 16f };
        float[] amplitudes = new float[] { 8f, 4f, 2f, 1f };

        // Initialize the y value to zero
        float y = 0f;

        // Loop through each octave
        for (int k = 0; k < octaves; k++)
        {
            // Add the noise value from this layer, scaled by its amplitude
            y += amplitudes[k] * Mathf.PerlinNoise(frequencies[k] * x * .3f, frequencies[k] * z * .3f);
        }

        return y;
    }
}
