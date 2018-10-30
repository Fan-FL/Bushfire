using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Controls the running of the simulation
/// </summary>
public class SimulationController : MonoBehaviour {

    private const string OUTPUT_SCENE = "Output";
    private const float LEAF_SPAWN_TIME = 0.25f;
    private const float BATCH_RUN_TIMESCALE = 8f;

    private LeafGenerator leafGen;
    private DensityCalculator denCalc;
    private int numLeavesCreated = 0;
    private GameObject[] leaves;

    private float dropAreaX = SimSettings.GetDropAreaX() / 2;
    private float dropAreaY = SimSettings.GetDropAreaY() / 2;
    private float height = SimSettings.GetDropHeight();
    private float densityIgnoreBorder = SimSettings.GetDensityIgnoreBorder();

    // Parameters to update the progress bar
    private int targetValueOnce = 100;
    private int runCount = 0;
    private int totalProgress = 100;

    private float leafSpawnTimer = LEAF_SPAWN_TIME;

    private System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

    // Use this for initialization
    void Start() {
        this.leafGen = new LeafGenerator(SimSettings.GetLeafSizesAndRatios(), this.dropAreaX, this.dropAreaY, this.height);
        this.denCalc = new DensityCalculator();

        // Batuch Run
        if (SimSettings.GetBatchrun()) {
            // Initialise the parameter for progres bar
            runCount = SimSettings.GetCurrentSimulationTimes();
            runCount++;
            SimSettings.SetCurrentSimulationTimes(runCount);

            if (BatchRunCsvLoader.batchrunLeafAndRatio.Count != 0) {
                targetValueOnce = 100 / (BatchRunCsvLoader.batchrunLeafAndRatio.Count * SimSettings.GetSimulationTimes());
            }
            totalProgress = targetValueOnce * runCount;

            this.SetTimeScale(BATCH_RUN_TIMESCALE);

            // Show the progress bar
            ProgressBarController.progressBar.gameObject.SetActive(true);
        }
        else {
            ProgressBarController.progressBar.gameObject.SetActive(false);
        }

    }

    void FixedUpdate() {

        leafSpawnTimer -= Time.deltaTime;

        if (this.CanCreateLeaf() && leafSpawnTimer <= 0) {
            this.CreateLeaf();
            leafSpawnTimer = LEAF_SPAWN_TIME;
        }
        else if (this.HasEnded()) {
            this.leaves = GameObject.FindGameObjectsWithTag("Leaf");
            this.FreezeAll(this.leaves);
            this.CalculateDensity(this.leaves);
        }

        LoadingProgressBar();
    }

    // The method to update the porgress bar
    private void LoadingProgressBar() {
        if (ProgressBarController.progressBar.curProValue < totalProgress) {
            ProgressBarController.progressBar.curProValue++;
        }
        ProgressBarController.progressBar.progressImg.fillAmount = ProgressBarController.progressBar.curProValue / 100f;
        ProgressBarController.progressBar.proText.text = ProgressBarController.progressBar.curProValue + "%";
    }

    /// <summary>
    /// Creates a leaf gameobject
    /// </summary>
    /// <returns>A leaf</returns>
    private GameObject CreateLeaf() {
        GameObject leaf = this.leafGen.GetNextLeaf(SimSettings.GetVisualize());
        this.numLeavesCreated++;
        return leaf;
    }

