using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MainScene : UI_Scene
{
    public override void Init()
    {
        base.Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClickStart()
    {

    }
    public void OnClickExit()
    {
        Application.Quit();
    }
}
