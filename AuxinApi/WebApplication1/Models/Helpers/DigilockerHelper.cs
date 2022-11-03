using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.Net; 
using System.Web.UI;
using Newtonsoft.Json;

/// <summary>
/// Summary description for SakalaHelper
/// </summary>
public class DigilockerHelper
{
    #region project DIGILOCKER
    public const string DIGI_API_SIGNIN = @"http://164.100.133.78:8090/DLGateway/API/CeGLockerAPI.svc/DigiLockerSignIn";
    public const string DIGI_API_GET_ID_FROM_TXNID = @"http://164.100.133.78:8090/DLGateway/API/CeGLockerAPI.svc/GetDigilockerIDFromTxnID";
    public const string DIGI_API_GET_EADHAR = @"http://164.100.133.78:8090/DLGateway/API/CeGLockerAPI.svc/GetEAadharDetails";
    public const string DIGI_API_PUSH_DOC = @"http://164.100.133.78:8090/DLGateway/API/CeGLockerAPI.svc/PushDocToDigiLocker";
    public const string DIGI_API_GET_DOC_LIST = @"http://164.100.133.78:8090/DLGateway/API/CeGLockerAPI.svc/GetIssuedDocList";
    public const string DIGI_API_GET_DOC = @"http://164.100.133.78:8090/DLGateway/API/CeGLockerAPI.svc/GetDocumentFromDigiLocker";
    public const string DIGI_DeptKey = "1234567890";
    public const string DIGI_DeptSecretKey = "60ec99bc-ae36-11e9-a2a3-2a2ae2dbcce4";
    public const string DIGI_IssuerID = "in.gov.karnataka.ksdb"; 
    #endregion

    public DigilockerHelper()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public static string DigiLockerSignIn(string appParam, string appRedirectURL)
    {

        string Content_Type = "application/json";
        string DeptKey = DIGI_DeptKey;
        string RedirectURL = appRedirectURL;
        string AppParam = appParam;
        string HmacKey = DIGI_DeptSecretKey;
        string Hmacvalue = DeptKey + AppParam + RedirectURL;

        string hashHMACHexDLFromTxnID = HashHMACHex(HmacKey, Hmacvalue);

        object input = new
        {
            DeptKey,
            RedirectURL,
            AppParam,
            DLFLow = "signup",
            APIMac = hashHMACHexDLFromTxnID
        };
        try
        {
            var serializer = new JavaScriptSerializer();

            // For simplicity just use Int32's max value.
            // You could always read the value from the config section mentioned above.
            serializer.MaxJsonLength = Int32.MaxValue;

            string inputJson = serializer.Serialize(input);
            WebClient client = new WebClient();
            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;
            string json = client.UploadString(DIGI_API_SIGNIN, inputJson);

            return json;
        }
        catch (Exception ex)
        {
            return "";
        }
    }


    public static string GetDigilockerIDFromTxnID(string txnid, string appParam)
    {
        string Content_Type = "application/json";
        string DeptKey = DIGI_DeptKey;
        string TxnID = txnid;
        string AppParam = appParam;
        string HmacKey = DIGI_DeptSecretKey;
        string Hmacvalue = DeptKey + AppParam + TxnID;

        string hashHMACHexDLFromTxnID = HashHMACHex(HmacKey, Hmacvalue);

        object input = new
        {
            DeptKey,
            AppParam,
            TxnID,
            APIMac = hashHMACHexDLFromTxnID
        };
        try
        {
            var serializer = new JavaScriptSerializer();

            // For simplicity just use Int32's max value.
            // You could always read the value from the config section mentioned above.
            serializer.MaxJsonLength = Int32.MaxValue;

            string inputJson = serializer.Serialize(input);
            WebClient client = new WebClient();
            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;
            string json = client.UploadString(DIGI_API_GET_ID_FROM_TXNID, inputJson);

            return json;

        }
        catch (Exception ex)
        {
            return "";
        }
    }

