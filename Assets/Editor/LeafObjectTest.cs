/** Created by Ze Wang 
 * Unit test for the leaf object
**/
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class LeafObjectTest {
	
	[Test]
	public void FlatLeafSetAndGetName() {
		//Arrage
		GameObject gm = GameObject.Instantiate((GameObject)Resources.Load("FlatLeaf"), new Vector3(0,10,0), Quaternion.Euler(0,0,0));
		string leafName="";
		//Act
		gm.GetComponent<Leaf>().SetName("FlatLeafName");
		leafName = gm.GetComponent<Leaf> ().GetName ();
		//Assert
		Assert.AreEqual(leafName, "FlatLeafName");
	}

	[Test]
	public void RoundLeafSetAndGetName() {
		//Arrage
		GameObject gm = GameObject.Instantiate((GameObject)Resources.Load("RoundLeaf"), new Vector3(0,10,0), Quaternion.Euler(0,0,0));
		string leafName="";
		//Act
		gm.GetComponent<Leaf>().SetName("RoundLeafName");
		leafName = gm.GetComponent<Leaf> ().GetName ();
		//Assert
		Assert.AreEqual(leafName, "RoundLeafName");
	}

	[Test]
	public void FlatLeafSetAndGetSize() {
		GameObject gm = GameObject.Instantiate((GameObject)Resources.Load("FlatLeaf"), new Vector3(0,10,0), Quaternion.Euler(0,0,0));
		gm.GetComponent<Leaf> ().SetSize (0, 10, 0);
		Vector3 v = new Vector3 (10, 0, 0);
		Assert.AreEqual (v, gm.GetComponent<Leaf> ().GetSize ());

	}
	[Test]
	public void RoundLeafSetAndGetSize() {
		GameObject gm = GameObject.Instantiate((GameObject)Resources.Load("RoundLeaf"), new Vector3(0,10,0), Quaternion.Euler(0,0,0));
		gm.GetComponent<Leaf> ().SetSize (0, 10, 0);
		Vector3 v = new Vector3 (0, 10, 0);
		Assert.AreEqual (v, gm.GetComponent<Leaf> ().GetSize ());

	}

    [Test]
    public void LeafCheckIfMoving() {
        GameObject gm = GameObject.Instantiate((GameObject)Resources.Load("FlatLeaf"), new Vector3(0, 10, 0), Quaternion.Euler(0, 0, 0));
        Leaf leaf = gm.GetComponent<Leaf>();

        float speed = 0.5f;
        float angularVelocity = 0.5f;
        bool result = leaf.IsMoving(speed, angularVelocity);

        Assert.IsTrue(result);

    }

    [Test]
    public void LeafCheckIfNotMoving() {
        GameObject gm = GameObject.Instantiate((GameObject)Resources.Load("FlatLeaf"), new Vector3(0, 10, 0), Quaternion.Euler(0, 0, 0));
        Leaf leaf = gm.GetComponent<Leaf>();

        float speed = 0.4f;
        float angularVelocity = 0.4f;
        bool result = leaf.IsMoving(speed, angularVelocity);

        Assert.IsFalse(result);

    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
	public IEnumerator LeafObjectTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}
}