    /// <summary>
    /// Check whether to create a leaf or not based on the
    /// number of leaves created or on the total leaf volume
    /// </summary>
    /// <returns>Can create a leaf</returns>
    public bool CanCreateLeaf() {
        // Limited
        if (SimSettings.GetUseLeafLimit() && this.numLeavesCreated < SimSettings.GetLeafLimit()) {
            return true;
        }

        // Unlimited
        if (!SimSettings.GetUseLeafLimit() && this.totalLeavesVolume(this.leaves) < SimSettings.GetLeafVolumeLimit()) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Calculates the total cumulative volume of all leaves
    /// in an array of leaves
    /// </summary>
    /// <param name="leaves">An array of leaf gameobjects</param>
    /// <returns>Total volume</returns>
    public float totalLeavesVolume(GameObject[] leaves) {
        float sum = 0f;

        foreach (GameObject leaf in leaves) {
            sum += leaf.GetComponent<Leaf>().GetVolume();
        }

        return sum;
    }

    /// <summary>
    /// Freezes all the given leaves so that they are not moving during density calculation
    /// </summary>
    /// <param name="leaves">All of the leaves in the world</param>
    public void FreezeAll(GameObject[] leaves) {
        foreach (GameObject lf in leaves) {
            lf.GetComponent<Leaf>().FreezeLeaf();
        }
    }

    /// <summary>
    /// Calculates the density of leaves and changes to the output scene
    /// to display the results
    /// </summary>
    /// <param name="leaves">All the current leaf objects in the world</param>
    private void CalculateDensity(GameObject[] leaves) {
        // Time how long it takes for the density to be computed (for optimisation use)
        stopWatch.Start();

        DensityCalculationCylinder calcArea = new DensityCalculationCylinder(
                                    leaves,
                                    (this.dropAreaX - this.densityIgnoreBorder),
                                    (this.dropAreaY - this.densityIgnoreBorder)
                                  );
        float density = denCalc.CalculateDensity(calcArea, SimSettings.GetMonteCarloNumIterations(), SimSettings.GetNumCylinderSlices());

        // Console log the density and the time it took to compute
        stopWatch.Stop();
        Debug.Log(string.Format("Density calculated as {0} in {1} seconds.",
                                System.Math.Round(density, 6),
                                System.Math.Round(stopWatch.ElapsedMilliseconds / 1000.0, 6)));

        Results.addResult(density);
        this.ChangeToOutputScene();
    }

    /// <summary>
    /// Used as a base line for volume density estimation, primarily for debugging. Compute the density
    /// by taking the volume of all leaves with centers within the computing area, and dividing by cylinder volume.
    /// Should always slightly over estimate volume due to parts of leaves sticking out of the calculating area
    /// that are still considered in the ratio
    /// </summary>
    /// <param name="leaves">All the current leaf objects in the world</param>
    private void CalculateDensityBaseline(GameObject[] leaves) {
        // Create cylinder to get the height to use
        DensityCalculationCylinder calcArea = new DensityCalculationCylinder(
                                    leaves,
                                    (this.dropAreaX - this.densityIgnoreBorder),
                                    (this.dropAreaY - this.densityIgnoreBorder)
                                  );
        float cylHeight = calcArea.ComputeCylinderHeightToUse();

        // Sum the volumes of leaves whose center point is within the cylinder elipse base
        float leafVol = 0.0f;
        foreach (GameObject lf in leaves) {
            if (Mathf.Abs(lf.GetComponent<Leaf>().GetCenter().x) < ((this.dropAreaX - this.densityIgnoreBorder)) &&
                Mathf.Abs(lf.GetComponent<Leaf>().GetCenter().z) < ((this.dropAreaY - this.densityIgnoreBorder)) &&
                lf.GetComponent<Leaf>().GetCenter().y < cylHeight) {
                leafVol += lf.GetComponent<Leaf>().GetVolume();
            }
        }

        // Compute the cylinder volume
        float cylVol = cylHeight * (Mathf.PI * (this.dropAreaX - this.densityIgnoreBorder) * (this.dropAreaY - this.densityIgnoreBorder));

        // Console log the density
        Debug.Log(string.Format("Baseline volume density calculated as {0}",
                                System.Math.Round(leafVol / cylVol, 6)));
    }

    /// <summary>
    /// Returns true when all leaf objects are kinematic (frozen)
    /// </summary>
    /// <param name="leaves">List of all leaves in the world</param>
    /// <returns>Whether all leaves have been frozen</returns>
    public bool HasEnded(GameObject[] leaves) {
        foreach (GameObject leaf in leaves) {
            if (!leaf.GetComponent<Rigidbody>().isKinematic) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Alternative stopping check. Returns true when the time since it's first call is longer than
    /// twice the time a leaf takes to fall to teh ground from the dropping height. Twice the time is used
    /// as a safe measure
    /// </summary>
    /// <returns>
    /// Whether or not the time between the first call and the current call of this method exceeds twice the
    /// time it takes a single leaf to fall from the dropping height to the ground
    /// </returns>
    public bool HasEnded() {
        // On fist call of the method, start running teh stopwatch, and return false
        if (!this.stopWatch.IsRunning) {
            this.stopWatch.Start();
            return false;
        }

        // Not the first time method run, get time for a leaf to fall, and time since first call of method
        double timeToFall = System.Math.Sqrt(SimSettings.GetDropHeight() / System.Math.Abs(Physics.gravity.y));
        double secondsSinceLastLeaf = this.stopWatch.ElapsedMilliseconds / 1000.0;

        // Reset the stopwatch if enough time has elapsed, and return true
        if (secondsSinceLastLeaf > timeToFall * 2 && !this.CanCreateLeaf()) {
            stopWatch.Stop();
            stopWatch.Reset();
            return true;
        }
        else {
            return false;
        }
    }

    /// <summary>
    /// Change to the output scene
    /// </summary>
    public void ChangeToOutputScene() {
        SceneManager.LoadScene(OUTPUT_SCENE);
    }

    /// <summary>
    /// Reset time scale back to normal (1f)
    /// </summary>
    public void SetTimeScaleNormal() {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Multiply time scale by scale.
    /// A scale between 0 and 1 will slow the simulation.
    /// A scale greater than 1 will speed up the simulation
    /// (don't go crazy though, I find the maximum is 8 on my desktop machine).
    /// </summary>
    /// <param name="scale"></param>
    public void SetTimeScale(float scale) {
        Time.timeScale = scale;
    }
}
