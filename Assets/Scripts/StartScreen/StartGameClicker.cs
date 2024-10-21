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
                SceneManager.LoadScene(1);
                return;
            }

            _slides[_currentIndex].SetActive(true);
            _bg.SetActive(false);
            _currentIndex++;
        }
    }
}