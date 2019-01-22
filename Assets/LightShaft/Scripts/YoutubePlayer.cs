using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using YoutubeLight;
using SimpleJSON;
using System.Text;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.Networking;

public class YoutubePlayer : RequestResolver
{
    #region ENUMS
    public enum YoutubeVideoQuality
    {
        LOW,
        HD,
        FULLHD,
        UHD1440,
        UHD2160
    }
    #endregion

    #region PUBLIC VARIABLES
    [Space]
    [Tooltip("You can put urls that start at a specific time example: 'https://youtu.be/1G1nCxxQMnA?t=67'")]
    public string youtubeUrl;

    [Space]
    [Tooltip("The desired video quality you want to play.")]
    public YoutubeVideoQuality videoQuality;

    [Space]
    [Tooltip("Start playing the video from a desired time")]
    public bool startFromSecond = false;
    [DrawIf("startFromSecond", true)]
    public int startFromSecondTime = 0;

    [Space]
    [Tooltip("Play the video when the script initialize")]
    public bool autoPlayOnStart = true;

    [Space]
    [Tooltip("Play or continue when OnEnable is called")]
    public bool autoPlayOnEnable = false;

    [Space]
    [Tooltip("If you want to use your custom player, you can enable this and set the callback OnYoutubeUrlLoaded and get the public variables audioUrl or videoUrl of that script.")]
    public bool loadYoutubeUrlsOnly = false;

    [Space]
    [Tooltip("If the video is a 3D video sidebyside or Over/Under")]
    public bool is3DLayoutVideo = false;

    [DrawIf("is3DLayoutVideo", true)]
    public Layout3D layout3d;

    public enum Layout3D
    {
        sideBySide,
        OverUnder
    }

    [Space]
    [Header("Video Controller Canvas")]
    public GameObject videoControllerCanvas;

    [Space]
    public Camera mainCamera;

    [Space]
    [Header("Loading Settings")]
    [Tooltip("This enable and disable related to the loading needs.")]
    public GameObject loadingContent;

    [Header("Custom user Events")]
    //User callbacks
    [Tooltip("When the url's are loaded")]
    public UnityEvent OnYoutubeUrlAreReady;
    [Tooltip("When the videos are ready to play")]
    public UnityEvent OnVideoReadyToStart;
    [Tooltip("When the video start playing")]
    public UnityEvent OnVideoStarted;
    [Tooltip("When the video pause")]
    public UnityEvent OnVideoPaused;
    [Tooltip("When the video finish")]
    public UnityEvent OnVideoFinished;

    [Space]
    [Header("Render the same video to more objects")]
    [Tooltip("Render the same video player material to a different materials, if you want")]
    public GameObject[] objectsToRenderTheVideoImage;

    [Space]
    [Header("The unity video players")][Tooltip("The unity video player")]
    public VideoPlayer videoPlayer;

    [Tooltip("The audio player, (Needed for videos that dont have audio included 720p+)")]
    public VideoPlayer audioPlayer;

    [Space]
    [Tooltip("Show the output in the console")]
    public bool debug;
    

    //Youtube formated urls
    [HideInInspector]
    public string videoUrl;
    [HideInInspector]
    private string audioUrl;

    [Space]
    public bool ForceGetWebServer = false;

    [Space]
    [Tooltip("Show the video controller in screen [slider with progress, video time, play pause, etc...]")]
    public bool showPlayerControls = false;

    #endregion

    #region PRIVATE VARIABLES
    //Request from youtube url timeout
    private int maxRequestTime = 5;
    private float currentRequestTime;
    //When the video fails how much time we will try until try to get from the webserver system.
    private int retryTimeUntilToRequestFromServer = 1;
    private int currentRetryTime = 0;

    //Check when we are trying to get the url
    private bool gettingYoutubeURL = false;

    //When the urls are done and the video are ready to start playing
    private bool videoAreReadyToPlay = false;
    
    //The system that get the urls from youtube.
    RequestResolver resolver;

    private float lastPlayTime;

    //When a video needs decryption, most common in music videos
    private bool audioDecryptDone = false;
    private bool videoDecryptDone = false;

    private bool checkIfVideoArePrepared = false;
    private float lastVideoReadyToPlay = 0;

    //Video ready checkers
    private bool videoPrepared;
    private bool audioPrepared;

    //Retry checker
    private bool isRetry = false;

    YoutubeVideoQuality lastTryQuality = YoutubeVideoQuality.UHD2160;
    private string lastTryVideoId;

    private float lastStartedTime;
    private bool youtubeUrlReady = false;

    #endregion

    #region SERVER VARIABLES

    private YoutubeResultIds newRequestResults;

    /*PRIVATE INFO DO NOT CHANGE THESE URLS OR VALUES, ONLY IF YOU WANT HOST YOUR OWN SERVER| TURORIALS IN THE PROJECT FILES*/
    private const string serverURI = "https://unity-dev-youtube.herokuapp.com/api/info?url=";
    private const string formatURI = "&format=best&flatten=true";
    private const string VIDEOURIFORWEBGLPLAYER = "https://youtubewebgl.herokuapp.com/download.php?mime=video/mp4&title=generatedvideo&token=";
    /*END OF PRIVATE INFO*/

    #endregion

