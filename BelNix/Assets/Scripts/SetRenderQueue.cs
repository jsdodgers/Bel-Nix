/*
	SetRenderQueue.cs
 
	Sets the RenderQueue of an object's materials on Awake. This will instance
	the materials, so the script won't interfere with other renderers that
	reference the same materials.
*/

using UnityEngine;

[AddComponentMenu("Rendering/SetRenderQueue")]

public class SetRenderQueue : MonoBehaviour {
	
	[SerializeField]
	protected int[] m_queues = new int[]{2000};
	
	protected void Awake() {
		setRendererQueue(renderer, m_queues);
	}

	public static void setRendererQueue(Renderer r, int[] queues) {
		Material[] materials = r.materials;
		for (int i = 0; i < materials.Length; ++i) {
			materials[i].renderQueue = queues[i%queues.Length];
		}
	}
}