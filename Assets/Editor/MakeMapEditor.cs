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
		}
		else
		{
			Debug.LogError("Folder not found: " + folderPath);
		}
	}

	private void OnSceneGUI()
	{
		Event currentEvent = Event.current;
		Ray ray;
		RaycastHit hit;

		if (pseudo_obj != null)
		{
			ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

			if (Physics.Raycast(ray, out hit,Mathf.Infinity , LayerMask.GetMask("Mapper")))
			{
				//Debug.Log(((Red)target).name);
				// Ray가 충돌한 지점에 오브젝트 배치
				pseudo_obj.transform.position = hit.point;
				//var a = Instantiate(prefab, hit.point, Quaternion.identity);

				//serializedObject.targetObject = (Red)target;
				//SerializedObject.targetObject = (Red)target;
			}
		}

		//Debug.Log("position..");
		if (Event.current != null && Event.current.type == EventType.KeyDown)
		{
			if (Event.current.keyCode == KeyCode.PageDown)
			{
				Debug.Log("prefab change past...");
				currentEvent.Use();

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
			currentEvent.Use();
		}
		
		//Selection.activeObject = ((Red)target).transform.gameObject;
	}
	//public GameObject prefab;





}
