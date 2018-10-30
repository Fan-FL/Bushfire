/*
 * Create by Marko Ristic
 * Modified by Michael Lumley
 * Container class for a leaf shape, used to store leaves from leaf trait csv, and
 * used when directing simulation which leaves and ratios to use
 */

using UnityEngine;

public class LeafData {

    // Can get all instance variables
    public string Name { get; set; }
    public string LeafForm { get; set; }
    public float ThicknessMean { get; set; }
    public float ThicknessRange { get; set; }
    public float WidthMean { get; set; }
    public float WidthRange { get; set; }
    public float LengthMean { get; set; }
    public float LengthRange { get; set; }

    // Constructor just takes all variables
    public LeafData(string name,
                    string leafForm,
                    float thicknessMean, 
                    float thicknessRange, 
                    float widthMean,
                    float widthRange, 
                    float lengthMean, 
                    float lengthRange) {
        this.Name = name;
        this.LeafForm = leafForm;
        this.ThicknessMean = thicknessMean;
        this.ThicknessRange = thicknessRange;
        this.WidthMean = widthMean;
        this.WidthRange = widthRange;
        this.LengthMean = lengthMean;
        this.LengthRange = lengthRange;
    }

    // Empty contructor that creates default leaf
    public LeafData() {
        this.Name = "";
        this.LeafForm = "";
        this.ThicknessMean = 1;
        this.ThicknessRange = 0.5f;
        this.WidthMean = 1;
        this.WidthRange = 0.5f;
        this.LengthMean = 1;
        this.LengthRange = 0.5f;
    }

    // Given a leaf shape, returns a size of that leaf, taking the dimensions and their ranges into account
    public Vector3 GetConcreteLeafSize() {
        // Three dimensions of the leaf
        float thickness = Random.Range(this.ThicknessMean - this.ThicknessRange / 2,
                                        this.ThicknessMean + this.ThicknessRange / 2);
        float width = Random.Range(this.WidthMean - this.WidthRange / 2,
                                        this.WidthMean + this.WidthRange / 2);
        float length = Random.Range(this.LengthMean - this.LengthRange / 2,
                                        this.LengthMean + this.LengthRange / 2);

        // Return as a vector for simplicity
        return new Vector3(thickness, width, length);
    }
}
