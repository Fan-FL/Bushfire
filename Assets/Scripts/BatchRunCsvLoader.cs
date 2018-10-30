/* 
 * Load the csv file for a batch run
 */

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BatchRunCsvLoader
{
    // Holds leaves and ratios for each run. 
    // key: run round id, the first run round is 1. value: Dictionary of this run's leaves and ratios
    public static Dictionary<int, Dictionary<LeafData, int>> batchrunLeafAndRatio = new Dictionary<int, Dictionary<LeafData, int>>();

    // Load the csv file. 
    // Path: file path.
    // string: error message. Null if no error.
    // return: 0 for normal. -1 for error.
    public static int LoadFile(string path, out string errorMsg)    {
        batchrunLeafAndRatio.Clear();
        SimSettings.SetRunTimesLeft(0);
        // Holds leaf types (by loading the first row of csv)
        List<LeafData> leafType = new List<LeafData>();

        StreamReader reader = new StreamReader(path, System.Text.Encoding.Default, false);
        int lineNum = 0;
        try
        {
            // Read file line by line
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (!string.IsNullOrEmpty(line.Trim()))
                {
                    Debug.Log(line);
                    // Split columns by ',' 
                    string[] parts = line.Split(',');
                    int columnNum = 0;
                    // Leaves and ratios of a row
                    Dictionary<LeafData, int> leafAndRatio = new Dictionary<LeafData, int>();
                    foreach (string columnData in parts)
                    {
                        // Get leaf types from first row
                        if (lineNum == 0)
                        {
                            // get leaf object by name.
							LeafData shape = DataImporter.Leaves.Find((LeafData l) => l.Name == columnData);
                            if (shape == null || shape.Name == "")
                            {
                                errorMsg = "Cannot find leaf with name " + columnData;
                                batchrunLeafAndRatio.Clear();
                                return -1;
                            }
                            else
                            {
                                leafType.Add(shape);
                            }
                        }
                        // get ratios from following rows
                        else
                        {
                            int ratio = 0;
                            bool result = Int32.TryParse(columnData, out ratio);
                            if (!result)
                            {
                                errorMsg = "Cannot cast ratio: " + columnData;
                                batchrunLeafAndRatio.Clear();
                                return -1;
                            }
                            else
                            {
                                leafAndRatio.Add(leafType[columnNum], ratio);
                                Debug.Log(leafType[columnNum].Name + ":" + ratio);
                            }

                        }
                        columnNum++;
                    }

                    if (columnNum != leafType.Count)
                    {
                        errorMsg = "Ratio number can't match leaf type number.";
                        batchrunLeafAndRatio.Clear();
                        return -1;
                    }
                    if (lineNum > 0)
                    {
                        // add this row data to the whole batch run collection
                        batchrunLeafAndRatio.Add(lineNum, leafAndRatio);
                    }
                }
                lineNum++;
            }
        }
        catch (Exception e) { Debug.Log(e); }
        finally
        {
            reader.Close();
        }

        if (lineNum <= 1)
        {
            errorMsg = "Empty file or No ratio lines.";
            batchrunLeafAndRatio.Clear();
            return -1;
        }

		// Passing input data (LeafData and ratios) to DatabaseOperator
		Dictionary<LeafData, int> leafAndRatios;
		for (int i = 1; i <= batchrunLeafAndRatio.Keys.Count; i++) {
			batchrunLeafAndRatio.TryGetValue (i, out leafAndRatios);
			DatabaseOperator.RecordLeafTypeAndRatio (leafAndRatios);
		}
        // set runtimes
        SimSettings.SetRunTimesLeft(batchrunLeafAndRatio.Count);
        Debug.Log("GetRunTimes = " + SimSettings.GetRunTimeesLeft());            
        errorMsg = "";
        return 0;
    }    
}
