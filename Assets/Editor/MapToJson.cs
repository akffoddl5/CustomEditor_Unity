using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MapToJson : MonoBehaviour
{
    // 가져온 자식들을 저장할 리스트
    public static MapList mp_list = new();
    public static int sequence;
    public static int depth;
    private static string playerDataPath = "Assets/Resources/Data/PlayerLevelData.json";
    static readonly string map_data_json = "Assets/Resources/Data/mapData.json";
    //string path = Path.Combine(Application.dataPath, $"MapData_{WIndowManager.instance.nickName}_{WIndowManager.instance.mapNum}.json");

    // Json 데이터 생성.
    [MenuItem("Map To Json/Create Data")]
    static void CreateData()
    {
        CreateMapData();
        
    }

    public static void Initialize() {
        mp_list.mp.Clear();
        sequence = 0;
        depth = 0;
    }

    static void CreateMapData()
    {
        Initialize();

        // 시작 시 자식들을 가져와 리스트에 저장
        GetAllChildrenRecursive(GameObject.Find("Map").transform,0);

        foreach (var child in mp_list.mp)
        {
            Debug.Log("Child Name: " + child.prefab_name + " " + child.primary_key + " " + child.depth + " " + child.parent_key + " " + child.position + " " + child.quaternion);
        }

        string str_mp_json = JsonUtility.ToJson(mp_list);

        Debug.Log(str_mp_json);
        
        JsonFileSave(map_data_json, str_mp_json);
        //JsonFileSave("{}", playerDataPath);


    }

    static void JsonFileSave(string path, string str_json)
    {
        File.WriteAllText(path, str_json);
    }


	


	//private static List<Transform> allChildren = new List<Transform>();

	// 재귀적으로 모든 자식을 가져와 리스트에 추가하는 함수
	static void GetAllChildrenRecursive(Transform parent, int parent_key)
    {
        depth++;
        foreach (Transform child in parent)
        {
            sequence++;
            
            // 현재 오브젝트가 프리팹을 사용하는지 확인
            PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(child.gameObject);
            PrefabInstanceStatus prefabInstanceStatus = PrefabUtility.GetPrefabInstanceStatus(child.gameObject);

            if (prefabType != PrefabAssetType.NotAPrefab /*&& prefabInstanceStatus != PrefabInstanceStatus.Connected*/)
            {
                // 프리팹 인스턴스일 경우, 해당 프리팹의 에셋 경로를 가져옴
                string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(child.gameObject);

                // 에셋 경로에서 파일 이름만 추출
                string prefabName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

                // 현재 자식을 리스트에 추가
                mp_list.mp.Add(new MapData(sequence, depth, parent_key, child.position, child.rotation, prefabName, child.gameObject.name));
            }
            else
            {
                mp_list.mp.Add(new MapData(sequence, depth, parent_key, child.position, child.rotation, "empty", child.gameObject.name));

                // 현재 자식의 자식들을 검색하기 위해 재귀 호출
                GetAllChildrenRecursive(child, sequence);
            }
            
        }
        depth--;
    }

    

}


[System.Serializable]
public class MapData
{
    public int primary_key;
    public int depth;
    public int parent_key;
    public Vector3 position;
    public Quaternion quaternion;
    public string prefab_name;
    public string obj_name;

	public MapData(int primary_key, int depth, int parent_key, Vector3 position, Quaternion quaternion, string prefab_name, string obj_name)
	{
		this.primary_key = primary_key;
		this.depth = depth;
		this.parent_key = parent_key;
		this.position = position;
		this.quaternion = quaternion;
		this.prefab_name = prefab_name;
		this.obj_name = obj_name;
	}
}

[System.Serializable]
public class MapList
{
    public List<MapData> mp = new();
}