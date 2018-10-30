using UnityEngine;

/// <summary>
/// Reprsents the volume that the density calculator compares with the 
/// volume of leaves.
/// </summary>
public class DensityCalculationCylinder
{

    private GameObject[] objectsInWorld;
    private float cylinderAreaX;
    private float cylinderAreaY;
    private float cylinderHeight;

    /// <summary>
    /// Creates a DensityCalculationCylinder
    /// </summary>
    /// <param name="objects">Objects in the world</param>
    /// <param name="cylinderAreaX">The X size of the cylinder</param>
    /// <param name="cylinderAreaY">The Y size of the cylinder</param>
    public DensityCalculationCylinder(GameObject[] objects, float cylinderAreaX, float cylinderAreaY)
    {
        this.objectsInWorld = objects;
        this.cylinderAreaX = cylinderAreaX;
        this.cylinderAreaY = cylinderAreaY;
        this.cylinderHeight = this.ComputeCylinderHeightToUse();
        Debug.Log("Height of the density calculating cylinder is: " + this.cylinderHeight);
    }

    /// <summary>
    /// Computes a height for the cylinder by taking the mean and 2 standard deviations of all the leaf
    /// heights, lowering teh chance of a stray high leaf affecting the height of the cylinder
    /// </summary>
    /// <returns>The height of the cylinder to be used</returns>
    public float ComputeCylinderHeightToUse()
    {
        // Used as sums and then divided to give the mean and standard deviations of leaf heights
        float avgHeight = 0.0f;
        float stdDevHeight = 0.0f;

        // If no objects, return 0 height and avoid division by zero when computing average
        if (this.objectsInWorld.Length < 1)
        {
            return 0;
        }

        // Compute the average height of the leaves (take height to be the leafs lowest point)
        foreach (GameObject obj in this.objectsInWorld)
        {
            avgHeight += obj.GetComponent<Leaf>().GetLowestHeight(); 
        }
        avgHeight /= this.objectsInWorld.Length;

        // If only 1 object return just the average and avoid division by zero when computing stddev
        if (this.objectsInWorld.Length < 2)
        {
            return avgHeight;
        }

        // Compute the sample standard deviation of the leaf heights
        foreach (GameObject obj in this.objectsInWorld)
        {
            stdDevHeight += Mathf.Pow(obj.GetComponent<Leaf>().GetLowestHeight() - avgHeight, 2);
        }
        stdDevHeight = Mathf.Sqrt(stdDevHeight / (this.objectsInWorld.Length - 1));

        // Return the height that has ~97.5% of the heights in it (0 to 2 standard deviations from mean)
        // This will exclude any potentially not-dropped-yet leaf
        return avgHeight + 2 * stdDevHeight;
    }

    /// <summary>
    /// Returns a random point within a horizontal slice of the cylinder
    /// </summary>
    /// <param name="numSections">The number of horizontal slices to divide the cylinder into</param>
    /// <param name="sectionIndex">The index of the slice to generate the point in. Indices start at 0, which represents the bottom slice</param>
    /// <returns>The random point</returns>
    public Vector3 RandomPointInCylinderSlice(int numSlices, int sliceIndex)
    {
        Vector2 UnitCirclePoint = Random.insideUnitCircle;

        float startHeight = sliceIndex * (this.cylinderHeight / numSlices);
        float endHeight = startHeight + (this.cylinderHeight / numSlices);

        // unit circle point values are multiplied by the area dimensions that are where the density is calculated
        float x = UnitCirclePoint.x * this.cylinderAreaX;
        float y = Random.Range(startHeight, endHeight);
        float z = UnitCirclePoint.y * cylinderAreaY;

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Checks if the given point is in any of the world objects. Uses the ray cast method to check 
    /// whether or not point is inside objects.
    /// Method adapted from https://answers.unity.com/questions/163864/test-if-point-is-in-collider.html
    /// </summary>
    /// <param name="point">The point which may be inside a world object</param>
    /// <returns>True if the point given is inside any world objects</returns>
    public bool IsPointInObjects(Vector3 point)
    {
        // Chose a point which will definitely not be inside an object as raycast origin. This is
        // chosen as a point just above the leaf dropping height
        Vector3 distantPoint = new Vector3(0, SimSettings.GetDropHeight() + 10, 0);

        // Gets the direction and distance from raycast origin, to the point we are checking
        Vector3 directionToPoint = point - distantPoint;
        float distance = directionToPoint.magnitude;
        directionToPoint.Normalize();

        // Get the number of collisions from the distant point to the point we are checking.
        // Note this has to be done in both directions due to colliders not being hit from the inside (back face)
        // and this also RELIES on convex objects in the world, as raycastting will only collide with the same
        // collider once
        int hits = 0;
        hits += Physics.RaycastAll(distantPoint, directionToPoint, distance).Length;
        hits += Physics.RaycastAll(point, -directionToPoint, distance).Length;

        // If odd number of hits, then point is in object
        return (hits % 2) == 1;
    }
}