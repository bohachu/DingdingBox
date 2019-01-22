using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace YoutubeLight
{
    public class RequestResolver : MonoBehaviour
    {
        
        private const string RateBypassFlag = "ratebypass";
        [HideInInspector]
        public const string SignatureQuery = "signature";
        [HideInInspector]
        public string encryptedSignatureVideo;
        [HideInInspector]
        public string encryptedSignatureAudio;
        [HideInInspector]
        private string masterURLForVideo;
        [HideInInspector]
        private string masterURLForAudio;

        public void SetMasterUrlForAudio(string url)
        {
            masterURLForAudio = url;
        }

        public void SetMasterUrlForVideo(string url)
        {
            masterURLForVideo = url;
        }


        public void DoRegexFunctionsForVideo()
        {
            string js = masterURLForVideo;
            //Find "C" in this: var A = B.sig||C (B.s)
            string functNamePattern = @"(\w+)\s*=\s*function\(\s*(\w+)\s*\)\s*{\s*\2\s*=\s*\2\.split\(\""\""\)\s*;(.+)return\s*\2\.join\(\""\""\)\s*}\s*;";

            var funcName = Regex.Match(js, functNamePattern).Groups[1].Value;

            if (funcName.Contains("$"))
            {
                funcName = "\\" + funcName; //Due To Dollar Sign Introduction, Need To Escape
            }

            string funcPattern = @"(?!h\.)" + @funcName + @"=function\(\w+\)\{.*?\}"; //Escape funcName string
            var funcBody = Regex.Match(js, funcPattern, RegexOptions.Singleline).Value; //Entire sig function
            var lines = funcBody.Split(';'); //Each line in sig function

            string idReverse = "", idSlice = "", idCharSwap = ""; //Hold name for each cipher method
            string functionIdentifier = "";
            string operations = "";

            foreach (var line in lines.Skip(1).Take(lines.Length - 2)) //Matches the funcBody with each cipher method. Only runs till all three are defined.
            {
                if (!string.IsNullOrEmpty(idReverse) && !string.IsNullOrEmpty(idSlice) &&
                    !string.IsNullOrEmpty(idCharSwap))
                {
                    break; //Break loop if all three cipher methods are defined
                }

                functionIdentifier = GetFunctionFromLine(line);
                string reReverse = string.Format(@"{0}:\bfunction\b\(\w+\)", functionIdentifier); //Regex for reverse (one parameter)
                string reSlice = string.Format(@"{0}:\bfunction\b\([a],b\).(\breturn\b)?.?\w+\.", functionIdentifier); //Regex for slice (return or not)
                string reSwap = string.Format(@"{0}:\bfunction\b\(\w+\,\w\).\bvar\b.\bc=a\b", functionIdentifier); //Regex for the char swap.

                if (Regex.Match(js, reReverse).Success)
                {
                    idReverse = functionIdentifier; //If def matched the regex for reverse then the current function is a defined as the reverse
                }

                if (Regex.Match(js, reSlice).Success)
                {
                    idSlice = functionIdentifier; //If def matched the regex for slice then the current function is defined as the slice.
                }

                if (Regex.Match(js, reSwap).Success)
                {
                    idCharSwap = functionIdentifier; //If def matched the regex for charSwap then the current function is defined as swap.
                }
            }

            foreach (var line in lines.Skip(1).Take(lines.Length - 2))
            {
                Match m;
                functionIdentifier = GetFunctionFromLine(line);

                if ((m = Regex.Match(line, @"\(\w+,(?<index>\d+)\)")).Success && functionIdentifier == idCharSwap)
                {
                    operations += "w" + m.Groups["index"].Value + " "; //operation is a swap (w)
                }

                if ((m = Regex.Match(line, @"\(\w+,(?<index>\d+)\)")).Success && functionIdentifier == idSlice)
                {
                    operations += "s" + m.Groups["index"].Value + " "; //operation is a slice
                }

                if (functionIdentifier == idReverse) //No regex required for reverse (reverse method has no parameters)
                {
                    operations += "r "; //operation is a reverse
                }
            }

            operations = operations.Trim();

            string magicResult = MagicHands.DecipherWithOperations(encryptedSignatureVideo, operations);
            decryptedVideoUrlResult = HTTPHelperYoutube.ReplaceQueryStringParameter(EncryptUrlForVideo, SignatureQuery, magicResult);
            decryptedUrlForVideo = true;
        }

        public void DoRegexFunctionsForAudio()
        {
            string js = masterURLForAudio;
            //Find "C" in this: var A = B.sig||C (B.s)
            string functNamePattern = @"(\w+)\s*=\s*function\(\s*(\w+)\s*\)\s*{\s*\2\s*=\s*\2\.split\(\""\""\)\s*;(.+)return\s*\2\.join\(\""\""\)\s*}\s*;";

            var funcName = Regex.Match(js, functNamePattern).Groups[1].Value;

            if (funcName.Contains("$"))
            {
                funcName = "\\" + funcName; //Due To Dollar Sign Introduction, Need To Escape
            }

            string funcPattern = @"(?!h\.)" + @funcName + @"=function\(\w+\)\{.*?\}"; //Escape funcName string
            var funcBody = Regex.Match(js, funcPattern, RegexOptions.Singleline).Value; //Entire sig function
            var lines = funcBody.Split(';'); //Each line in sig function

            string idReverse = "", idSlice = "", idCharSwap = ""; //Hold name for each cipher method
            string functionIdentifier = "";
            string operations = "";

            foreach (var line in lines.Skip(1).Take(lines.Length - 2)) //Matches the funcBody with each cipher method. Only runs till all three are defined.
            {
                if (!string.IsNullOrEmpty(idReverse) && !string.IsNullOrEmpty(idSlice) &&
                    !string.IsNullOrEmpty(idCharSwap))
                {
                    break; //Break loop if all three cipher methods are defined
                }

                functionIdentifier = GetFunctionFromLine(line);
                string reReverse = string.Format(@"{0}:\bfunction\b\(\w+\)", functionIdentifier); //Regex for reverse (one parameter)
                string reSlice = string.Format(@"{0}:\bfunction\b\([a],b\).(\breturn\b)?.?\w+\.", functionIdentifier); //Regex for slice (return or not)
                string reSwap = string.Format(@"{0}:\bfunction\b\(\w+\,\w\).\bvar\b.\bc=a\b", functionIdentifier); //Regex for the char swap.

                if (Regex.Match(js, reReverse).Success)
                {
                    idReverse = functionIdentifier; //If def matched the regex for reverse then the current function is a defined as the reverse
                }

                if (Regex.Match(js, reSlice).Success)
                {
                    idSlice = functionIdentifier; //If def matched the regex for slice then the current function is defined as the slice.
                }

                if (Regex.Match(js, reSwap).Success)
                {
                    idCharSwap = functionIdentifier; //If def matched the regex for charSwap then the current function is defined as swap.
                }
            }

            foreach (var line in lines.Skip(1).Take(lines.Length - 2))
            {
                Match m;
                functionIdentifier = GetFunctionFromLine(line);

                if ((m = Regex.Match(line, @"\(\w+,(?<index>\d+)\)")).Success && functionIdentifier == idCharSwap)
                {
                    operations += "w" + m.Groups["index"].Value + " "; //operation is a swap (w)
                }

                if ((m = Regex.Match(line, @"\(\w+,(?<index>\d+)\)")).Success && functionIdentifier == idSlice)
                {
                    operations += "s" + m.Groups["index"].Value + " "; //operation is a slice
                }

                if (functionIdentifier == idReverse) //No regex required for reverse (reverse method has no parameters)
                {
                    operations += "r "; //operation is a reverse
                }
            }

            operations = operations.Trim();

            string magicResult = MagicHands.DecipherWithOperations(encryptedSignatureAudio, operations);

            decryptedAudioUrlResult = HTTPHelperYoutube.ReplaceQueryStringParameter(EncryptUrlForAudio, SignatureQuery, magicResult);
            decryptedUrlForAudio = true;
        }

        [HideInInspector]
        public bool decryptedUrlForVideo = false;
        [HideInInspector]
        public bool decryptedUrlForAudio = false;
        [HideInInspector]
        public string decryptedVideoUrlResult = "";
        [HideInInspector]
        public string decryptedAudioUrlResult = "";


        private static string GetFunctionFromLine(string currentLine)
        {
            Regex matchFunctionReg = new Regex(@"\w+\.(?<functionID>\w+)\("); //lc.ac(b,c) want the ac part.
            Match rgMatch = matchFunctionReg.Match(currentLine);
            string matchedFunction = rgMatch.Groups["functionID"].Value;
            return matchedFunction; //return 'ac'
        }

        public IEnumerator WebGlRequest(Action<string> callback, string id, string host)
        {
            Debug.Log(host + "getvideo.php?videoid=" + id + "&type=Download");
            WWW request = new WWW(host + "getvideo.php?videoid=" + id + "&type=Download");
            yield return request;
            callback.Invoke(request.text);
        }

        public List<VideoInfo> videoInfos;
        public void GetDownloadUrls(Action callback, string videoUrl, YoutubePlayer player)
        {
            if (videoUrl != null) { /*Debug.Log("Youtube: " + videoUrl);*/ } else { /*Debug.Log("Youtube url null!");*/ }
            if (videoUrl == null)
                throw new ArgumentNullException("videoUrl");

//#if UNITY_WSA
//            videoUrl = "https://youtube.com/watch?v=" + videoUrl;
//#else
//            Uri uriResult;
//            bool result = Uri.TryCreate(videoUrl, UriKind.Absolute, out uriResult)
//                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
//            if (!result)
//                videoUrl = "https://youtube.com/watch?v=" + videoUrl;
//#endif


            bool isYoutubeUrl = TryNormalizeYoutubeUrl(videoUrl, out videoUrl);
            if (!isYoutubeUrl)
            {
                throw new ArgumentException("URL is not a valid youtube URL!");
            }
            StartCoroutine(DownloadYoutubeUrl(videoUrl, callback, player));
            
        }

        void YoutubeURLDownloadFinished(Action callback, YoutubePlayer player)
        {
            if (downloadYoutubeUrlResponse.isValid)
            {
                if (IsVideoUnavailable(downloadYoutubeUrlResponse.data)) { throw new VideoNotAvailableException(); }

                try
                {
                    
                    var dataRegex = new Regex(@"ytplayer\.config\s*=\s*(\{.+?\});", RegexOptions.Multiline);
                    string extractedJson = dataRegex.Match(downloadYoutubeUrlResponse.data).Result("$1");
                    JObject json = JObject.Parse(extractedJson);
                    string videoTitle = GetVideoTitle(json);
                    IEnumerable<ExtractionInfo> downloadUrls = ExtractDownloadUrls(json);
                    List<VideoInfo> infos = GetVideoInfos(downloadUrls, videoTitle).ToList();
                    Html5PlayerResult htmlPlayerVersion = GetHtml5PlayerVersion(json);
                    if (htmlPlayerVersion.isValid)
                    {
                        videoInfos = infos;
                        foreach(VideoInfo info in videoInfos)
                        {
                            info.HtmlPlayerVersion = htmlPlayerVersion.result;
                        }
                        callback.Invoke();
                    }

                }
                catch (Exception e)
                {
                    Debug.Log("Resolver Exception!: " + e.Message);
                    //string filePath = Application.persistentDataPath + "/log_download_exception_" + DateTime.Now.ToString("ddMMyyyyhhmmssffff") + ".txt";
                    //Debug.Log("DownloadUrl content saved to " + filePath);
                    //File.WriteAllText(filePath, downloadUrlResponse.data);
                    Debug.Log("retry!");
                    if(player != null)
                    {
                        player.RetryPlayYoutubeVideo();
                    }
                    else
                    {
                        Debug.LogError("Connection to Youtube Server Error! Try Again");
                    }
                }
            }
        }


        public static bool TryNormalizeYoutubeUrl(string url, out string normalizedUrl)
        {
            url = url.Trim();

            url = url.Replace("youtu.be/", "youtube.com/watch?v=");
            url = url.Replace("www.youtube", "youtube");
            url = url.Replace("youtube.com/embed/", "youtube.com/watch?v=");

            if (url.Contains("/v/"))
            {
                url = "https://youtube.com" + new Uri(url).AbsolutePath.Replace("/v/", "/watch?v=");
            }

            url = url.Replace("/watch#", "/watch?");

            IDictionary<string, string> query = HTTPHelperYoutube.ParseQueryString(url);

            string v;

            if (!query.TryGetValue("v", out v))
            {
                normalizedUrl = null;
                return false;
            }

            normalizedUrl = "https://youtube.com/watch?v=" + v;

            return true;
        }

        private static IEnumerable<ExtractionInfo> ExtractDownloadUrls(JObject json)
        {
            string[] splitByUrls = GetStreamMap(json).Split(',');
            string[] adaptiveFmtSplitByUrls = GetAdaptiveStreamMap(json).Split(',');
            splitByUrls = splitByUrls.Concat(adaptiveFmtSplitByUrls).ToArray();

            foreach (string s in splitByUrls)
            {
                IDictionary<string, string> queries = HTTPHelperYoutube.ParseQueryString(s);
                string url;

                bool requiresDecryption = false;

                if (queries.ContainsKey("s") || queries.ContainsKey("sig"))
                {
                    requiresDecryption = queries.ContainsKey("s");
                    string signature = queries.ContainsKey("s") ? queries["s"] : queries["sig"];

                    url = string.Format("{0}&{1}={2}", queries["url"], SignatureQuery, signature);

                    string fallbackHost = queries.ContainsKey("fallback_host") ? "&fallback_host=" + queries["fallback_host"] : String.Empty;

                    url += fallbackHost;
                }

                else
                {
                    url = queries["url"];
                }

                url = HTTPHelperYoutube.UrlDecode(url);
                url = HTTPHelperYoutube.UrlDecode(url);

                IDictionary<string, string> parameters = HTTPHelperYoutube.ParseQueryString(url);
                if (!parameters.ContainsKey(RateBypassFlag))
                    url += string.Format("&{0}={1}", RateBypassFlag, "yes");

                yield return new ExtractionInfo { RequiresDecryption = requiresDecryption, Uri = new Uri(url) };
            }
        }

        private static string GetAdaptiveStreamMap(JObject json)
        {
            JToken streamMap = json["args"]["adaptive_fmts"];

            // bugfix: adaptive_fmts is missing in some videos, use url_encoded_fmt_stream_map instead
            if (streamMap == null)
            {
                streamMap = json["args"]["url_encoded_fmt_stream_map"];
            }

            return streamMap.ToString();
        }

        public class Html5PlayerResult
        {
            public string scriptName;
            public string result;
            public bool isValid = false;
            public Html5PlayerResult(string _script, string _result, bool _valid)
            {
                scriptName = _script;
                result = _result;
                isValid = _valid;
            }
        }
        private static Html5PlayerResult GetHtml5PlayerVersion(JObject json)
        {
            var regex = new Regex(@"player(.+?).js");
            string js = json["assets"]["js"].ToString();
            Match m = regex.Match(js);
            if (!m.Success)
            {
                var regex2 = new Regex(@"player_remote_ux-(.+?).js");
                Match m2 = regex2.Match(js);
                if (m2.Success) { return new Html5PlayerResult("player_remote_ux", regex2.Match(js).Result("$1"), true); }
                else { return new Html5PlayerResult("", "", false); }
            }
            else { return new Html5PlayerResult("player", regex.Match(js).Result("$1"), true); }
        }

        private static string GetStreamMap(JObject json)
        {
            JToken streamMap = json["args"]["url_encoded_fmt_stream_map"];

            string streamMapString = streamMap == null ? null : streamMap.ToString();

            if (streamMapString == null || streamMapString.Contains("been+removed"))
            {
                throw new VideoNotAvailableException("Video is removed or has an age restriction.");
            }

            return streamMapString;
        }

        private static IEnumerable<VideoInfo> GetVideoInfos(IEnumerable<ExtractionInfo> extractionInfos, string videoTitle)
        {
            var downLoadInfos = new List<VideoInfo>();

            foreach (ExtractionInfo extractionInfo in extractionInfos)
            {
                string itag = HTTPHelperYoutube.ParseQueryString(extractionInfo.Uri.Query)["itag"];

                int formatCode = int.Parse(itag);

                VideoInfo info = VideoInfo.Defaults.SingleOrDefault(videoInfo => videoInfo.FormatCode == formatCode);

                if (info != null)
                {
                    info = new VideoInfo(info)
                    {
                        DownloadUrl = extractionInfo.Uri.ToString(),
                        Title = videoTitle,
                        RequiresDecryption = extractionInfo.RequiresDecryption
                    };
                }

                else
                {
                    info = new VideoInfo(formatCode)
                    {
                        DownloadUrl = extractionInfo.Uri.ToString()
                    };
                }

                downLoadInfos.Add(info);
            }

            return downLoadInfos;
        }

        private static string GetVideoTitle(JObject json)
        {
            JToken title = json["args"]["title"];

            return title == null ? String.Empty : title.ToString();
        }

        private static bool IsVideoUnavailable(string pageSource)
        {
            const string unavailableContainer = "<div id=\"watch-player-unavailable\">";

            return pageSource.Contains(unavailableContainer);
        }

        [HideInInspector]
        public string EncryptUrlForVideo;
        [HideInInspector]
        public string EncryptUrlForAudio;

        private class DownloadUrlResponse
        {
            public string data = null;
            public bool isValid = false;
            public long httpCode = 0;
            public DownloadUrlResponse() { data = null; isValid = false; httpCode = 0; }
        }
        private DownloadUrlResponse downloadYoutubeUrlResponse;
        bool downloadString = false;


        IEnumerator DownloadUrl(string url, Action<string> callback, VideoInfo videoInfo)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:10.0) Gecko/20100101 Firefox/10.0 (Chrome)");
            yield return request.Send();
            if (request.isNetworkError) { Debug.Log("Youtube UnityWebRequest isNetworkError!"); }
            else if (request.isHttpError) { Debug.Log("Youtube UnityWebRequest isHttpError!"); }
            else if (request.responseCode == 200)
            {
                //FinishDecrypt(callback, videoInfo, request.downloadHandler.text);
            }
            else
            { Debug.Log("Youtube UnityWebRequest responseCode:" + request.responseCode); }
        }

        IEnumerator DownloadYoutubeUrl(string url, Action callback, YoutubePlayer player)
        {
            downloadYoutubeUrlResponse = new DownloadUrlResponse();
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:10.0) Gecko/20100101 Firefox/10.0 (Chrome)");
            yield return request.Send();
            downloadYoutubeUrlResponse.httpCode = request.responseCode;
            if (request.isNetworkError) { Debug.Log("Youtube UnityWebRequest isNetworkError!"); }
            else if (request.isHttpError) { Debug.Log("Youtube UnityWebRequest isHttpError!"); }
            else if (request.responseCode == 200)
            {

                //Debug.Log("Youtube UnityWebRequest responseCode 200: OK!");
                if (request.downloadHandler != null && request.downloadHandler.text != null)
                {
                    if (request.downloadHandler.isDone)
                    {
                        downloadYoutubeUrlResponse.isValid = true;
                        downloadYoutubeUrlResponse.data = request.downloadHandler.text;
                    }
                }
                else { Debug.Log("Youtube UnityWebRequest Null response"); }
            }
            else
            { Debug.Log("Youtube UnityWebRequest responseCode:" + request.responseCode); }
            YoutubeURLDownloadFinished(callback, player);
        }

        private static void ThrowYoutubeParseException(Exception innerException, string videoUrl)
        {
            throw new YoutubeParseException("Could not parse the Youtube page for URL " + videoUrl + "\n" +
                                            "This may be due to a change of the Youtube page structure.\n" +
                                            "Please report this bug at kelvinparkour@gmail.com with a subject message 'Parse Error' ", innerException);
        }

        private class ExtractionInfo
        {
            public bool RequiresDecryption { get; set; }

            public Uri Uri { get; set; }
        }
    }
}