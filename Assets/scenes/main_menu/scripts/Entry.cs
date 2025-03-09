using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

using TMPro;
using Zenject;
using DG.Tweening;
using Cysharp.Threading.Tasks;

using IDBunker.LittleWarrior.Configs;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace IDBunker.LittleWarrior
{
    public class Entry : MonoBehaviour
    {
        [SerializeField] private Button _playBtn;
        [SerializeField] private Image _progressBar;
        [SerializeField] private TextMeshProUGUI _messageTxt;
        [SerializeField] private TextMeshProUGUI _downloadedTxt;
        [SerializeField] private CanvasGroup _playBtnCanvasGroup;
        [SerializeField] private CanvasGroup _downloadInfoCanvasGroup;
        [SerializeField] private AssetReference _sceneToLoadAssetRef;

        private LoaderModule _loaderModule;
        private SceneInstance _loadedSceneInstance;
        private ScreenFadeController _screenFadeController;
 
        [Inject]
        private void Construct(ScreenFadeController screenFadeController, LoaderModule loaderModule)
        {
            _loaderModule = loaderModule;
            _screenFadeController = screenFadeController;
        }

        private async void Start()
        {
            _screenFadeController.FadeIn(atOnce: true).Forget(); // Прячем экран, чтобы подготовить данные в UI. Текст, числа

            _playBtnCanvasGroup.alpha = 0f;
            _downloadInfoCanvasGroup.alpha = 0f;

            _playBtn.onClick.AddListener(OnPlayBtnClicked);

            SetProgressBarDisplayValue(0f);
            SetProgressDisplayTxtInfo("Looking for updates...", string.Empty);

            await _screenFadeController.FadeOut(); // Показываем экран, данные 
            await _downloadInfoCanvasGroup.DOFade(1f, Globals.FADETIME);

            AsyncOperationHandle<SceneInstance> sceneInstanceAsyncOp = await HandleSceneDownloading();

            await DisplayLoadingProgressAsync(sceneInstanceAsyncOp);

            HandleLoadingErrors(sceneInstanceAsyncOp);

            _loadedSceneInstance = sceneInstanceAsyncOp.Result;

            await UniTask.WaitForSeconds(1f); // По красоте задержку добавляем

            await _downloadInfoCanvasGroup.DOFade(0f, Globals.FADETIME);
            await _playBtnCanvasGroup.DOFade(1f, Globals.FADETIME);
        }

        private async UniTask<AsyncOperationHandle<SceneInstance>> HandleSceneDownloading()
        {
            UniTask<long> downloadSizeTask = Addressables.GetDownloadSizeAsync(Globals.PRELOADING).ToUniTask();
            long downloadSize = await downloadSizeTask;

            if (downloadSize > 0)
            {
                // Логика загрузки бандлов из стороннего сервера
            }
            else
            {
                // логика если бандлы есть локально и ничего грузить с сервера не нужно
                SetProgressBarDisplayValue(1f);
                SetProgressDisplayTxtInfo("No updates", "");
            }

            return Addressables.LoadSceneAsync(_sceneToLoadAssetRef, activateOnLoad: false); // обновлений нет, сцена загрузится из локального бандла
        }

        private async UniTask DisplayLoadingProgressAsync(AsyncOperationHandle asyncOp)
        {
            while (asyncOp.PercentComplete < 1)
            {
                SetProgressBarDisplayValue(asyncOp.PercentComplete); // Прогресс бар идет от 0 до 1, а не от 0 до 100
                await UniTask.NextFrame();
            }

            SetProgressBarDisplayValue(asyncOp.PercentComplete); // Нет, это не лишняя строка :)
        }

        private void HandleLoadingErrors(AsyncOperationHandle asyncOp)
        {
            if (asyncOp.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"Failed to load scene {asyncOp.OperationException.Message}");
                return;
            }
        }

        private void SetProgressBarDisplayValue(float value)
        {
            _progressBar.fillAmount = value;
        }

        private void SetProgressDisplayTxtInfo(string messageTxt, string downloadedTxt)
        {
            _messageTxt.text = messageTxt;
            _downloadedTxt.text = downloadedTxt;
        }

        private void OnPlayBtnClicked()
        {
            _loaderModule.ActivateLoadedSceneInstance(_loadedSceneInstance);
        }

        private void OnDestroy()
        {
            _playBtn.onClick.RemoveAllListeners();
        }
    }
}

