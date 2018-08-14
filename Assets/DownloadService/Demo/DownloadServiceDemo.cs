using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cameo.DownloadService;

public class DownloadServiceDemo : MonoBehaviour
{
    public Text DownloadResultText;
    public Text ProgressText;
    public Text FileCountText;

	private void Update()
	{
        ProgressText.text = DownloadService.Instance.DownloadFileProgress.ToString();
        FileCountText.text = DownloadService.Instance.WaitingFileCount.ToString();		
	}

	public void OnStartDownloadBtnClicked()
    {
        DownloadResultText.text = "";

        List<DownloadCommand> commands = new List<DownloadCommand>();
        commands.Add(new DownloadCommand("http://downloadservicedemo.tapmovie.com/photo.jpg", Path.Combine(Application.persistentDataPath, "DownloadServiceDemo/photo.jpg")));
        commands.Add(new DownloadCommand("http://downloadservicedemo.tapmovie.com/text.json", Path.Combine(Application.persistentDataPath, "DownloadServiceDemo/text.json")));
        DownloadService.Instance.AddDownloadCommand(commands);
        DownloadService.Instance.AddDownloadMonitor(commands, onDownloadCompleted);
        DownloadService.Instance.StartDownload();
    }

    private void onDownloadCompleted(DownloadMonitorResult result)
    {
        DownloadResultText.text = "Download Completed!";
    }
}
