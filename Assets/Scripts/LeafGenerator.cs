using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates leaves is the drop area at the set height
/// </summary>
public class LeafGenerator {

    private const int MaxYPosition = 24;
    private const int MaxZPosition = 14;

    private int totalRatioWeights;
    private float dropAreaX;
    private float dropAreaY;
    private float height;
    private LeafColorer leafColorer;
    private Dictionary<LeafData, int> leafShapes;

    /// <summary>
    /// Creates a LeafGenerator
    /// </summary>
    /// <param name="leafRatios">
    ///     A dictionary of leafData to be used and
    ///     the percentage of that type of leaf to create
    /// </param>
    /// <param name="dropAreaX">The X size of the drop area</param>
    /// <param name="dropAreaY">The Y size of the drop area</param>
    /// <param name="height">The height of the drop area</param>
    public LeafGenerator(Dictionary<LeafData, int> leafRatios, float dropAreaX, float dropAreaY, float height) {
        int sum = 0;

        foreach (LeafData ls in leafRatios.Keys) {
            sum += leafRatios[ls];
        }

        this.totalRatioWeights = sum;
        this.dropAreaX = dropAreaX;
        this.dropAreaY = dropAreaY;
        this.height = height;
        this.leafColorer = new LeafColorer();
        this.leafShapes = leafRatios;
    }

    /// <summary>
    /// Instantiates a leaf in the world
    /// </summary>
    /// <param name="visualize">Should the leaf be visable</param>
    /// <returns>The leaf instantiated</returns>
    public GameObject GetNextLeaf(bool visualize) {

        LeafData nextLeafShape = this.GetLeafData(this.leafShapes);
        Vector3 leafData = nextLeafShape.GetConcreteLeafSize();

        Vector3 randomPoint = this.GetRandomPointInDropArea(this.dropAreaX, this.dropAreaY, this.height);     

        GameObject model = new GameObject();
        model.tag = "Leaf";
        model.name = "Leaf";
        model.transform.localScale = new Vector3(1, 1, 1);
        model.transform.localPosition = randomPoint;
        model.AddComponent<Rigidbody>();
        Leaf leaf = model.AddComponent<Leaf>();
        leaf.SetName(nextLeafShape.Name);

        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.SetParent(model.transform);
        cylinder.transform.localPosition = new Vector3(0, 0, 0);
        leaf.SetSize(leafData.x, leafData.y, leafData.z);
        cylinder.GetComponent<MeshRenderer>().material.color = this.leafColorer.GetColor(nextLeafShape);
        Object.Destroy(cylinder.GetComponent<CapsuleCollider>());
        cylinder.GetComponent<Renderer>().enabled = visualize;
        this.Bend(cylinder.GetComponent<MeshFilter>().mesh);

        this.AddCollider(cylinder.transform.localScale, cylinder.GetComponent<MeshFilter>().mesh, model.transform);

        model.transform.eulerAngles = this.GetRandomAngle();

        return model;
    }

    /// <summary>
    /// Returns a random euler angle
    /// </summary>
    /// <returns>The angle</returns>
    private Vector3 GetRandomAngle() {
        float x = Random.Range(0f, 360f);
        float y = Random.Range(0f, 360f);
        float z = Random.Range(0f, 360f);

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Return a random point in the drop area
    /// </summary>
    /// <param name="dropAreaX">The X size of the drop area</param>
    /// <param name="dropAreaY">THe Y size of the drop area</param>
    /// <param name="height">The height of the drop area</param>
    /// <returns>The point</returns>
    public Vector3 GetRandomPointInDropArea(float dropAreaX, float dropAreaY, float height) {
        Vector2 random2DPoint = Random.insideUnitCircle;
        return new Vector3(random2DPoint.x * dropAreaX, height, random2DPoint.y * dropAreaY);
    }

    /// <summary>
    /// Return the LeafData of a leaf using the ratio of leaves to drop
    /// </summary>
    /// <param name="leafData">The dictionary of LeafData for the simulation</param>
    /// <returns>The selected LeafData</returns>
    private LeafData GetLeafData(Dictionary<LeafData, int> leafData) {
        int cumulativeSum = 0;
        float randomNumber = Random.Range(0, this.totalRatioWeights);

        foreach (LeafData leafShape in leafData.Keys) {
            cumulativeSum += leafData[leafShape];
            if (randomNumber < cumulativeSum) {
                return leafShape;
            }
        }

        return new LeafData();
    }

    /// <summary>
    /// Bend the leaf randomly
    /// </summary>
    /// <param name="meshToBend">Mesh for the rendering of the leaf</param>
    private void Bend(Mesh meshToBend)
    {
        float gamma = Random.Range(1.0f, 500.0f);
        Vector3[] verts = meshToBend.vertices;
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i].y += verts[i].z * verts[i].z * gamma;
        }
        meshToBend.vertices = verts;
        meshToBend.RecalculateBounds();
        meshToBend.RecalculateNormals();
    }

    /// <summary>
    /// Add colliders to the leaf
    /// </summary>
    /// <param name="renderScale">Size of the leaf</param>
    /// <param name="renderMesh">Mesh for the rendering of the leaf</param>
    /// <param name="parentTransform">Transform of the parent object</param>
    private void AddCollider(Vector3 renderScale, Mesh renderMesh, Transform parentTransform)
    {
        float deltaY = renderScale.y * (renderMesh.vertices[MaxYPosition].y - 1);
        float deltaZ = renderScale.z * renderMesh.vertices[MaxZPosition].z;
        float angleRight = (float)Mathf.Atan2(deltaY, deltaZ) * Mathf.Rad2Deg;
        float angleLeft = (float)Mathf.Atan2(deltaY, -deltaZ) * Mathf.Rad2Deg;

        GameObject leftCollider = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftCollider.name = "leftCollider";
        leftCollider.transform.SetParent(parentTransform);
        leftCollider.transform.localPosition = new Vector3(0, deltaY / 3, 0.25f * renderScale.z);
        leftCollider.transform.localScale = new Vector3(renderScale.x, 2 * renderScale.y, renderScale.z / 2);
        leftCollider.transform.localEulerAngles = new Vector3(angleLeft, 0, 0);
        Object.Destroy(leftCollider.GetComponent<MeshRenderer>());

        GameObject rightCollider = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightCollider.name = "rightCollider";
        rightCollider.transform.SetParent(parentTransform);
        rightCollider.transform.localPosition = new Vector3(0, deltaY / 3, -0.25f * renderScale.z);
        rightCollider.transform.localScale = new Vector3(renderScale.x, 2 * renderScale.y, renderScale.z / 2);
        rightCollider.transform.localEulerAngles = new Vector3(angleRight, 0, 0);
        Object.Destroy(rightCollider.GetComponent<MeshRenderer>());
    }
}
