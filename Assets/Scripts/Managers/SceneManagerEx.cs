using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    private AsyncOperation _asyncLoadSceneOper;
    public AsyncOperation AsyncLoadSceneOper { get => _asyncLoadSceneOper; private set => _asyncLoadSceneOper = value; }

    private UI_LoadingScene _loadingScene;
    public UI_LoadingScene LoadingScene { get => _loadingScene; private set => _loadingScene = value; }
    public BaseScene CurrentScene
    {
        get { return GameObject.FindObjectOfType<BaseScene>(); }
    }
    public void LoadScene(Define.Scene type)
    {
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }
    string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }
    public void Clear()
    {
        CurrentScene.Clear();

        if(AsyncLoadSceneOper != null)
            AsyncLoadSceneOper = null;

        if (LoadingScene != null)
            LoadingScene = null;
    }

    public IEnumerator LoadSceneAsync(Define.Scene type)
    {
        Managers.Clear();

        LoadingScene = Managers.UI.ShowSceneUI<UI_LoadingScene>();
        AsyncLoadSceneOper = SceneManager.LoadSceneAsync(GetSceneName(type));
        AsyncLoadSceneOper.allowSceneActivation = false; // Scene 로드 끝나도 화면 활성화 안 함

        while(!AsyncLoadSceneOper.isDone)
        {
            if(LoadingScene.LoadingBarIMG != null)
                LoadingScene.LoadingBarIMG.fillAmount = AsyncLoadSceneOper.progress;

            if (AsyncLoadSceneOper.progress >= 0.9f)
                break;

            yield return null;
        }

        if (LoadingScene.LoadingBarIMG != null)
            LoadingScene.LoadingBarIMG.fillAmount = 1;
    }
}
