using System;
using UnityEngine;

namespace Core
{
    public class OnEscapeExit : MonoBehaviour
    {
        private void Start()
        {
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}