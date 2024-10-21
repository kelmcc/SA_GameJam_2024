using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class StartGameClicker : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private GameObject _bg;
        [SerializeField] private List<GameObject> _slides = new List<GameObject>();
        private int _currentIndex = 0;

        [SerializeField] private bool backToMain = false;
        
        private void Awake()
        {
            ResetSlides();

            _startButton.onClick.AddListener(OnClick);
        }

        private void ResetSlides()
        {
            foreach (var slide in _slides)
            {
                slide.SetActive(false);
            }
        }

        private void OnClick()
        {
            ResetSlides();

            if (_currentIndex == _slides.Count)
            {
                TimeTicker.Instance.transform.parent = transform;

                if (backToMain)
                {
                    SceneManager.LoadScene(0);
                }
                else
                {
                    SceneManager.LoadScene(1);
                    SceneManager.LoadScene(2, LoadSceneMode.Additive);
                }
               
                return;
            }

            _slides[_currentIndex].SetActive(true);
            _bg.SetActive(false);
            _currentIndex++;
        }
    }
}