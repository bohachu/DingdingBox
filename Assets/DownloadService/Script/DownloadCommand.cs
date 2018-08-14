using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cameo.DownloadService
{
    public class DownloadCommand
    {
        public string DownloadUrl;
        public string SavedFileUrl;

        public DownloadCommand(string downloadUrl, string savedFieUrl = null)
        {
            DownloadUrl = downloadUrl;
            SavedFileUrl = savedFieUrl;
        }
    }
}