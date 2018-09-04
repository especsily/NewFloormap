using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Google.GData.Client;
using Google.GData.Spreadsheets;

// [InitializeOnLoad]
public class SpreadsheetManager : MonoBehaviour, ISpreadSheet
{
    //based on row 
    // -> access listData[row][col] 
    // -> first row sáng/chiều/tối
    // -> begin with column 1
    List<List<RoomData>> listData = new List<List<RoomData>>();

    // enter Client ID and Client Secret values
    public string _ClientId = "842924670934-7pkkanve9fu70mtt1t61gh9np7oask9e.apps.googleusercontent.com";
    public string _ClientSecret = "9IMwJTcuJyBAYYUtOOL7wJM5";
    // enter Access Code after getting it from auth url
    public string _AccessCode = "4/SwC-XidXn6bdHJWKi1Vo4pmirZAaXOmxeAP9tk6numrWNMkIPYwGTY4";
    // enter Auth 2.0 Refresh Token and AccessToken after succesfully authorizing with Access Code
    public string _RefreshToken = "1/MbDfoh-Y9fDM26SiNhqTfF4AYnHGg2sop-DbTz5TbtU";
    public string _AccessToken = "ya29.GlsJBiiuHGGw1gNjU77iIf-7j8U2gWPwClJakXpgH_OuLBNpZmG2lo_0wLySB0l-sUcwkpMKiLKzqa8zndoCD52yoXamKPECRP89UMwdoiIW_Wfxl29pmsy3NFTv";

    public string _SpreadsheetName = "NestCoworkingSpace";

    SpreadsheetsService service;

    public GOAuth2RequestFactory RefreshAuthenticate()
    {
        OAuth2Parameters parameters = new OAuth2Parameters()
        {
            RefreshToken = _RefreshToken,
            AccessToken = _AccessToken,
            ClientId = _ClientId,
            ClientSecret = _ClientSecret,
            Scope = "https://www.googleapis.com/auth/drive https://spreadsheets.google.com/feeds",
            AccessType = "offline",
            TokenType = "refresh"
        };
        string authUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);
        return new GOAuth2RequestFactory("spreadsheet", "MySpreadsheetIntegration-v1", parameters);
    }

    void Auth()
    {
        GOAuth2RequestFactory requestFactory = RefreshAuthenticate();

        service = new SpreadsheetsService("MySpreadsheetIntegration-v1");
        service.RequestFactory = requestFactory;
    }

    // access spreadsheet data
    void AccessSpreadsheet(int day, SpreadsheetFeed feed, List<List<RoomData>> listData)
    {

        string name = _SpreadsheetName;
        SpreadsheetEntry spreadsheet = null;

        foreach (AtomEntry sf in feed.Entries)
        {
            if (sf.Title.Text == name)
            {
                spreadsheet = (SpreadsheetEntry)sf;
            }
        }

        if (spreadsheet == null)
        {
            Debug.Log("There is no such spreadsheet with such title in your docs.");
            return;
        }


        // Get the first worksheet of the first spreadsheet.
        WorksheetFeed wsFeed = spreadsheet.Worksheets;
        if (day <= wsFeed.Entries.Count)
        {
            WorksheetEntry worksheet = (WorksheetEntry)wsFeed.Entries[day];

            // Define the URL to request the list feed of the worksheet.
            AtomLink listFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);

            // Fetch the list feed of the worksheet.
            ListQuery listQuery = new ListQuery(listFeedLink.HRef.ToString());
            ListFeed listFeed = service.Query(listQuery);
			
            //access spreadsheet data here
            for (int i = 0; i < listFeed.Entries.Count; i++)
            {
                ListEntry row = (ListEntry)listFeed.Entries[i];
                List<RoomData> listValue = new List<RoomData>();
                foreach (ListEntry.Custom item in row.Elements)
                {
					if(item.LocalName.Trim().Substring(0, 1).ToLower() == "p")
						listValue.Add(new RoomData(item.LocalName.Substring(1, item.LocalName.Length -1), item.Value));
					else if(item.LocalName.Trim().ToLower() == "opennest")
						listValue.Add(new RoomData("OpenSpace", item.Value));
					else if(item.LocalName.Trim().ToLower() == "cafe")
						listValue.Add(new RoomData("Cafeteria", item.Value));
                }
                listData.Add(listValue);
            }
        }
		else
		{
			Debug.Log("There is no entry on that day!");
		}
    }

    void Init()
    {

        ////////////////////////////////////////////////////////////////////////////
        // STEP 1: Configure how to perform OAuth 2.0
        ////////////////////////////////////////////////////////////////////////////

        if (_ClientId == "" && _ClientSecret == "")
        {
            Debug.Log("Please paste Client ID and Client Secret");
            return;
        }

        string CLIENT_ID = _ClientId;

        string CLIENT_SECRET = _ClientSecret;

        string SCOPE = "https://www.googleapis.com/auth/drive https://spreadsheets.google.com/feeds https://docs.google.com/feeds";

        string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";

        string TOKEN_TYPE = "refresh";

        ////////////////////////////////////////////////////////////////////////////
        // STEP 2: Set up the OAuth 2.0 object
        ////////////////////////////////////////////////////////////////////////////

        // OAuth2Parameters holds all the parameters related to OAuth 2.0.
        OAuth2Parameters parameters = new OAuth2Parameters();

        parameters.ClientId = CLIENT_ID;

        parameters.ClientSecret = CLIENT_SECRET;

        parameters.RedirectUri = REDIRECT_URI;

        ////////////////////////////////////////////////////////////////////////////
        // STEP 3: Get the Authorization URL
        ////////////////////////////////////////////////////////////////////////////

        parameters.Scope = SCOPE;

        parameters.AccessType = "offline"; // IMPORTANT and was missing in the original

        parameters.TokenType = TOKEN_TYPE; // IMPORTANT and was missing in the original

        // Authorization url.

        string authorizationUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);
        Debug.Log(authorizationUrl);
        Debug.Log("Please visit the URL above to authorize your OAuth "
                  + "request token.  Once that is complete, type in your access code to "
                  + "continue...");

        parameters.AccessCode = _AccessCode;

        if (parameters.AccessCode == "")
        {
            Application.OpenURL(authorizationUrl);
            return;
        }
        ////////////////////////////////////////////////////////////////////////////
        // STEP 4: Get the Access Token
        ////////////////////////////////////////////////////////////////////////////

        OAuthUtil.GetAccessToken(parameters);
        string accessToken = parameters.AccessToken;
        string refreshToken = parameters.RefreshToken;
        Debug.Log("OAuth Access Token: " + accessToken + "\n");
        Debug.Log("OAuth Refresh Token: " + refreshToken + "\n");

    }

    public List<List<RoomData>> GetSpreadSheetData(int day)
    {
        if (_RefreshToken == "" && _AccessToken == "")
        {
            Init();
            return null;
        }

        Auth();

        Google.GData.Spreadsheets.SpreadsheetQuery query = new Google.GData.Spreadsheets.SpreadsheetQuery();

        // Make a request to the API and get all spreadsheets.
        SpreadsheetFeed feed = service.Query(query);

        if (feed.Entries.Count == 0)
        {
            Debug.Log("There are no spreadsheets in your docs.");
            return null;
        }

        AccessSpreadsheet(day, feed, listData);
        return listData;
    }
}

