using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SingleRunUIController : MonoBehaviour {
    private UIController uiController;

    // Whether visualized or not for single run
    public Toggle visualizeToggle;

    // Dropdown menu to select types of leaves
    public Dropdown leafDropdown;

    // The list to save the selected name
    private List<string> type = new List<string>();

    // The input field and slider to set the ratio
    public Slider inputRatioSlider;
    public Text inputRatioText;

    // Dictionary to save the type-ratio value pair
    private Dictionary<string, int> typeWithRatio;

    // Dictionary of leaf shapes and their ratio (used by LeafGenerator class)
    public static Dictionary<LeafData, int> leavesAndRatios;

    // Component for ListView of selected leaves 
    public GameObject leafButton;
    public Transform listContent;
    // Save the clicked leaf button for deletion 
    private LeafButton leafButtonClicked;

    // String to save the warning message
    private string message;

    // Use this for initialization
    void Start () {
        GameObject controller = GameObject.FindWithTag("MenuController");
        uiController = controller.GetComponent<UIController>();
        uiController.setSingleRunUIController(this);

        typeWithRatio = new Dictionary<string, int>();

        // Add the type to the dropdown menu
        InitializeLeafDropdown();

        // Tel main controller this one is ready
        uiController.singleReady = true;
    }

    // Read leaf name from database and add them to the dropdown menu
    private void InitializeLeafDropdown()
    {
        // Read leaf trait database
		DataImporter.ReadDatabase();

		foreach (LeafData l in DataImporter.Leaves)
        {
            type.Add(l.Name);
        }

        leafDropdown.options.Clear();
        Dropdown.OptionData tempData;
        for (int i = 0; i < type.Count; i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = type[i];
            leafDropdown.options.Add(tempData);
        }
        // Update the name show on the label of dropdown
        leafDropdown.captionText.text = type[0];
    }

    // Set the ratio of the slider
    public void UpdateRatio()
    {
        //inputRatioText = GetComponent<Text>();
        inputRatioText.text = Mathf.Round(inputRatioSlider.value).ToString();
    }

    /* 
     * The response of clicking add button.
     * Display the selected type with ratio 
     *      and add to the dictionary which just save the name and ratio.
     */
    public void ConfirmOnClick()
    {
        // The int number to save the ratio of each type
        int ratioInt = 0;
        // Type conversion, string to int
        if (System.Int32.TryParse(Mathf.Round(inputRatioSlider.value).ToString(), out ratioInt))
        {

            string typeString = leafDropdown.captionText.text;

            // Check if the same leaf type is selected
            if (typeWithRatio.ContainsKey(typeString))
            {
                message = "You have already chosen this type of leaf.\n" +
                    "Please check your selection.";
                uiController.DisplayMessage(message);
                return;
            }

            typeWithRatio.Add(typeString, ratioInt);

            // Add a leafButton
            GameObject newButton = Instantiate(leafButton) as GameObject;
            LeafButton button = newButton.GetComponent<LeafButton>();
            button.leafName.text = typeString;
            button.leafRatio.text = ratioInt.ToString();

            newButton.transform.SetParent(listContent);

            // Listen to the leaf button
            button.onClick.AddListener(
                    delegate ()
                    {
                        leafButtonClicked = button;
                        LeafButtonClick();
                    }
                );
        }
        else
        {
            message = "Please check the ratio.";
            uiController.DisplayMessage(message);
        }

    }

    // Click the buttion to delete the selection 
    private void LeafButtonClick()
    {
        message = "Are you sure you want to delete?";
        uiController.okButtonText.text = "Cancel";
        uiController.deleteButton.gameObject.SetActive(true);
        uiController.DisplayMessage(message);
    }

    // Confirm the deletion 
    public void DeleteOnClick()
    {
        uiController.okButtonText.text = "OK";
        uiController.deleteButton.gameObject.SetActive(false);
        uiController.messageBox.gameObject.SetActive(false);
        Destroy(leafButtonClicked.gameObject);
        typeWithRatio.Remove(leafButtonClicked.leafName.text);
    }
       
    // Clear all selected leaves
    public void ClearSelectedLeaves()
    {
        typeWithRatio.Clear();
        GameObject[] leafButtons = GameObject.FindGameObjectsWithTag("LeafButton");
        if (leafButtons.Length > 0)
        {
            foreach (GameObject o in leafButtons)
            {
                Destroy(o);
            }
        }
    }

    public void run()
    {
        // To pass the dictionary leavesAndRatios to the LeafGenerator
        // Get the LeafShap based on the leaf name
        GetLeafShape(typeWithRatio);
        SimSettings.SetBatchrun(false);
        if (leavesAndRatios.Count == 0)
        {
            message = "Please input leaves and ratios.";
            uiController.DisplayMessage(message);
            return;
        }

        SimSettings.SetSimulationTimes(1);
        SimSettings.ResetSimulationTimesLeft();

        SimSettings.SetLeafSizesAndRatios(leavesAndRatios);
		DatabaseOperator.RecordLeafTypeAndRatio (leavesAndRatios);
        // set visualize flag according to visualizeToggle's status
        SimSettings.SetVisualize(visualizeToggle.isOn);
        SceneManager.LoadScene("Simulation");
    }

    // Get the the selected LeafData according to the name and saved as an dictionary
    private void GetLeafShape(Dictionary<string, int> nameDictionary)
    {
        leavesAndRatios = new Dictionary<LeafData, int>();
        LeafData temp;

        foreach (KeyValuePair<string, int> pair in typeWithRatio)
        {
			temp = DataImporter.Leaves.Find((LeafData l) => l.Name == pair.Key);
            leavesAndRatios.Add(temp, pair.Value);
            Debug.Log(temp.Name + ":" + pair.Value);
        }
    }

    /*
     * The response of clicking reset button.
     * Reset all setting
     * Clear the dictionary typeWithRatio and selected leaf types
     */
    public void Reset()
    {
        typeWithRatio.Clear();
        GameObject[] leafButtons = GameObject.FindGameObjectsWithTag("LeafButton");
        if (leafButtons.Length > 0)
        {
            foreach (GameObject o in leafButtons)
            {
                Destroy(o);
            }
        }
    }
}
