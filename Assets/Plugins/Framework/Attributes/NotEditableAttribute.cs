using UnityEngine;

namespace Framework
{
    public class NotEditableAttribute : PropertyAttribute
    {
        public bool OnlyShowInPlayMode => _onlyShowInPlayMode;

        private bool _onlyShowInPlayMode;

        public NotEditableAttribute(bool onlyShowInPlayMode = false)
        {
            _onlyShowInPlayMode = onlyShowInPlayMode;
        }
    }
}


