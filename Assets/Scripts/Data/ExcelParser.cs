using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;

/// <summary>
/// Excel 파일을 읽어오는 매니저
/// </summary>
public static class ExcelParser
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    /// <summary>
    /// T Type 클래스의 이름으로 csv파일을 읽어와서 List<T>로 반환해주는 함수
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public static List<T> Read<T>(string name = null)
    {
        var list = new List<T>();

        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        string path = $"Excel/{name}";
        TextAsset data = Resources.Load<TextAsset>(path);
        if (data == null)
        {
            Debug.LogError($"ExcelManager.Read() : File is null. path = {path}");
            return list;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE); // 줄 단위로 나누기
        foreach (string element in lines)
        {
            if (element == lines[0]) continue;
            string[] datas = Regex.Split(element, SPLIT_RE); // 요소 단위로 나누기
            if (datas[0] == "") break;
            list.Add(StringToType<T>(datas));
        }

        return list;
    }

    /// <summary>
    /// String을 T Type으로 변환해서 반환해주는 함수
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="datas"></param>
    /// <param name="childType"></param>
    /// <returns></returns>
    public static T StringToType<T>(string[] datas, string childType = "")
    {
        object data;

        if (string.IsNullOrEmpty(childType) || Type.GetType(childType) == null)
        {
            data = Activator.CreateInstance(typeof(T));
        }
        else
        {
            data = Activator.CreateInstance(Type.GetType(childType));
        }

        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        for (int i = 0; i < datas.Length; i++)
        {
            try
            {
                Type type = fields[i].FieldType;
                if (string.IsNullOrEmpty(datas[i])) continue;

                if (type == typeof(int))
                    fields[i].SetValue(data, int.Parse(datas[i]));

                else if (type == typeof(float))
                    fields[i].SetValue(data, float.Parse(datas[i]));

                else if (type == typeof(bool))
                    fields[i].SetValue(data, bool.Parse(datas[i]));

                else if (type == typeof(string))
                    fields[i].SetValue(data, datas[i]);

                // enum
                else
                    fields[i].SetValue(data, Enum.Parse(type, datas[i]));
            }
            catch (Exception e)
            {
                Debug.LogError($"ExcelManager.StringToType() : {e.Message}");
            }
        }

        return (T)data;
    }
}

