using Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class StartGameClicker : MonoBehaviour
    {
        [SerializeField] private Button _startButton;

        private void Awake()
        {
            _startButton.onClick.AddListener(() =>
            {
                TimeTicker.Instance.transform.parent = transform;
                SceneManager.LoadScene(1);
            });
        }
    }
}