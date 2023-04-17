using CommonCore.Scripting;
using CommonCore.State;
using CommonCore.StringSub;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CommonCore.UI;
using System.Reflection;
using CommonCore.Messaging;

namespace CommonCore.Experimental.Collectibles
{

    /// <summary>
    /// Experimental module for handling collectible things in game
    /// </summary>
    public class CollectiblesExperimentModule : CCModule
    {
        public const string CollectibleNameList = "COLLECTIBLES_NAME";
        private const string CollectiblesCollectionKey = "CollectiblesExperimentModule_CollectiblesCollection";
        private const string CollectiblesVisualPath = "Modules/CollectiblesExperiment/Visuals/";

        public CollectiblesExperimentParams Params { get; private set; }

        public CollectiblesExperimentModule()
        {
            Params = GetParamsFromCollection(CoreParams.GetParamsForModule<CollectiblesExperimentModule>());
        }

        public override void OnAllModulesLoaded()
        {
            if(Params.InjectIGUIPanel)
            {
                CCBase.GetModule<UIModule>().RegisterIGUIPanel("CollectiblesPanel", 100, "Collectibles", t => GameObject.Instantiate(CoreUtils.LoadResource<GameObject>("Modules/CollectiblesExperiment/CollectiblesPanel"), t));
            } 
        }

        //public utility methods (API)

        /// <summary>
        /// Grants a collectible in the current context, optionally saving as persistent
        /// </summary>
        public void GrantCollectible(string key, bool persist)
        {
            var collection = persist ? GetPersistentCollectiblesCollection() : GetCurrentCollectiblesCollection();
            bool isFirstGrant = false;
            string campaignId = GameState.Exists ? GameState.Instance.CampaignIdentifier : "";
            var now = DateTime.Now;
            if (!collection.ContainsKey(key))
            {
                collection.Add(key, new CollectibleRecordInternal()
                {
                    Granted = now,
                    GrantedCampaignId = campaignId
                });
                isFirstGrant = true;
            }

            ScriptingModule.CallNamedHooked("CollectiblesOnGrant", this, new CollectibleRecord() { 
                Key = key,
                Type = persist ? CollectibleRecordType.Persistent : CollectibleRecordType.InGame,
                Granted = now,
                GrantedCampaignId = campaignId,
                Name = GetNameForCollectible(key)
            }, isFirstGrant);

            QdmsMessageBus.Instance.PushBroadcast(new QdmsKeyValueMessage("CollectibleGranted", new Dictionary<string, object>()
            {
                { "CollectibleRecord", 
                    new CollectibleRecord() {
                        Key = key,
                        Type = persist ? CollectibleRecordType.Persistent : CollectibleRecordType.InGame,
                        Granted = now,
                        GrantedCampaignId = campaignId,
                        Name = GetNameForCollectible(key)
                    } 
                },
                { "IsFirstGrant", isFirstGrant }
            }));
        }

        /// <summary>
        /// Checks if a collectible is granted, either in the current context or persistent
        /// </summary>
        public bool IsCollectibleGranted(string key) => IsCollectibleGranted(key, true);

        /// <summary>
        /// Checks if a collectible is granted
        /// </summary>
        public bool IsCollectibleGranted(string key, bool considerPersist)
        {
            if(considerPersist)
            {
                if (GetPersistentCollectiblesCollection().ContainsKey(key))
                    return true;
            }

            return GetCurrentCollectiblesCollection()?.ContainsKey(key) ?? false;
        }

        /// <summary>
        /// Gets the record for a collectible
        /// </summary>
        public CollectibleRecord GetCollectibleRecord(string key)
        {
            CollectibleRecordInternal internalRecord;

            if (GetPersistentCollectiblesCollection().TryGetValue(key, out internalRecord))
                return GetRecordForInternal(key, CollectibleRecordType.Persistent, internalRecord);

            if(GetCurrentCollectiblesCollection()?.TryGetValue(key, out internalRecord) ?? false)
                return GetRecordForInternal(key, CollectibleRecordType.InGame, internalRecord);

            return null;
        }

        /// <summary>
        /// Enumerates all collectibles (will include duplicates if both persist and non-persist exist)
        /// </summary>
        public IEnumerable<CollectibleRecord> EnumerateAllCollectibles()
        {
            return EnumerateCollectiblesInternal(false);
        }

        /// <summary>
        /// Enumerates collectibles (will only return one record for each key)
        /// </summary>
        public IEnumerable<CollectibleRecord> EnumerateDistinctCollectibles()
        {
            return EnumerateCollectiblesInternal(true);
        }

