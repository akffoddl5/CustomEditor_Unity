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
	public LayerMask origin_layer;

	public List<GameObject> pseudo_list = new();
	public List<GameObject> prefab_list = new();
	public List<GameObject> install_list = new();
	public string last_command;
	GameObject root;
	LayerMask avail_layer;

	//LayerMask avail_layer = ~ (LayerMask.GetMask("Default"));

	private void OnEnable()
	{
		//Debug.Log("on enable");
		root = GameObject.Find("Map");
		if (root == null) root = new GameObject("Map");
		Initialize();
		LoadPrefabsFromFolder();

		EditorGUI.BeginChangeCheck();
	}


	private void OnDisable()
	{
		EditorGUI.EndChangeCheck();
	}

	public void Initialize()
	{
		

		avail_layer = ~LayerMask.GetMask("Ignore");

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

				//GameObject instantiatedPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
				
				if (prefab != null)
				{
					// 프리팹의 이름 출력
					//Debug.Log("Prefab Name: " + prefab.name);
					prefab_list.Add(Resources.Load<GameObject>("Prefabs/" + prefab.name));
					prefab = Resources.Load<GameObject>("Prefabs/" + prefab.name);
					//prefab = GameObject.Instantiate(prefab);
					prefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
					prefab.transform.parent = ((EditObject)target).transform;
					prefab.SetActive(false);
					pseudo_list.Add(prefab);

				}
			}

			pseudo_obj = pseudo_list[0];
			pseudo_obj.SetActive(true);
			origin_layer = pseudo_obj.layer;
			pseudo_obj.layer = LayerMask.NameToLayer("Ignore");
			foreach (Transform t in pseudo_obj.transform)
			{
				t.gameObject.layer = LayerMask.NameToLayer("Ignore");
			}
			//pseudo_obj.layer = LayerMask.GetMask("Ignore");
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
			pre_obj.GetComponent<Renderer>().sharedMaterial = origin_material;//

			//재귀로 끝까지 돌려야하지만 편의상 하위오브젝트까지만 적용
			foreach (Transform tf in pre_obj.transform)
			{
				tf.GetComponent<Renderer>().sharedMaterial = origin_material;
			}
		}

		origin_material = obj.GetComponent<Renderer>().sharedMaterial;

		//Material mat = virtual_material;
		Color origin_color = origin_material.color;
		//Debug.Log("origin color : " + origin_color.r + " " + origin_color.g + " " + origin_color.b + " " + origin_color.a);
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
		if (((EditObject)target).last_command != "")
		{
			last_command = ((EditObject)target).last_command;
			((EditObject)target).last_command = "";
		}
		
		//Scene view에 메뉴얼 표시
		Handles.BeginGUI();
		//var oldbgcolor = GUI.backgroundColor;
		GUI.backgroundColor = Color.cyan;     // 배경 색 지정
		GUI.color = Color.cyan;
		GUIStyle guiBoxStyle = new GUIStyle(GUI.skin.box);
		guiBoxStyle.fontSize = 14;
		guiBoxStyle.alignment = TextAnchor.UpperLeft;
		float guiBoxWidth = 33.1536f;
		float guiBoxHeight = 5.4227f; //텍스트 1줄 : 1.3461
		GUI.Box(new Rect(43, 0, guiBoxWidth * guiBoxStyle.fontSize, guiBoxHeight * guiBoxStyle.fontSize),
			"<Map Maker Status>\n설치한 오브젝트 수 : "+ install_list.Count +
			"\n현재 프리팹 이름 : " + prefab_list[pseudo_list.IndexOf(pseudo_obj)].name +
			"\nlast command:" + last_command, guiBoxStyle);
		//GUI.backgroundColor = oldbgcolor;
		Handles.EndGUI();

		Event currentEvent = Event.current;
		Ray ray;
		RaycastHit hit;

		//component.pointPositions[i] = Handles.PositionHandle(component.pointPositions[i], Quaternion.identity);
		foreach (GameObject obj in install_list)
		{
			if (obj == null) continue;

			//Debug.Log("handdle 생성");
			if (Tools.current == Tool.Move)
				obj.transform.position = Handles.PositionHandle(obj.transform.position, Quaternion.identity);
			else if (Tools.current == Tool.Rotate)
				obj.transform.rotation = Handles.RotationHandle(obj.transform.rotation, obj.transform.position);
				
		}

		//Debug.Log("position..");
		if (currentEvent != null && currentEvent.type == EventType.KeyDown)
		{
			if (currentEvent.keyCode == KeyCode.PageUp || currentEvent.keyCode == KeyCode.PageDown)
			{
				//Debug.Log("X...");
				int current_idx = pseudo_list.IndexOf(pseudo_obj);
				int next_idx = current_idx;

				if (currentEvent.keyCode == KeyCode.PageDown)
				{
					next_idx = current_idx + 1;
					if (next_idx >= pseudo_list.Count)
					{
						next_idx = 0;
					}
				}
				else
				{
					next_idx = current_idx - 1;
					if (next_idx < 0)
					{
						next_idx = pseudo_list.Count - 1;
					}
				}

				pseudo_obj = pseudo_list[next_idx];
				pseudo_list[current_idx].SetActive(false);
				pseudo_list[next_idx].SetActive(true);
				pseudo_list[current_idx].layer = origin_layer;
				foreach (Transform t in pseudo_list[current_idx].transform)
				{
					t.gameObject.layer = origin_layer;
				}
				pseudo_obj.layer = LayerMask.NameToLayer("Ignore");
				foreach (Transform t in pseudo_obj.transform)
				{
					t.gameObject.layer = LayerMask.NameToLayer("Ignore");
				}
				//Debug.Log(current_idx + " " + next_idx + "  change");
				MaterialChange(pseudo_list[next_idx], pseudo_list[current_idx]);

				//currentEvent.Use();
			}
			else if (currentEvent.keyCode == KeyCode.R || currentEvent.keyCode == KeyCode.T)
			{
				float rotate_amount = 5f;
				if (currentEvent.keyCode == KeyCode.T) rotate_amount *= -1;
				//pseudo_obj.transform.localRotation = pseudo_obj.transform.localRotation * (new Vector3(0, rotate_amount, 0));
				pseudo_obj.transform.Rotate(Vector3.up, rotate_amount, Space.Self);
				last_command = "Y축 회전";
			}
			else if (currentEvent.keyCode == KeyCode.F || currentEvent.keyCode == KeyCode.G)
			{
				float rotate_amount = 5f;
				if (currentEvent.keyCode == KeyCode.G) rotate_amount *= -1;
				pseudo_obj.transform.Rotate(Vector3.forward, rotate_amount, Space.Self);
				currentEvent.Use();
				last_command = "Z축 회전";
			}
			else if (currentEvent.keyCode == KeyCode.V || currentEvent.keyCode == KeyCode.B)
			{
				float rotate_amount = 5f;
				if (currentEvent.keyCode == KeyCode.B) rotate_amount *= -1;
				pseudo_obj.transform.Rotate(Vector3.right, rotate_amount, Space.Self);
				last_command = "X축 회전";
			}

			//currentEvent.Use();
		}



		// 마우스 클릭 감지
		if (Event.current != null && Event.current.type == EventType.MouseDown && Event.current.button == 0)
		{
			// Ray를 마우스 클릭 위치로 발사
			ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

			if (Physics.Raycast(ray, out hit))
			{
				// 현재 선택된 객체 해제 (중요)
				Selection.activeObject = null;
				//var a = Instantiate(pseudo_obj, pseudo_obj.transform.position, pseudo_obj.transform.rotation);


				var a = PrefabUtility.InstantiatePrefab(prefab_list[pseudo_list.IndexOf(pseudo_obj)]) as GameObject;
				a.transform.position = pseudo_obj.transform.position;
				a.transform.rotation = pseudo_obj.transform.rotation;

				a.GetComponent<Renderer>().sharedMaterial = origin_material;
				a.layer = origin_layer;
				a.transform.parent = root.transform;

				install_list.Add(a);
				//재귀로 끝까지 돌려야하지만 편의상 하위오브젝트까지만 적용
				foreach (Transform tf in a.transform)
				{
					tf.GetComponent<Renderer>().sharedMaterial = origin_material;
				}
				a.transform.localScale = pseudo_obj.transform.lossyScale;
				last_command = a.name + $"설치 {a.transform.position}";
				//Undo.RegisterCompleteObjectUndo(a, "undo");
				// 속성 변경 기록 시작
				//Undo.RecordObject(a, "undo");
				Undo.RegisterCreatedObjectUndo(a, "undo");
				
				//Undo.RegisterCompleteObjectUndo(a, "undo");
				EditorUtility.SetDirty(a);
				//Debug.Log(((Red)target).name);
				// Ray가 충돌한 지점에 오브젝트 배치
				//var a = Instantiate(prefab, hit.point, Quaternion.identity);

				//serializedObject.targetObject = (Red)target;
				//SerializedObject.targetObject = (Red)target;
			}

			// 이벤트를 소비하여 SceneView가 클릭 이벤트를 처리하지 않도록 합니다.
			currentEvent.Use();
		}

		if (pseudo_obj != null)
		{
			ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			//Debug.Log(avail_layer + " avail .. ");
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, avail_layer))
			{
				//Debug.Log(avail_layer + " avail in .. ");
				//Debug.Log(((Red)target).name);
				// Ray가 충돌한 지점에 오브젝트 배치
				//Debug.Log(hit.collider.gameObject.name);
				
				pseudo_obj.transform.position = hit.point;
				//var a = Instantiate(prefab, hit.point, Quaternion.identity);

			}
		}
		else
		{
			Debug.Log(pseudo_obj + " is null");
		}
		Selection.activeObject = target;
		//Selection.activeObject = ((Red)target).transform.gameObject;
	}
	//public GameObject prefab;





}
