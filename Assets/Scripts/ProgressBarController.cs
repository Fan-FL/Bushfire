/*
 * Created by Yanwei Li
 * Display the progress bar in every scene,
 * and save the relevant parameters
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour {

    // Create an instance to svae the UI and parameters
    public static ProgressBarController progressBar;

    // The filled image to show the progress
    public Image progressImg;

    // The text to show the percentage
    public Text proText;

    // The integer to contral the width of the progressImg;
    public int curProValue;

	// Use this for initialization
	void Start () {
        if (progressBar == null)
        {
            // Don't destroy the object to transer UI and parameter
            DontDestroyOnLoad(gameObject);
            progressBar = this;
        }
        else if (progressBar != null)
        {
            // Avoid create the instance twice
            Destroy(gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
