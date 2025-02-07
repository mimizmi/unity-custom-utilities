using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Mimizh.UnityUtilities.SceneManagement
{
    public class SceneLoader : MonoBehaviour, IDependencyProvider
    {
        [Provide]
        public SceneLoader Provide()
        {
            return this;
        }
        [SerializeField] private Image loadingBar;
        [SerializeField] float fillSpeed = 0.5f;
        [SerializeField] Canvas loadingCanvas;
        [SerializeField] Camera loadingCamera;
        [SerializeField] SceneGroup[] sceneGroups;
        
        float targetProgress;
        private bool isLoading;
        
        public readonly SceneGroupManager Manager = new SceneGroupManager();

        private void Awake()
        {
            Manager.OnSceneLoaded += sceneName => Debug.Log("Loaded :" + sceneName);
            Manager.OnSceneUnloaded += sceneName => Debug.Log("Unloaded :" + sceneName);
            Manager.OnGroupSceneLoaded += () => Debug.Log("Group loaded");
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
            EnableLoadingCanvas();
            
            await Manager.LoadSceneAsync(sceneGroups[index], progress);
            EnableLoadingCanvas(false);
        }

        public void EnableLoadingCanvas(bool enable = true)
        {
            isLoading = enable;
            loadingCanvas.gameObject.SetActive(enable);
            loadingCamera.gameObject.SetActive(enable);
        }
    }
}