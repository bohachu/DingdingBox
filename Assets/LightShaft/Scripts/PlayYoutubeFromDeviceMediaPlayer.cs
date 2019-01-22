using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YoutubeLight;
using System;
using SimpleJSON;
using UnityEngine.Events;
using System.Threading;
using UnityEngine.Networking;

public class PlayYoutubeFromDeviceMediaPlayer : RequestResolver {

    RequestResolver resolver;
    public string videoUrl;
    public bool getFromWebServer = false;

    public bool playOnStart;

    public UnityEvent VideoFinished;

    private void Start()
    {
        resolver = gameObject.AddComponent<RequestResolver>();
        if (playOnStart)
            PlayVideo(videoUrl);
    }

    public void PlayVideo(string url)
    {
        CheckVideoUrlAndExtractThevideoId(videoUrl);
        if (!getFromWebServer)
            resolver.GetDownloadUrls(FinishLoadingUrls, url, null);
        else
            StartCoroutine(NewRequest(url));
        
    }

    void FinishLoadingUrls()
    {
        List<VideoInfo> videoInfos = resolver.videoInfos;
        foreach (VideoInfo info in videoInfos)
        {
            if (info.VideoType == VideoType.Mp4 && info.Resolution == (360))
            {
                if (info.RequiresDecryption)
                {
                    //The string is the video url
                    DecryptDownloadUrl(info.DownloadUrl, info.HtmlPlayerVersion, false);
                    break;
                }
                else
                {
                    StartCoroutine(Play(info.DownloadUrl));
                }
                break;
            }
        }
    }

    public void DecryptionFinished(string url)
    {
        StartCoroutine(Play(url));
    }

    IEnumerator Play(string url)
    {
        Debug.Log("Play!");
#if UNITY_IPHONE || UNITY_ANDROID
        Handheld.PlayFullScreenMovie(url, Color.black, FullScreenMovieControlMode.Full, FullScreenMovieScalingMode.Fill);
#else
        Debug.Log("This only runs in mobile");
#endif
        yield return new WaitForSeconds(1);

        if(VideoFinished != null)
            VideoFinished.Invoke();
    }

    private const string serverURI = "https://unity-dev-youtube.herokuapp.com/api/info?url=https://www.youtube.com/watch?v=";
    private const string formatURI = "&format=best&flatten=true";
    public YoutubeResultIds newRequestResults;

    IEnumerator NewRequest(string videoID)
    {
        WWW request = new WWW(serverURI + "" + videoID + "" + formatURI);
        yield return request;
        var requestData = JSON.Parse(request.text);
        var videos = requestData["videos"][0]["formats"];
        newRequestResults.bestFormatWithAudioIncluded = requestData["videos"][0]["url"];

        videoUrl = newRequestResults.bestFormatWithAudioIncluded;
#if UNITY_WEBGL
        //videoUrl = ConvertToWebglUrl(videoUrl);
        //audioVideoUrl = ConvertToWebglUrl(audioVideoUrl);
#endif
        StartCoroutine(Play(videoUrl));
    }

    private string CheckVideoUrlAndExtractThevideoId(string url)
    {
        if (url.Contains("?t="))
        {
            int last = url.LastIndexOf("?t=");
            string copy = url;
            string newString = copy.Remove(0, last);
            newString = newString.Replace("?t=", "");
            url = url.Remove(last);
        }

        if (!url.Contains("youtu"))
        {
            url = "youtube.com/watch?v=" + url;
        }

        bool isYoutubeUrl = TryNormalizeYoutubeUrlLocal(url, out url);
        if (!isYoutubeUrl)
        {
            Debug.LogError("ITS NOT A YOUTUBE URL");
        }

        return url;
    }

    private bool TryNormalizeYoutubeUrlLocal(string url, out string normalizedUrl)
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

    public void DecryptDownloadUrl(string encryptedUrl, string htmlVersion, bool audioDecryption)
    {
        if (audioDecryption)
            EncryptUrlForAudio = encryptedUrl;
        else
            EncryptUrlForVideo = encryptedUrl;

        IDictionary<string, string> queries = HTTPHelperYoutube.ParseQueryString(encryptedUrl);
        if (queries.ContainsKey(SignatureQuery))
        {
            if (audioDecryption)
                encryptedSignatureAudio = queries[SignatureQuery];
            else
                encryptedSignatureVideo = queries[SignatureQuery];

            //decrypted = GetDecipheredSignature( encryptedSignature);
            //MagicHands.DecipherWithVersion(encryptedSignature, videoInfo.HtmlPlayerVersion);
            //string jsUrl = string.Format("http://s.ytimg.com/yts/jsbin/{0}-{1}.js", videoInfo.HtmlscriptName, videoInfo.HtmlPlayerVersion);
            string jsUrl = string.Format("http://s.ytimg.com/yts/jsbin/player{0}.js", htmlVersion);
            if (audioDecryption)
                StartCoroutine(Downloader(jsUrl, true));
            else
                StartCoroutine(Downloader(jsUrl, false));

        }

    }

    private void FixedUpdate()
    {
        if (decryptedUrlForVideo)
        {
            decryptedUrlForVideo = false;
            DecryptionFinished(decryptedVideoUrlResult);
        }
    }

    private Thread thread1;
    public void ReadyForExtract(string r, bool audioExtract)
    {
        if (audioExtract)
        {
            SetMasterUrlForAudio(r);
#if UNITY_WEBGL
            DoRegexFunctionsForAudio();
#else
            if (SystemInfo.processorCount > 1)
            {
                thread1 = new Thread(DoRegexFunctionsForAudio);
                thread1.Start();
            }
            else
            {
                DoRegexFunctionsForAudio();
            }
#endif


        }
        else
        {
            SetMasterUrlForVideo(r);
#if UNITY_WEBGL
            DoRegexFunctionsForVideo();
#else
            if (SystemInfo.processorCount > 1)
            {
                thread1 = new Thread(DoRegexFunctionsForVideo);
                thread1.Start();
            }
            else
            {
                DoRegexFunctionsForVideo();
            }
#endif

        }

    }

    IEnumerator Downloader(string uri, bool audio)
    {
        UnityWebRequest request = UnityWebRequest.Get(uri);
        request.SetRequestHeader("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:10.0) Gecko/20100101 Firefox/10.0 (Chrome)");
        yield return request.Send();
        ReadyForExtract(request.downloadHandler.text, audio);
    }

}
