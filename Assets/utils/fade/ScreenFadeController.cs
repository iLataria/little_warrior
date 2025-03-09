using UnityEngine;
using UnityEngine.UI;

using Zenject;
using DG.Tweening;
using Cysharp.Threading.Tasks;

using IDBunker.LittleWarrior.Configs;

namespace IDBunker
{
    public class ScreenFadeController : MonoBehaviour
    {
        private Image _fadeImg;

        [Inject]
        private void Construct(Image fadeImg)
        {
            _fadeImg = fadeImg;
        }

        public async UniTask FadeIn(bool atOnce = false)
        {
            _fadeImg.raycastTarget = true;

            if (atOnce)
                _fadeImg.color = Color.black;
            else
            {
                UniTask task = _fadeImg.DOFade(1f, Globals.FADETIME).ToUniTask();
                await task;
            }
        }

        public async UniTask FadeOut()
        {
            UniTask task = _fadeImg.DOFade(0f, Globals.FADETIME).ToUniTask();
            await task;
            _fadeImg.raycastTarget = false;
        }
    }
}

