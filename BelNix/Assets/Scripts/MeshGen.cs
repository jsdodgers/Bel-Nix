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

//	static MeshFilter aiMesh;
	// Use this for initialization
	void Start ()  {
	/*	Debug.Log("Start");
		if (aiMesh == null) {
			Debug.Log("aiMesh is Null");
			GameObject aiMeshObj = GameObject.Find("AIMesh");
			aiMesh = aiMeshObj.GetComponent<MeshFilter>();
			Debug.Log("Is aiMesh null? " + (aiMesh==null));
		}*/
	//	createMesh();    
	}

	private void tangentSolver(Mesh theMesh)
	{
		int vertexCount = theMesh.vertexCount;
		Vector3[] vertices = theMesh.vertices;
		Vector3[] normals = theMesh.normals;
		Vector2[] texcoords = theMesh.uv;
		int[] triangles = theMesh.triangles;
		int triangleCount = triangles.Length / 3;
		Vector4[] tangents = new Vector4[vertexCount];
		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];
		int tri = 0;
		for (int i = 0; i < (triangleCount); i++)
		{
			int i1 = triangles[tri];
			int i2 = triangles[tri + 1];
			int i3 = triangles[tri + 2];
			
			Vector3 v1 = vertices[i1];
			Vector3 v2 = vertices[i2];
			Vector3 v3 = vertices[i3];
			
			Vector2 w1 = texcoords[i1];
			Vector2 w2 = texcoords[i2];
			Vector2 w3 = texcoords[i3];
			
			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;
			
			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;
			
			float r = 1.0f / (s1 * t2 - s2 * t1);
			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
			
			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;
			
			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
			
			tri += 3;
		}
		
		for (int i = 0; i < (vertexCount); i++)
		{
			Vector3 n = normals[i];
			Vector3 t = tan1[i];
			
			// Gram-Schmidt orthogonalize
			Vector3.OrthoNormalize(ref n, ref t);
			
			tangents[i].x = t.x;
			tangents[i].y = t.y;
			tangents[i].z = t.z;
			
			// Calculate handedness
			tangents[i].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0) ? -1.0f : 1.0f;
		}
		theMesh.tangents = tangents;
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
		Color[] colors = new Color[totalVerts];
		for (int n=0;n<totalVerts;n++)  {
			colors[n] = new Color(1.0f,0.0f,0.0f,0.5f);
		}
		
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
		newMesh.colors = colors;
		tangentSolver(newMesh);
	}

	
	public void createMesh(Vector2[] points, Vector2 origin, bool combine = false)  {
	/*	if (aiMesh == null) {
			Debug.Log("aiMesh is Null");
			GameObject aiMeshObj = GameObject.Find("AIMesh");
			aiMesh = aiMeshObj.GetComponent<MeshFilter>();
			Debug.Log("Is aiMesh null? " + (aiMesh==null));
		}*/
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
		Vector2[] UV2 = new Vector2[] {new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),new Vector2(1,0)};
		int[] tris = new int[(totalVerts-1)*3];
		for(int n=0;n<totalVerts;n++)  {
			norms[n] = Vector3.up;
			if (n == 0) verts[n] = new Vector3(origin.x, origin.y, -1.0f);
			else  {
				verts[n] = new Vector3(points[n-1].x, points[n-1].y, -1.0f);
				uvs[n] = UV2[n%4];
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
		//uvs[0] = new Vector2(0,0);
		
		
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
		GetComponent<MeshRenderer>().sortingOrder = MapGenerator.aiViewOrder;
		//assign mesh to filter/collider/renderer
		//		MeshCollider meshCollider = GetComponent<MeshCollider>();
		//		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		
		//assign data to mesh
		newMesh.vertices = verts;
		newMesh.uv = uvs;
		newMesh.normals = norms;
		newMesh.triangles = tris;
//		tangentSolver(newMesh);
		newMesh.RecalculateNormals();
		newMesh.RecalculateBounds();
	/*	if (combine) {
			List<MeshFilter> meshFilters = new List<MeshFilter>();
			MapGenerator mg = MapGenerator.mg;
			foreach (Enemy e in mg.enemies) {
				if (e.meshGen != null && e.meshGen.GetComponent<MeshFilter>().mesh != null) {
					meshFilters.Add(e.meshGen.GetComponent<MeshFilter>());
				}
			}
			CombineInstance[] combines = new CombineInstance[meshFilters.Count];
			for (int n=0;n<meshFilters.Count;n++) {
				combines[n].mesh = meshFilters[n].sharedMesh;
				combines[n].transform = meshFilters[n].transform.localToWorldMatrix;
			}
			if (aiMesh.mesh == null) {
				aiMesh.mesh = new Mesh();
			}
			aiMesh.sharedMesh.CombineMeshes(combines);
		}*/
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
