using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;
using UnityEngine.Assertions;

namespace FrameworkWIP
{

    public abstract partial class Tween
    {



        protected static class TweenManaager
        {

            private static List<Tween> _runningTweens = new List<Tween>();

            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
            static void Init()
            {
                _runningTweens = new List<Tween>();

                Runtime.UpdateCallback -= Update;
                Runtime.UpdateCallback += Update;
            }


            static void Update()
            {
                for (int i = 0; i < _runningTweens.Count; i++)
                {
                    Tween tween = _runningTweens[i];
                    if (!tween.IsPaused)
                    {
                        tween.Update(tween._useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
                        if (tween.IsComplete)
                        {
                            tween.OnCompleted();
                            _runningTweens.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }

            public static void RunTween(Tween tween)
            {
                if (!tween._isRunning)
                {
                    tween.OnStarted();
                    _runningTweens.Add(tween);
                }
            }

            public static void StopRunningTween(Tween tween)
            {
                if (tween._isRunning)
                {
                    tween.OnStopped();
                    _runningTweens.Remove(tween);
                }
            }

        }
    }


}

