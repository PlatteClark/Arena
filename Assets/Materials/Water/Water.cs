using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaterWave : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    Vector2[] uvs;
    int[] triangles;

    public int xSize = 20;
    public int zSize = 20;

    public float waveHeight = 0.5f;
    public float waveFrequency = 0.5f;
    public float waveSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        uvs = new Vector2[vertices.Length];
        float halfX = xSize / 2f;
        float halfZ = zSize / 2f;

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * waveFrequency, z * waveFrequency) * waveHeight;
                vertices[i] = new Vector3(x - halfX, y, z - halfZ);
                uvs[i] = new Vector2((float)x / xSize, (float)z / zSize);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
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
            }
            vert++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                Vector3 vert = vertices[i];
                vert.y = Mathf.PerlinNoise(x * waveFrequency + Time.time * waveSpeed, z * waveFrequency + Time.time * waveSpeed) * waveHeight;
                vertices[i] = vert;
                i++;
            }
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
        }
    }
}



