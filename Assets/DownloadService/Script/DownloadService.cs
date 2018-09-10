using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cameo.DownloadService
{
    public class DownloadService : Singleton<DownloadService>
    {
        /// <summary>
        /// 顯示現在正在下載的檔案的進度
        /// </summary>
        public float DownloadFileProgress
        {
            get
            {
                return (www == null) ? 0 : www.progress;
            }
        }

        /// <summary>
        /// 顯示剩下多少檔案需要下載
        /// </summary>
        public int WaitingFileCount
        {
            get
            {
                return waitingCommandList.Count;
            }
        }

        /// <summary>
        /// 是否在下載中
        /// </summary>
        public bool IsDownloading { get; private set; }

        private List<DownloadCommand> waitingCommandList = new List<DownloadCommand>();
        private List<DownloadMonitor> monitorList = new List<DownloadMonitor>();
        private WWW www = null;

		public void AddDownloadCommand(List<DownloadCommand> commands)
        {
            for (int i = 0; i < commands.Count; ++i)
            {
                if(!waitingCommandList.Contains(commands[i]))
                {
                    waitingCommandList.Add(commands[i]);
                }
            }
        }

        /// <summary>
        /// 加入下載結果監控器
        /// </summary>
        /// <returns>下載結果監控器(DownloadMonitor)</returns>
        /// <param name="commands">要監控的下載命令陣列，當這些命令全部執行完畢，則會回傳結果</param>
        /// <param name="resultHandler">處理下載結果的函式</param>
        public DownloadMonitor AddDownloadMonitor(List<DownloadCommand> commands, Action<DownloadMonitorResult> resultHandler)
        {
            DownloadMonitor monitor = new DownloadMonitor(commands, resultHandler);
            monitorList.Add(monitor);
            return monitor;
        }

        /// <summary>
        /// 停止監控下載進度
        /// </summary>
        public void StopMonitor(DownloadMonitor monitor)
        {
            if(monitorList.Contains(monitor))
            {
                monitor.Stop();
                monitorList.Remove(monitor);
            }
        }

        public void StartDownload()
        {
            IsDownloading = true;
            StartCoroutine("excuteDownloadCommandProcess");
        }

        /// <summary>
        /// 暫停下載，可呼叫StartDownload恢復下載
        /// </summary>
        public void Pause()
        {
            IsDownloading = false;
            StopCoroutine("excuteDownloadCommandProcess");
        }

        /// <summary>
        /// 停止下載，會清空所有等待下載的命令以及下載結果監視器
        /// </summary>
        public void Stop()
        {
            IsDownloading = false;
            StopCoroutine("excuteDownloadCommandProcess");
            for (int i = 0; i < monitorList.Count; ++i)
            {
                monitorList[i].Stop();
            }
            monitorList.Clear();
            waitingCommandList.Clear();
        }

        /// <summary>
        /// 依序執行所有等待中的下載命令
        /// </summary>
        private IEnumerator excuteDownloadCommandProcess()
        {
            while(waitingCommandList.Count > 0)
            {
                DownloadCommand command = waitingCommandList[0];

                www = new WWW(waitingCommandList[0].DownloadUrl);
                yield return www;

                DownloadResult result = new DownloadResult();
                result.Command = command;
                result.DownloadedTime = DateTime.Now;
                result.ErrorMsg = www.error;
                result.Bytes = www.bytes;

                if(!string.IsNullOrEmpty(result.ErrorMsg))
                {
                    Debug.LogErrorFormat("[DownloadService.excuteDownloadCommandProcess] Download {1} error: {0}", waitingCommandList[0].DownloadUrl, result.ErrorMsg);
                    DebugLogger.Instance.LogError("[DownloadService.excuteDownloadCommandProcess] Download error: " + result.ErrorMsg);
                }
                else
                {
                    if(!string.IsNullOrEmpty(command.SavedFileUrl))
                    {
                        writeToFile(result.Bytes, command.SavedFileUrl);
                    }
                }

                for (int i = monitorList.Count-1; i >= 0; --i)
                {
                    monitorList[i].InvokeDownloadCommandCompleted(waitingCommandList[0], result);
                    if(monitorList[i].IsCompleted)
                    {
                        monitorList.RemoveAt(i);
                    }
                }

                waitingCommandList.RemoveAt(0);
            }

            IsDownloading = false;
        }

        /// <summary>
        /// 把byte array寫入檔案
        /// </summary>
        private void writeToFile(byte[] bytes, string strFilePath)
        {
            string directory = Path.GetDirectoryName(strFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            BinaryWriter writer = new BinaryWriter(File.Open(strFilePath, FileMode.Create));
            writer.Write(bytes);
            writer.Flush();
            writer.Close();
            Debug.Log("[[DownloadManager.writeToFile] Save to " + strFilePath);
        }
    }

}