    public static string GetDigilockerEAdhar(string DigilockerID)
    {
        string Content_Type = "application/json";
        string DeptKey = DIGI_DeptKey; 
        string HmacKey = DIGI_DeptSecretKey;
        string Hmacvalue = DeptKey + DigilockerID;

        string hashHMACHexDLFromTxnID = HashHMACHex(HmacKey, Hmacvalue);

        object input = new
        {
            DeptKey,
            DigilockerID,
            APIMac = hashHMACHexDLFromTxnID
        };
        try
        {
            var serializer = new JavaScriptSerializer();

            // For simplicity just use Int32's max value.
            // You could always read the value from the config section mentioned above.
            serializer.MaxJsonLength = Int32.MaxValue;

            string inputJson = serializer.Serialize(input);
            WebClient client = new WebClient();
            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;
            string json = client.UploadString(DIGI_API_GET_EADHAR, inputJson);

            return json;

        }
        catch (Exception ex)
        {
            return "";
        }
    }

    public static string PushDocumentToDigiLocker(string DigilockerID, string DocNo, string DocType, string IssuedDate, string DocDesc)
    {
        string Content_Type = "application/json";
        string DeptKey = DIGI_DeptKey;
        string HmacKey = DIGI_DeptSecretKey;
        string Hmacvalue = DeptKey + DocType + DocNo + IssuedDate;

        string hashHMACHexDLFromTxnID = HashHMACHex(HmacKey, Hmacvalue);

        object input = new
        {
            DeptKey,
            DigilockerID,
            DocNo,
            DocType,
            IssuedDate,
            DocDesc,
            APIMac = hashHMACHexDLFromTxnID
        };
        try
        {
            var serializer = new JavaScriptSerializer();

            // For simplicity just use Int32's max value.
            // You could always read the value from the config section mentioned above.
            serializer.MaxJsonLength = Int32.MaxValue;

            string inputJson = serializer.Serialize(input);
            WebClient client = new WebClient();
            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;
            string json = client.UploadString(DIGI_API_PUSH_DOC, inputJson);

            return json;

        }
        catch (Exception ex)
        {
            return "";
        }
    }

    /// <summary>
    /// //////////////////
    /// </summary>
    /// <param name="obj_Json"></param> 
    /// <returns></returns>
    public static List<AuthData> parseAuthDataToList(ResponseObject obj_Json)
    {
        List<AuthData> AuthList = new List<AuthData>();
        try
        {
            string ResponseObj = obj_Json.RespObj.ToString();
            JObject JO = JObject.Parse(ResponseObj);

            if (JO == null)
            {
                return null;
            }

            AuthData TempDetails = new AuthData();
            foreach (JProperty prop in (JToken)JO)
            {
                string tempValue = prop.Value.ToString(); // This is not allowed 
                switch (prop.Name)
                {
                    case "RedirectURL":
                        TempDetails.RedirectURL = tempValue;
                        break;
                    case "error":
                        TempDetails.error = tempValue;
                        break;
                    case "error_desc":
                        TempDetails.error_desc = tempValue;
                        break;
                }
            }
            AuthList.Add(TempDetails);
        }
        catch (Exception ex)
        {
            //throw ex;
            string msg = ex.Message;
            // Get stack trace for the exception with source file information
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            var err= ("AuthCodeModel Class Page - Method Name: parseAuthDataToList " + msg + ":=> " + line);
            throw;
        }
        return AuthList;
    }

    public static string HashHMACHex(string key, string message)
    {
        string hashHMACHex = string.Empty;
        try
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(key);
            HMACSHA256 hmacsha1 = new HMACSHA256(keyByte);
            byte[] messageBytes = encoding.GetBytes(message);

            byte[] hash = HashHMAC(keyByte, messageBytes);
            hashHMACHex = HashEncode(hash);
        }
        catch (Exception ex)
        {
            //throw ex;
            string msg = ex.Message;
            // Get stack trace for the exception with source file information
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            var err = "Class file clsComoon - Method Name:  HashHMACHex Error :- " + msg + ":=> " + line;
            throw;
        }
        finally
        {

        }
        return hashHMACHex;
    }

    public static byte[] HashHMAC(byte[] key, byte[] message)
    {
        var hash = new HMACSHA256(key);
        return hash.ComputeHash(message);
    }

    public static string HashEncode(byte[] hash)
    {
        return Convert.ToBase64String(hash);
    }

}
