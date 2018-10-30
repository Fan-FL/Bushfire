/* 
 * User interface for select:
 *      number of leaves to simulate
 *      single run & batch run
 *      select the type of leaf with ratio
 *      visualization
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Crosstales.FB;
using System.IO;

public class UIController : MonoBehaviour
{
    // One of these two toggles must be on but cannot be on at the same time    
    public Toggle batchrunToggle;
    public Toggle singlerunToggle;

    // Panels seperating batch run and single run
    public GameObject singleRunPanel;
    public GameObject batchRunPanel;

    // InputField on the canvas
    public InputField leafNumField;

    // Limit of leaf to be set
    private int leafNum;

    // The flag whether the user click the un limited button
    private bool isUnlimited;
   
    // Component for message box
    // String to save the warning message
    private string message;
    public Image messageBox;
    public Text messageBoxConent;
    public Text okButtonText;
    public Button deleteButton;

    private BatchRunUIController batchRunUIController;
    private SingleRunUIController singleRunUIController;

    // Flag for when both ui controller initialised properly
    public bool batchReady = false;
    public bool singleReady = false;
    // Flag for when commandline has been read
    private bool readCmdLn = false;

    public void setBatchRunUIController(BatchRunUIController batchRunUIController)
    {
        this.batchRunUIController = batchRunUIController;
    }

    public void setSingleRunUIController(SingleRunUIController singleRunUIController)
    {
        this.singleRunUIController = singleRunUIController;
    }


    // Initialisation
    private void Start()
    {     
        isUnlimited = false;

        messageBox.gameObject.SetActive(false);

        message = "";

        // Setting up which panel need to be default at the begining
        batchRunPanel.gameObject.SetActive(false);
        singleRunPanel.gameObject.SetActive(true);

        // Hide the progress bar canvas
        ProgressBarController.progressBar.gameObject.SetActive(false);

        // Set the default input value
        leafNumField.text = "5000";
      
        // Reset the progress bar 
        ProgressBarController.progressBar.curProValue = 0;
        ProgressBarController.progressBar.progressImg.fillAmount = 0;
        ProgressBarController.progressBar.proText.text = ProgressBarController.progressBar.curProValue + "%";

    }

    private void Update()
    {
        // Since cmdline can only read once all ui controllers have loaded, do it once (and only once) once they have
        if (singleReady && batchReady && !readCmdLn)
        {
            // Scan the command line parameters, and if a batch file was passed this way, read it and being simulation auotmatically
            RunBatchFromCommandLine();
            readCmdLn = true;
        }
        
    }

    /// <summary>
    /// Reads commandline arguments, search for a -batch flag and batch run filepath. If found, automatically begins a 
    /// batch run with the supplied batch file
    /// </summary>
    public void RunBatchFromCommandLine()
    {
        // Read the command line arguments, and search for -batch flag
        string[] args = Environment.GetCommandLineArgs();
        string batchFile = "";
        for (int i = 0; i < (args.Length - 1); i++)
        {
            // If found -batch flag, next argument should be the batch filepath, save it
            if (args[i] == "-batch")
            {
                batchFile = args[i + 1];
                break;
            }
        }

        // If no batch filepath was given, return without beginning cmdln batch run
        if (batchFile == "")
        {
            return;
            // If one was passed but the file doesn't exist, warn the user, and return without starting the batch run
        }
        else if (!File.Exists(batchFile))
        {
            DisplayMessage("Batch file passed via commandline does not exist!");
            return;
        }
        

        // If a batch filepath was supplied, try to load it with the batch run loader
        string errorMsg = "";
        // If failed, display error and don't start batch automatically
        if (BatchRunCsvLoader.LoadFile(batchFile, out errorMsg) != 0)
        {
            batchRunUIController.batchrunFileLoadSuccess = false;
            DisplayMessage(errorMsg);
        }
        // If succeeded, automatically set the batch toggle to true, the visualise toggle to false, and being the batch simulation
        else
        {
            batchRunUIController.batchrunFileLoadSuccess = true;
            batchrunToggle.isOn = true;
            singlerunToggle.isOn = false;

            // Mark that the program was run from the command line. This is checked when simulation complete, and exits the process
            // on completion if it is true (after writing results).
            SimSettings.SetWasRunWithFlags(true);

            // Begins the sim
            this.ChangeScene();
        }
    }
		
    // Invoke when Start button clicked
    public void StartOnClick()
    {
        // Actions to submit the number of leaves
        // Check if input leaf limit is valid
        if (System.Int32.TryParse(leafNumField.text, out leafNum))
        {
            // Check if inputed leaf number is greater than 0
            if (leafNum >= 0)
            {
                Debug.Log("You selected " + leafNum + " leafs.");
                SimSettings.SetLeafLimit(leafNum);

                ChangeScene();
            }
            else
            {
                Debug.Log("Invalid number.");

                message = "Invalid number. Please check the leaf quantity.";
                DisplayMessage(message);
            }
        }
        // Click the unlimited button, nothing in input field
        else if (isUnlimited == true)
        {

            ChangeScene();

        }
        else
        {
            Debug.Log("Invalid input.\n"
                + "Please check the leaf quantity. ");

            message = "Invalid number. Please check the leaf quantity.";
            DisplayMessage(message);
        }
    }

    //Batch run toggle click
    public void BatchRunToggleClick()
    {
        batchRunPanel.gameObject.SetActive(true);
        singleRunPanel.gameObject.SetActive(false);
    }

    //Single run toggle click
    public void SingleToggleClick()
    {
        batchRunPanel.gameObject.SetActive(false);
        singleRunPanel.gameObject.SetActive(true);
    }

    // Load the simulation
    private void ChangeScene()
    {
        // If single run toggle is choosen
        if (singlerunToggle.isOn)
        {
            singleRunUIController.run();
        }
        // If batch run toggle is choosen
        else if (batchrunToggle.isOn)
        {
            batchRunUIController.Run();
        }
    }

    // Actions when click unlimited button
    public void UnlimitedOnClick()
    {
        isUnlimited = true;
        SimSettings.RemoveLeafLimit();
        Debug.Log("Leaf limit set to unlimited.");
        leafNumField.text = "Set as Unlimited";
    }

    // Invoke when Quit button clicked
    public void QuitOnClick()
    {
        Debug.Log("quit");
        Application.Quit();
    }

    /************************
     * Message box part start
     * ********************
    **/
    // The method to disaplay the message box
    public void DisplayMessage(string str)
    {
        messageBox.gameObject.SetActive(true);
        // Bring the components to front
        messageBox.gameObject.transform.SetAsLastSibling();
        messageBoxConent.text = str;
    }

    // Cancel the deletion
    public void OkOnClick()
    {
        messageBox.gameObject.SetActive(false);
        Debug.Log("Click Cancel button");
    }
    /************************
     * Message box part end
     * *********************
    **/

    /*
     * The response of clicking reset button.
     * Reset all setting
     * Clear the dictionary typeWithRatio and the display text
     */
    public void ResetOnClick()
    {        
        leafNumField.text = "";
        isUnlimited = false;
        singleRunUIController.Reset();
    }

    // Clear the dictionary typeWithRatio and selected leaf types
    public void ResetSingleRun()
    {
        singleRunUIController.Reset();
    }
}