    #region Unity Functions
    public void Start()
    {
        FixCameraEvent();
        Skybox3DSettup();
        videoPlayer.skipOnDrop = true;
        audioPlayer.skipOnDrop = true;

#if UNITY_WEBGL
        ForceGetWebServer = true;
#endif

        PrepareVideoPlayerCallbacks();
        CheckRequestResolver();

        if (autoPlayOnStart)
        {

            PlayYoutubeVideo(youtubeUrl);
        }

        //VideoController Area
        if (videoQuality == YoutubeVideoQuality.LOW)
            lowRes = true;
        else
            lowRes = false;


        
    }

    private void TryToLoadThumbnailBeforeOpenVideo(string id)
    {
        string tempId = id.Replace("https://youtube.com/watch?v=", "");
        StartCoroutine(DownloadThumbnail(tempId));
    }

    IEnumerator DownloadThumbnail(string videoId)
    {
        WWW www = new WWW("https://img.youtube.com/vi/" + videoId + "/0.jpg");
        yield return www;
        Texture2D thumb = www.texture;
        videoPlayer.targetMaterialRenderer.material.mainTexture = thumb;
    }

    private void Skybox3DSettup()
    {
        if (is3DLayoutVideo)
        {
            if(layout3d == Layout3D.OverUnder)
            {
                RenderSettings.skybox = (Material)Resources.Load("Materials/PanoramicSkybox3DOverUnder") as Material;
            }else if( layout3d == Layout3D.sideBySide)
            {
                RenderSettings.skybox = (Material)Resources.Load("Materials/PanoramicSkybox3Dside") as Material;
            }
        }
    }

    private void FixCameraEvent()
    {
        if(mainCamera == null)
        {
            if (Camera.main != null)
                mainCamera = Camera.main;
            else
                Debug.LogError("Add the main camera to the mainCamera field");
        }

        videoControllerCanvas.GetComponent<Canvas>().worldCamera = mainCamera;
        if (videoPlayer.renderMode == VideoRenderMode.CameraFarPlane || videoPlayer.renderMode == VideoRenderMode.CameraNearPlane)
            videoPlayer.targetCamera = mainCamera;
    }

