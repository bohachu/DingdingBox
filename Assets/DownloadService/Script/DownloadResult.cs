using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cameo.DownloadService
{
    public class DownloadResult
    {
        public DownloadCommand Command;
        public DateTime DownloadedTime;
        public byte[] Bytes;
        public string ErrorMsg;
    }
}