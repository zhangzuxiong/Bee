using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace MeshMakerNamespace
{
	public class CSG
	{
		public static float EPSILON = 0.00001f; //< Tolerance used by `splitPolygon()` to decide if a point is on the plane.
		
		// Options
		public bool hideGameObjects = true;
		public bool keepSubmeshes = true;
		public bool useCustomMaterial = false;
		
		// Materials
		public Material customMaterial;
		
		public enum Operation { Subtract, Union, Intersection };
		[SerializeField] private Operation operation = Operation.Subtract;
		
		[SerializeField] private GameObject gameObjectA;
		[SerializeField] private GameObject gameObjectB;
		
		public Operation OperationType
		{
			get
			{
				return operation;
			}
			set
			{
				if (value.GetType() == typeof(Operation))
				{
					operation = value;
				}
			}
		}
		
		public GameObject Target
		{
			get
			{
				if (gameObjectA != null)
					return gameObjectA;
				else
					return null;
			}
			set
			{
				if (value != null && value.GetType() == typeof(GameObject))
				{
					gameObjectA = value;
				}
			}
		}
		
		public GameObject Brush
		{
			get
			{
				if (gameObjectB != null)
					return gameObjectB;
				else
					return null;
			}
			set
			{
				if (value != null && value.GetType() == typeof(GameObject))
				{
					gameObjectB = value;
				}
			}
		}
		
		private GameObject CreateNewObject()
		{
			GameObject tempObject = new GameObject();
			MeshRenderer meshRenderer = tempObject.AddComponent<MeshRenderer>();
			//EditorUtility.SetSelectedWireframeHidden(meshRenderer, true);
			Material[] material = new Material[1];
			material[0] = new Material(Shader.Find("Standard"));
			meshRenderer.sharedMaterials = material; 
			MeshFilter meshFilter = tempObject.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = new UnityEngine.Mesh();
			meshFilter.sharedMesh.name = "Level Editor Mesh";
			//meshCollider = tempObject.AddComponent<MeshCollider>();
			//meshCollider.sharedMesh = meshFilter.sharedMesh;
			//meshFilter.sharedMesh.MarkDynamic();
			tempObject.AddComponent<Rigidbody>();
			Rigidbody tempRigidBody = tempObject.GetComponent<Rigidbody>();
			tempRigidBody.isKinematic = true; 
			tempObject.name = "New Model";// + Part.GetPartId();
			//tempObject.transform.parent = parentObject.transform;
			tempObject.SetActive(true);
			//tempObject.hideFlags = HideFlags.HideInHierarchy;
			//tempObject.hideFlags = HideFlags.HideAndDontSave;
			
			return tempObject;
		}
		
		public GameObject PerformCSG()
		{
			if (gameObjectA == null || gameObjectB == null)
				return null;
			
			GameObject newGameObject = CreateNewObject();
			UnityEngine.Mesh newMesh = null;

			switch (operation)
			{
				case Operation.Subtract:
					newMesh = CSG.Subtract(gameObjectA, gameObjectB, keepSubmeshes, useCustomMaterial);
					break;
				case Operation.Union:
					newMesh = CSG.Union(gameObjectA, gameObjectB, keepSubmeshes, useCustomMaterial);
					break;
				case Operation.Intersection:
					newMesh = CSG.Intersect(gameObjectA, gameObjectB, keepSubmeshes, useCustomMaterial);
					break;
			}
			if (newMesh != null)
			{
				UpdateGoMaterials(newGameObject);
				newGameObject.GetComponent<MeshFilter>().sharedMesh = newMesh;
			}
			
			if (hideGameObjects == true)
			{
				gameObjectA.SetActive(false);
				gameObjectB.SetActive(false);
			}

			newGameObject.transform.position = gameObjectA.transform.position;
			newGameObject.transform.rotation = gameObjectA.transform.rotation;
			newGameObject.transform.localScale = gameObjectA.transform.localScale;
			
			return newGameObject;			
		}

		/**
		 * update go materials
		 */
		public void UpdateGoMaterials(GameObject newGameObject)
		{
			if (keepSubmeshes == true && useCustomMaterial == false)
			{
				List<Material> theMaterials = new List<Material>();
				theMaterials.AddRange(gameObjectA.GetComponent<MeshRenderer>().sharedMaterials.ToList());
				theMaterials.AddRange(gameObjectB.GetComponent<MeshRenderer>().sharedMaterials.ToList());
				newGameObject.GetComponent<MeshRenderer>().sharedMaterials = theMaterials.ToArray();
			}
			else if (keepSubmeshes == false && useCustomMaterial == false)
			{
				newGameObject.GetComponent<MeshRenderer>().sharedMaterial = gameObjectA.GetComponent<MeshRenderer>().sharedMaterial;
			}
			else if (useCustomMaterial == true)
			{
				List<Material> theMaterials = new List<Material>();
				theMaterials.AddRange(gameObjectA.GetComponent<MeshRenderer>().sharedMaterials.ToList());
				theMaterials.Add(customMaterial);
				newGameObject.GetComponent<MeshRenderer>().sharedMaterials = theMaterials.ToArray();
			}
		}
		
		/**
		 * Returns a new mesh by merging @lhs with @rhs.
		 */
		public static Mesh Union(GameObject lhs, GameObject rhs, bool submeshes, bool customMaterial)
		{
			CSG_Model result = null;
			
			CSG_Model.submeshIndices = new List<List<int>>();			
			
			CSG_Model csg_model_a = new CSG_Model(lhs, false, false);
			CSG_Model csg_model_b = new CSG_Model(rhs, false, customMaterial);
			
			CSG_Node a = new CSG_Node( csg_model_a.ToPolygons());
			CSG_Node b = new CSG_Node( csg_model_b.ToPolygons());
			
			try
			{
				List<CSG_Polygon> polygons = CSG_Node.Union(a, b).AllPolygons();
				
				result = new CSG_Model(polygons);
			}
			catch
			{
				Debug.Log("Error During CSG Operation");
			}
			
			if (result != null)
			{
				return result.ToMesh(submeshes, lhs.transform);
			}
			
			return null;
		}
		
		/**
		 * Returns a new mesh by subtracting @rhs from @lhs.
		 */
		public static Mesh Subtract(GameObject lhs, GameObject rhs, bool submeshes, bool customMaterial)
		{
			CSG_Model result = null;
			
			CSG_Model.submeshIndices = new List<List<int>>();
			
			CSG_Model csg_model_a = new CSG_Model(lhs, false, false); 
			CSG_Model csg_model_b = new CSG_Model(rhs, true, customMaterial);
			
			CSG_Node a = new CSG_Node( csg_model_a.ToPolygons());
			CSG_Node b = new CSG_Node( csg_model_b.ToPolygons());
			
			try
			{
				List<CSG_Polygon> polygons = CSG_Node.Subtract(a, b).AllPolygons();
				
				result = new CSG_Model(polygons);
			}
			catch
			{
				Debug.Log("Error During CSG Operation");
			}
			
			if (result != null)
				return result.ToMesh(submeshes, lhs.transform);
			
			return null;
		}
		
		/**
		 * Return a new mesh by intersecting @lhs with @rhs.  This operation
		 * is non-commutative, so set @lhs and @rhs accordingly.
		 */
		public static Mesh Intersect(GameObject lhs, GameObject rhs, bool submeshes, bool customMaterial)
		{
			CSG_Model result = null;
			
			CSG_Model.submeshIndices = new List<List<int>>();			
			
			CSG_Model csg_model_a = new CSG_Model(lhs, false, false);
			CSG_Model csg_model_b = new CSG_Model(rhs, false, customMaterial); 
			
			CSG_Node a = new CSG_Node( csg_model_a.ToPolygons());
			CSG_Node b = new CSG_Node( csg_model_b.ToPolygons());
			try
			{
				List<CSG_Polygon> polygons = CSG_Node.Intersect(a, b).AllPolygons();
				
				result = new CSG_Model(polygons);
			}
			catch
			{
				Debug.Log("Error During CSG Operation");
			}
			
			if (result != null)
				return result.ToMesh(submeshes, lhs.transform);
			
			return null;
		}
		
	}
}