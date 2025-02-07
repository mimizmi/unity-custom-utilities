using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Mimizh.UnityUtilities.PersistentSystem
{
    public abstract class FileDataService<TGameData> : IDataService<TGameData> where TGameData : IGameData
    {
        protected ISerializer _serializer;
        protected string _dataPath;
        protected string _fileExtension;

        public FileDataService(ISerializer serializer)
        {
            _dataPath = Application.persistentDataPath;
            _fileExtension = "json";
            _serializer = serializer;
        }

        string GetPathToFile(string fileName)
        {
            return Path.Combine(_dataPath, string.Concat(fileName, ".", _fileExtension));
        }
        
        public void Save(TGameData data, bool overwrite = true)
        {
            string fileLocation = GetPathToFile(data.Name);

            if (!overwrite && File.Exists(fileLocation))
            {
                throw new IOException($"File {fileLocation} already exists.");
            }
            File.WriteAllText(fileLocation, this._serializer.Serialize(data));
        }

        public TGameData Load(string name)
        {
            string fileLocation = GetPathToFile(name);
            if (!File.Exists(fileLocation))
            {
                throw new FileNotFoundException($"File {fileLocation} not found.");
            }
            return _serializer.Deserialize<TGameData>(File.ReadAllText(fileLocation));
        }

        public void Delete(string name)
        {
            string fileLocation = GetPathToFile(name);
            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
            }
        }

        public void DeleteAll()
        {
            foreach (var filePath in Directory.GetFiles(_dataPath))
            {
                File.Delete(filePath);
            }
        }

        public IEnumerable<string> ListSaves()
        {
            foreach (var path in Directory.EnumerateFiles(_dataPath))
            {
                if (Path.GetExtension(path) == _fileExtension)
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
            
        }
    }
}