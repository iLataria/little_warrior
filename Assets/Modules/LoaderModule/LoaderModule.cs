using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.ResourceProviders;

using Cysharp.Threading.Tasks;

namespace IDBunker
{
    public class LoaderModule
    {
        private readonly ScreenFadeController _fadeController;

        public LoaderModule(ScreenFadeController fadeController)
        {
            _fadeController = fadeController;
        }

        public async void ActivateLoadedSceneInstance(SceneInstance sceneInstance, LoadSceneMode mode = LoadSceneMode.Single, bool activeOnLoad = true)
        {
            await _fadeController.FadeIn();
            await sceneInstance.ActivateAsync().ToUniTask();
            _fadeController.FadeOut().Forget(); // В будущем фейд аут будет вызывать сама вторая сцена, когда все приготовит
        }
    }
}

