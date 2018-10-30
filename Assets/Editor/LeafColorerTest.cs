/*
 * Created by Yudong Gao.
 * Unit Test for LeafColorer class.
 * 
 */

using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

public class LeafColorerTest {

	LeafColorer lc = new LeafColorer();

	/// <summary>
	/// Color is assigned, and equals to the first color "Green".
	/// </summary>
	[Test]
	public void ColorIsAssigned(){
		//List<Color> presetColor = lc.getPresetColors ();

		Color current_color = lc.GetColor (new LeafData());

		Assert.IsTrue (current_color.Equals (Color.green));
	}

	/// <summary>
	/// Color is removed from the presetColor list. 
	/// </summary>
	[Test]
	public void ColorIsRemoved(){
		
		List<Color> presetColor = lc.getPresetColors ();

		Color removed_color = lc.GetColor (new LeafData());

		Assert.IsFalse (presetColor.Contains (removed_color));
	}

	/// <summary>
	/// Run 10 rounds to use all colors in the presetColor list, 
	/// the presetColor therefore should be empty.
	/// </summary>
	[Test]
	public void presetColorEmpty(){
		List<Color> presetColor = lc.getPresetColors ();

		Color removed_color;

		// run out of the colors in list
		for (int i = 0; i < 9; i++){
			removed_color = lc.GetColor (new LeafData("ALeaf", "Flat", i, i, i, i, i, i));
		}

		Assert.IsTrue (presetColor.Count == 0);
	}

	/// <summary>
	/// The generated random color shouldn't be the same with anyone in the preset list. 
	/// </summary>
	[Test]
	public void randomColor(){
		List<Color> usedColor = new List<Color> ();

		Color removed_color;

		for (int i = 0; i < 9; i++){
			removed_color = lc.GetColor (new LeafData("ALeaf", "Flat", i, i, i, i, i, i));

			usedColor.Add (removed_color);
		}

		// this is a random color, it shouldn't be included in 'usedColor' list
		Color current_color = lc.GetColor (new LeafData());

		Assert.IsFalse (usedColor.Contains(current_color));
	}
}
