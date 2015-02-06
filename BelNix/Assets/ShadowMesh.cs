using UnityEngine;
using System.Collections;

//[RequireComponent (typeof (MeshFilter), typeof (MeshRenderer))]
public class ShadowMesh : MonoBehaviour {
	[SerializeField] private GameObject playerUnit;
	[SerializeField] private int scalar = 1;


	// Use this for initialization
	void Start () {
		Vector3[] parentVerts = transform.parent.gameObject.GetComponent<MeshFilter>().mesh.vertices; 
		gameObject.AddComponent("MeshFilter");
		gameObject.AddComponent("MeshRenderer");

		Mesh newMesh = new Mesh();

		Vector3[] rootVerts = getRootVerts();
		Vector3[] tailVerts = getTailVerts();
		Vector3[] verts = new Vector3[] { rootVerts[0], rootVerts[1],	tailVerts[0], tailVerts[1] };
		int[] triangles = new int[] { 0, 1, 3,
									  2, 1, 3 };
		newMesh.Clear();
		newMesh.vertices = verts;
		newMesh.triangles = triangles;
		newMesh.uv = new Vector2[] {new Vector2(1,1), new Vector2(1,0), new Vector2(0,1), new Vector2(0,0)};

		gameObject.GetComponent<MeshFilter>().mesh = newMesh;
	}
	
	// Update is called once per frame
	void Update () {
		updatePolygon();
	}

	private void updatePolygon()
	{
		GetComponent<MeshFilter>().mesh.Clear();
		//Vector3[] rootVerts = getRootVerts();
		//Vector3[] tailVerts = getTailVerts();
		//Vector3[] verts = new Vector3[] {	rootVerts[0], rootVerts[1],
		//									tailVerts[0], tailVerts[1] };
		//Debug.Log(tailVerts);
		//GetComponent<MeshFilter>().mesh.vertices = verts;
	}

	// Attach the shadow to the two root coordinates using this method
	private Vector3[] getRootVerts()
	{
		Vector3[] parentVerts = transform.parent.gameObject.GetComponent<MeshFilter>().mesh.vertices; 
		Vector2 r1 = parentVerts[0];
		Vector2 r2 = parentVerts[parentVerts.Length - 1];
		return new Vector3[] {r1, r2};
	}

	private Vector3[] getTailVerts()
	{
		Vector3[] rootVerts = getRootVerts();
		Vector2 rootA = transform.TransformPoint(rootVerts[0]);
		Vector2 rootB = transform.TransformPoint(rootVerts[1]);

		Vector2 playerLocation = playerUnit.transform.position;

		Vector2 ray1 = rootA - playerLocation;
		Vector2 ray2 = rootB - playerLocation;

		Debug.Log(transform.InverseTransformPoint((playerLocation + ray1) * 5));

		return new Vector3[] { 	transform.InverseTransformPoint((playerLocation + ray1) * scalar),
								transform.InverseTransformPoint((playerLocation + ray2) * scalar)};
	}
}
