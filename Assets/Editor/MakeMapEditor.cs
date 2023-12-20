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
			MaterialChange(pseudo_obj, null);
		}
		else
		{
			Debug.LogError("Folder not found: " + folderPath);
		}
	}


	public void MaterialChange(GameObject obj, GameObject pre_obj)
	{
		//���� object ���͸��� ����� ��������
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

		//��ͷ� ������ ������������ ���ǻ� ����������Ʈ������ ����
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
			//currentEvent.Use();
		}

		if (pseudo_obj != null)
		{
			ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Mapper")))
			{
				//Debug.Log(((Red)target).name);
				// Ray�� �浹�� ������ ������Ʈ ��ġ
				pseudo_obj.transform.position = hit.point;
				//var a = Instantiate(prefab, hit.point, Quaternion.identity);

			}
		}
		else
		{
			Debug.Log(pseudo_obj + " �� ���̾�");
		}

		//Selection.activeObject = ((Red)target).transform.gameObject;
	}
	//public GameObject prefab;





}
