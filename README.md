# UnityUtilities

<p align="center">
    <a href="https://github.com/adammyhre/Unity-Utils" target="_blank">
        <img alt="Inspired From" title="adammyhre's github" src="https://img.shields.io/badge/git--amend_utilities-white?style=flat&logo=github&labelColor=black">
        <img alt="youtube channel" title="adammyhre's youtube" src="https://img.shields.io/badge/git--amend-red?style=flat&logo=youtube&labelColor=red&color=white"></a>
</p>
<hr>
unity utilities is a collection of some useful tools in unity, inspired by [git-amend](https://github.com/adammyhre/Unity-Utils).



## Installation

Use PackageManager with git link

```bash
https://github.com/mimizmi/unity-custom-utilities.git
```

Add to manifest.json

```json
"com.mimizh.unity-utilities": "git@github.com:mimizmi/unity-custom-utilities.git"
```

[TOC]

## Usage

### Extension

| IEnumerable                        |                                                           |
| ---------------------------------- | --------------------------------------------------------- |
| get a random element from sequence | `public static T Random<T>(this IEnumerable<T> sequence)` |

| List                                                         |                                                              |
| ------------------------------------------------------------ | ------------------------------------------------------------ |
| Swaps two elements in the list at the specified indices.     | `public static void Swap<T>(this IList<T> list, int indexA, int indexB)` |
| Shuffles the elements in the list using the Durstenfeld implementation of the Fisher-Yates algorithm. | `public static IList<T> Shuffle<T>(this IList<T> list)`      |
| Filters a collection based on a predicate and returns a new list | `public static IList<T> Filter<T>(this IList<T> source, Predicate<T> predicate)` |

| VisualElement                           |                                                              |
| --------------------------------------- | ------------------------------------------------------------ |
| create a VisualElement child with style | `public static VisualElement CreateChild(this VisualElement parent, params string[] classes)` |
| create a type T child with style        | `public static T CreateChild<T>(this VisualElement parent, params string[] classes) where T : VisualElement, new()` |
| add to a parent                         | `public static T AddTo<T>(this T child, VisualElement parent) where T : VisualElement` |
| add manipulator when creating           | `public static T WithManipulator<T>(this T visualElement, IManipulator manipulator) where T : VisualElement` |

| Vector3 |                                                              |
| ------- | ------------------------------------------------------------ |
|         | `public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)` |
|         | `public static Vector3 Add(this Vector3 vector, float? x = null, float? y = null, float? z = null)` |
|         | `RandomPointInAnnulus(this Vector3 origin, float minRadius, float maxRadius)` |

| GameObject |                                                              |
| ---------- | ------------------------------------------------------------ |
|            | `public static T GetOrAdd<T>(this GameObject gameObject) where T : Component` |
|            | `public static T OrNull<T> (this T obj) where T : Object => obj ? obj : null;` |



### Singleton

- Normal Singleton

- PersistentSingleton (destroy any new singleton)

- RegulatorSingleton (destory any old singleton)



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

   

   * implement DataService just giving a type T

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

1. Create a bootstraper scene and add the script for bootstrap

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

2. Create a SceneLoader as below

   ```c#
   using System.Threading.Tasks;
   using Mimizh.UnityUtilities;
   using Mimizh.UnityUtilities.SceneManagement;
   using PrimeTween;
   using UnityEngine;
   using UnityEngine.UI;
   
   namespace _Project._Scripts.PersistentSystem
   {
       public class MySceneLoader : PersistentSingleton<MySceneLoader> 
       {
           [SerializeField] private Image loadingBar;
           [SerializeField] float fillSpeed = 0.5f;
           [SerializeField] Canvas loadingCanvas;
           [SerializeField] Camera loadingCamera;
           [SerializeField] SceneGroup[] sceneGroups;
           [SerializeField] private SceneFader fader;
           
           float targetProgress;
           private bool isLoading;
           
           public readonly SceneGroupManager Manager = new ();
           
           protected override void Awake()
           {
               base.Awake();
               Manager.OnSceneLoaded += sceneName => Debug.Log("loaded :" + sceneName);
               Manager.OnSceneUnloaded += sceneName => Debug.Log("Unloaded :" + sceneName);
               //Manager.OnGroupSceneLoaded += InitializeBattleHUD;
           }
   
           async void Start()
           {
               await LoadSceneGroup(0);
           }
   
           void Update()
           {
               if (!isLoading) return;
               
               float currentFillAmount = loadingBar.fillAmount;
               float progressDifference = Mathf.Abs(currentFillAmount - targetProgress);
               
               float dynamicFillSpeed = progressDifference * fillSpeed;
               
               loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, targetProgress, dynamicFillSpeed * Time.deltaTime);
           }
   
           public async Task LoadSceneGroup(int index)
           {
               loadingBar.fillAmount = 0;
               targetProgress = 1f;
               if (index < 0 || index >= sceneGroups.Length)
               {
                   Debug.LogError("Scene Group index is out of range.");
                   return;
               }
               
               LoadingProgress progress = new LoadingProgress();
               progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);
               await EnableLoadingCanvas();
               
               await Manager.LoadSceneAsync(sceneGroups[index], progress);
               await EnableLoadingCanvas(false);
           }
   
           private async Task EnableLoadingCanvas(bool enable = true)
           {
               isLoading = enable;
               //var canvasGroup = loadingCanvas.gameObject.GetOrAdd<CanvasGroup>();
               if (isLoading)
               {
                   loadingCanvas.gameObject.SetActive(enable);
                   loadingCamera.gameObject.SetActive(enable);
                   
                   await fader.FadeOut();
               }
               else
               {
                   await fader.FadeIn();
                   
                   loadingCanvas.gameObject.SetActive(enable);
                   loadingCamera.gameObject.SetActive(enable);
               }
               
           }
           
       }
   }
   ```

   ```c#
   using System;
   using System.Threading.Tasks;
   using PrimeTween;
   using UnityEngine;
   using UnityEngine.UI;
   
   namespace _Project._Scripts.PersistentSystem
   {
       public class SceneFader : MonoBehaviour
       {
           public float FadeDuration = 1f;
           [SerializeField] private FadeType fadeType = FadeType.PlainBlack;
           private int _fadeAmount = Shader.PropertyToID("_FadeAmount");
           private int _useShutters = Shader.PropertyToID("_UseShutters");
           private int _useGoop = Shader.PropertyToID("_UseGoop");
           private int _usePlainBlack = Shader.PropertyToID("_UsePlainBlack");
           private int _useRadialWipe = Shader.PropertyToID("_UseRadialWipe");
           
           private int? _lastEffect;
           
           private Material _material;
           
           private Image _image;
           
           public enum FadeType
           {
               Shutters,
               RadialWipe,
               PlainBlack,
               Goop
           }
   
           private void Awake()
           {
               _image = GetComponent<Image>();
   
               Material mat = _image.material;
               _image.material = new Material(mat);
               _material = _image.material;
   
               _lastEffect = _useShutters;
           }
   
           public async Task FadeOut()
           {
               ChangeFadeEffect(fadeType);
               await StartFadeOut();
           }
           
           public async Task FadeIn()
           {
               ChangeFadeEffect(fadeType);
               await StartFadeIn();
           }
   
           private async Task StartFadeOut()
           {
               _material.SetFloat(_fadeAmount, 0f);
               var tween = Tween.MaterialProperty(_material, _fadeAmount, 1f, FadeDuration, Ease.InOutSine);
               await tween;
           }
   
           private async Task StartFadeIn()
           {
               _material.SetFloat(_fadeAmount, 1f);
               var tween = Tween.MaterialProperty(_material, _fadeAmount, 0f, FadeDuration, Ease.InOutSine);
               await tween;
           }
           
   
           private void ChangeFadeEffect(FadeType fadeType)
           {
               if (_lastEffect.HasValue)
               {
                   _material.SetFloat(_lastEffect.Value, 0f);
               }
   
               switch (fadeType)
               {
                   case FadeType.Shutters:
                       SwitchEffect(_useShutters);
                       break;
                   case FadeType.RadialWipe:
                       SwitchEffect(_useRadialWipe);
                       break;
                   case FadeType.PlainBlack:
                       SwitchEffect(_usePlainBlack);
                       break;
                   case FadeType.Goop:
                       SwitchEffect(_useGoop);
                       break;
               }
           }
   
           private void SwitchEffect(int effectToTurnOn)
           {
               _material.SetFloat(effectToTurnOn, 1f);
   
               _lastEffect = effectToTurnOn;
           }
       }
   }
   ```

   

### ImprovedTimers

use timer automatically inject in unity PlayerLoop

Usage

1. CountDownTimer

   ```c#
   float timerDuration = 10f;
   var timer = new CountDownTimer(timerDuration);
   timer.OnTimerStart += () => Debug.Log("Timer started")
   timer.OnTimerStop += () => Debug.Log("Timer stopped")
   
   timer.Start(); //
   
   // Dispose
   void OnDestory(){
       timer.Dispose();
   }
   ```

2. FrequencyTimer

   ```c#
   int tickpersecond = 2;
   var timer = new FrequencyTimer(tickpersecond);
   
   timer.Start();
   
   // reset
   timer.Reset(); // reset to 0
   timer.Reset(5); // reset to 0 with a new frequency
   
   void OnDestory(){
       timer.Dispose();
   }
   ```

3. StopwatchTimer