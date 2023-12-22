using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditObject : MonoBehaviour
{
    public static EditObject _instance;

    public static EditObject Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EditObject>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject("EditObject");
                    _instance = obj.AddComponent<EditObject>();
                }
            }

            return _instance;
        }
    }

   

	public string last_command;
    
}
