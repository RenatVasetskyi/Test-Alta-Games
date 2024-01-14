using UnityEngine;

namespace UI.Game
{
    public class GameOverWindow : MonoBehaviour
    {
        private const float ScaleDuration = 1f;
        
        [SerializeField] private GameObject _text;
        
        public void ShowText()
        {
            _text.transform.localScale = Vector3.zero;

            LeanTween.scale(_text, Vector3.one, ScaleDuration);
        }
    }
}