/*
 * Created by Jing Bi
 * Class for database operation, 
 * including open, close, insert and read functions.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite; 
using System.Data;
using System;

public class DatabaseOperator {

	// Objects for connecting Sqlite database
	private static SqliteConnection sqlConn;
	private static SqliteCommand sqlCmd;
	private static SqliteDataReader dbReader;

	// List for saving LeafName, LeafId and their ratios
	private static List<string> leafNameList = new List<string>();
	private static List<int> leafTypeIdList = new List<int>();
	private static List<int> leafRatioList = new List<int>();

	// Connect and open database
	public static void ConnAndOpenDB(string dbPath){
		
		try{
			sqlConn = new SqliteConnection(dbPath);
			sqlConn.Open();
		}
		catch(Exception e){
			Debug.Log ("Failed to open database. \n" + e.ToString ());
		}
	}

	// Close database connection
	public static void CloseConnection(){

		if (sqlCmd != null) {
			sqlCmd.Cancel();
			sqlCmd = null;
		}

		if (dbReader != null) {
			dbReader.Close();
			dbReader = null;
		}

		if (sqlConn != null) {
			sqlConn.Close();
			sqlConn = null;
		}
	}

	// Execute the query 
	public static SqliteDataReader ExecuteQuery(string query){

		sqlCmd = sqlConn.CreateCommand();
		sqlCmd.CommandText = query;
		dbReader = sqlCmd.ExecuteReader();
		return dbReader;
	}

	// Read leaf traits from database
	public static SqliteDataReader ReadLeafTraits(string tableName){

		string query = "SELECT name, leafForm, thickness, thicknessRange, width, " +
						"widthRange, len, lenRange From " + tableName;
		return ExecuteQuery (query);
	}

	// Insert values into table ResultOut
	public static void InsValToResultsOut(string tableName, 
									List<float> aveList, 
									List<double> staDevList, 
									List<float> medList, 
									int numOfRuns){

		Debug.Log("Writing results to database ... \n");

		for (int i = 0; i < aveList.Count; i++) {
			//Form query "Insert into ResultOut 
			//(averageDensity, stddevDensity, median, numbersRuns) VALUES
			//(X.xxx, Y.yyy, Z.zzz, A.aaa)"
			string query = "INSERT INTO " +
			               tableName +
			               "(averageDensity, stddevDensity, median, numbersRuns) VALUES (" +
			               aveList[i] + ", " +
						   staDevList[i] + ", " +
						   medList[i] + ", " +
						   numOfRuns + ")";

			ExecuteQuery (query);
		}
	}

	// Insert val into table RatioMap
	public static void InsValToRatioMap(string tableName, int startId){

		int resultId = startId + 1;
		for (int i = 0; i < leafTypeIdList.Count; i++) {

			if ((leafTypeIdList [0] == leafTypeIdList [i])&& i != 0)
				resultId += 1;

			string query = "INSERT INTO " +
				tableName +
				" VALUES (" +
				leafTypeIdList [i] + "," +
				resultId + "," +
				leafRatioList [i] + ")";

			ExecuteQuery (query);
		}
	}

	// Record the input leafData and ratio
	public static void RecordLeafTypeAndRatio(Dictionary<LeafData, int> leafAndRatio){

		foreach (LeafData leaf in leafAndRatio.Keys) {
			
			//Record each leaf name and ratio
			leafNameList.Add(leaf.Name);
			leafRatioList.Add (leafAndRatio [leaf]);
		}
	}

	// Get leaf id by their names
	public static void GetLeafIdFromLeafType(){

		foreach (string leafName in leafNameList) {
			// Form query :"SELECT LeafTypeId FROM LeafType WHERE name = 'x' ";
			string query = "SELECT LeafTypeId FROM LeafType WHERE name = '" + leafName + "'";
			// Transfer the name into corresponding id
			leafTypeIdList.Add(Convert.ToInt32(ExecuteQuery(query)[0]));
		}
	}
		
	// Get the last resultOutId 
	public static int GetLastIdFromResultOut(){
		string query = "SELECT resultOutId FROM ResultOut ORDER BY resultOutId DESC LIMIT 0,1";
		SqliteDataReader reader = ExecuteQuery (query);
		// Check if there is no record in database
		if (reader.HasRows)
			return Convert.ToInt32(reader[0]);
		else
			return 0;
	}

	// Clear all lists
	public static void Clear(){
		
		leafTypeIdList.Clear();
		leafNameList.Clear();
		leafRatioList.Clear();
	}
}

