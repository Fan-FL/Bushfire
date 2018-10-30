/* Created by Yanwei Li
 * Unit test for LeafData
 */

using UnityEngine;
using NUnit.Framework;


public class LeafDataTest {

    // There should be a constructor of LeafData without parameters
    [Test]
    public void LeafDataConstructorTest() {
        LeafData leafDataNoPatams = new LeafData();
        Assert.NotNull(leafDataNoPatams);
    }


    // There should be a contructor of LeafData with parameters
    // Test set methods for parameters and get for Name
    [Test]
    public void LeafDataConstructorWithParamsTest()
    {
        LeafData leafData = new LeafData("Acacia Melanoxylon", "Flat", 0.21f, 0.1f, 18, 12, 100, 40);
        Assert.IsTrue(leafData.Name.Equals("Acacia Melanoxylon"));
    }

    // Test get WidthMean (all parameters are same, can be omitted)
    [Test]
    public void LeafDataSetAndGetTest()
    {
        LeafData leafData = new LeafData("Acacia Melanoxylon", "Flat", 0.21f, 0.1f, 18, 12, 100, 40);
        Assert.IsTrue(leafData.LengthRange.Equals(40f));
    }

    // Test the method to get the concrete leaf size
    [Test]
    public void GetConcreteSizeTest()
    {
        LeafData newLeaf = new LeafData();
        Vector3 vector = newLeaf.GetConcreteLeafSize();
        Assert.NotNull(vector);
    }

}
