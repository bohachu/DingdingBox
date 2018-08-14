using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using YoutubeLight;
using SimpleJSON;
using System.Text;
using System;

public class NewPlaybackSystem : MonoBehaviour
{
    /*PRIVATE INFO DO NOT CHANGE THESE URLS OR VALUES*/
    private const string serverURI = "https://unity-dev-youtube.herokuapp.com/api/info?url=https://www.youtube.com/watch?v=";
    private const string formatURI = "&format=best&flatten=true";
    private const string videoURI = "https://youtubewebgl.herokuapp.com/download.php?mime=video/mp4&title=generatedvideo&token=";
    /*END OF PRIVATE INFO*/

    [SerializeField]
    public YoutubeNewResultIds results;

    public string videoId = "bc0sJvtKrRM";
    public enum VideoQuality
    {
        HD720,
        HD1080,
        HD2160
    }
    public VideoQuality videoQuality;
    private string videoUrl;
    private string audioVideoUrl;
    //If you will use high quality playback we need one video player that only will run the audio.
    public VideoPlayer unityVideoPlayer;
    //start playing the video
    public bool playOnStart = false;
    [Tooltip("Use the SD video instead of a video and a audio separated, use this if you want to play sd videos, for hd+ you need to leave this setted to false")]
    public bool use_sd_videoWithAudioIncluded = false;

    private bool noHD = false;

    RequestResolver resolver;

    public void Start()
    {
        resolver = gameObject.AddComponent<RequestResolver>();
        if (playOnStart)
        {
            PlayYoutubeVideo(videoId);
        }
    }


    public void PlayYoutubeVideo(string _videoId)
    {
        if (this.GetComponent<VideoController>() != null)
        {
            this.GetComponent<VideoController>().ShowLoading("Loading...");
        }
        videoId = _videoId;
        StartCoroutine(Request(videoId));
    }

    IEnumerator Request(string videoID)
    {
        WWW request = new WWW(serverURI + "" + videoID + "" + formatURI);
        yield return request;
        var requestData = JSON.Parse(request.text);
        var videos = requestData["videos"][0]["formats"];
        results.bestFormatWithAudioIncluded = requestData["videos"][0]["url"];

        for (int counter = 0; counter < videos.Count; counter++)
        {
            if (videos[counter]["format_id"] == "160")
            {
                results.lowQuality = videos[counter]["url"];
            }
            else if (videos[counter]["format_id"] == "133")
            {
                results.lowQuality = videos[counter]["url"];   //if have 240p quality overwrite the 144 quality as low quality.
            }
            else if (videos[counter]["format_id"] == "134")
            {
                results.standardQuality = videos[counter]["url"];  //360p
            }
            else if (videos[counter]["format_id"] == "135")
            {
                results.mediumQuality = videos[counter]["url"];  //480p
            }
            else if (videos[counter]["format_id"] == "136")
            {
                results.hdQuality = videos[counter]["url"];  //720p
            }
            else if (videos[counter]["format_id"] == "137")
            {
                results.fullHdQuality = videos[counter]["url"];  //1080p
            }
            else if (videos[counter]["format_id"] == "266")
            {
                results.ultraHdQuality = videos[counter]["url"];  //@2160p 4k
            }
            else if (videos[counter]["format_id"] == "18")
            {
                results.audioUrl = videos[counter]["url"];  //AUDIO
            }
        }
        if(use_sd_videoWithAudioIncluded)
            //quality selection will be implemented later for webgl, i recomend use the  results.bestFormatWithAudioIncluded
            GetSDVideo(results.bestFormatWithAudioIncluded);
        else
        {
            switch (videoQuality)
            {
                case VideoQuality.HD720:
                    {
                        GetVideo(results.hdQuality, results.audioUrl);
                        break;
                    }
                case VideoQuality.HD1080:
                    {
                        GetVideo(results.fullHdQuality, results.audioUrl);
                        break;
                    }
                case VideoQuality.HD2160:
                    {
                        GetVideo(results.ultraHdQuality, results.audioUrl);
                        break;
                    }
            }
        }
            
    }

    public void GetSDVideo(string videourl)
    {
        byte[] bytesToEncode = Encoding.UTF8.GetBytes(videourl);
        string encodedText = Convert.ToBase64String(bytesToEncode);
        videoUrl = videoURI + "" + encodedText;

        Debug.Log("Play!! " + videoUrl);
        unityVideoPlayer.source = VideoSource.Url;
        unityVideoPlayer.url = videoUrl;

        unityVideoPlayer.Prepare();

        videoPrepared = false;
        unityVideoPlayer.prepareCompleted += VideoPrepared;
    }


