using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace UI
{
    public class SlideShow : MonoBehaviour
    {
        private int _currentIndex = 0;
        [SerializeField] private List<GameObject> _slides = new List<GameObject>();

        private void Start()
        {
            TimeTicker.Instance.TickInterval = 2f;
            TimeTicker.OnTick += OnTick;


            _currentIndex = 0;
            ResetSlides();
            _slides[_currentIndex].SetActive(true);
        }

        private void OnTick()
        {
            _currentIndex = (_currentIndex + 1) % _slides.Count;
            ResetSlides();
            _slides[_currentIndex].SetActive(true);
        }

        private void ResetSlides()
        {
            foreach (var slide in _slides)
            {
                slide.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            TimeTicker.OnTick -= OnTick;
        }
    }
}