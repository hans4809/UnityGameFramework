using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleSheetManager
{
    // 읽어올 구글스프레드시트 주소
    protected const string ADDRESS = "https://docs.google.com/spreadsheets/d/1WZj4mF3424Ta28-ENZQJycE76xYHYTHQKQ14rxHVgjo";
    // 해당 구글스프레드시트의 AppScript 주소
    protected const string SaveADDRESS = "https://script.google.com/macros/s/AKfycbyzwhIItQkkamKuxFhcMIaEr-uIjx1abNgQF1-qHQLzW4jGQpObv38ziHHw8ZdW4YnYwQ/exec";
    // 구글 스프레드 시트 주소에서 아이템 데이터를 읽어올 행의 범위
    protected const string ItmeRANGE = "A2:F";
    // 구글 스프레드 시트 주소에서 게임 데이터를 읽어올 행의 범위
    protected const string GameDataRANGE = "A2:B";
    // 첫 번째 시트의 SHEET_ID는 0인데 그 다음 부터는 SHEET마다 ID가 붙어서 필요함
    protected const long SHEET_ID = 55073727;
    /*
        Key -> 해당 스프레드 시트에서 읽어올 데이터를 정의해둔 타입
        Value -> 해당 스프레드 시트에서 읽어온 데이터들을 쭉 나열
    */
    public static Dictionary<Type, string> sheetDatas = new Dictionary<Type, string>();

    public static void Init()
    {
        // 처음에 각 데이터를 읽어올 주소를 일단 SheetDatas에 넣음
        //if (!sheetDatas.ContainsKey(typeof(ItemTable)))
        //{
        //    sheetDatas.Add(typeof(ItemTable), GetTSVAddress(ADDRESS, ItmeRANGE));
        //}
        //if (!sheetDatas.ContainsKey(typeof(ScoreData)))
        //{
        //    sheetDatas.Add(typeof(ScoreData), GetTSVAddress(ADDRESS, GameDataRANGE, SHEET_ID));
        //}
    }
    /// <summary>
    /// 함수 이름 : GetTSVAddress
    /// 기능 : 구글스프레드시트 주소에서 TSV파일을 긁어오기 위한 주소를 만듬
    /// 반환 값 : TSV파일을 긁어오기 위한 주소
    /// </summary>
    /// <param name="address"></param> 기본 스프레드시트 주소
    /// <param name="range"></param> 읽어올 범위
    /// <param name="sheetID"></param> 시트 아이디, 첫번 째 시트일때는 ID가 필요 없으므로 default값을 0으로 설정
    /// <returns></returns>
    public static string GetTSVAddress(string address, string range, long sheetID = 0)
    {
        if (sheetID == 0)
        {
            return $"{address}/export?format=tsv&range={range}";
        }
        return $"{address}/export?format=tsv&range={range}&gid={sheetID}";
    }
    /// <summary>
    /// 함수 이름 : LoadData
    /// 기능 : 구글스프레드 시트에서 읽어온 문자열을 파싱해서 각 리스트에 저장
    /// </summary>
    /// <returns></returns>
    public static IEnumerator LoadData()
    {
        Init();
        List<Type> sheetTypes = new List<Type>(sheetDatas.Keys);
        // 각 데이터 타입 마다 실행
        foreach (Type type in sheetTypes)
        {
            // 해당 주소로 request보냄
            UnityWebRequest www = UnityWebRequest.Get(sheetDatas[type]);
            // request보내고 응답 받을 동안은 유니티 상의 코드 실행
            yield return www.SendWebRequest();

            if (www.isDone)
                Debug.Log(www.downloadHandler.text);
            // 오류 났을 때 예외 처리
            else
            {
                Debug.Log(www.error);
                yield return null;
            }
            // sheeDatas에 주소를 받은 응답의 text로 변경 
            sheetDatas[type] = www.downloadHandler.text;

            // 데이터 타입에 맞게 파싱해서 리스트에 저장
            //if (type == typeof(ItemTable))
            //{
                //itemList = GetDatas<ItemTable>(sheetDatas[type]);
            //}
            //if (type == typeof(ScoreData))
            //{
            //    scoreList = GetDatas<ScoreData>(sheetDatas[type]);
            //}
        }
    }
    /// <summary>
    /// 함수 이름 : SaveData
    /// 기능 : 스프레드 시트에 데이터 보냄
    /// </summary>
    /// <param name="MaxScore"></param>
    /// <param name="FirstPlay"></param>
    /// <returns></returns>
    public static IEnumerator SaveData(int MaxScore)
    {
        WWWForm form = new WWWForm();
        form.AddField("score", MaxScore);
        using (UnityWebRequest www = UnityWebRequest.Post(SaveADDRESS, form))
        {
            yield return www.SendWebRequest();
            if (www.isDone)
                Debug.Log(www.downloadHandler.text);
            else
                Debug.Log("Error");
        }
    }
    /// <summary>
    /// 함수 이름 : GetData
    /// 기능 : 파싱한 데이터를 데이터 타입에 맞게 반환
    /// </summary>
    /// <typeparam name="T"></typeparam> T 타입의 데이터
    /// <param name="datas"></param> 파싱한 데이터
    /// <param name="childType"></param> 
    /// <returns></returns>
    protected static T GetData<T>(string[] datas, string childType = "")
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
                Debug.LogError($"SpreadSheet Error : {e.Message}");
            }
        }
        return (T)data;
    }
    /// <summary>
    /// 함수 이름 : GetDatas
    /// 기능 : 스프레드시트에서 읽어온 스트링을 파싱해서 List로 반환
    /// </summary>
    /// <typeparam name="T"></typeparam> 반환할 리스트의 데이터 타입
    /// <param name="data"></param> 파싱할 스트링
    /// <returns></returns>
    protected static List<T> GetDatas<T>(string data)
    {
        List<T> returnList = new List<T>();
        string[] splitedData = data.Split('\n');

        foreach (string element in splitedData)
        {
            string[] datas = element.Split('\t');
            if (datas[0] == " ") { break; }
            returnList.Add(GetData<T>(datas));
        }
        return returnList;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    protected static List<T> GetDatasAsChildren<T>(string data)
    {
        List<T> returnList = new List<T>();
        string[] splitedData = data.Split('\n');

        foreach (string element in splitedData)
        {
            string[] datas = element.Split('\t');
            returnList.Add(GetData<T>(datas, datas[0]));
        }
        return returnList;
    }
}
