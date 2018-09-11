using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google.Apis.Sheets.v4;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;

public class HttpClientTest : MonoBehaviour
{
    public Text text;
    static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly, };
    const string ApplicationName = "nestcoworkingspace";
    const string spreadsheetId = "1o_2vhLZjkUAd0bSt3Nn_VEiupVr1R1Va0N99qXWTaEM";
    const string range = "Thứ 2 bs";

    string credPath = "";
    string client = "";
    JObject secrets;
    UserCredential credential;
    ValueRange response;

    //private async Task CreateUserCreadential()
    //{
    //    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
    //        new ClientSecrets() { ClientId = secrets["installed"]["client_id"].Value<string>(), ClientSecret = secrets["installed"]["client_secret"].Value<string>() },
    //        Scopes,
    //        "user",
    //        CancellationToken.None,
    //        new FileDataStore(credPath, true));
    //}

    //private async Task GetResponse(SpreadsheetsResource.ValuesResource.GetRequest request)
    //{
    //    response = await request.ExecuteAsync();
    //    Debug.Log(response.MajorDimension);
    //    Debug.Log("Get Response!");
    //}

    private void GetData()
    {
        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            new ClientSecrets() { ClientId = secrets["installed"]["client_id"].Value<string>(), ClientSecret = secrets["installed"]["client_secret"].Value<string>() },
            Scopes,
            "user",
            CancellationToken.None,
            new FileDataStore(credPath, true)).Result;

        // Create Google Sheets API service.
        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
        Debug.Log("Create google Sheets API service");

        // Define request parameters.
        Debug.Log("Define request parameters");
        SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        Debug.Log(request.SpreadsheetId);
        Debug.Log(request.Range);

        // Prints the names and majors of students in a sample spreadsheet:
        // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit

        response = request.Execute();
        Debug.Log(response.MajorDimension);
        Debug.Log("Get Response!");
    }

    private IEnumerator LoadData()
    {
        //Create client secrets
        string filePath = Path.Combine(Application.streamingAssetsPath, "credentials.json");
        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();
        client = www.downloadHandler.text;
        secrets = (JObject)JsonConvert.DeserializeObject(client);

        //Get data from server
        GetData();
        yield return new WaitUntil(() => response != null && response.Values.Count > 0);

        //Read data
        Debug.Log("Get value");
        IList<IList<object>> values = response.Values;
        Debug.Log(response.Values.Count);

        if (values != null && values.Count > 0)
        {
            text.text = values[1][1].ToString();
            foreach (var row in values)
            {
                // Print columns A and E, which correspond to indices 0 and 4.
                foreach (var col in row)
                {
                    Debug.Log(col);
                }
            }
        }
        else
        {
            Debug.Log("No data found.");
        }
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        credPath = Application.streamingAssetsPath + "/token.json";
        StartCoroutine(LoadData());
    }
}
