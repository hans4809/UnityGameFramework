using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_LoadingScene : UI_Scene
{
    [SerializeField] Image _loadingBarIMG;
    public Image LoadingBarIMG { get => _loadingBarIMG; private set => _loadingBarIMG = value; }
    public enum Images
    {
        BackGround,
        LoadingBar,
    }

    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
        LoadingBarIMG = Get<Image>((int)Images.LoadingBar);
    }

    public void OnclickedBackGround(PointerEventData eventData)
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            var asyncOper = Managers.Scene.AsyncLoadSceneOper;
            if (asyncOper != null)
            {
                if (asyncOper.progress >= 0.9f)
                    asyncOper.allowSceneActivation = true;
            }
        }
    }
}
