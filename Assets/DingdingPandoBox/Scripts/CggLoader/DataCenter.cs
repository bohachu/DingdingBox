using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;
using Cameo;
using Cameo.DownloadService;

namespace Cameo.PandoBox
{
    public class DataCenter : Singleton<DataCenter>
    {
        [SerializeField]
        private CggLoaderInfo cggLoaderInfo;

        public Action OnDataReady = delegate { };
        public Action OnDataFail = delegate { };

        private CggLoader cggLoader = null;
        private List<PandoBoxInfo> infoList = null;
        private int totalDownloadFileCount = 0;

        public List<PandoBoxInfo> InfoList
        {
            get
            {
                return infoList;
            }
        }

        public void LoadCgg()
        {
            cggLoader = new CggLoader(cggLoaderInfo.ApiUrl, cggLoaderInfo.Key, cggLoaderInfo.IV);
            cggLoader.Load(onCggInfoLoadReturn);
        }

        private void onCggInfoLoadReturn(bool isSuccess, string result)
        {
            parseData(result);
            cggLoader = null;
        }

        private void parseData(string dataString)
        {
            Debug.Log(dataString);

            JsonData jsonData = JsonMapper.ToObject(dataString);

            if (!(jsonData as IDictionary).Contains("returnCode") || jsonData["returnCode"].ToString() != "00")
            {
                DebugLogger.Instance.LogError("[DataCenter] Load Data Fail / return message: " + jsonData["returnMessage"].ToString());
                OnDataFail();
                return;
            }

            if (!(jsonData as IDictionary).Contains("data"))
            {
                DebugLogger.Instance.LogError("[DataCenter] Load Data Fail/ Not contain data value.");
                OnDataFail();
                return;
            }

            string productInfoJsonString = JsonMapper.ToJson(jsonData["data"]);
            DebugLogger.Instance.Log("[DataCenter] parseData / productInfoJsonString: " + productInfoJsonString);

            infoList = new List<PandoBoxInfo>();
            for (int i = 0; i < jsonData["data"].Count; ++i)
            {
                PandoBoxInfo pandoBoxInfo = new PandoBoxInfo();
                pandoBoxInfo.FromJsonData(jsonData["data"][i]);
                infoList.Add(pandoBoxInfo);
            }

            infoList.Sort(delegate (PandoBoxInfo x, PandoBoxInfo y)
            {
                if (x.weights == y.weights) return 0;
                else if (x.weights < y.weights) return 1;
                else if (x.weights > y.weights) return -1;
                else return 0;
            });

            OnDataReady();
            //暫時改成不下載，動態直接使用連結取得圖片
            //DownloadMediaFile();
        }

        private void DownloadMediaFile()
        {
            List<DownloadCommand> commands = new List<DownloadCommand>();

            for (int i = 0; i < infoList.Count; i++)
            {
                if (infoList[i].photoUrl == "")
                    continue;

                totalDownloadFileCount = totalDownloadFileCount + 1;
                string photoUrl = infoList[i].photoUrl;
                string saveFilePath = "PandoBoxInfo/" + MD5.getStrMd5String(photoUrl) + Path.GetExtension(photoUrl);
                Debug.Log("Download " + saveFilePath);
                commands.Add(new DownloadCommand(photoUrl, Path.Combine(Application.persistentDataPath, saveFilePath)));
                List<PandoBoxActionInfo> actionInfoList = infoList[i].action;

                if(actionInfoList != null)
                {
                    for (int j = 0; j < actionInfoList.Count; j++)
                    {
                        if (actionInfoList[j].actPhotoUrl == "")
                        {
                            continue;
                        }
                        totalDownloadFileCount = totalDownloadFileCount + 1;
                        string actPhotoUrl = actionInfoList[j].actPhotoUrl;
                        string actPhotoSaveFilePath = "PandoBoxInfo/" + MD5.getStrMd5String(actPhotoUrl) + Path.GetExtension(actPhotoUrl);
                        commands.Add(new DownloadCommand(actPhotoUrl, Path.Combine(Application.persistentDataPath, actPhotoSaveFilePath)));
                    }
                }

            }

            if(commands.Count > 0)
            {
                DownloadService.DownloadService.Instance.AddDownloadCommand(commands);
                DownloadService.DownloadService.Instance.AddDownloadMonitor(commands, onDownloadCompleted);
                DownloadService.DownloadService.Instance.StartDownload();
            }
            else
            {
                OnDataReady();
            }
        }

        public float GetDownloadProgress()
        {
            if (totalDownloadFileCount == 0)
            {
                return 0f;
            }

            float progress = (totalDownloadFileCount - DownloadService.DownloadService.Instance.WaitingFileCount) * 1.0f / totalDownloadFileCount;

            return progress;
        }

        private void onDownloadCompleted(DownloadMonitorResult result)
        {
            Debug.Log("[DataCenter] onDownloadCompleted");
            OnDataReady();
        }
    }
}