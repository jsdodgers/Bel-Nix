using UnityEngine;
using System.Collections;

public class CharacterColors {
	public Color characterColor = Color.white;
	public Color headColor = Color.white;
	public Color primaryColor = Color.white;
	public Color secondaryColor = Color.white;

	public CharacterColors(Color characterColor, Color headColor, Color primaryColor, Color secondaryColor) {
		this.characterColor = characterColor;
		this.headColor = headColor;
		this.primaryColor = primaryColor;
		this.secondaryColor = secondaryColor;
	}
}
