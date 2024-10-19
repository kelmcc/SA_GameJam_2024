using UnityEngine;

namespace Core
{
    public class OnEscapeExit : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}