        /// <summary>
        /// Gets the resource containing the visual for a collectible (may be a prefab, audioclip, sprite, or texture)
        /// </summary>
        public UnityEngine.Object GetVisualForCollectible(string key)
        {
            string fullPath = CollectiblesVisualPath + key;

            if (CoreUtils.CheckResource<GameObject>(fullPath))
                return CoreUtils.LoadResource<GameObject>(fullPath);

            if (CoreUtils.CheckResource<AudioClip>(fullPath))
                return CoreUtils.LoadResource<AudioClip>(fullPath);

            if (CoreUtils.CheckResource<Sprite>(fullPath))
                return CoreUtils.LoadResource<Sprite>(fullPath);

            if (CoreUtils.CheckResource<Texture2D>(fullPath))
                return CoreUtils.LoadResource<Texture2D>(fullPath);

            if (CoreUtils.CheckResource<TextAsset>(fullPath))
                return CoreUtils.LoadResource<TextAsset>(fullPath);

            return null;
        }

        public string GetNameForCollectible(string key)
        {
            return Sub.Replace(key, CollectibleNameList);
        }

        //private helpers

        //we don't store persistent references to the collections
        //slower, but safer, perfect for a PoC

        private IEnumerable<CollectibleRecord> EnumerateCollectiblesInternal(bool distinct)
        {
            List<CollectibleRecord> allCollectibleRecords = new List<CollectibleRecord>();

            var persistCollectibles = GetPersistentCollectiblesCollection();
            allCollectibleRecords.AddRange(persistCollectibles.Select(c => GetRecordForInternal(c.Key, CollectibleRecordType.Persistent, c.Value)));

            var currentCollectibles = GetCurrentCollectiblesCollection();
            if (currentCollectibles != null && currentCollectibles.Count > 0)
            {
                foreach(var collectible in currentCollectibles)
                {
                    if (distinct && persistCollectibles.ContainsKey(collectible.Key))
                        continue;

                    allCollectibleRecords.Add(GetRecordForInternal(collectible.Key, CollectibleRecordType.InGame, collectible.Value));
                }
            }

            return allCollectibleRecords.OrderBy(r => r.Key).ToList();
        }

        private IDictionary<string, CollectibleRecordInternal> GetCurrentCollectiblesCollection()
        {
            if (!GameState.Exists)
                return null;

            if(GameState.Instance.GlobalDataState.TryGetValue(CollectiblesCollectionKey, out var oCollection) && oCollection is IDictionary<string, CollectibleRecordInternal> collection)
            {
                return collection;
            }

            var newCollection = new Dictionary<string, CollectibleRecordInternal>();
            GameState.Instance.GlobalDataState[CollectiblesCollectionKey] = newCollection;
            return newCollection;
        }

        private IDictionary<string, CollectibleRecordInternal> GetPersistentCollectiblesCollection()
        {
            if(PersistState.Instance.ExtraStore.TryGetValue(CollectiblesCollectionKey, out var oCollection) && oCollection is IDictionary<string, CollectibleRecordInternal> collection)
            {
                return collection;
            }

            var newCollection = new Dictionary<string, CollectibleRecordInternal>();
            PersistState.Instance.ExtraStore[CollectiblesCollectionKey] = newCollection;
            return newCollection;
        }

        private CollectibleRecord GetRecordForInternal(string key, CollectibleRecordType type, CollectibleRecordInternal internalRecord)
        {
            return new CollectibleRecord()
            {
                Key = key,
                Type = type,
                Granted = internalRecord.Granted,
                GrantedCampaignId = internalRecord.GrantedCampaignId,
                Name = GetNameForCollectible(key) ?? key
            };
        }

        //translate params object from module params collection (will be implemented in core in 5.x)
        private static CollectiblesExperimentParams GetParamsFromCollection(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            var result = new CollectiblesExperimentParams();
            if(parameters != null && parameters.Any())
            {
                foreach(var p in parameters)
                {
                    var prop = result.GetType().GetProperty(p.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if(prop != null)
                    {
                        object cValue = TypeUtils.CoerceValue(p.Value, prop.PropertyType);
                        prop.SetValue(result, cValue);
                    }
                }
            }
            return result;
        }

        //private types

        [JsonObject]
        private class CollectibleRecordInternal
        {
            public DateTime Granted { get; set; }
            public string GrantedCampaignId { get; set; }
        }
    }
}