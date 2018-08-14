using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cameo;

namespace Cameo.PandoBox
{
    public class RecordCenter : Singleton<RecordCenter>
    {
        public void AddScan(string key)
        {
            PlayerPrefs.SetInt(key, 0);
        }

        public void SetExchangeCompleted(string key)
        {
            PlayerPrefs.SetInt(key, 1);
        }

        public bool CheckExchangeCompleted(string key)
        {
            return PlayerPrefs.HasKey(key) && PlayerPrefs.GetInt(key) == 1;
        }

        public bool CheckScanCompleted(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
    }

}
