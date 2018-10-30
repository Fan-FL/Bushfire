using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulationUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    // Method called when return to menu button pressed on simulation UI
    public void ChangeLeafSettings()
    {
        SceneManager.LoadScene("Menu");
    }
}
