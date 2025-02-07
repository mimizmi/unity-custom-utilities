namespace Mimizh.UnityUtilities.PersistentSystem
{
    public interface IGameData
    {
        public string Name { get; set; }
        public int CurrentSceneGroupIndex { get; set; }
    }
}