/** Created by Ze Wang 
 * Unit test for the leaf object
**/

using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class LeafGeneratorTest {

	[Test]
	public void LeafGeneratorCreation() {
		LeafData ld1 = new LeafData ();
		LeafData ld2 = new LeafData ();
		Dictionary<LeafData, int> leafShapes = new Dictionary<LeafData, int>();
		leafShapes.Add(ld1, 200);
		leafShapes.Add(ld2, 100);
		LeafGenerator leafGen = new LeafGenerator(leafShapes, 50, 50, 100);
	}
//	[Test]
//	public void GetNextLeafTest() {
//		GameObject nextLeaf = null;
//		LeafData ld1 = new LeafData ();
//		Dictionary<LeafData, int> leafShapes = new Dictionary<LeafData, int>();
//		leafShapes.Add(ld1, 200);
//		LeafGenerator leafGen = new LeafGenerator(leafShapes, 50, 50, 100);
//		nextLeaf = leafGen.GetNextLeaf (false);
//
//	}
	[Test]
	public void GetRandomPointInDropAreaTest() {
		LeafData ld1 = new LeafData ();
		Dictionary<LeafData, int> leafShapes = new Dictionary<LeafData, int>();
		leafShapes.Add(ld1, 200);
		LeafGenerator leafGen = new LeafGenerator(leafShapes, 50, 50, 100);
		Vector3 v = leafGen.GetRandomPointInDropArea (50, 50, 100);
	}
	[Test]
	public void LeafGeneratorTestSimplePasses() {
		// Use the Assert class to test conditions.
	}

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator LeafGeneratorTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}
}
