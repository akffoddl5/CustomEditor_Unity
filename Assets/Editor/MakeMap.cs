using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditObject))]
public class MakeMap : Editor
{
    public GameObject prefab;


	Event currentEvent = Event.current;

	


	//	prefab = GameObject.Find("Cube");
	//    Debug.Log("position..");
	//    if (Event.current != null && Event.current.type == EventType.KeyDown)
	//    {
	//        Debug.Log("button.." + Event.current.keyCode);
	//    }

	//// 마우스 클릭 감지
	//if (Event.current != null && Event.current.type == EventType.MouseDown && Event.current.button == 0)
	//{
	//	Debug.Log(Event.current.mousePosition);
	//	// Ray를 마우스 클릭 위치로 발사
	//	Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
	//	RaycastHit hit;

	//	if (Physics.Raycast(ray, out hit))
	//	{
	//		Debug.Log(((Red)target).name);
	//		// Ray가 충돌한 지점에 오브젝트 배치
	//		var a = Instantiate(prefab, hit.point, Quaternion.identity);

	//		//serializedObject.targetObject = (Red)target;
	//		//SerializedObject.targetObject = (Red)target;
	//	}

	//	// 이벤트를 소비하여 SceneView가 클릭 이벤트를 처리하지 않도록 합니다.
	//}
	//currentEvent.Use();
	//Selection.activeObject = ((Red)target).transform.gameObject;


}
