using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoundManager
{
    [Serializable]
    public abstract class ComponentSoundBankFilter<T> : SoundBankFilter where T : Behaviour
    {
        protected List<T> _filterComponents = new List<T>();

        public override void OnAdded(EffectSoundInstance sound)
        {
            base.OnAdded(sound);

            for (int i = 0; i < _filterComponents.Count; i++)
            {
                if (_filterComponents[i] == null)
                {
                    _filterComponents.RemoveAt(i);
                    i--;
                }
            }

            T component = sound.GetComponent<T>();
            if (component == null)
            {
                component = sound.gameObject.AddComponent<T>();
            }

            _filterComponents.Add(component);
            component.enabled = true;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            for (int i = 0; i < _filterComponents.Count; i++)
            {
                if (_filterComponents[i] == null)
                {
                    _filterComponents.RemoveAt(i);
                    i--;
                }
            }
        }

        public override void OnRemoved(EffectSoundInstance sound)
        {
            base.OnRemoved(sound);

            for (int i = 0; i < _filterComponents.Count; i++)
            {
                if (_filterComponents[i] == null)
                {
                    _filterComponents.RemoveAt(i);
                    i--;
                }
            }

            T component = sound.GetComponent<T>();
            if (component != null)
            {
                component.enabled = false;
                _filterComponents.Remove(component);
            }
        }
    }
}
