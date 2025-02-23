# UnityUtilities

<p align="center">
    <a href="https://github.com/adammyhre/Unity-Utils" target="_blank">
        <img alt="Inspired From" title="adammyhre's github" src="https://img.shields.io/badge/git--amend_utilities-white?style=flat&logo=github&labelColor=black">
        <img alt="youtube channel" title="adammyhre's youtube" src="https://img.shields.io/badge/git--amend-red?style=flat&logo=youtube&labelColor=red&color=white"></a>
</p>
<hr>

### EnumeratorExtension

```c#
public static IEnumerable<T> GetEnumerator<T>(this IEnumerator<T> enumerator)
```



### Singleton

1. Normal Singleton
2. PersistentSingleton (destroy any new singleton)
3. RegulatorSingleton (destory any old singleton)



### TransformExtensions

```c#
public static void DestroyChildren(this Transform parent)
public static void EnableChildren(this Transform parent)
public static void DisableChildren(this Transform parent)
```



### Vector3Extensions

```c#
public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
public static Vector3 Add(this Vector3 vector, float? x = null, float? y = null, float? z = null)
```



### GameObjectExtensions

```c#
public static T GetOrAdd<T>(this GameObject gameObject)
public static T OrNull<T> (this T obj)
public static void DestroyChildren(this GameObject gameObject)
```



### TypeFilter

Auto get all subtype from interface or abstract class

usage:

```c#
public class TestTypeFilter : MonoBehaviour
    {
        [Header("State Machine")] 
        [TypeFilter(typeof(IState))] [SerializeField]
        private SerializableType staringState;

        private void Start()
        {
            Debug.Log(staringState.Type);
            object instance = Activator.CreateInstance(staringState.Type);
            
            IState currentState = (IState)instance;
            currentState.OnEnter();
        }
    }

    public interface IState
    {
        void OnEnter();
        void OnExit();
    }

    public abstract class baseState : IState
    {
        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }
    }

    public class IdleState : baseState
    {
        public override void OnEnter()
        {
            Debug.Log("Idle");
        }
    }

    public class RunState : baseState
    {
        public override void OnEnter()
        {
            Debug.Log("Run");
        }
    }
```



### SaveLoad System

1. GameData  SaveGameScene etc.

2. CustomData Save Useage

   * Create your Own savable data & entity

   ```c#
   public interface ISavable
   {
       SerializableGuid Id { get; set; }
   }
   public interface IBind<TData> where TData : ISavable
   {
       SerializableGuid Id { get; set; }
       void Bind(TData data);
   }
   
   
   [Serializable]
   public class PlayerData : ISavable
   {
       [field: SerializeField]public SerializableGuid Id { get; set; }
       public Vector3 position;
       public Quaternion rotation;
   }
   
   public class Hero : MonoBehaviour, IBind<PlayerData>
   {
       [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
       [SerializeField] PlayerData data;
   
       public void Bind(PlayerData data)
       {
           this.data = data;
           this.data.Id = Id;
           transform.position = data.position;
           transform.rotation = data.rotation;
       }
   
       void Update()
       {
           data.position = transform.position;
           data.rotation = transform.rotation;
       }
   }
   
   ```

   

   * implement DataService *just giving a type T

   ```c#
   public class GameFileDataService : FileDataService<MyGameData>
   {
       public GameFileDataService(ISerializer serializer) : base(serializer)
       {
       }
   }
   ```

   

   * Implement from the SaveLoadSystem & CreateYourGameData

   ```c#
   [Serializable]
    public class MyGameData : IGameData
    {
        public SavePlayerData savePlayerData;
        public InventoryData inventoryData;
        [SerializeField] private string name;
        [SerializeField] private int currentSceneGroupIndex;
        public string Name
        {
            get => name;
            set => name = value;
        }
        public int CurrentSceneGroupIndex
        {
            get => currentSceneGroupIndex;
            set => currentSceneGroupIndex = value;
        }
    }
    public class MySaveLoadSystem : SaveLoadSystem<MyGameData>
    {
        [Inject] private SceneLoader _sceneLoader;
        protected override void Awake()
        {
            base.Awake();
            dataService = new GameFileDataService(new JsonSerializer());
        }

        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Bind<Hero, SavePlayerData>(gameData.savePlayerData);
            Bind<Inventory.Inventory, InventoryData>(gameData.inventoryData);
        }

        //private void Start() => NewGame();

        public override void NewGame()
        {
            gameData = new MyGameData()
            {
                Name = "Game",
                CurrentSceneGroupIndex = 0
            };
            //SceneManager.LoadScene(gameData.CurrentLevelName);
            Task a = _sceneLoader.LoadSceneGroup(gameData.CurrentSceneGroupIndex);
        }

        public override void SaveGame() => dataService.Save(gameData);

        public override void LoadGame(string gameName)
        {
            gameData = dataService.Load(gameName);

            if (gameData.CurrentSceneGroupIndex < 0 )
            {
                gameData.CurrentSceneGroupIndex = 0;
            }
            
            //SceneManager.LoadScene(gameData.CurrentLevelName);
            Task a = _sceneLoader.LoadSceneGroup(gameData.CurrentSceneGroupIndex);
        }

        public override void ReloadGame() => LoadGame(gameData.Name);

        public override void DeleteGame(string gameName) => dataService.Delete(gameName);
    }
   ```

   * Create Editor Tool for test

   ```c#
   [CustomEditor(typeof(MySaveLoadSystem))]
   public class MySaveManagerEditor : UnityEditor.Editor
   {
       public override void OnInspectorGUI()
       {
           MySaveLoadSystem saveLoadSystem = (MySaveLoadSystem)target;
           string gameName = saveLoadSystem.gameData.Name;
   
           DrawDefaultInspector();
           if (GUILayout.Button("New Game"))
               saveLoadSystem.NewGame();
           if (GUILayout.Button("Save Game"))
               saveLoadSystem.SaveGame();
           if (GUILayout.Button("Load Game"))
               saveLoadSystem.LoadGame(gameName);
           if (GUILayout.Button("Delete Game")) 
               saveLoadSystem.DeleteGame(gameName);
       }
   }
   ```


### SceneManager 

#### Bootstrapper
   ```c#
        public class Bootstrapper : PersistentSingleton<Bootstrapper>
       {
           [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
           static async void Init()
           {
               Debug.Log("Bootstrapper init");
               await SceneManager.LoadSceneAsync("Bootstrapper", LoadSceneMode.Single);
           }
       }
   ```

### How to DownLoad

#### manifest.json
   ```json
      "com.mimizh.unity-utilities": "git@github.com:mimizmi/unity-custom-utilities.git"
   ```
