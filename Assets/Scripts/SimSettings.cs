/*
 * Created by Yuanyu Guo.
 * Class to store static variables.
 */


using System.Collections.Generic;

public class SimSettings {
    // Visual simulation settings
    private static bool visualize;

    // Spacial simulation settings
    private static float dropHeight = 100;
    private static float dropAreaX = 100;
    private static float dropAreaY = 100;

    // Leaf simulation settings
    private static Dictionary<LeafData, int> leafSizesAndRatios;
    private static bool useLeafLimit = true;
    private static int leafLimit = 1000;
    private static float leafVolumeLimit = 15;

    // Command line settings
    private static bool wasRunWithFlags = false;

    // Batch run setting
    private static bool batchRun = false;
    // Remaining run times
    private static int runTimesLeft = 1;

    // Simulation times for single run in batch run
    private static int simulationTimes = 10;
    // Simulation times left
    private static int simulationTimesLeft = 10;
    // Count the simulation times 
    private static int currentSimulationTimes = 0;

    // Density calculation settings
    private static float densityIgnoreBorder = 10;
    private static int monteCarloNumIterations = 1000;
    private static int numCylinderSlices = 10;

    // Set Simulation times
    public static void SetSimulationTimes(int simulationTimes)
    {
        SimSettings.simulationTimes = simulationTimes;
    }

    // Get Simulation times
    public static int GetSimulationTimes()
    {
        return simulationTimes;
    }

    // Set batchrun times
    public static int GetSimulationTimesLeft()
    {
        return simulationTimesLeft;
    }

    // return true if simulationTimesLeft > 1 and decrease simulationTimesLeft by 1
    // NOT > 0 because the first run did not decrease the number
    public static bool DecreaseSimulationTimesLeft()
    {
        if (simulationTimesLeft > 1)
        {
            simulationTimesLeft = simulationTimesLeft - 1;
            return true;
        }
        else
        {
            return false;
        }
    }
    // Reset simulationTimesLeft
    public static void ResetSimulationTimesLeft()
    {
        simulationTimesLeft = simulationTimes;
    }


    // Get visualization flag
    public static bool GetVisualize()
    {
        return SimSettings.visualize;
    }

    // Set visualization flag
    public static void SetVisualize(bool isVisualize)
    {
        SimSettings.visualize = isVisualize;
    }

    // Get batchrun flag
    public static bool GetBatchrun()
    {
        return batchRun;
    }

    // Set batchrun flag
    public static void SetBatchrun(bool isBatchrun)
    {
        batchRun = isBatchrun;
    }

    // Get batchrun times
    public static int GetRunTimeesLeft()
    {
        return runTimesLeft;
    }

    // Set batchrun times
    public static void SetRunTimesLeft(int runTimesLeft)
    {
        SimSettings.runTimesLeft = runTimesLeft;
    }

    // Check if a leaf limit exists
    public static bool GetUseLeafLimit()
    {
        return useLeafLimit;
    }

    // Get the leaf limit is (regardless of whether or not if it's in use)
    public static int GetLeafLimit()
    {
        return leafLimit;
    }

    // Set a maximum number of leaves for the simulation to stop at
    public static void SetLeafLimit(int numberLimit)
    {
        leafLimit = numberLimit;
        useLeafLimit = true;
    }

    // Remove a maximum number of leaves limit if it existed
    public static void RemoveLeafLimit()
    {
        useLeafLimit = false;
    }

    // Get leaf volume limit
    public static float GetLeafVolumeLimit()
    {
        return leafVolumeLimit;
    }

    // Get the drop height of the simulation
    public static float GetDropHeight()
    {
        return dropHeight;
    }

    // Set the drop height of the simulation
    public static void SetDropHeight(float dropHeight)
    {
        SimSettings.dropHeight = dropHeight;
    }

    // Get the leaf dropping area X-axis of the simulation
    public static float GetDropAreaX()
    {
        return dropAreaX;
    }

    // Get the leaf dropping area Y-axis of the simulation
    public static float GetDropAreaY()
    {
        return dropAreaY;
    }

    // Set the drop height of the simulation
    public static void SetDropArea(float dropAreaX, float dropAreaY)
    {
        SimSettings.dropAreaX = dropAreaX;
        SimSettings.dropAreaY = dropAreaY;
    }

    // Get the list of leaves and their relative ratios to use in the simulation
    public static Dictionary<LeafData, int> GetLeafSizesAndRatios()
    {
        return leafSizesAndRatios;
    }

    // Set the list of leaves and their relative ratios to use in the simulation
    public static void SetLeafSizesAndRatios(Dictionary<LeafData, int> leafSizesAndRatios)
    {
        SimSettings.leafSizesAndRatios = leafSizesAndRatios;
    }

    // Get the distance from the edge of the dropping area to ignore when calculating density
    // This avoid edge effects where the density may be different when leaves slope to the ground
    public static float GetDensityIgnoreBorder()
    {
        return densityIgnoreBorder;
    }

    // Get the number of iterations to run the monte carlo method when calculating the leaf litter density (uses volume intersaction)
    // This is the number of points sampled in EACH horizontal slice of the cylinder (numCylinderSlices)
    public static int GetMonteCarloNumIterations()
    {
        return monteCarloNumIterations;
    }

    // Get the number of horizontal slices to split the density calculation cylinder into and average the result over
    public static int GetNumCylinderSlices()
    {
        return numCylinderSlices;
    }

    // Set the current simulation times
    public static void SetCurrentSimulationTimes(int currentSimulationTimes) 
    {
        SimSettings.currentSimulationTimes = currentSimulationTimes;
    }

    // Get the current simulation times
    public static int GetCurrentSimulationTimes()
    {
        return currentSimulationTimes;
    }

    // Set whether or not the program was run with flags (i.e. via the command line)
    public static void SetWasRunWithFlags(bool wasRunWithFlags)
    {
        SimSettings.wasRunWithFlags = wasRunWithFlags;
    }

    // Check if the program was run with command line arguments
    public static bool GetWasRunWithFlags()
    {
        return wasRunWithFlags;
    }
}
