using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Wattle.UI
{
    public class UIRadialIndicator : MonoBehaviour
    {
        [SerializeField] private Image loadingImage;
        [SerializeField] private float fillRate;

        private Tweener tweener = null;

        private void Start()
        {
            loadingImage.fillAmount = 0;
        }

        public void StartIndicator()
        {
            if (tweener != null)
                return;

            loadingImage.fillClockwise = true;
            loadingImage.fillAmount = 0;

            tweener = DOTween.To(() => loadingImage.fillAmount, x => loadingImage.fillAmount = x, 1, 1.2f)
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetEase(Ease.InOutQuad).OnStepComplete(() =>
                     {
                         loadingImage.fillClockwise = !loadingImage.fillClockwise;
                     });
        }

        public void StopIndicator()
        {
            tweener.Kill();
            tweener = null;
        }
    }
}