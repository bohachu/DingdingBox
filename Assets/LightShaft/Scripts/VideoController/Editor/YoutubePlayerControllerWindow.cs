using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEditor;

public class YoutubePlayerControllerWindow : EditorWindow {
    [MenuItem("Window/Youtube/Add Controller")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(YoutubePlayerControllerWindow));
    }

    public GameObject controller;

    VideoPlayer video ;
    VideoPlayer audio;
    Slider videoSlider;
    bool hdVideo = true;
    bool hideControl = true;
    int hideTime = 3;
   
    private void OnGUI()
    {
        GUILayout.Label("Config to create the video controller", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Works for Highquality playback script in player only");
        video = EditorGUILayout.ObjectField("Main video Player", video, typeof(VideoPlayer), true) as VideoPlayer;
        hdVideo = EditorGUILayout.Toggle("1080+ playback?",hdVideo);

        if (hdVideo)
        {
            audio = EditorGUILayout.ObjectField("The audio video player", audio, typeof(VideoPlayer), true) as VideoPlayer;
        }

        hideControl = EditorGUILayout.Toggle("Hide video controller auto", hideControl);
        if (hideControl)
        {
            hideTime = EditorGUILayout.IntField("Seconds to hide the controller", hideTime);
        }
        if (GUILayout.Button("Generate Controller"))
        {
            GenerateController();
        }
    }

    void GenerateController()
    {
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject newCanvas = new GameObject();
            canvas = newCanvas.AddComponent<Canvas>();
            canvas.gameObject.name = "Canvas";
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        GameObject newController = Instantiate(controller, canvas.transform);

        VideoController vController = newController.GetComponentInChildren<VideoController>();
        vController.sourceVideo = video;
        if(hdVideo)
            vController.sourceAudioVideo = audio;
        vController.hideControls = hideControl;
        vController.secondsToHideScreen = hideTime;
    }
}
