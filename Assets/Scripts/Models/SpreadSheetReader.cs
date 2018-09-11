using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SpreadSheetReader : MonoBehaviour, ISpreadSheet
{
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    [SerializeField] private string docId;
    [SerializeField] private string monId, tueId, wedId, thurId, friId, satId, sunId;
    List<List<RoomData>> listData = null;
    private Action<string> GetData;

    void Start()
    {
        GetData += ReadData;
        StartCoroutine(DownloadCSVCoroutine(docId, GetData, false, "test", GetSheetIdByDay(DateTime.Today.DayOfWeek)));
    }
    void ReadData(string text)
    {
        listData = new List<List<RoomData>>();
        List<List<string>> listCellData = ParseCSV(text, true);

        //parse data into list
        for (int row = 1; row < listCellData.Count; row++)
        {
            List<RoomData> listValue = new List<RoomData>();
            for (int col = 1; col < listCellData[row].Count; col++)
            {
                if (listCellData[0][col].Trim().Substring(0, 1).ToLower() == "p")
                    listValue.Add(new RoomData(listCellData[0][col].Substring(1, listCellData[0][col].Length - 1), listCellData[row][col]));
                else if (listCellData[0][col].Trim().ToLower() == "open nest")
                    listValue.Add(new RoomData("OpenSpace", listCellData[row][col]));
                else if (listCellData[0][col].Trim().ToLower() == "cafe")
                    listValue.Add(new RoomData("Cafeteria", listCellData[row][col]));
            }
            listData.Add(listValue);
        }
    }
    IEnumerator DownloadCSVCoroutine(string docId, Action<string> callback,
                                                   bool saveAsset = false, string assetName = null, string sheetId = null)
    {
        string url = "https://docs.google.com/spreadsheets/d/" + docId + "/export?format=csv";
        if (!string.IsNullOrEmpty(sheetId))
        {
            url += "&gid=" + sheetId;
        }

        string path = "https://docs.google.com/spreadsheets/d/e/2PACX-1vT7qThY9WVgGN8JON52h_HmWy69fo1ZPGuLrs5SW50uTaVQ_EL64SBJqCMqRZlCco9UZHNHaftgP0ys/pub?gid=0&single=true&output=csv";
        // if (!string.IsNullOrEmpty(sheetId))
        // {
        //     url += "&gid=" + sheetId;
        // }
        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            callback(www.downloadHandler.text);
            if (saveAsset)
            {
                if (!string.IsNullOrEmpty(assetName))
                    File.WriteAllText("Assets/Resources/" + assetName + ".csv", www.downloadHandler.text);
                else
                {
                    throw new Exception("assetName is null");
                }
            }
        }
    }

    //------------------------------------------ Read CVS file ---------------------------------------------
    public static readonly string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    public static readonly string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    public static readonly char[] TRIM_CHARS = { '\"' };

    List<List<string>> ReadCSV(string file)
    {
        var data = Resources.Load(file) as TextAsset;
        return ParseCSV(data.text);
    }
    string CleanReturnInCsvTexts(string text)
    {
        text = text.Replace("\"\"", "'").Trim();

        if (text.IndexOf("\"") > -1)
        {
            string clean = "";
            bool insideQuote = false;
            for (int j = 0; j < text.Length; j++)
            {
                if (!insideQuote && text[j] == '\"')
                {
                    insideQuote = true;
                }
                else if (insideQuote && text[j] == '\"')
                {
                    insideQuote = false;
                }
                else if (insideQuote)
                {
                    if (text[j] == '\n')
                        clean += "<br>";
                    else if (text[j] == ',')
                        clean += "<c>";
                    else
                        clean += text[j];
                }
                else
                {
                    clean += text[j];
                }
            }
            text = clean;
        }
        return text;
    }

    List<List<string>> ParseCSV(string text, bool jumpedFirst = false)
    {
        text = CleanReturnInCsvTexts(text);

        var list = new List<List<string>>();
        var lines = Regex.Split(text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);

        foreach (var line in lines)
        {
            if (!jumpedFirst)
            {
                jumpedFirst = true;
                continue;
            }
            var values = Regex.Split(line, SPLIT_RE);

            var entry = new List<string>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                var value = values[j];
                value = DecodeSpecialCharsFromCSV(value);
                entry.Add(value);
            }
            list.Add(entry);
        }
        return list;
    }

    public static string DecodeSpecialCharsFromCSV(string value)
    {
        value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "").Replace("<br>", "\n").Replace("<c>", ",");
        return value;
    }

    string GetSheetIdByDay(DayOfWeek day)
    {
        if (day == DayOfWeek.Monday)
            return monId;
        else if (day == DayOfWeek.Tuesday)
            return tueId;
        else if (day == DayOfWeek.Wednesday)
            return wedId;
        else if (day == DayOfWeek.Thursday)
            return thurId;
        else if (day == DayOfWeek.Friday)
            return friId;
        else if (day == DayOfWeek.Saturday)
            return satId;
        else if (day == DayOfWeek.Sunday)
            return sunId;
        else
            return monId;
    }

    //------------------------- ISpreadSheet ---------------------------
    public List<List<RoomData>> GetSpreadSheetData()
    {
        return listData;
        //return null;
    }
}
