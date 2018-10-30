/**
 * Created by Chao Li
 * Unit test for data importer
 **/
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class CsvImporterTest {

    [Test]
    public void CsvImporterRead() {
		DataImporter.ReadDatabase();
		DataImporter.PrintLeaves();
    }
}
