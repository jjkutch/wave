using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingWaves : MonoBehaviour
{
    public int Dimensions = 10;
    public Octave[] Octaves;
    public float UVScale;

    protected MeshFilter MeshFilter;
    protected Mesh Mesh;

    // Start is called before the first frame update
    void Start()
    {
        // mesh setup
        Mesh = new Mesh();
        Mesh.name = gameObject.name;

        Mesh.vertices = GenerateVertices();
        Mesh.triangles = GenerateTriangles();
        Mesh.uv = GenerateUVs();
        Mesh.RecalculateBounds();
        Mesh.RecalculateNormals();

        MeshFilter = gameObject.AddComponent<MeshFilter>();
        MeshFilter.mesh = Mesh;
    }

    public float GetHeight(Vector3 position)
    {
        // scale factor and position in local space
        Vector3 scale = new Vector3(1 / transform.lossyScale.x, 0, 1 / transform.lossyScale.z);
        Vector3 localPos = Vector3.Scale((position - transform.position), scale);

        // get edge points
        Vector3 p1 = new Vector3(Mathf.Floor(localPos.x), 0, Mathf.Floor(localPos.z));
        Vector3 p2 = new Vector3(Mathf.Floor(localPos.x), 0, Mathf.Ceil(localPos.z));
        Vector3 p3 = new Vector3(Mathf.Ceil(localPos.x), 0, Mathf.Floor(localPos.z));
        Vector3 p4 = new Vector3(Mathf.Ceil(localPos.x), 0, Mathf.Ceil(localPos.z));

        // clamp to plane size
        p1.x = Mathf.Clamp(p1.x, 0, Dimensions);
        p1.z = Mathf.Clamp(p1.z, 0, Dimensions);
        p2.x = Mathf.Clamp(p2.x, 0, Dimensions);
        p2.z = Mathf.Clamp(p2.z, 0, Dimensions);
        p3.x = Mathf.Clamp(p3.x, 0, Dimensions);
        p3.z = Mathf.Clamp(p3.z, 0, Dimensions);
        p4.x = Mathf.Clamp(p4.x, 0, Dimensions);
        p4.z = Mathf.Clamp(p4.z, 0, Dimensions);

        // get max dist to an edge and compute max - dist
        float p1Dist = Vector3.Distance(p1, localPos);
        float p2Dist = Vector3.Distance(p2, localPos);
        float p3Dist = Vector3.Distance(p3, localPos);
        float p4Dist = Vector3.Distance(p4, localPos);
        float max = Mathf.Max(p1Dist, p2Dist, p3Dist, p4Dist + Mathf.Epsilon);
        float dist = (max - p1Dist + max - p2Dist + max - p3Dist + max - p4Dist + Mathf.Epsilon);
        // weighted sum
        float height = Mesh.vertices[index((int) p1.x, (int) p1.z)].y * (max - p1Dist)
                     + Mesh.vertices[index((int) p2.x, (int) p2.z)].y * (max - p2Dist)
                     + Mesh.vertices[index((int) p3.x, (int) p3.z)].y * (max - p3Dist)
                     + Mesh.vertices[index((int) p4.x, (int) p4.z)].y * (max - p4Dist);

        return height * transform.lossyScale.y / dist;
    }

    private int index(int x, int z)
    {
        return x * (Dimensions + 1) + z;
    }

    private Vector3[] GenerateVertices()
    {
        Vector3[] vertices = new Vector3[(Dimensions + 1) * (Dimensions + 1)];

        // equally distributed vertices
        for (int x = 0; x <= Dimensions; x++)
        {
            for (int z = 0; z <= Dimensions; z++)
            {
                int ind = index(x, z);
                vertices[ind] = new Vector3(x, 0, z);
            }
        }

        return vertices;
    }

    private int[] GenerateTriangles()
    {
        int[] triangles = new int[Mesh.vertices.Length * 6];
        for (int x = 0; x < Dimensions; x++)
        {
            for (int z = 0; z < Dimensions; z++)
            {
                int ind = index(x, z);
                triangles[ind * 6 + 0] = index(x, z);
                triangles[ind * 6 + 1] = index(x+1, z+1);
                triangles[ind * 6 + 2] = index(x+1, z);
                triangles[ind * 6 + 3] = index(x, z);
                triangles[ind * 6 + 4] = index(x, z+1);
                triangles[ind * 6 + 5] = index(x+1, z+1);
            }
        }

        return triangles;
    }

    private Vector2[] GenerateUVs()
    {
        Vector2[] uvs = new Vector2[Mesh.vertices.Length];

        for (int x = 0; x <= Dimensions; x++)
        {
            for (int z = 0; z <= Dimensions; z++)
            {
                Vector2 v = new Vector2((x / UVScale) % 2, (z / UVScale) % 2);
                int ind = index(x, z);
                uvs[ind] = new Vector2(v.x <= 1 ? v.x : 2 - v.x,
                    v.y <= 1 ? v.y : 2 - v.y);
            }
        }

        return uvs;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] vertices = Mesh.vertices;
        for (int x = 0; x <= Dimensions; x++)
        {
            for (int z = 0; z <= Dimensions; z++)
            {
                float y = 0f;
                for (int o = 0; o < Octaves.Length; o++)
                {
                    if (Octaves[o].alternate)
                    {
                        float perlin = Mathf.PerlinNoise((x * Octaves[o].scale.x) / Dimensions,
                            (z * Octaves[o].scale.y) / Dimensions) * Mathf.PI * 2f;

                        y += Mathf.Cos(perlin * Octaves[o].speed.magnitude * Time.time)
                            * Octaves[o].height;
                    } else
                    {
                        float perlin = Mathf.PerlinNoise((x * Octaves[o].scale.x + Time.time * Octaves[o].speed.x) / Dimensions,
                            (z * Octaves[o].scale.y + Time.time * Octaves[o].speed.y) / Dimensions) - 0.5f;
                        y += perlin * Octaves[o].height;
                    }
                }
                int ind = index(x, z);
                vertices[ind] = new Vector3(x, y, z);
            }
        }

        Mesh.vertices = vertices;
        Mesh.RecalculateNormals();
    }

    [Serializable]
    public struct Octave
    {
        public Vector2 speed;
        public Vector2 scale;
        public float height;
        public bool alternate;
    }
}
