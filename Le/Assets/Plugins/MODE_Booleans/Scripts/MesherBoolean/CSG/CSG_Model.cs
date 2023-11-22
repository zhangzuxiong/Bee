using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace MeshMakerNamespace
{
	class CSG_Model
	{
		public List<CSG_Vertex> vertices;
		public List<int> indices;
		public static List<List<int>> submeshIndices;
		public static bool keepSubmeshes = true;

		public CSG_Model()
		{
			this.vertices = new List<CSG_Vertex>();
			this.indices = new List<int>();
		}

		/**
		 * Initialize a CSG_Model with the mesh of a gameObject.
		 */
		public CSG_Model(GameObject go, bool reverseNormals, bool useCustomMaterial)
		{
			this.vertices = new List<CSG_Vertex>();
			this.indices = new List<int>();
			
			Mesh m = go.GetComponent<MeshFilter>().sharedMesh;
			Transform trans = go.GetComponent<Transform>();

			int vertexCount = m.vertexCount;
			
			Vector3[] v = m.vertices;
			Vector3[] n = m.normals;
			int[] t = m.triangles;

			if (reverseNormals == true)
			{
				for (int i = 0; i < n.Length; i++)
				{
					n[i] = -n[i];
				}
			}

			Vector2[] u = m.uv;
			Color[] c = m.colors;
			int count = 0;
			
			if (useCustomMaterial == true)
			{
				for (int i = 0; i < t.Length; i++)
				{
					vertices.Add( new CSG_Vertex(trans.TransformPoint(v[t[i]]),
						trans.TransformDirection(n[t[i]]),
						u == null ? Vector2.zero : u[t[i]],
						c == null || c.Length != vertexCount ? Color.white : c[t[i]],
						CSG_Model.submeshIndices.Count + 1));
					this.indices.Add(count++);
				}
				CSG_Model.submeshIndices.Add(this.indices);
			}
			else
			{
				for (int i = 0; i < m.subMeshCount; i++)
				{
					int[] submeshIndices = m.GetTriangles(i);
	
					CSG_Model.submeshIndices.Add(submeshIndices.ToList());
					for (int j = 0; j < submeshIndices.Length; j++)
					{
						vertices.Add( new CSG_Vertex(trans.TransformPoint(v[submeshIndices[j]]),
							trans.TransformDirection(n[submeshIndices[j]]),
							u == null ? Vector2.zero : u[submeshIndices[j]],
							c == null || c.Length != vertexCount ? Color.white : c[submeshIndices[j]],
							CSG_Model.submeshIndices.Count));
						this.indices.Add(count++);
					}
				}
			}
		}

		public CSG_Model(List<CSG_Polygon> list)
		{
			this.vertices = new List<CSG_Vertex>();
			this.indices = new List<int>();

			for (int i = 0; i < list.Count; i++)
			{
				CSG_Polygon poly = list[i];

				for (int j = 2; j < poly.vertices.Count; j++)
				{
					this.vertices.Add(poly.vertices[0]);
					this.vertices.Add(poly.vertices[j - 1]);	
					this.vertices.Add(poly.vertices[j]);		
				}
			}
		}

		public List<CSG_Polygon> ToPolygons()
		{
			List<CSG_Polygon> list = new List<CSG_Polygon>();

			for (int i = 0; i < indices.Count; i += 3)
			{
				List<CSG_Vertex> triangle = new List<CSG_Vertex>()
				{
					vertices[indices[i]],
					vertices[indices[i + 1]],
					vertices[indices[i + 2]]
				};

				list.Add(new CSG_Polygon(triangle));
			}

			return list;
		}

		/**
		 * Converts a CSG_Model to a Unity mesh.
		 */
		public Mesh ToMesh(bool keepSubmeshes, Transform targetTransform)
		{			
			int count = 0;

			Mesh m = new Mesh();
			m.name = "New Mesh";

			int vc = vertices.Count;

			Vector3[] v = new Vector3[vc];
			Vector3[] n = new Vector3[vc];
			Vector2[] u = new Vector2[vc];
			Color[] c = new Color[vc];
			Vector3 tempVector3 = Vector3.zero;

			List<CSG_Vertex> orderedVertices = new List<CSG_Vertex>();

			for (int i = 0; i < CSG_Model.submeshIndices.Count; i++)
			{
				for (int j = 0; j < vc; j++)
				{
					if ((this.vertices[j].textureId - 1) == i)
					{
						tempVector3 = targetTransform.InverseTransformPoint(this.vertices[j].position);
						CSG_Vertex tempVertex = this.vertices[j];
						tempVertex.position = tempVector3;
						this.vertices[j] = tempVertex;
						orderedVertices.Add(this.vertices[j]);
					}
				}
			}

			this.indices = new List<int>();
			
			int[] indexCountPerSubmesh = new int[CSG_Model.submeshIndices.Count];
			
			for (int i = 0; i < vc; i++)
			{
				indexCountPerSubmesh[orderedVertices[i].textureId - 1] += 1;
				this.indices.Add(count++);
			}
			
			for (int i = 0; i < vc; i++)
			{
				v[i] = orderedVertices[i].position;
				n[i] = orderedVertices[i].normal;
				u[i] = orderedVertices[i].uv;
				c[i] = orderedVertices[i].color;
			}

			
			m.vertices = v;
			m.normals = n;
			m.colors = c;
			m.uv = u;
			m.triangles = this.indices.ToArray();

			if (keepSubmeshes == true)
			{
				count = 0;
				m.subMeshCount = 0;
	
				for (int i = 0; i < indexCountPerSubmesh.Length; i++)
				{
					if (indexCountPerSubmesh[i] > 0)
					{
						m.subMeshCount++;
						m.SetTriangles(this.indices.GetRange(count, indexCountPerSubmesh[i]), m.subMeshCount - 1);
						count += indexCountPerSubmesh[i];			
					}
				}
			}	

			return m;
		}
	}
}