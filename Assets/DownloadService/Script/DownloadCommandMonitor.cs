using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cameo.DownloadService
{
    public enum DownloadMonitorResultType
    {
        ALL_COMPLETED,
        STOPPED,
    }

    public class DownloadMonitor
    {
        public bool IsCompleted
        {
            get
            {
                return monitorCommandList.Count == 0;
            }
        }

        private List<DownloadCommand> monitorCommandList;
        private DownloadMonitorResult monitorResult;
        private Action<DownloadMonitorResult> onCommandAllCompleted = delegate{};

        public DownloadMonitor(List<DownloadCommand> commands, Action<DownloadMonitorResult> onCommandAllCompleted)
        {
            monitorCommandList = commands;
            this.onCommandAllCompleted = onCommandAllCompleted;
            monitorResult = new DownloadMonitorResult();
        }

        public void InvokeDownloadCommandCompleted(DownloadCommand command, DownloadResult result)
        {
            if(monitorCommandList.Contains(command))
            {
                if(!monitorResult.DownloadResultMapping.ContainsKey(command))
                {
                    monitorResult.DownloadResultMapping.Add(command, result);
                }

                monitorCommandList.Remove(command);

                if(monitorCommandList.Count == 0)
                {
                    monitorResult.ResultType = DownloadMonitorResultType.ALL_COMPLETED;
                    onCommandAllCompleted(monitorResult);
                }
            }
        }

        public void Stop()
        {
            monitorResult.ResultType = DownloadMonitorResultType.STOPPED;
            onCommandAllCompleted(monitorResult);
        }
    }

    public class DownloadMonitorResult
    {
        public Dictionary<DownloadCommand, DownloadResult> DownloadResultMapping;
        public DownloadMonitorResultType ResultType;

        public DownloadMonitorResult()
        {
            DownloadResultMapping = new Dictionary<DownloadCommand, DownloadResult>();
        }
    }
}
