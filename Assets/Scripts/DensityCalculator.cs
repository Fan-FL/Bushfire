using UnityEngine;

/// <summary>
/// Script of calculating the density of leaf litter
/// as the volume ratio of leaves to air.
/// </summary>
public class DensityCalculator {

    /// <summary>
    /// Calculates the density of leaf litter as a volume ratio by randomly sampling
    /// the area to check if a point exists in a leaf or not
    /// </summary>
    /// <param name="calcArea">The area to calculate the density for</param>
    /// <param name="sampleSize">The number of points to sample</param>
    /// <returns></returns>
    public float CalculateDensity(DensityCalculationCylinder calcArea, int sliceSampleSize, int numSlices) {
        // An array of counter, keeping track of how many random points happen to be in leaves, in each of the cylinder slices
        int[] sliceLeafPointCounts = new int[numSlices];

        // First loop each slice
        for (int i=0; i < sliceLeafPointCounts.Length; i++)
        {
            // Initialise number of points in leaves to 0
            sliceLeafPointCounts[i] = 0;

            // For each random point in the current slice, check if its in a leaf, and if so add it to the current slices counter
            for (int j=0; j < sliceSampleSize; j++)
            {
                Vector3 pointInCylinderSlice = calcArea.RandomPointInCylinderSlice(numSlices, i);
                if (calcArea.IsPointInObjects(pointInCylinderSlice)){
                    sliceLeafPointCounts[i]++;
                }
            }
        }

        // Density is computed as the number of points in leaves over the number of points in total for each slice.
        // The densities in the slices are then averaged for a final value
        if (sliceSampleSize > 0) {

            // Add density of each slice to sum
            float densitySum = 0.0f;
            for (int i=0; i< sliceLeafPointCounts.Length; i++)
            {
                densitySum += (sliceLeafPointCounts[i] / (float)sliceSampleSize);
            }

            // Divide and return sum for density
            return densitySum / sliceLeafPointCounts.Length;
        }
        else
        {
            return 0;
        }
    }
}
