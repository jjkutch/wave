using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateWave : MonoBehaviour
{
    public Transform surfboardTransform;

    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    private Mesh bakedMesh;
    Vector3[] vertices;
    int[] triangles;
    int vertexNum;

    int blendShapeCount;
    int playIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        blendShapeCount = skinnedMesh.blendShapeCount;

        bakedMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = bakedMesh;

    }

    // Update is called once per frame
    void Update()
    {

        if (playIndex > 0) skinnedMeshRenderer.SetBlendShapeWeight(playIndex - 1, 0f);
        if (playIndex == 0) skinnedMeshRenderer.SetBlendShapeWeight(blendShapeCount - 1, 0f);

        skinnedMeshRenderer.SetBlendShapeWeight(playIndex, 100f);

        playIndex++;
        if (playIndex > blendShapeCount - 1) playIndex = 0;

        bakedMesh.Clear();
        skinnedMeshRenderer.BakeMesh(bakedMesh, true);
        vertices = bakedMesh.vertices;

        vertexNum = 7600;

        Vector3 target = new Vector3(vertices[vertexNum].x,
            0.01f + vertices[vertexNum].y, vertices[vertexNum].z);

        surfboardTransform.position = target;

        //// skinnedMeshRenderer.SetBlendShapeWeight(150, 100f);



        //////dynamicMesh.Clear();
        ////dynamicMesh.vertices = bakedMesh.vertices;
        ////dynamicMesh.triangles = bakedMesh.triangles;
        ////dynamicMesh.RecalculateNormals();

        ////bakedMesh.RecalculateNormals();
        //meshCollider.sharedMesh = bakedMesh;

        //Vector3 position = new Vector3(-17.1f, 4.27f, 41.24f);
        //Vector3 closestPoint = meshCollider.ClosestPoint(position);
        //Debug.Log(closestPoint);


    }
}