    //A workaround for mobile bugs.
    private void OnApplicationPause(bool pause)
    {
        if (videoPlayer.isPrepared)
        {
            if(audioPlayer != null)
                audioPlayer.Pause();

            videoPlayer.Pause();
        }
        
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus == true)
        {
            if (videoPlayer.isPrepared)
            {
                if(audioPlayer != null)
                    audioPlayer.Play();

                videoPlayer.Play();
            }
            
        }
    }

    private void OnEnable()
    {
        if (autoPlayOnEnable)
        {
            StartCoroutine(WaitThingsGetDone());
        }
    }

    IEnumerator WaitThingsGetDone()
    {
        yield return new WaitForSeconds(1);
        if (youtubeUrlReady && videoPlayer.isPrepared)
        {
            if (videoQuality == YoutubeVideoQuality.LOW)
                videoPlayer.Play();
            else
            {
                audioPlayer.Play();
                videoPlayer.Play();
            }
        }
        else
        {
            if (!youtubeUrlReady)
                LoadYoutubeVideo(youtubeUrl);

        }

    }

    void FixedUpdate()
    {
        if (gettingYoutubeURL)
        {
            currentRequestTime += Time.deltaTime;
            if (currentRequestTime >= maxRequestTime)
            {
                gettingYoutubeURL = false;
                if(debug)
                    Debug.Log("<color=blue>Max time reached, trying again!</color>");
                Debug.Break();
            }
        }

        //used this to play in main thread.
        if (videoAreReadyToPlay)
        {
            videoAreReadyToPlay = false;

            
        }

        ErrorCheck();

        //Video controller area
        if (showPlayerControls)
        {
            if (videoQuality != YoutubeVideoQuality.LOW)
                lowRes = false;
            else
                lowRes = true;

            if (currentTimeString != null && totalTimeString != null)
            {
                currentTimeString.text = FormatTime(Mathf.RoundToInt(currentVideoDuration));
                if (!lowRes)
                {
                    if (audioDuration < totalVideoDuration && (audioPlayer.url != ""))
                        totalTimeString.text = FormatTime(Mathf.RoundToInt(audioDuration));
                    else
                        totalTimeString.text = FormatTime(Mathf.RoundToInt(totalVideoDuration));
                }
                else
                {
                    totalTimeString.text = FormatTime(Mathf.RoundToInt(totalVideoDuration));
                }
               
            }
        }

        if (!showPlayerControls)
        {
            if(mainControllerUi != null)
                mainControllerUi.SetActive(false);
        }
        else
            mainControllerUi.SetActive(true);
        //End video controller area

        if (decryptedUrlForAudio)
        {
            decryptedUrlForAudio = false;
            DecryptAudioDone(decryptedAudioUrlResult);
        }

        if (decryptedUrlForVideo)
        {
            decryptedUrlForVideo = false;
            DecryptVideoDone(decryptedVideoUrlResult);
        }

        //webgl
        if (videoPlayer.isPrepared)
        {
            if (!startedPlayingWebgl)
            {
                logTest = videoUrl + " Let's play";
                startedPlayingWebgl = true;
                StartCoroutine(WebGLPlay());
            }
        }

    }

    #endregion

    #region ASSET FUNCTIONS

    private void CheckRequestResolver()
    {
        //if (gameObject.GetComponent<RequestResolver>() == null)
            resolver = this;
    }

    private void PrepareVideoPlayerCallbacks()
    {
        videoPlayer.started += VideoStarted;
        videoPlayer.errorReceived += VideoErrorReceived;
        videoPlayer.frameDropped += VideoFrameDropped;
    }


    private void ShowLoading()
    {
        if(loadingContent != null)
            loadingContent.SetActive(true);
    }

    private void HideLoading()
    {
        if(loadingContent != null)
            loadingContent.SetActive(false);
    }

    public void LoadYoutubeVideo(string url)
    {
        PlayYoutubeVideo(url);
    }

    private string CheckVideoUrlAndExtractThevideoId(string url)
    {
        if (url.Contains("?t="))
        {
            int last = url.LastIndexOf("?t=");
            string copy = url;
            string newString = copy.Remove(0, last);
            newString = newString.Replace("?t=","");
            startFromSecond = true;
            startFromSecondTime = int.Parse(newString);
            url = url.Remove(last);
        }

        if (!url.Contains("youtu"))
        {
            url = "youtube.com/watch?v=" + url;
        }

        bool isYoutubeUrl = TryNormalizeYoutubeUrlLocal(url,out url);
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

    private void PlayYoutubeVideo(string _videoId)
    {
        _videoId = CheckVideoUrlAndExtractThevideoId(_videoId);
        //Thumbnail
        if (showThumbnailBeforeVideoLoad)
            TryToLoadThumbnailBeforeOpenVideo(_videoId);
        youtubeUrlReady = false;
        //Show loading
        ShowLoading();

        youtubeUrl = _videoId;
        //loading for fist time, so it's not a retry
        isRetry = false;
        //store some variables to control
        lastTryQuality = videoQuality;
        lastTryVideoId = _videoId;
        lastPlayTime = Time.time;
        lastVideoReadyToPlay = 0;

#if UNITY_WEBGL
        StartCoroutine(WebGlRequest(youtubeUrl));
#else
        if (!ForceGetWebServer)
        {
            currentRequestTime = 0;
            gettingYoutubeURL = true;
            resolver.GetDownloadUrls(UrlsLoaded, youtubeUrl, this);
        }
        else
            StartCoroutine(WebRequest(youtubeUrl));

        
#endif
    }

    //When the audio decryption is done
    public void DecryptAudioDone(string url)
    {
        audioUrl = url;
        audioDecryptDone = true;

        if (videoDecryptDone)
        {
            videoAreReadyToPlay = true;
            
            OnYoutubeUrlsLoaded();
        }
            
    }

    //When the Video decryption is done
    public void DecryptVideoDone(string url)
    {
        videoUrl = url;
        videoDecryptDone = true;

        if (audioDecryptDone) {
            videoAreReadyToPlay = true;
            OnYoutubeUrlsLoaded();
        }
        else
        {
            if(videoQuality == YoutubeVideoQuality.LOW)
            {
                videoAreReadyToPlay = true;
                OnYoutubeUrlsLoaded();
            }
        }

            
    }

    //The callback when the url's are loaded.
    private void UrlsLoaded()
    {
        gettingYoutubeURL = false;
        List<VideoInfo> videoInfos = resolver.videoInfos;
        videoDecryptDone = false;
        audioDecryptDone = false;

        decryptedUrlForVideo = false;
        decryptedUrlForAudio = false;

        if(videoQuality == YoutubeVideoQuality.LOW)
        {
            //Get the video with audio first
            foreach (VideoInfo info in videoInfos)
            {
                if (info.VideoType == VideoType.Mp4 && info.Resolution == (360))
                {
                    if (info.RequiresDecryption)
                    {
                        //The string is the video url with audio
                        DecryptDownloadUrl(info.DownloadUrl, info.HtmlPlayerVersion, false);
                    }
                    else
                    {
                        videoUrl = info.DownloadUrl;
                        videoAreReadyToPlay = true;
                        OnYoutubeUrlsLoaded();
                    }
                    break;
                }
            }
        }
        else
        {
            //Get the video with audio first
            foreach (VideoInfo info in videoInfos)
            {
                if (info.VideoType == VideoType.Mp4 && info.Resolution == (360))
                {

                    if (info.RequiresDecryption)
                    {
                        //The string is the video url with audio
                        DecryptDownloadUrl(info.DownloadUrl, info.HtmlPlayerVersion, true);
                    }
                    else
                    {
                        audioUrl = info.DownloadUrl;
                    }
                    break;
                }
            }

            //Then we will get the desired video quality.
            int quality = 360;
            switch (videoQuality)
            {
                case YoutubeVideoQuality.LOW:
                    quality = 360;
                    break;
                case YoutubeVideoQuality.HD:
                    quality = 720;
                    break;
                case YoutubeVideoQuality.FULLHD:
                    quality = 1080;
                    break;
                case YoutubeVideoQuality.UHD1440:
                    quality = 1440;
                    break;
                case YoutubeVideoQuality.UHD2160:
                    quality = 2160;
                    break;
            }

            bool foundVideo = false;
            //Get the high quality video
            foreach (VideoInfo info in videoInfos)
            {
                if (info.VideoType == VideoType.Mp4 && info.Resolution == (quality))
                {
                    if (info.RequiresDecryption)
                    {
                        if (debug)
                            Debug.Log("REQUIRE DECRYPTION!");
                        //The string is the video url
                        DecryptDownloadUrl(info.DownloadUrl, info.HtmlPlayerVersion, false);
                    }
                    else
                    {
                        videoUrl = info.DownloadUrl;
                        videoAreReadyToPlay = true;
                        OnYoutubeUrlsLoaded();
                    }
                    foundVideo = true;

                    if (info.AudioType != YoutubeLight.AudioType.Unknown)//there's no audio atacched.
                    {
                        audioPlayer.audioOutputMode = VideoAudioOutputMode.None;
                    }
                    else
                    {
                        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
                    }

                    break;
                }
            }

            if (!foundVideo && quality == 2160)
            {
                foreach (VideoInfo info in videoInfos)
                {
                    if (info.FormatCode == 313)
                    {
                        if (debug)
                            Debug.Log("Found but with unknow format in results, check to see if the video works normal.");
                        if (info.RequiresDecryption)
                        {
                            //The string is the video url
                            DecryptDownloadUrl(info.DownloadUrl, info.HtmlPlayerVersion, false);
                        }
                        else
                        {
                            videoUrl = info.DownloadUrl;
                            videoAreReadyToPlay = true;
                            OnYoutubeUrlsLoaded();
                        }
                        foundVideo = true;

                        if (info.AudioType != YoutubeLight.AudioType.Unknown)//there's no audio atacched.
                        {
                            audioPlayer.audioOutputMode = VideoAudioOutputMode.None;
                        }
                        else
                        {
                            videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
                        }

                        break;
                    }
                }
            }

            //if desired quality not found try another lower quality.
            if (!foundVideo)
            {
                if (debug)
                    Debug.Log("Desired quality not found, playing with low quality, check if the video id: " + youtubeUrl + " support that quality!");
                foreach (VideoInfo info in videoInfos)
                {
                    if (info.VideoType == VideoType.Mp4 && info.Resolution == (360))
                    {
                        if (info.RequiresDecryption)
                        {
                            videoQuality = YoutubeVideoQuality.LOW;
                            //The string is the video url
                            DecryptDownloadUrl(info.DownloadUrl, info.HtmlPlayerVersion, false);
                        }
                        else
                        {
                            videoUrl = info.DownloadUrl;
                            videoAreReadyToPlay = true;
                            OnYoutubeUrlsLoaded();
                        }
                        if (info.AudioType != YoutubeLight.AudioType.Unknown)//there's no audio atacched.
                        {
                            audioPlayer.audioOutputMode = VideoAudioOutputMode.None;
                        }
                        else
                        {
                            videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
                        }
                        break;
                    }
                }
            }
        }
    }

    private void LoadPrepareCallbacks()
    {
            videoPrepared = false;
            videoPlayer.prepareCompleted += VideoPrepared;
            if (videoQuality != YoutubeVideoQuality.LOW)
            {
                audioPrepared = false;
                audioPlayer.prepareCompleted += AudioPrepared;
            }
    }

    private void Play()
    {
        videoPlayer.loopPointReached += PlaybackDone;
        StartPlayback();
    }

    private void StartPlayback()
    {
        
        if (videoQuality != YoutubeVideoQuality.LOW)
        {
            audioPlayer.Play();
        }

        videoPlayer.Play();

       HideLoading();

        if (startFromSecond)
            videoPlayer.time = startFromSecondTime;
    }


    private void ErrorCheck()
    {
        if (!ForceGetWebServer)
        {
            if (!isRetry && lastStartedTime < lastErrorTime && lastErrorTime > lastPlayTime)
            {
                if (debug)
                    Debug.Log("Error detected!, retry with low quality!");
                lastTryQuality = YoutubeVideoQuality.LOW;
                RetryPlayYoutubeVideo();
            }
        }
    }

    //It's not in use, maybe can be usefull to you, it's just a test.
    public int GetMaxQualitySupportedByDevice()
    {
        if (Screen.orientation == ScreenOrientation.Landscape)
        {
            //use the height
            return Screen.currentResolution.height;
        }
        else if (Screen.orientation == ScreenOrientation.Portrait)
        {
            //use the width
            return Screen.currentResolution.width;
        }
        else
        {
            return Screen.currentResolution.height;
        }
    }

    IEnumerator WebRequest(string videoID)
    {
        WWW request = new WWW(serverURI + "" + videoID + "" + formatURI);
        yield return request;
        newRequestResults = new YoutubeResultIds();
        var requestData = JSON.Parse(request.text);
        var videos = requestData["videos"][0]["formats"];
        newRequestResults.bestFormatWithAudioIncluded = requestData["videos"][0]["url"];
        for (int counter = 0; counter < videos.Count; counter++)
        {
            if (videos[counter]["format_id"] == "160")
            {
                newRequestResults.lowQuality = videos[counter]["url"];
            }
            else if (videos[counter]["format_id"] == "133")
            {
                newRequestResults.lowQuality = videos[counter]["url"];   //if have 240p quality overwrite the 144 quality as low quality.
            }
            else if (videos[counter]["format_id"] == "134")
            {
                newRequestResults.standardQuality = videos[counter]["url"];  //360p
            }
            else if (videos[counter]["format_id"] == "136")
            {
                newRequestResults.hdQuality = newRequestResults.bestFormatWithAudioIncluded;  //720p
            }
            else if (videos[counter]["format_id"] == "137")
            {
                newRequestResults.fullHdQuality = videos[counter]["url"];  //1080p
            }
            else if (videos[counter]["format_id"] == "266")
            {
                newRequestResults.ultraHdQuality = videos[counter]["url"];  //@2160p 4k
            }
            else if (videos[counter]["format_id"] == "139")
            {
                newRequestResults.audioUrl = videos[counter]["url"];  //AUDIO
            }
        }

        
            audioUrl = newRequestResults.bestFormatWithAudioIncluded;
            videoUrl = newRequestResults.bestFormatWithAudioIncluded;

        switch (videoQuality)
        {
            case YoutubeVideoQuality.LOW:
                videoUrl = newRequestResults.bestFormatWithAudioIncluded;
                break;
            case YoutubeVideoQuality.HD:
                videoUrl = newRequestResults.hdQuality;
                break;
            case YoutubeVideoQuality.FULLHD:
                videoUrl = newRequestResults.fullHdQuality;
                break;
            case YoutubeVideoQuality.UHD1440:
                videoUrl = newRequestResults.fullHdQuality;
                break;
            case YoutubeVideoQuality.UHD2160:
                videoUrl = newRequestResults.ultraHdQuality;
                break;
        }

        if (videoUrl == "")
            videoUrl = newRequestResults.bestFormatWithAudioIncluded;

#if UNITY_WEBGL
        videoUrl = ConvertToWebglUrl(videoUrl);
#endif
        videoAreReadyToPlay = true;
        OnYoutubeUrlsLoaded();
    }

    private string ConvertToWebglUrl(string url)
    {
        byte[] bytesToEncode = Encoding.UTF8.GetBytes(url);
        string encodedText = Convert.ToBase64String(bytesToEncode);
        if (debug)
            Debug.Log(url);
        string newUrl = VIDEOURIFORWEBGLPLAYER + "" + encodedText;
        return newUrl;
    }


    public void RetryPlayYoutubeVideo()
    {
        currentRetryTime++;
        if (currentRetryTime < retryTimeUntilToRequestFromServer)
        {
            if (!ForceGetWebServer)
            {
                StopIfPlaying();
                if (debug)
                    Debug.Log("Youtube Retrying...:" + lastTryVideoId);
                isRetry = true;
                ShowLoading();
                youtubeUrl = lastTryVideoId;
                PlayYoutubeVideo(youtubeUrl);
            }
        }
        else
        {
            currentRetryTime = 0;
            if (debug)
                Debug.Log("Playing From WEbServer. There's a error in the local player.");
            //Get from webserver becuase there's something wrong with the offline system.
            StopIfPlaying();
            ShowLoading();
            ForceGetWebServer = true;
            youtubeUrl = lastTryVideoId;
            PlayYoutubeVideo(youtubeUrl);
        }

    }


    private void StopIfPlaying()
    {
        if (debug)
            Debug.Log("Stopping video");
        if (videoPlayer.isPlaying) { videoPlayer.Stop(); }
        if (audioPlayer.isPlaying) { audioPlayer.Stop(); }
    }
    #endregion

    #region CALLBACKS

    public void OnYoutubeUrlsLoaded()
    {
        youtubeUrlReady = true;
        if (!loadYoutubeUrlsOnly) //If want to load urls only the video will not play
        {
            lastVideoReadyToPlay = Time.time;

            if (debug)
                Debug.Log("Play!!" + videoUrl);

            LoadPrepareCallbacks();
            lastVideoReadyToPlay = Time.time;
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = videoUrl;
            checkIfVideoArePrepared = true;
            videoPlayer.Prepare();
            if (videoQuality != YoutubeVideoQuality.LOW)
            {
                audioPlayer.source = VideoSource.Url;
                audioPlayer.url = audioUrl;
                audioPlayer.Prepare();
            }
        }
        OnYoutubeUrlAreReady.Invoke();
    }

    public void OnYoutubeVideoAreReadyToPlay()
    {
        OnVideoReadyToStart.Invoke();
        Play();
    }

    public void OnVideoPlayerFinished()
    {
        if (videoPlayer.isPrepared)
        {
            if (debug)
                Debug.Log("Finished");
            if (videoPlayer.isLooping)
            {
                videoPlayer.time = 0;
                videoPlayer.frame = 0;
                audioPlayer.time = 0;
                audioPlayer.frame = 0;
                videoPlayer.Play();
                audioPlayer.Play();
            }
            OnVideoFinished.Invoke();
        }
    }

    //UNITY VIDEO PLAYER CALLBACK
    private void AudioPrepared(VideoPlayer vPlayer)
    {
        audioPlayer.prepareCompleted -= AudioPrepared;
        audioPrepared = true;
        if (audioPrepared && videoPrepared)
            OnYoutubeVideoAreReadyToPlay();
    }

    //UNITY VIDEO PLAYER CALLBACK
    private void VideoPrepared(VideoPlayer vPlayer)
    {
        videoPlayer.prepareCompleted -= VideoPrepared;
        videoPrepared = true;
        if (videoQuality == YoutubeVideoQuality.LOW)
        {
            OnYoutubeVideoAreReadyToPlay();
        }
        else
        {
            if (audioPrepared && videoPrepared)
                OnYoutubeVideoAreReadyToPlay();
        }

    }

    //Unity Video player callback
    private void PlaybackDone(VideoPlayer vPlayer)
    {
        finished = true;
        OnVideoPlayerFinished();
    }

    private void VideoFrameDropped(VideoPlayer source)
    {
        if (debug)
            Debug.Log("Youtube VideoFrameDropped!"); //[NOT IMPLEMENTED UNITY 2017.2]
    }

    private void VideoStarted(VideoPlayer source)
    {
        lastStartedTime = Time.time;
        lastErrorTime = lastStartedTime;
        if (debug)
            Debug.Log("Youtube Video Started");

        //Render to more materials
        if(objectsToRenderTheVideoImage.Length > 0)
        {
            foreach (GameObject obj in objectsToRenderTheVideoImage)
            {
                obj.GetComponent<Renderer>().material.mainTexture = videoPlayer.texture;
            }
        }
        
        OnVideoStarted.Invoke();
    }

    float lastErrorTime;
    private void VideoErrorReceived(VideoPlayer source, string message)
    {
        lastErrorTime = Time.time;
        if (debug)
            Debug.Log("Youtube VideoErrorReceived!:" + message);
    }

    public void PlayPause()
    {
        if (youtubeUrlReady && videoPlayer.isPrepared)
        {
            if (videoPlayer.isPlaying)
            {
                if (videoQuality == YoutubeVideoQuality.LOW)
                {
                    videoPlayer.Pause();
                }
                else
                {
                    videoPlayer.Pause();
                    audioPlayer.Pause();
                }
                OnVideoPaused.Invoke();
            }
            else
            {
                if (videoQuality == YoutubeVideoQuality.LOW)
                {
                    videoPlayer.Play();
                }
                else
                {
                    videoPlayer.Play();
                    audioPlayer.Play();
                }
            }
        }
    }

    #endregion

    #region VIDEO CONTROLLER
    [DrawIf("showPlayerControls", true)]
    [Tooltip("Hide the controls if use not interact in desired time, 0 equals to not hide")]
    public int autoHideControlsTime = 0;

    [DrawIf("showPlayerControls", true)]
    [Tooltip("The main controller ui parent")]
    public GameObject mainControllerUi;

    [DrawIf("showPlayerControls", true)]
    [Tooltip("Slider with duration and progress")]
    public Slider progressSlider;


    [DrawIf("showPlayerControls", true)]
    [Tooltip("Volume slider")]
    public Slider volumeSlider;

    [DrawIf("showPlayerControls", true)]
    [Tooltip("Playback speed")]
    public Slider playbackSpeed;

    [DrawIf("showPlayerControls", true)]
    [Tooltip("Current Time")]
    public Text currentTimeString;

    [DrawIf("showPlayerControls", true)]
    [Tooltip("Total Time")]
    public Text totalTimeString;

    private float totalVideoDuration;
    private float currentVideoDuration;
    private bool videoSeekDone = false;
    private bool videoAudioSeekDone = false;
    private bool lowRes;
    private float hideScreenTime = 0;
    private float audioDuration;
    private int savedTime = 0;
    private bool useBackwardSyncImprovementWorkaround = false;
    private bool finished = false;
    private bool showingPlaybackSpeed = false;
    private bool showingVolume = false;


    private void Update()
    {
        if (showPlayerControls)
        {
            if (videoPlayer.isPlaying && progressSlider != null)
            {
                totalVideoDuration = Mathf.RoundToInt(videoPlayer.frameCount / videoPlayer.frameRate);
                if (!lowRes)
                {
                    audioDuration = Mathf.RoundToInt(audioPlayer.frameCount / audioPlayer.frameRate);
                    if (audioDuration < totalVideoDuration && (audioPlayer.url != ""))
                    {
                        currentVideoDuration = Mathf.RoundToInt(audioPlayer.frame / audioPlayer.frameRate);
                        progressSlider.maxValue = audioDuration;
                    }
                    else
                    {
                        currentVideoDuration = Mathf.RoundToInt(videoPlayer.frame / videoPlayer.frameRate);
                        progressSlider.maxValue = totalVideoDuration;
                    }
                }
                else
                {
                    currentVideoDuration = Mathf.RoundToInt(videoPlayer.frame / videoPlayer.frameRate);
                    progressSlider.maxValue = totalVideoDuration;
                }

                progressSlider.Set(currentVideoDuration);
            }

            //if (currentVideoDuration >= totalVideoDuration)
            //{
            //    if (!finished)
            //        Finished();
            //}

            if (autoHideControlsTime > 0)
            {
                if (UserInteract())
                {
                    hideScreenTime = 0;
                    if (mainControllerUi != null)
                        mainControllerUi.SetActive(true);
                }
                else
                {
                    hideScreenTime += Time.deltaTime;
                    if (hideScreenTime >= autoHideControlsTime)
                    {
                        //not increment
                        hideScreenTime = autoHideControlsTime;
                        HideControllers();
                    }
                }
            }
        }
    }

    private void HideControllers()
    {
        if (mainControllerUi != null)
        {
            mainControllerUi.SetActive(false);
            showingVolume = false;
            showingPlaybackSpeed = false;
            volumeSlider.gameObject.SetActive(false);
            playbackSpeed.gameObject.SetActive(false);
        }
    }

    public void Seek()
    {
        isSyncing = true;
        //check if can seek
        if (Mathf.RoundToInt(currentVideoDuration) != Mathf.RoundToInt(totalVideoDuration))
        {
            finished = false;
            if (videoPlayer.canSetTime && videoPlayer.canStep)
            {
                ShowLoading();
                //Pause the video to prevent audio error
                //workaround related to the unity but, when you seek backward the video there's a big delay to seek:
                if (Application.isMobilePlatform)
                {
                    float currentTime = (float)videoPlayer.time;
                    if (Mathf.RoundToInt(progressSlider.value) > currentTime)
                    {
                        videoPlayer.Pause();
                        if(!lowRes)
                            audioPlayer.Pause();
                    }
                    else
                    {
                        if (useBackwardSyncImprovementWorkaround)
                        {
                            videoPlayer.Stop();
                            if (!lowRes)
                                audioPlayer.Stop();

                            savedTime = Mathf.RoundToInt(progressSlider.value);
                            StartCoroutine(WorkAroundToUnityBackwardSeek());
                        }
                        else
                        {
                            videoPlayer.Pause();
                            if (!lowRes)
                                audioPlayer.Pause();
                        }
                    }
                }
                else
                {
                    videoPlayer.Pause();
                    if (!lowRes)
                        audioPlayer.Pause();
                }


                //reset variables
                videoSeekDone = false;
                videoAudioSeekDone = false;
                if (!lowRes)
                {
                    //callbacks
                    audioPlayer.seekCompleted += SeekVideoAudioDone;
                    videoPlayer.seekCompleted += SeekVideoDone;
                    //change the time
                    if (Mathf.RoundToInt(progressSlider.value) == 0)
                    {
                        audioPlayer.time = 1;
                        videoPlayer.time = 1;
                    }
                    else
                    {
                        audioPlayer.time = Mathf.RoundToInt(progressSlider.value);
                        videoPlayer.time = Mathf.RoundToInt(progressSlider.value);
                    }

                }
                else
                {
                    //callback
                    videoPlayer.seekCompleted += SeekVideoDone;
                    if (Mathf.RoundToInt(progressSlider.value) == 0)
                        videoPlayer.time = 1;
                    else
                        videoPlayer.time = Mathf.RoundToInt(progressSlider.value);
                }
            }
        }
        else
        {
            //			if (sourceVideo.isPlaying) {
            //				if (!finished)
            //					Finished();
            //			}
        }
    }


    public void Finished()
    {
        finished = true;
        OnVideoPlayerFinished();
    }

    public void Volume()
    {
        if (videoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
            audioPlayer.SetDirectAudioVolume(0, volumeSlider.value);
        else if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
            videoPlayer.GetComponent<AudioSource>().volume = volumeSlider.value;
        else
            videoPlayer.GetComponent<AudioSource>().volume = volumeSlider.value;
    }

    public void Speed()
    {
        if (videoPlayer.canSetPlaybackSpeed)
        {
            if (playbackSpeed.value == 0)
            {
                videoPlayer.playbackSpeed = 0.5f;
                audioPlayer.playbackSpeed = 0.5f;
            }
            else
            {
                videoPlayer.playbackSpeed = playbackSpeed.value;
                audioPlayer.playbackSpeed = playbackSpeed.value;
            }
        }
    }

    public void PlayButton()
    {
        if (!videoPlayer.isPlaying)
        {

#if !UNITY_WEBGL
            if (!lowRes)
            {
                audioPlayer.time = videoPlayer.time;
                audioPlayer.frame = videoPlayer.frame;
            }
            PlayController();
#else
            PlayController();
#endif

        }
        else
        {
#if !UNITY_WEBGL
            Pause();
#else
            Pause();
#endif
        }

    }


    public void VolumeSlider()
    {
        if (showingVolume)
        {
            showingVolume = false;
            volumeSlider.gameObject.SetActive(false);
        }
        else
        {
            showingVolume = true;
            volumeSlider.gameObject.SetActive(true);
        }
    }

    public void PlaybackSpeedSlider()
    {
        if (showingPlaybackSpeed)
        {
            showingPlaybackSpeed = false;
            playbackSpeed.gameObject.SetActive(false);
        }
        else
        {
            showingPlaybackSpeed = true;
            playbackSpeed.gameObject.SetActive(true);
        }
    }


    public void Pause()
    {
        videoPlayer.Pause();
        if (!lowRes)
        {
            audioPlayer.Pause();
        }

    }


    IEnumerator WorkAroundToUnityBackwardSeek()
    {
        yield return new WaitForSeconds(0.1f);
        videoPrepared = false;
        audioPrepared = false;
        if(!lowRes)
            audioPlayer.prepareCompleted += AudioPreparedSeek;
        videoPlayer.prepareCompleted += VideoPreparedSeek;
        if(!lowRes)
            audioPlayer.Prepare();
        videoPlayer.Prepare();
    }

    void VideoPreparedSeek(VideoPlayer p)
    {
        videoPrepared = true;
        videoPlayer.prepareCompleted -= VideoPrepared;
        if (videoPrepared && audioPrepared)
        {
            progressSlider.value = savedTime;
        }

    }

    void AudioPreparedSeek(VideoPlayer p)
    {
        audioPrepared = true;
        audioPlayer.prepareCompleted -= AudioPrepared;
        if (videoPrepared && audioPrepared)
        {
            progressSlider.value = savedTime;
        }
    }

    public void Stop()
    {
        videoPlayer.Stop();
        if (!lowRes)
            audioPlayer.Stop();
    }

    void SeekVideoDone(VideoPlayer vp)
    {
        videoSeekDone = true;
        videoPlayer.seekCompleted -= SeekVideoDone;
        if (!lowRes)
        {
            //check if the two videos are done the seek | if are play the videos
            if (videoSeekDone && videoAudioSeekDone)
            {
                isSyncing = false;
                //long frame = sourceVideo.frame;
                //sourceVideo.frame = frame;
                //sourceAudioVideo.frame = frame;
                StartCoroutine(SeekFinished());

                //HideLoading();
                //Play();
            }
        }
        else
        {
            isSyncing = false;

            HideLoading();
            PlayController();
        }
    }

    void SeekVideoAudioDone(VideoPlayer vp)
    {
        videoAudioSeekDone = true;
        audioPlayer.seekCompleted -= SeekVideoAudioDone;
        if (!lowRes)
        {
            if (videoSeekDone && videoAudioSeekDone)
            {
                isSyncing = false;
                //long frame = sourceVideo.frame;
                //sourceVideo.frame = frame;
                //sourceAudioVideo.frame = frame;
                StartCoroutine(SeekFinished());

                //HideLoading();
                //Play();
            }
        }
    }

    IEnumerator SeekFinished()
    {
        yield return new WaitForEndOfFrame();
        HideLoading();
        PlayController();
    }

    private string FormatTime(int time)
    {
        int hours = time / 3600;
        int minutes = (time % 3600) / 60;
        int seconds = (time % 3600) % 60;
        if (hours == 0 && minutes != 0)
        {
            return minutes.ToString("00") + ":" + seconds.ToString("00");
        }
        else if (hours == 0 && minutes == 0)
        {
            return "00:" + seconds.ToString("00");
        }
        else
        {
            return hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }

    bool UserInteract()
    {
        if (Application.isMobilePlatform)
        {
            if (Input.touches.Length >= 1)
                return true;
            else
                return false;
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
                return true;
            return (Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0);
        }

    }


    private void PlayController()
    {
        if (videoQuality != YoutubeVideoQuality.LOW)
        {
            audioPlayer.Play();
        }

        videoPlayer.Play();
    }

    #endregion

    #region Decryption System
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
    #endregion

    #region WEBGLREQUEST
    YoutubeResultIds webGlResults;
    IEnumerator WebGlRequest(string videoID)
    {
        WWW request = new WWW(serverURI + "" + videoID + "" + formatURI);
        startedPlayingWebgl = false;
        yield return request;
        webGlResults = new YoutubeResultIds();
        Debug.Log(request.url);
        var requestData = JSON.Parse(request.text);
        var videos = requestData["videos"][0]["formats"];
        webGlResults.bestFormatWithAudioIncluded = requestData["videos"][0]["url"];
        logTest = "EEDone";
        for (int counter = 0; counter < videos.Count; counter++)
        {
            if (videos[counter]["format_id"] == "160")
            {
                webGlResults.lowQuality = videos[counter]["url"];
            }
            else if (videos[counter]["format_id"] == "133")
            {
                webGlResults.lowQuality = videos[counter]["url"];   //if have 240p quality overwrite the 144 quality as low quality.
            }
            else if (videos[counter]["format_id"] == "134")
            {
                webGlResults.standardQuality = videos[counter]["url"];  //360p
            }
            else if (videos[counter]["format_id"] == "136")
            {
                webGlResults.hdQuality = videos[counter]["url"];  //720p
            }
            else if (videos[counter]["format_id"] == "137")
            {
                webGlResults.fullHdQuality = videos[counter]["url"];  //1080p
            }
            else if (videos[counter]["format_id"] == "266")
            {
                webGlResults.ultraHdQuality = videos[counter]["url"];  //@2160p 4k
            }
            else if (videos[counter]["format_id"] == "139")
            {
                webGlResults.audioUrl = videos[counter]["url"];  //AUDIO
            }
        }
        //quality selection will be implemented later for webgl, i recomend use the  webGlResults.bestFormatWithAudioIncluded
        WebGlGetVideo(webGlResults.bestFormatWithAudioIncluded);
    }



    //WEBGL only...
    public void WebGlGetVideo(string url)
    {
        logTest = "Getting Url Player";
        byte[] bytesToEncode = Encoding.UTF8.GetBytes(url);
        string encodedText = Convert.ToBase64String(bytesToEncode);
        videoUrl = VIDEOURIFORWEBGLPLAYER + "" + encodedText;
        videoQuality = YoutubeVideoQuality.LOW;
        logTest = videoUrl+" Done";
        Debug.Log("Play!! " + videoUrl);
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = videoUrl;
        videoPlayer.Prepare();
        videoPrepared = false;
        videoPlayer.prepareCompleted += WeblPrepared; ;
    }

    private void WeblPrepared(VideoPlayer source)
    {
        startedPlayingWebgl = true;
        StartCoroutine(WebGLPlay());
        logTest = "Playing!!";
    }

    IEnumerator WebGLPlay() //The prepare not respond so, i forced to play after some seconds
    {
        yield return new WaitForSeconds(2f);
        Play();
    }
    bool startedPlayingWebgl = false;

    public class YoutubeResultIds
    {
        public string lowQuality;
        public string standardQuality;
        public string mediumQuality;
        public string hdQuality;
        public string fullHdQuality;
        public string ultraHdQuality;
        public string bestFormatWithAudioIncluded;
        public string audioUrl;

    }

    private string logTest = "/";
    private void OnGUI()
    {
        if(debug)
            GUI.Label(new Rect(0, 0, 400, 30), logTest);
    }
    #endregion
    [HideInInspector]
    public bool isSyncing = false;

    [Header("Experimental")]
    public bool showThumbnailBeforeVideoLoad = false;
    private string thumbnailVideoID;
}
