using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditObject))]
public class MakeMapEditor : Editor
{
	public string folderPath = "Assets/Resources/Prefabs"; // �������� ����� ������ ���
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

		// ��� �ڽ� ��ü ����
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
				// �������� �ҷ�����
				GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

				if (prefab != null)
				{
					// �������� �̸� ���
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
				// Ray�� �浹�� ������ ������Ʈ ��ġ
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

		// ���콺 Ŭ�� ����
		if (Event.current != null && Event.current.type == EventType.MouseDown && Event.current.button == 0)
		{
			Debug.Log(Event.current.mousePosition);
			// Ray�� ���콺 Ŭ�� ��ġ�� �߻�
			ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

			if (Physics.Raycast(ray, out hit))
			{
				//Debug.Log(((Red)target).name);
				// Ray�� �浹�� ������ ������Ʈ ��ġ
				//var a = Instantiate(prefab, hit.point, Quaternion.identity);

				//serializedObject.targetObject = (Red)target;
				//SerializedObject.targetObject = (Red)target;
			}

			// �̺�Ʈ�� �Һ��Ͽ� SceneView�� Ŭ�� �̺�Ʈ�� ó������ �ʵ��� �մϴ�.
			currentEvent.Use();
		}
		
		//Selection.activeObject = ((Red)target).transform.gameObject;
	}
	//public GameObject prefab;





}
