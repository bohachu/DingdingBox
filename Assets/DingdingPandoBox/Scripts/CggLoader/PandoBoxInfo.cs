using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class PandoBoxActionInfo
{
    public int actionId;
    public string actionName;
    public string actPhotoUrl;
    public string actDescription;
}

public class PandoBoxInfo
{
    public string location;
    public string videoUrl;
    public string photoUrl;
    public string urlLink;
    public string description;
    public string beaconId;
    public string picTypeId;
    public string slamId;
    public string title;
    public int weights;
    public List<PandoBoxActionInfo> action;

    public void FromJsonData(JsonData jsonData)
    {
        location = jsonData.Keys.Contains("location") && jsonData["location"] != null? jsonData["location"].ToString() : null;
        videoUrl = jsonData.Keys.Contains("videoUrl") && jsonData["videoUrl"] != null? jsonData["videoUrl"].ToString() : null;
        photoUrl = jsonData.Keys.Contains("photoUrl") && jsonData["photoUrl"] != null? jsonData["photoUrl"].ToString() : null;
        urlLink = jsonData.Keys.Contains("urlLink") && jsonData["urlLink"] != null? jsonData["urlLink"].ToString() : null;
        description = jsonData.Keys.Contains("description") && jsonData["description"] != null ? jsonData["description"].ToString() : null;
        beaconId = jsonData.Keys.Contains("beaconId") && jsonData["beaconId"] != null ? jsonData["beaconId"].ToString() : null;
        picTypeId = jsonData.Keys.Contains("picTypeId") && jsonData["picTypeId"] != null ? jsonData["picTypeId"].ToString() : null;
        slamId = jsonData.Keys.Contains("slamId") && jsonData["slamId"] != null ? jsonData["slamId"].ToString() : null;
        title = jsonData.Keys.Contains("title") && jsonData["title"] != null ? jsonData["title"].ToString() : null;
        action = jsonData.Keys.Contains("action") && jsonData["action"] != null? null : JsonMapper.ToObject<List<PandoBoxActionInfo>>(jsonData["action"].ToJson());

        string weighsStr = jsonData.Keys.Contains("weights") && jsonData["weights"] != null ? jsonData["weights"].ToString() : "0";
        weighsStr = weighsStr.Replace('B', '-');
        weights = int.Parse(weighsStr);



    }
}