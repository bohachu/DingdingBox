using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using UnityEngine;
using System.Text;
using System;
using LitJson;
using CI.HttpClient;
using Cameo;

namespace Cameo.PandoBox
{
    public class CggLoader
    {
        private Action<Boolean, string> callBack;

        private string apiUrl;
        private string key;
        private string iv;
        private string responseResult = "";
        private byte[] encrypted = null;
        HttpClient httpClient = new HttpClient();
        private string result = "";

        public CggLoader(string apiUrl, string key, string iv)
        {
            this.apiUrl = apiUrl;
            this.key = key;
            this.iv = iv;
        }

        public void Load(Action<Boolean, string> callBackFunc)
        {
            callBack = callBackFunc;

            System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) =>
            {
                return true;
            };

            Dictionary<string, string> test = new Dictionary<string, string>();
            test.Add("location", "02");
            test.Add("type", "3");
            string original = JsonMapper.ToJson(test);
            string originalUrlEncode = WWW.EscapeURL(original);
            encrypted = EncryptStringToBytes_Aes(originalUrlEncode, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv));

            StringContent stringContent = new StringContent(Convert.ToBase64String(encrypted), Encoding.UTF8, "text/plain");
            httpClient.Post(new Uri(apiUrl), stringContent, (r) =>
            {
                // Raised when the download completes
                responseResult = r.Data;
                Debug.Log("[TestApi] response result: " + responseResult);
                Decrypt();
            });

            Debug.Log("Original: " + original);
            Debug.Log("Encrypted (b64-encode): " + Convert.ToBase64String(encrypted));

        }

        public void LoadPromoUrl(Action<Boolean, string> callBackFunc)
        {
            callBack = callBackFunc;

            System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) =>
            {
                return true;
            };

            Dictionary<string, string> test = new Dictionary<string, string>();
            test.Add("location", "01");
            test.Add("type", "1");
            string original = JsonMapper.ToJson(test);
            string originalUrlEncode = WWW.EscapeURL(original);
            encrypted = EncryptStringToBytes_Aes(originalUrlEncode, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv));

            StringContent stringContent = new StringContent(Convert.ToBase64String(encrypted), Encoding.UTF8, "text/plain");
            httpClient.Post(new Uri(apiUrl), stringContent, (r) =>
            {
                // Raised when the download completes
                responseResult = r.Data;
                Debug.Log("[TestApi] response result: " + responseResult);
                Decrypt();
            });

            Debug.Log("Original: " + original);
            Debug.Log("Encrypted (b64-encode): " + Convert.ToBase64String(encrypted));

        }

        private void Decrypt()
        {
            string roundtrip = DecryptStringFromBytes_Aes(Convert.FromBase64String(responseResult), Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv));
            Debug.Log("Round Trip: " + WWW.UnEscapeURL(roundtrip));
            result = WWW.UnEscapeURL(roundtrip);

            callBack(true, result);
        }

        private byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] iv)
        {
            byte[] sourceBytes = Encoding.UTF8.GetBytes(plainText);
            var aes = new RijndaelManaged();
            aes.Key = Key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform transform = aes.CreateEncryptor();

            return transform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length);
        }

        private string DecryptStringFromBytes_Aes(byte[] cipherTextCombined, byte[] Key, byte[] iv)
        {
            var aes = new RijndaelManaged();
            aes.Key = Key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform transform = aes.CreateDecryptor();

            return Encoding.UTF8.GetString(transform.TransformFinalBlock(cipherTextCombined, 0, cipherTextCombined.Length));
        }
    }    

    [Serializable]
    public class CggLoaderInfo
    {
        public string ApiUrl;
        public string Key;
        public string IV;
    }
}
