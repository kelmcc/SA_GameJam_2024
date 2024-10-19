using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace SteppedAnimations
{
    [RequireComponent(typeof(Animator))]
    public class SimpleAnimationClipAnimatorPlayer : MonoBehaviour
    {
        private bool _initialized = false;
        
        [SerializeField] private new AnimationClip animation;
        [SerializeField] private bool loop;
        
        private Animator _animator;
        private PlayableGraph _graph;
        private AnimationClipPlayable _clipPlayable;
        private AnimationPlayableOutput _animationOutput;


        void Initialize()
        {
            if (!_initialized)
            {
                _initialized = true;
                _graph = PlayableGraph.Create();
                _animator = GetComponent<Animator>();
                _graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
                _animationOutput = AnimationPlayableOutput.Create(_graph, "Animation", _animator);
                _graph.Play();
            }
        }

        public float GetLength()
        {
            Initialize();
            
            if (!_clipPlayable.IsNull())
            {
                return _clipPlayable.GetAnimationClip().length;    
            }

            return 0;
        }

        public void OnEnable()
        {
            // SetTimeOffset(0);
            IsPlaying = true;
        }

        public void SetTimeOffset(double offset)
        {   
            Initialize();
            if (!_graph.GetRootPlayable(0).IsNull())
            {
                _graph.GetRootPlayable(0).SetTime(offset);
                _graph.Evaluate();
            }
        }

        public void SetSpeed(float speed)
        {
            Initialize();
            _graph.GetRootPlayable(0).SetSpeed(speed);
        }

        void Awake()
        {
            ResetPlayable();
        }


        private void ResetPlayable()
        {
            if (!_graph.IsValid())
            {
                _initialized = false;
            }

            Initialize();

            if (!_clipPlayable.IsNull())
            {
                _clipPlayable.SetDone(true);
                _clipPlayable.Destroy();
            }


            _clipPlayable = default;

            if (animation)
            {
                if (!_graph.IsValid())
                {
                    _graph = PlayableGraph.Create();
                }

                _clipPlayable = AnimationClipPlayable.Create(_graph, animation);
                _clipPlayable.Play();
                _clipPlayable.SetPropagateSetTime(true);
                _graph.GetRootPlayable(0).SetPropagateSetTime(true);
                IsPlaying = true;
                _animationOutput.SetSourcePlayable(_clipPlayable);
            }
        }




        void Update()
        {
            if (!_clipPlayable.IsNull())
            {
                var deltaTime = Time.deltaTime;
                
                var length = _clipPlayable.GetAnimationClip().length;

                if (_clipPlayable.GetTime() > length - Time.deltaTime)
                {
                    if (loop)
                    {
                        SetTimeOffset((_clipPlayable.GetTime() + Time.deltaTime) % length);
                    }
                    else
                    {
                        IsPlaying = false;
                    }
                }

                _graph.Evaluate(deltaTime);
            }
        }

        private void OnDestroy()
        {
            if (!_clipPlayable.IsNull())
            {
                _clipPlayable.Destroy();
            }

            if (_graph.IsValid())
                _graph.Destroy();
        }

        public void SetClip(AnimationClip animationClip, bool shouldLoop = false,  bool restartIfAlreadyPlaying=true)
        {
            if (restartIfAlreadyPlaying || animationClip != this.animation)
            {
                Initialize();
                animation = animationClip;
                IsPlaying = true;
                ResetPlayable();
            }
        }

        public void ClearClip()
        {
            Initialize();
            animation = null;
            ResetPlayable();
            IsPlaying = false;
        }

        public bool IsPlaying { get; private set; }
    }
}