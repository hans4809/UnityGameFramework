using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : BaseScene
{
    public override void Clear()
    {
    }

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Unknown;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Managers.Scene.LoadSceneAsync(Define.Scene.MainScene));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
