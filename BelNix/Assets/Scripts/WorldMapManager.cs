using UnityEngine;
using System.Collections;

public class WorldMapManager : MonoBehaviour {

	public void loadScene(int sceneNumber)
	{
		Application.LoadLevel(sceneNumber);
	}
}
