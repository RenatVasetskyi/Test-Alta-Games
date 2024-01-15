using System.Collections;
using UnityEngine;

namespace UI.Base
{
    public class LoadingCurtain : MonoBehaviour
    {
        private const float DisappearTimeSpeed = 0.03f;
        private const float DisappearStep = 0.01f;
        
        [SerializeField] private CanvasGroup _curtain;
        
        public void Show()
        {
            gameObject.SetActive(true);
            
            _curtain.alpha = 1;
        }

        public void Hide()
        {
            StartCoroutine(DoFadeIn());
        }

        private IEnumerator DoFadeIn()
        {
            while (_curtain.alpha > 0)
            {
                _curtain.alpha -= DisappearStep;
                
                yield return new WaitForSeconds(DisappearTimeSpeed);
            }

            Destroy(gameObject);
        }
    }
}
