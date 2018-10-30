using Crosstales.FB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BatchRunUIController : MonoBehaviour {
    // Indicate whether loading batch run file successfully
    public bool batchrunFileLoadSuccess = false;

    // InputField for simulation times of mulitrun with same parameters
    public InputField simulationTimesField;

    // Simulation times for multirun
    private int SimulationTimes;

    // String to save the warning message
    private string message;

    private UIController uiController;

    // Use this for initialization
    void Start () {
        GameObject controller = GameObject.FindWithTag("MenuController");
        uiController = controller.GetComponent<UIController>();
        uiController.setBatchRunUIController(this);
        // Set the default input value
        simulationTimesField.text = "10";

        // Tel main controller this one is ready
        uiController.batchReady = true;
    }

    // Click the button to choose the file
    public void LoadBatchRunCsvClick()
    {
        uiController.ResetSingleRun();
        string extensions = "csv";
        string path = FileBrowser.OpenSingleFile("Open File", "", extensions);
        Debug.Log("Selected file: " + path);
        uiController.batchrunToggle.isOn = true;
        // Click cancel or didn't choose file
        if (path == "")
        {
            batchrunFileLoadSuccess = false;
            return;
        }
        string errorMsg = "";
        if (BatchRunCsvLoader.LoadFile(path, out errorMsg) != 0)
        {
            batchrunFileLoadSuccess = false;
            uiController.DisplayMessage(errorMsg);
        }
        else
        {
            batchrunFileLoadSuccess = true;
        }
    }

    // Simulate several times with the same ratios
    public void MultiRun()
    {
        if (!System.Int32.TryParse(simulationTimesField.text, out SimulationTimes))
        {
            message = "Invalid simulation number. Please enter an interger.";
            uiController.DisplayMessage(message);
            return;
        }
        else
        {
            SimSettings.SetSimulationTimes(SimulationTimes);
            SimSettings.ResetSimulationTimesLeft();
        }
    }

    public void OnBatchrunToggleChanged(bool check)
    {
        uiController.ResetSingleRun();
    }

    public void Run()
    {
        uiController.ResetSingleRun();

        MultiRun();

        if (!batchrunFileLoadSuccess)
        {
            message = "Load batch run data error.";
            uiController.DisplayMessage(message);
            return;
        }
        SimSettings.SetVisualize(false);
        SimSettings.SetBatchrun(true);
        int runRound = BatchRunCsvLoader.batchrunLeafAndRatio.Keys.Count - SimSettings.GetRunTimeesLeft() + 1;
        Debug.Log("current round = " + runRound);
        Dictionary<LeafData, int> leafSizesAndRatios;
        BatchRunCsvLoader.batchrunLeafAndRatio.TryGetValue(runRound, out leafSizesAndRatios);
        SimSettings.SetLeafSizesAndRatios(leafSizesAndRatios);
        SceneManager.LoadScene("Simulation");
    }
}