    public void GetVideo(string videourl, string audiourl)
    {
        videoUrl = videourl;

        audioVideoUrl = audiourl;

        Debug.Log("Play!! " + videoUrl);
        unityVideoPlayer.source = VideoSource.Url;
        unityVideoPlayer.url = videoUrl;

        unityVideoPlayer.Prepare();
        audioVplayer.source = VideoSource.Url;
        audioVplayer.url = audioVideoUrl;
        audioVplayer.Prepare();

        videoPrepared = false;
        unityVideoPlayer.prepareCompleted += VideoPrepared;

        audioPrepared = false;
        audioVplayer.prepareCompleted += AudioPrepared;

    }

    IEnumerator WebGLPlay() //The prepare not respond so, i forced to play after some seconds
    {
        yield return new WaitForSeconds(2f);
        Play();
    }


    private bool audioDecryptDone = false;
    private bool videoDecryptDone = false;

    public VideoPlayer audioVplayer;

    bool startedPlaying = false;

    void FixedUpdate()
    {
        if (unityVideoPlayer.isPrepared)
        {
            if (!startedPlaying)
            {
                startedPlaying = true;
                StartCoroutine(WebGLPlay());
            }
        }

        CheckIfIsDesync();
    }

    private bool videoPrepared;
    private bool audioPrepared;

    void AudioPrepared(VideoPlayer vPlayer)
    {
        audioVplayer.prepareCompleted -= AudioPrepared;
        audioPrepared = true;
        if (audioPrepared && videoPrepared)
            Play();
    }

    void VideoPrepared(VideoPlayer vPlayer)
    {
        unityVideoPlayer.prepareCompleted -= VideoPrepared;
        videoPrepared = true;
        Debug.Log("Playing youtube video only, the audio separated will be implemented in the final release of webgl support");
        
        if (use_sd_videoWithAudioIncluded)
        {
            noHD = true; //force now to prevent bugs...
            Play();
        }
        else
        {
            noHD = false; //force now to prevent bugs...
            if (audioPrepared && videoPrepared)
                Play();
        }
            
    }

    public void Play()
    {
        unityVideoPlayer.loopPointReached += PlaybackDone;
        StartCoroutine(WaitAndPlay());
    }

    private void PlaybackDone(VideoPlayer vPlayer)
    {
        OnVideoFinished();
    }

    IEnumerator WaitAndPlay()
    {

        if (!noHD)
        {
            audioVplayer.Play();
            if (syncIssue)
                yield return new WaitForSeconds(0.35f);
            else
                yield return new WaitForSeconds(0);
        }
        else
        {
            if (syncIssue)
                yield return new WaitForSeconds(1f);//if is no hd wait some more
            else
                yield return new WaitForSeconds(0);
        }
        unityVideoPlayer.Play();
        if (this.GetComponent<VideoController>() != null)
        {
            this.GetComponent<VideoController>().HideLoading();
        }
    }

    IEnumerator StartVideo()
    {
#if UNITY_IPHONE || UNITY_ANDROID
		Handheld.PlayFullScreenMovie (videoUrl);
#endif
        yield return new WaitForSeconds(1.4f);
        OnVideoFinished();
    }

    public void OnVideoFinished()
    {
        if (unityVideoPlayer.isPrepared)
        {
            Debug.Log("Finished");
            if (unityVideoPlayer.isLooping)
            {
                unityVideoPlayer.time = 0;
                unityVideoPlayer.frame = 0;
                audioVplayer.time = 0;
                audioVplayer.frame = 0;
                unityVideoPlayer.Play();
                audioVplayer.Play();
            }
        }
    }

    [HideInInspector]
    public bool isSyncing = false;

    [Header("If you think audio is out of sync enable this bool below")]
    [Header("This happens in some unity versions, the most stable is the 5.6.1p1")]
    public bool syncIssue;

    //Experimental
    private void CheckIfIsDesync()
    {
        if (!noHD)
        {
            //Debug.Log(unityVideoPlayer.time+" "+ audioVplayer.time);
            double t = unityVideoPlayer.time - audioVplayer.time;
            if (!isSyncing)
            {
                if (t != 0)
                {
                    Sync();
                }
                else if (unityVideoPlayer.frame != audioVplayer.frame)
                {
                    Sync();
                }
            }
        }
        else
        {
            //unityVideoPlayer.frame -= 1;
        }
    }

    private void Sync()
    {
        VideoController controller = GameObject.FindObjectOfType<VideoController>();
        if (controller != null)
        {
            //isSyncing = true;
            //audioVplayer.time = unityVideoPlayer.time;
            //audioVplayer.frame = unityVideoPlayer.frame;
            //controller.Seek();
        }
        else
        {
            Debug.LogWarning("Please add a video controller to your scene to make the sync work! Will be improved in the future.");
        }
    }

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
}

[System.Serializable]
public class YoutubeNewResultIds
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
