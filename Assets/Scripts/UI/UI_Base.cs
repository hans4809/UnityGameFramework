using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public abstract class UI_Base : MonoBehaviour
{
    /// <summary>
    /// 관리할 object(GameObject, Text, Button, Image 등)들을 저장하는 dictionary
    /// </summary>
    Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    /// <summary>
    /// 가상함수로 Init()을 만들어서 상속받은 클래스에서 구현을 강제화
    public abstract void Init();

    /// <summary>
    /// Type을 받아서 해당 Type에 맞는 object를 _objects에 저장(바인딩)하는 함수
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"> 해당 타입 정보 가진 enum </param>
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
            {
                objects[i] = Util.FindChild(gameObject, names[i], true);
            }
            else
            {
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);
            }
            if (objects[i] == null)
            {
                Debug.Log($"Failed to bind {names[i]}");
            }
        }
    }

    /// <summary>
    /// _objects에서 해당 Type에 맞는 object를 가져오는 함수
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    /// <returns></returns>
    protected T Get<T>(int index) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
        {
            return null;
        }
        return objects[index] as T;
    }

    /// <summary>
    /// 자주 쓰는 Type에 맞는 object를 가져오는 함수
    /// </summary>
    protected Text GetText(int index) { return Get<Text>(index); }
    protected Button GetButton(int index) { return Get<Button>(index); }
    protected Image GetImage(int index) { return Get<Image>(index); }

    /// <summary>
    /// GameObject에 Event를 추가하는 함수
    /// </summary>
    /// <param name="go"></param>
    /// <param name="action"></param>
    /// <param name="type"></param>
    public static void AddUIEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_EventHandler _event = Util.GetOrAddComponent<UI_EventHandler>(go);
        switch (type)
        {
            case Define.UIEvent.Click:
                _event.OnClickHandler -= action;
                _event.OnClickHandler += action;
                break;
            case Define.UIEvent.BeginDrag:
                _event.OnBeginDragHandler -= action;
                _event.OnBeginDragHandler += action;
                break;
            case Define.UIEvent.Drag:
                _event.OnDragHandler -= action;
                _event.OnDragHandler += action;
                break;
            case Define.UIEvent.DragEnd:
                _event.OnEndDragHandler -= action;
                _event.OnEndDragHandler += action;
                break;
            case Define.UIEvent.Slider:
                _event.OnSliderHandler -= action;
                _event.OnSliderHandler += action;
                break;
        }
    }

    public IEnumerator FadeImage(Image image, float fadeDuration, bool isFadeIn = true)
    {
        float elapsedTime = 0f;
        Color color = image.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            if (isFadeIn)
                color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            else
                color.a = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            image.color = color;
            yield return null;
        }
        if (isFadeIn)
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
            if (isFadeIn)
                a = Mathf.Lerp(0, 1, alpha);
            else
                a = 1 - Mathf.Lerp(0, 1, alpha);
            text.color = new Color(text.color.r, text.color.g, text.color.b, a);

            yield return null;
        }

        if (isFadeIn)
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
    public IEnumerator RotateUI(RectTransform recTransform, float rotationDegree, float rotateDuration, int iteration = 1)
    {
        for(int i = 0; i < iteration; i++)
        {
            yield return StartCoroutine(RotateUI(recTransform, 0, rotationDegree, rotateDuration / (iteration * 4)));
            yield return StartCoroutine(RotateUI(recTransform, rotationDegree, 0, rotateDuration / (iteration * 4)));
            yield return StartCoroutine(RotateUI(recTransform, 0, -rotationDegree, rotateDuration / (iteration * 4)));
            yield return StartCoroutine(RotateUI(recTransform, -rotationDegree, 0, rotateDuration / (iteration * 4)));
        }

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    private IEnumerator RotateUI(RectTransform recTransform, float from, float to, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            float textRotation = Mathf.Lerp(from, to, alpha);
            recTransform.rotation = Quaternion.Euler(new Vector3(0, 0, textRotation));
            yield return null;
        }
        recTransform.rotation = Quaternion.Euler(new Vector3(0, 0, to));
    }
}
