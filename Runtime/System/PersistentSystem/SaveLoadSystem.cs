﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mimizh.UnityUtilities.PersistentSystem
{
    public interface ISavable
    {
        SerializableGuid Id { get; set; }
    }
    public interface IBind<in TData> where TData : ISavable
    {
        SerializableGuid Id { get; set; }
        void Bind(TData playerData);
    }
    public abstract class SaveLoadSystem<TGameData> : PersistentSingleton<SaveLoadSystem<TGameData>> where TGameData : IGameData
    {
        [SerializeField] public TGameData gameData;
        
        protected IDataService<TGameData> dataService;

        protected virtual void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        protected virtual void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        protected abstract void OnSceneLoaded(Scene scene, LoadSceneMode mode);

        protected void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISavable, new()
        {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
            if (entity != null)
            {
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                }
                entity.Bind(data);
            }
        }

        protected void Bind<T, TData>(List<TData> dataList) where T : MonoBehaviour, IBind<TData> where TData : ISavable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

            foreach (var entity in entities)
            {
                var data = dataList.FirstOrDefault(x => x.Id == entity.Id);
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                    dataList.Add(data);
                }
                entity.Bind(data);
            }
        }

        public abstract void NewGame();


        public abstract void SaveGame();

        public abstract void LoadGame(string gameName);

        public abstract void ReloadGame();

        public abstract void DeleteGame(string gameName);
    }
}