using System.Collections.Generic;

namespace Mimizh.UnityUtilities.PersistentSystem
{
    public interface IDataService<TGameDta> where TGameDta : IGameData
    {
        void Save(TGameDta data, bool overwrite = true);
        TGameDta Load(string name);
        void Delete(string name);
        void DeleteAll();
        IEnumerable<string> ListSaves();
    }
}