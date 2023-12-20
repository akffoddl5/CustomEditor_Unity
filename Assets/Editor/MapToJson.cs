using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MapToJson : MonoBehaviour
{
    // ������ �ڽĵ��� ������ ����Ʈ
    public static MapList mp_list = new();
    public static int sequence;
    public static int depth;
    private static string playerDataPath = "Assets/Resources/Data/PlayerLevelData.json";
    static readonly string map_data_json = "Assets/Resources/Data/mapData.json";
    //string path = Path.Combine(Application.dataPath, $"MapData_{WIndowManager.instance.nickName}_{WIndowManager.instance.mapNum}.json");

    // Json ������ ����.
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

        // ���� �� �ڽĵ��� ������ ����Ʈ�� ����
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

	// ��������� ��� �ڽ��� ������ ����Ʈ�� �߰��ϴ� �Լ�
	static void GetAllChildrenRecursive(Transform parent, int parent_key)
    {
        depth++;
        foreach (Transform child in parent)
        {
            sequence++;
            
            // ���� ������Ʈ�� �������� ����ϴ��� Ȯ��
            PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(child.gameObject);
            PrefabInstanceStatus prefabInstanceStatus = PrefabUtility.GetPrefabInstanceStatus(child.gameObject);

            if (prefabType != PrefabAssetType.NotAPrefab /*&& prefabInstanceStatus != PrefabInstanceStatus.Connected*/)
            {
                // ������ �ν��Ͻ��� ���, �ش� �������� ���� ��θ� ������
                string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(child.gameObject);

                // ���� ��ο��� ���� �̸��� ����
                string prefabName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

                // ���� �ڽ��� ����Ʈ�� �߰�
                mp_list.mp.Add(new MapData(sequence, depth, parent_key, child.position, child.rotation, prefabName, child.gameObject.name));
            }
            else
            {
                mp_list.mp.Add(new MapData(sequence, depth, parent_key, child.position, child.rotation, "empty", child.gameObject.name));

                // ���� �ڽ��� �ڽĵ��� �˻��ϱ� ���� ��� ȣ��
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