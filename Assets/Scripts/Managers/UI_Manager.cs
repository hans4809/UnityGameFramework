using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

public class UI_Manager
{
    int _order = 10;

    Stack<UI_Popup> _popUpStack = new Stack<UI_Popup>();

    UI_Scene _sceneUI = null;
    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_root");
            if (root == null)
            {
                root = new GameObject { name = "@UI_root" };
            }
            return root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        CanvasScaler canvasScaler = Util.GetOrAddComponent<CanvasScaler>(go);

        if(canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        if(canvas.worldCamera == null)
            canvas.worldCamera = Camera.main;
        if(canvas.overrideSorting == false)
            canvas.overrideSorting = true;

        if(canvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        if(canvasScaler.referenceResolution != new Vector2(1920f, 1080f))
            canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        if(canvasScaler.screenMatchMode != CanvasScaler.ScreenMatchMode.Expand)
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 5;
        }
    }
    public T ShowAnyUI<T>(string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }
        GameObject go = Managers.Resource.Instantiate($"UI/{name}");
        T anyUI = Util.GetOrAddComponent<T>(go);
        go.transform.SetParent(Root.transform);
        return anyUI;
    }
    public T ShowPopUpUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }
        GameObject go = Managers.Resource.Instantiate($"UI/PopUp/{name}");
        T popUp = Util.GetOrAddComponent<T>(go);
        _popUpStack.Push(popUp);
        go.transform.SetParent(Root.transform);
        return popUp;
    }
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }
        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;
        go.transform.SetParent(Root.transform);
        return sceneUI;
    }

    public void ClosePopUpUI()
    {
        if (_popUpStack.Count == 0)
        {
            return;
        }
        UI_Popup popUP = _popUpStack.Pop();
        Managers.Resource.Destroy(popUP.gameObject);
        popUP = null;

        _order--;
    }
    public void ClosePopUpUI(UI_Popup popUp)
    {
        if (_popUpStack.Count == 0)
        {
            return;
        }
        if (_popUpStack.Peek() != popUp)
        {
            Debug.Log("Close PopUp Failed");
            return;
        }
        ClosePopUpUI();

    }
    public void CloseAllPopUPUI()
    {
        while (_popUpStack.Count > 0)
        {
            ClosePopUpUI();
        }
    }

    public IEnumerator FadeImage(Image image, float fadeDuration, bool isFadeIn = true)
    {
        float elapsedTime = 0f;
        Color color = image.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            if(isFadeIn)
                color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            else
                color.a = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            image.color = color;
            yield return null;
        }
        if(isFadeIn)
            color.a = 1f;
        else
            color.a = 0f;

        image.color = color;
    }

    public IEnumerator FadeText(TMP_Text text, float fadeDuration, bool isFadeIn = true)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            float a;
            if(isFadeIn)
                a = Mathf.Lerp(0, 1, alpha);
            else
                a = 1 - Mathf.Lerp(0, 1, alpha);
            text.color = new Color(text.color.r, text.color.g, text.color.b, a);

            yield return null;
        }

        if(isFadeIn)
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        else
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
    }

    public IEnumerator BlinkText(TextMeshProUGUI textMesh, float blinkDuration)
    {
        while (true)
        {
            textMesh.gameObject.SetActive(true);
            yield return new WaitForSeconds(blinkDuration);
            textMesh.gameObject.SetActive(false);
            yield return new WaitForSeconds(blinkDuration);
        }
    }

    public void Clear()
    {
        CloseAllPopUPUI();
        _sceneUI = null;
    }
}
