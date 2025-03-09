using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using TMPro;
using Cysharp.Threading.Tasks;

using IDBunker.LittleWarrior.Configs;


namespace IDBunker.LittleWarrior
{
    public class Entry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _messageTxt;
        [SerializeField] private TextMeshProUGUI _downloadedTxt;
        [SerializeField] private Image _progressBar;

        private void Awake()
        {
            _messageTxt.text = "";
            _downloadedTxt.text = "";
            _progressBar.fillAmount = 0;
        }

        private async void Start()
        {
            Caching.ClearCache();
            await Addressables.CleanBundleCache();

            UniTask<long> downloadSizeTask = Addressables.GetDownloadSizeAsync(Globals.PRELOADING).ToUniTask();

            long downloadSize = await downloadSizeTask;
            long downloadedSize = 0;

            if (downloadSize > 0)
            {
                _messageTxt.text = "Downloading...";
                _downloadedTxt.text = $"{downloadedSize / 1024}KB / {downloadSize / 1024}KB";
                AsyncOperationHandle asyncOp = Addressables.DownloadDependenciesAsync(Globals.PRELOADING);
                await asyncOp;

                UpdateAsync(asyncOp);
            }
            else
            {
                _messageTxt.text = "No updates";
                _downloadedTxt.text = "";
            }

     

            Debug.Log($"Size {downloadSize / 1024} KB");
            //load first level
        }

        private UniTask UpdateAsync(AsyncOperationHandle handle)
        {
            while (handle.GetDownloadStatus().DownloadedBytes < handle.GetDownloadStatus().TotalBytes)
            {
                _progressBar.fillAmount = handle.GetDownloadStatus().DownloadedBytes / handle.GetDownloadStatus().TotalBytes;
                _downloadedTxt.text = $"{handle.GetDownloadStatus().DownloadedBytes / 1024}KB / {handle.GetDownloadStatus().TotalBytes / 1024}KB";
                UniTask.NextFrame();
            }

            _progressBar.fillAmount = 1f;
            _messageTxt.text = "Done";
            _downloadedTxt.text = $"{handle.GetDownloadStatus().TotalBytes / 1024}KB / {handle.GetDownloadStatus().TotalBytes / 1024}KB";
            Debug.Log($"Finished");
            return UniTask.CompletedTask;
        }
    }
}

