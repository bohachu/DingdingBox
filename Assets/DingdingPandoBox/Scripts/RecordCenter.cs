using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cameo;
using System;

namespace Cameo.PandoBox
{
    public class RecordCenter : Singleton<RecordCenter>
    {
        public void Clear()
        {
            PlayerPrefs.DeleteAll();  
        }

        public void AddScan(string key)
        {
            PlayerPrefs.SetInt(key, 0); 
        }

        public bool CheckScanCompleted(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public bool CheckExchangedCompleted()
        {
            return PlayerPrefs.HasKey("Exchanged");
        }

        public void SetExchangedCompleted()
        {
            PlayerPrefs.SetString("Exchanged", DateTime.Now.ToLongDateString());
        }
    }
}
