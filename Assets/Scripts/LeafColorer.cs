using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Get a colour for a given leaf type
/// </summary>
public class LeafColorer {

    private List<Color> presetColors = new List<Color>{Color.green, Color.red, Color.cyan, Color.yellow,
        Color.magenta, Color.blue,Color.gray, Color.white, Color.black };

    private Dictionary<LeafData, Color> leafColours = new Dictionary<LeafData, Color>();

    /// <summary>
    /// Return the colour assigned to a leaf type or
    /// assign a colour to a leaf type if one doesn't exist
    /// </summary>
    /// <param name="leafData">The leafData</param>
    /// <returns>The colour</returns>
    public Color GetColor(LeafData leafData) {
        bool colourAlreadyAssigned = leafColours.ContainsKey(leafData);

        // If colour not assigned and presets available
        if (!colourAlreadyAssigned && presetColors.Count > 0) {
            return this.SelectPresetColor(leafData);
        }
        // If colour not assigned and presets not available
        else if (!colourAlreadyAssigned && presetColors.Count == 0) {
            return this.GetRandomColor(leafData);
        }
        // Colour already assigned
        else {
            return leafColours[leafData];
        }
    }

    /// <summary>
    /// Return the first color in the list of preset colours
    /// and then remove it from the list so it is not selected
    /// again
    /// </summary>
    /// <param name="leafData">The leafData</param>
    /// <returns>The colour selected</returns>
    private Color SelectPresetColor(LeafData leafData) {
        Color selectedColor = presetColors[0];

        leafColours.Add(leafData, selectedColor);
        presetColors.Remove(selectedColor);

        return selectedColor;
    }

    /// <summary>
    /// Return a random colour that doesn't exist in
    /// leafColours
    /// </summary>
    /// <param name="leafData">The leafData</param>
    /// <returns>The colour</returns>
    private Color GetRandomColor(LeafData leafData) {
        while (true) {
            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);
            Color randomColor = new Color(r, g, b);

            if (!leafColours.ContainsValue(randomColor)) {
                leafColours.Add(leafData, randomColor);
                return randomColor;
            }
        }
    }

	public List<Color> getPresetColors(){
		return this.presetColors;
	}

}
