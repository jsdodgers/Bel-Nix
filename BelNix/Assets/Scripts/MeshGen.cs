using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MeshGen : MonoBehaviour  {
	
	// Use this for initialization
	//size of mesh in tiles
//	public int sizeX = 2;
//	public int sizeZ = 2;
	
	//size of each tile
//	public float tileSize = 1.0f;
	
	// Use this for initialization
	void Start ()  {
	//	createMesh();    
	}
	
	void createMesh()  {
		//declare mesh variables
	//	int totalTiles = sizeX * sizeZ;
	//	int totalTris = totalTiles * 2;
		//vertices, uvs, and normals
	//	int numVertsX = sizeX + 1;
	//	int numVertsZ = sizeZ + 1;
		int totalVerts = 5;//7;//numVertsX * numVertsZ;
		int totalTris = totalVerts - 2;	
		Vector3[] verts = new Vector3[]  {new Vector3(17.0f, -35.0f, -1.0f), new Vector3(18.0f, -35.0f, -1.0f), new Vector3(20.0f, -36.0f, -1.0f), new Vector3(19.0f, -38.0f, -1.0f), new Vector3(16.0f, -38.0f, -1.0f)};//, new Vector3(28.0f, -40.0f, -1.0f), new Vector3(29.0f, -38.0f, -1.0f) };
		//Vector3[] verts = new Vector3[]  {new Vector3(28.0f, -35.0f, -1.0f), new Vector3(27.0f, -35.0f, -1.0f), new Vector3(26.0f, -38.0f, -1.0f), new Vector3(27.0f, -39.0f, -1.0f)};//, new Vector3(27.0f, -40.0f, -1.0f), new Vector3(28.0f, -40.0f, -1.0f), new Vector3(29.0f, -38.0f, -1.0f) };
		//	Vector3[] verts = new Vector3[]  {new Vector3(1.0f, -1.0f, -1.0f), new Vector3(4.0f, -1.0f, -1.0f), new Vector3(4.0f, -4.0f, -1.0f), new Vector3(1.0f, -4.0f, -1.0f) };
		Vector3[] norms = new Vector3[totalVerts];
		Vector2[] uvs = new Vector2[totalVerts];
		Color[] colors = new Color[totalVerts];
		for (int n=0;n<totalVerts;n++)  {
			colors[n] = Color.clear;
		}
		
		//triangles
		int[] tris = new int[totalTris * 3];
		
		//generate mesh data
		//vertices
	/*	for(int z = 0; z < numVertsZ; z++)  {
			for(int x = 0; x < numVertsX; x++)
			 {
				verts[z * numVertsX + x] = new Vector3(x * tileSize, z * tileSize, -1.0f);
				norms[z * numVertsX + x] = Vector3.up;
			}
		}
		*/
		for(int n=0;n<totalVerts;n++)  {
			norms[n] = Vector3.up;
		}
		//uvs
		//no texture, just a simple red material
		uvs[0] = new Vector2(0,0);
		
		
		//triangles
/*		int i = 0;
		for(int z = 0; z < sizeZ; z++)  {
			i = i + z;
			
			for(int x = 0; x < sizeX; x++)
			 {
				i = i + x;
				
				tris[i] = (x * numVertsX) + z + 0;
				tris[++i] = (x * numVertsX) + z + 3;
				tris[++i] = (x * numVertsX) + z + 4;
				
				tris[++i] = (x * numVertsX) + z + 4;
				tris[++i] = (x * numVertsX) + z + 1;
				tris[++i] = (x * numVertsX) + z + 0;
				
			}
			
		}*/
		for (int n=0;n<totalTris;n++)  {
			tris[n*3] = 0;
			tris[n*3+1] = n+1;
			tris[n*3+2] = n+2;
			Debug.Log(n);
		}
		
		//create a new mesh
		Mesh newMesh = new Mesh();
		
		//assign mesh to filter/collider/renderer
		MeshCollider meshCollider = GetComponent<MeshCollider>();
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		meshFilter.mesh = newMesh;
		
		//assign data to mesh
		newMesh.vertices = verts;
		newMesh.uv = uvs;
		newMesh.normals = norms;
		newMesh.triangles = tris;
		newMesh.colors = colors;
		
	}

	
	public void createMesh(Vector2[] points, bool print = false)  {
		//declare mesh variables
		//	int totalTiles = sizeX * sizeZ;
		//	int totalTris = totalTiles * 2;
		//vertices, uvs, and normals
		//	int numVertsX = sizeX + 1;
		//	int numVertsZ = sizeZ + 1;
		int totalVerts = points.Length;//7;//numVertsX * numVertsZ;
		//		int totalTris = totalVerts - 2;	
		//		Vector3[] verts = new Vector3[]  {new Vector3(17.0f, -35.0f, -1.0f), new Vector3(18.0f, -35.0f, -1.0f), new Vector3(20.0f, -36.0f, -1.0f), new Vector3(19.0f, -38.0f, -1.0f), new Vector3(16.0f, -38.0f, -1.0f)};//, new Vector3(28.0f, -40.0f, -1.0f), new Vector3(29.0f, -38.0f, -1.0f) };
		//Vector3[] verts = new Vector3[]  {new Vector3(28.0f, -35.0f, -1.0f), new Vector3(27.0f, -35.0f, -1.0f), new Vector3(26.0f, -38.0f, -1.0f), new Vector3(27.0f, -39.0f, -1.0f)};//, new Vector3(27.0f, -40.0f, -1.0f), new Vector3(28.0f, -40.0f, -1.0f), new Vector3(29.0f, -38.0f, -1.0f) };
		//	Vector3[] verts = new Vector3[]  {new Vector3(1.0f, -1.0f, -1.0f), new Vector3(4.0f, -1.0f, -1.0f), new Vector3(4.0f, -4.0f, -1.0f), new Vector3(1.0f, -4.0f, -1.0f) };
		Vector3[] verts = new Vector3[totalVerts];
		Vector3[] norms = new Vector3[totalVerts];
		Vector2[] uvs = new Vector2[totalVerts];
		//		Color[] colors = new Color[totalVerts];
		//		for (int n=0;n<totalVerts;n++)  {
		//			colors[n] = Color.clear;
		//		}
		
		//triangles
		//		int[] tris = new int[totalTris * 3];
		
		//generate mesh data
		//vertices
		/*	for(int z = 0; z < numVertsZ; z++)  {
			for(int x = 0; x < numVertsX; x++)
			 {
				verts[z * numVertsX + x] = new Vector3(x * tileSize, z * tileSize, -1.0f);
				norms[z * numVertsX + x] = Vector3.up;
			}
		}
		*/
		for(int n=0;n<totalVerts;n++)  {
			norms[n] = Vector3.up;
			verts[n] = new Vector3(points[n].x, points[n].y, -1.0f);
		}
		//uvs
		//no texture, just a simple red material
		uvs[0] = new Vector2(0,0);
		
		
		//triangles
		/*		int i = 0;
		for(int z = 0; z < sizeZ; z++)  {
			i = i + z;
			
			for(int x = 0; x < sizeX; x++)
			 {
				i = i + x;
				
				tris[i] = (x * numVertsX) + z + 0;
				tris[++i] = (x * numVertsX) + z + 3;
				tris[++i] = (x * numVertsX) + z + 4;
				
				tris[++i] = (x * numVertsX) + z + 4;
				tris[++i] = (x * numVertsX) + z + 1;
				tris[++i] = (x * numVertsX) + z + 0;
				
			}
			
		}*/
		/*	for (int n=0;n<totalTris;n++)  {
			tris[n*3] = 0;
			tris[n*3+1] = n+1;
			tris[n*3+2] = n+2;
			Debug.Log(n);
		}
	*/
		List<int> triangles = new List<int>();
		List<int> indices = new List<int>();
		List<Vector2> pointsList = new List<Vector2>();
		for (int n=0;n<totalVerts;n++)  {
			indices.Add(n);
			pointsList.Add(points[n]);
		}
		int max = 5000;
		while (pointsList.Count > 2 && max > 0)  {
			max--;
			if (rightTurn(pointsList[0], pointsList[1], pointsList[2], print))  {
				triangles.Add(indices[0]);
				triangles.Add(indices[1]);
				triangles.Add(indices[2]);
				pointsList.RemoveAt(1);
				indices.RemoveAt(1);
			}
			else  {
				Vector2 p = pointsList[0];
				int i = indices[0];
				pointsList.RemoveAt(0);
				indices.RemoveAt(0);
				//		pointsList.Add(p);
				//		indices.Add(i);
			}
		}
		Debug.Log("MeshThingy max: " + max + "   Points Length: " + pointsList.Count);
		int[] tris = triangles.ToArray();
		//create a new mesh
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh newMesh = meshFilter.mesh;
		if (newMesh == null)  {
			newMesh = new Mesh();
			meshFilter.mesh = newMesh;
		}
		//assign mesh to filter/collider/renderer
		//		MeshCollider meshCollider = GetComponent<MeshCollider>();
		//		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		
		//assign data to mesh
		newMesh.vertices = verts;
		newMesh.uv = uvs;
		newMesh.normals = norms;
		newMesh.triangles = tris;
		//		newMesh.colors = colors;
	}

	
	public void createMesh(Vector2[] points, Vector2 origin, bool print = false)  {
		//declare mesh variables
		//	int totalTiles = sizeX * sizeZ;
		//	int totalTris = totalTiles * 2;
		//vertices, uvs, and normals
		//	int numVertsX = sizeX + 1;
		//	int numVertsZ = sizeZ + 1;
		int totalVerts = points.Length+1;//7;//numVertsX * numVertsZ;
		//		int totalTris = totalVerts - 2;	
		//		Vector3[] verts = new Vector3[]  {new Vector3(17.0f, -35.0f, -1.0f), new Vector3(18.0f, -35.0f, -1.0f), new Vector3(20.0f, -36.0f, -1.0f), new Vector3(19.0f, -38.0f, -1.0f), new Vector3(16.0f, -38.0f, -1.0f)};//, new Vector3(28.0f, -40.0f, -1.0f), new Vector3(29.0f, -38.0f, -1.0f) };
		//Vector3[] verts = new Vector3[]  {new Vector3(28.0f, -35.0f, -1.0f), new Vector3(27.0f, -35.0f, -1.0f), new Vector3(26.0f, -38.0f, -1.0f), new Vector3(27.0f, -39.0f, -1.0f)};//, new Vector3(27.0f, -40.0f, -1.0f), new Vector3(28.0f, -40.0f, -1.0f), new Vector3(29.0f, -38.0f, -1.0f) };
		//	Vector3[] verts = new Vector3[]  {new Vector3(1.0f, -1.0f, -1.0f), new Vector3(4.0f, -1.0f, -1.0f), new Vector3(4.0f, -4.0f, -1.0f), new Vector3(1.0f, -4.0f, -1.0f) };
		Vector3[] verts = new Vector3[totalVerts];
		Vector3[] norms = new Vector3[totalVerts];
		Vector2[] uvs = new Vector2[totalVerts];
		//		Color[] colors = new Color[totalVerts];
		//		for (int n=0;n<totalVerts;n++)  {
		//			colors[n] = Color.clear;
		//		}
		
		//triangles
		//		int[] tris = new int[totalTris * 3];
		
		//generate mesh data
		//vertices
		/*	for(int z = 0; z < numVertsZ; z++)  {
			for(int x = 0; x < numVertsX; x++)
			 {
				verts[z * numVertsX + x] = new Vector3(x * tileSize, z * tileSize, -1.0f);
				norms[z * numVertsX + x] = Vector3.up;
			}
		}
		*/
		int[] tris = new int[(totalVerts-1)*3];
		for(int n=0;n<totalVerts;n++)  {
			norms[n] = Vector3.up;
			if (n == 0) verts[n] = new Vector3(origin.x, origin.y, -1.0f);
			else  {
				verts[n] = new Vector3(points[n-1].x, points[n-1].y, -1.0f);
				int tri = (n-1)*3;
				int m = n+1;
				if (m >= totalVerts) m = 1;
				tris[tri] = 0;
				tris[tri+1] = n;
				tris[tri+2] = m;
			}
		}
		//uvs
		//no texture, just a simple red material
		uvs[0] = new Vector2(0,0);
		
		
		//triangles
		/*		int i = 0;
		for(int z = 0; z < sizeZ; z++)  {
			i = i + z;
			
			for(int x = 0; x < sizeX; x++)
			 {
				i = i + x;
				
				tris[i] = (x * numVertsX) + z + 0;
				tris[++i] = (x * numVertsX) + z + 3;
				tris[++i] = (x * numVertsX) + z + 4;
				
				tris[++i] = (x * numVertsX) + z + 4;
				tris[++i] = (x * numVertsX) + z + 1;
				tris[++i] = (x * numVertsX) + z + 0;
				
			}
			
		}*/
		/*	for (int n=0;n<totalTris;n++)  {
			tris[n*3] = 0;
			tris[n*3+1] = n+1;
			tris[n*3+2] = n+2;
			Debug.Log(n);
		}
	*/
	/*	List<int> triangles = new List<int>();
		List<int> indices = new List<int>();
		List<Vector2> pointsList = new List<Vector2>();
		for (int n=0;n<totalVerts;n++)  {
			indices.Add(n);
			pointsList.Add(points[n]);
		}
		int max = 5000;
		while (pointsList.Count > 2 && max > 0)  {
			max--;
			if (rightTurn(pointsList[0], pointsList[1], pointsList[2], print))  {
				triangles.Add(indices[0]);
				triangles.Add(indices[1]);
				triangles.Add(indices[2]);
				pointsList.RemoveAt(1);
				indices.RemoveAt(1);
			}
			else  {
				Vector2 p = pointsList[0];
				int i = indices[0];
				pointsList.RemoveAt(0);
				indices.RemoveAt(0);
				//		pointsList.Add(p);
				//		indices.Add(i);
			}
		}*/
//		Debug.Log("MeshThingy max: " + max + "   Points Length: " + pointsList.Count);
//		int[] tris = triangles.ToArray();
		//create a new mesh
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh newMesh = meshFilter.mesh;
		if (newMesh == null)  {
			newMesh = new Mesh();
			meshFilter.mesh = newMesh;
		}
		//assign mesh to filter/collider/renderer
		//		MeshCollider meshCollider = GetComponent<MeshCollider>();
		//		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		
		//assign data to mesh
		newMesh.vertices = verts;
		newMesh.uv = uvs;
		newMesh.normals = norms;
		newMesh.triangles = tris;
		newMesh.RecalculateBounds();
		//		newMesh.colors = colors;
	}


	static bool printed = false;
	bool rightTurn(Vector2 p, Vector2 q, Vector2 r, bool print = false)	 {
		float val = (q.y - p.y) * (r.x - q.x) -
			(q.x - p.x) * (r.y - q.y);
		if (!printed || print)  {
			Debug.Log("Val: " + p + "  " + q + "  " + r + "  " + val);
			printed = true;
		}
//		return val >= 0.05f;
//		return val >= .0001f || (val <= 0.0f && val >= -0.00002f);
		return val > Mathf.Epsilon;
//		if (val == 0) return 0;  // colinear
//		return (val > 0)? 1: 2; // clock or counterclock wise
	}
}
