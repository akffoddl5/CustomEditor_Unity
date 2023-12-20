using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditObject))]
public class MakeMapEditor : Editor
{
	public string folderPath = "Assets/Resources/Prefabs"; // 프리팹이 저장된 폴더의 경로
	public GameObject pseudo_obj;
	public Material virtual_material;
	public Material origin_material;
	public List<GameObject> pseudo_list = new();

	private void OnEnable()
	{
		Debug.Log("on enable");
		Initialize();
		LoadPrefabsFromFolder();

	}

	public void Initialize()
	{
		pseudo_list.Clear();
		pseudo_obj = null;
		virtual_material = Resources.Load<Material>("Material/VirtualMaterial");

		Transform[] children = new Transform[((EditObject)target).transform.childCount];
		for (int i = 0; i < ((EditObject)target).transform.childCount; i++)
		{
			children[i] = ((EditObject)target).transform.GetChild(i);
		}

		// 모든 자식 객체 제거
		foreach (Transform child in children)
		{
			DestroyImmediate(child.gameObject);
		}
		
	}


	void LoadPrefabsFromFolder()
	{
		if (Directory.Exists(folderPath))
		{
			
			string[] prefabPaths = Directory.GetFiles(folderPath, "*.prefab");

			foreach (string prefabPath in prefabPaths)
			{
				// 프리팹을 불러오기
				GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

				if (prefab != null)
				{
					// 프리팹의 이름 출력
					Debug.Log("Prefab Name: " + prefab.name);
					prefab = Resources.Load<GameObject>("Prefabs/" + prefab.name);
					prefab = GameObject.Instantiate(prefab);
					prefab.transform.parent = ((EditObject)target).transform;
					prefab.SetActive(false);
					pseudo_list.Add(prefab);
				}
			}

			pseudo_obj = pseudo_list[0];
			pseudo_obj.SetActive(true);
			MaterialChange(pseudo_obj, null);
		}
		else
		{
			Debug.LogError("Folder not found: " + folderPath);
		}
	}


	public void MaterialChange(GameObject obj, GameObject pre_obj)
	{
		//예전 object 매터리얼 제대로 돌려놓기
		if (pre_obj != null)
		{
			
		}

		origin_material = obj.GetComponent<Renderer>().sharedMaterial;
		
		//Material mat = virtual_material;
		Color origin_color = origin_material.color;
		Debug.Log("origin color : " + origin_color.r + " " + origin_color.g + " " + origin_color.b + " " + origin_color.a);
		origin_color.a = 0.3f;
		virtual_material.color = origin_color;
		obj.GetComponent<Renderer>().sharedMaterial = virtual_material;

		//재귀로 끝까지 돌려야하지만 편의상 하위오브젝트까지만 적용
		foreach (Transform tf in obj.transform)
		{
			tf.GetComponent<Renderer>().sharedMaterial = virtual_material;
		}
	}

	private void OnSceneGUI()
	{
		Event currentEvent = Event.current;
		Ray ray;
		RaycastHit hit;

		

		//Debug.Log("position..");
		if (currentEvent != null && currentEvent.type == EventType.KeyDown)
		{
			if (currentEvent.keyCode == KeyCode.X)
			{
				Debug.Log("X...");
				int current_idx= pseudo_list.IndexOf(pseudo_obj);
				int next_idx = current_idx + 1;
				if (next_idx >= pseudo_list.Count)
				{
					next_idx = 0;
				}
				pseudo_obj = pseudo_list[next_idx];
				pseudo_list[current_idx].SetActive(false);
				pseudo_list[next_idx].SetActive(true);
				MaterialChange(pseudo_list[current_idx], pseudo_list[next_idx]);
				
				//currentEvent.Use();

			}
			
		}

		// 마우스 클릭 감지
		if (Event.current != null && Event.current.type == EventType.MouseDown && Event.current.button == 0)
		{
			Debug.Log(Event.current.mousePosition);
			// Ray를 마우스 클릭 위치로 발사
			ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

			if (Physics.Raycast(ray, out hit))
			{
				//Debug.Log(((Red)target).name);
				// Ray가 충돌한 지점에 오브젝트 배치
				//var a = Instantiate(prefab, hit.point, Quaternion.identity);

				//serializedObject.targetObject = (Red)target;
				//SerializedObject.targetObject = (Red)target;
			}

			// 이벤트를 소비하여 SceneView가 클릭 이벤트를 처리하지 않도록 합니다.
			//currentEvent.Use();
		}

		if (pseudo_obj != null)
		{
			ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Mapper")))
			{
				//Debug.Log(((Red)target).name);
				// Ray가 충돌한 지점에 오브젝트 배치
				pseudo_obj.transform.position = hit.point;
				//var a = Instantiate(prefab, hit.point, Quaternion.identity);

			}
		}
		else
		{
			Debug.Log(pseudo_obj + " 가 널이야");
		}

		//Selection.activeObject = ((Red)target).transform.gameObject;
	}
	//public GameObject prefab;





}
