using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DataDefine;

public class DataManager : MonoBehaviour
{
    public static DataManager singleTon;
    public JsonManager jsonManager;
    public GameObject _gameObject;
    // Start is called before the first frame update


    public void Awake()
    {
        jsonManager = new JsonManager();
        if (singleTon == null)
        {
            singleTon = this;
            DontDestroyOnLoad(_gameObject);
        }
        else if (singleTon != this)
        {
            Destroy(singleTon._gameObject);
        }
    }

    private void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
    }
}
