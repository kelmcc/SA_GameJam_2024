using Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class StartGameClicker : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private List<GameObject> _slides = new List<GameObject>();
        [SerializeField] private int _currentIndex;

        private void Awake()
        {
            ResetSlides();
            
            _startButton.onClick.AddListener(() =>
            {
                if (_slides)
                {
                    
                }
                TimeTicker.Instance.transform.parent = transform;
                SceneManager.LoadScene(1);
            });
        }
        
        private void ResetSlides()
        {
            foreach (var slide in _slides)
            {
                slide.SetActive(false);
            }
        }
    }
}