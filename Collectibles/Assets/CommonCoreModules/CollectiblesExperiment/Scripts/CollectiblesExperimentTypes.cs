using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonCore.State;
using System;

namespace CommonCore.Experimental.Collectibles
{
    public class CollectiblesExperimentParams
    {
        public bool InjectIGUIPanel { get; private set; } = false;
        //public bool InjectMainMenuPanel { get; set; } = false; //will be implemented in 5.x

        public bool PlayAudioLogsImmediately { get; private set; } = true;

        public int IngameViewWidth { get; private set; } = 1600;
        public int IngameViewHeight { get; private set; } = 900;
    }

    //external, returned from methods
    public class CollectibleRecord
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public CollectibleRecordType Type { get; set; }

        public DateTime Granted { get; set; }
        public string GrantedCampaignId { get; set; }
    }

    public enum CollectibleRecordType
    {
        Unknown = 0,
        InGame = 1, 
        Persistent = 2
    }
}