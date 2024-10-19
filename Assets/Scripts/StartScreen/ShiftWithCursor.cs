using UnityEngine;

namespace UI
{
    public class ShiftWithCursor : MonoBehaviour
    {
        [SerializeField] private float _shiftOffset = 0.5f;

        private void Update()
        {
            Vector2 shift = -Input.mousePosition * _shiftOffset;
            transform.localPosition = shift;
        }
    }
}