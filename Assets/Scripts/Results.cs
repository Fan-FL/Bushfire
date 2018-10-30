/*
 * Created by Marko Ristic.
 * Modified by Yudong Gao.
 * Class to store static variables between simulation and output scenes.
 * also for other error calculations
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;

public class Results {

	private static float average;

	private static double standard_deviation;

	private static float median;

    // store results for batch run, each element is a single run result of average
    private static List<float> batchrunAve = new List<float>();

	// store results for batch run strandard deviation
	private static List<double> batchrunStaDev = new List<double>();

	// store results for batch run median
	private static List<float> batchrunMed = new List<float>();

    /// <summary>
    /// a resultset used to store all density results
    /// </summary>
    private static List<float> resultset = new List<float>();

	/// <summary>
	/// Gets the value of average density.
	/// </summary>
	/// <returns>The average density(float).</returns>
	public static float GetAverage(){
		return average;
	}

	/// <summary>
	/// Calculate the average density,
	/// then set it to the static variable "average".
	/// </summary>
	public static void SetAverage(){
		float sum = 0;

		for (int i = 0; i < resultset.Count; i++) 
		{
			sum += resultset [i];
		}

		average = sum / resultset.Count;

        // Add this average to batch run result
		batchrunAve.Add(average);
    }

	/// <summary>
	/// Gets the Standard Deviation.
	/// </summary>
	/// <returns>The Standard Deviation(double).</returns>
	public static double GetSD(){
		return standard_deviation;
	}

	/// <summary>
	/// Calculate the Standard Deviation,
	/// then set it to the static variable "standard_deviation".
	/// </summary>
	public static void SetSD(){
		double sum = 0;

		foreach (int i in resultset) {
			// SD = Sqrt( ∑(xi - avg)^2 / n )
			sum = sum + Math.Pow((i - average), 2);
		}

		// calculate the standard deviation 
		// then set the value
		standard_deviation = System.Math.Sqrt (sum / resultset.Count);
		batchrunStaDev.Add (standard_deviation);
	}

	/// <summary>
	/// Gets the median.
	/// </summary>
	/// <returns>The median(float).</returns>
	public static float GetMedian(){
		return median;
	}

	/// <summary>
	/// Sets the median, then set it to the static variable "median".
	/// </summary>
	public static void SetMedian(){
		// index for 
		int index;
		// sort the result array
		resultset.Sort();

		// get the correct index in accordance with the length of the result array
		if (resultset.Count % 2 == 0) {
			index = resultset.Count / 2 - 1;
		} else {
			index = (resultset.Count - 1) / 2;
		}

		// set the value of median
		median = resultset[index];
		batchrunMed.Add (median);
	}

	/// <summary>
	/// Adds the result to the "resultset" list
	/// </summary>
	/// <param name="rs">one density result(float)</param>
	public static void addResult(float rs){
		resultset.Add (rs);
	}

    // Clear resultset
    public static void ClearResultSet()
    {
        resultset.Clear();
    }

    // Get the list of batch run average
    public static List<float> GetBatchRunAve()
    {
		return batchrunAve;
    }

	// Get the list of batch run standard deviation
	public static List<double> GetBatchRunStaDev()
	{
		return batchrunStaDev;
	}

	// Get the list of batch run standard median
	public static List<float> GetBatchRunMed()
	{
		return batchrunMed;
	}
}
