using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{



    public static class AnimatorExtensions
    {
        public static T GetCurrentOrNextStateBehaviour<T>(this Animator animator, int layer) where T : StateMachineBehaviour
        {
            AnimatorStateInfo state = animator.IsInTransition(layer) ? animator.GetNextAnimatorStateInfo(layer) : animator.GetCurrentAnimatorStateInfo(layer);
            StateMachineBehaviour[] behaviours = animator.GetBehaviours(state.fullPathHash, layer);

            for (int i = 0; i < behaviours.Length; i++)
            {
                if (typeof(T).IsAssignableFrom(behaviours[i].GetType())) return (T)behaviours[i];
            }

            return null;
        }

        public static T GetCurrentStateBehaviour<T>(this Animator animator, int layer) where T : StateMachineBehaviour
        {
            StateMachineBehaviour[] behaviours = animator.GetBehaviours(animator.GetCurrentAnimatorStateInfo(layer).fullPathHash, layer);

            for (int i = 0; i < behaviours.Length; i++)
            {
                if (typeof(T).IsAssignableFrom(behaviours[i].GetType())) return (T)behaviours[i];
            }

            return null;
        }

        public static T GetNextStateBehaviour<T>(this Animator animator, int layer) where T : StateMachineBehaviour
        {
            StateMachineBehaviour[] behaviours = animator.GetBehaviours(animator.GetNextAnimatorStateInfo(layer).fullPathHash, layer);

            for (int i = 0; i < behaviours.Length; i++)
            {
                if (typeof(T).IsAssignableFrom(behaviours[i].GetType())) return (T)behaviours[i];
            }

            return null;
        }

        public static void GetCurrentAndNextStateBehaviour<T>(this Animator animator, int layer, out T currentStateBehaciour, out T nextStateBehaviour) where T : StateMachineBehaviour
        {
            if (animator.IsInTransition(layer))
            {
                currentStateBehaciour = GetCurrentStateBehaviour<T>(animator, layer);
                nextStateBehaviour = GetNextStateBehaviour<T>(animator, layer);
            }
            else
            {
                currentStateBehaciour = GetCurrentStateBehaviour<T>(animator, layer);
                nextStateBehaviour = null;
            }
        }


        public static T[] GetCurrentStateBehaviours<T>(this Animator animator, int layer) where T : StateMachineBehaviour
        {
            Type type = typeof(T);
            List<T> list = new List<T>();
            StateMachineBehaviour[] behaviours = animator.GetBehaviours(animator.GetCurrentAnimatorStateInfo(layer).fullPathHash, layer);

            for (int i = 0; i < behaviours.Length; i++)
            {
                if (type.IsAssignableFrom(behaviours[i].GetType()))
                {
                    list.Add((T)behaviours[i]);
                }
            }

            return list.ToArray();
        }

        public static T[] GetNextStateBehaviours<T>(this Animator animator, int layer) where T : StateMachineBehaviour
        {
            Type type = typeof(T);
            List<T> list = new List<T>();
            StateMachineBehaviour[] behaviours = animator.GetBehaviours(animator.GetNextAnimatorStateInfo(layer).fullPathHash, layer);

            for (int i = 0; i < behaviours.Length; i++)
            {
                if (type.IsAssignableFrom(behaviours[i].GetType()))
                {
                    list.Add((T)behaviours[i]);
                }
            }

            return list.ToArray();
        }

        public static void GetCurrentAndNextStateBehaviours<T>(this Animator animator, int layer, out T[] currentStateBehaviours, out T[] nextStateBehaviours) where T : StateMachineBehaviour
        {

            if (animator.IsInTransition(layer))
            {
                currentStateBehaviours = GetCurrentStateBehaviours<T>(animator, layer);
                nextStateBehaviours = GetNextStateBehaviours<T>(animator, layer);
            }
            else
            {
                currentStateBehaviours = GetCurrentStateBehaviours<T>(animator, layer);
                nextStateBehaviours = new T[0];
            }
        }

        public static float GetTransitionProgress(this Animator animator, int layer = 0)
        {
            return animator.GetAnimatorTransitionInfo(layer).normalizedTime;
        }

        public static bool IsCurrentState(this Animator animator, string stateName, bool includeTransitioningTo, int layer = 0)
        {
            int nameHash = Animator.StringToHash(stateName);
            return animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == nameHash || (includeTransitioningTo && animator.IsInTransition(layer) && animator.GetNextAnimatorStateInfo(layer).shortNameHash == nameHash);
        }

        public static bool IsNextState(this Animator animator, string stateName, int layer = 0)
        {
            return animator.GetNextAnimatorStateInfo(layer).shortNameHash == Animator.StringToHash(stateName);
        }

        public static bool HasFinishedState(this Animator animator, string stateName, int layer = 0)
        {
            if (animator.IsInTransition(layer)) return false;

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
            if (stateInfo.shortNameHash != Animator.StringToHash(stateName)) return false;

            return stateInfo.normalizedTime >= 1f;
        }

        public static bool IsTransitioningFromState(this Animator animator, string stateName, int layer = 0)
        {
            return animator.IsInTransition(layer) && animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == Animator.StringToHash(stateName);
        }

        public static bool IsTransitioningToState(this Animator animator, string stateName, int layer = 0)
        {
            return animator.IsInTransition(layer) && animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == Animator.StringToHash(stateName);
        }




        public static AnimationClip GetClip(this Animator animator, string clipName)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name == clipName)
                {
                    return clips[i];
                }
            }

            return null;
        }

        public static bool HasParameter(this Animator animator, string parameterName)
        {
            if (animator.runtimeAnimatorController == null) return false;

            for (int i = 0; i < animator.parameters.Length; i++)
            {
                if (animator.parameters[i].name == parameterName) return true;
            }

            return false;
        }

        public static object GetParameterValue(this Animator animator, int parameterIndex)
        {
            AnimatorControllerParameter parameter = animator.parameters[parameterIndex];

            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Float: return animator.GetFloat(parameter.nameHash);
                case AnimatorControllerParameterType.Int: return animator.GetInteger(parameter.nameHash);
                case AnimatorControllerParameterType.Bool: return animator.GetBool(parameter.nameHash);
            }

            throw new ArgumentOutOfRangeException();
        }

        public static void SetParameterValue(this Animator animator, int parameterIndex, object value)
        {
            AnimatorControllerParameter parameter = animator.parameters[parameterIndex];

            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(parameter.nameHash, (float)value);
                    return;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(parameter.nameHash, (int)value);
                    return;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(parameter.nameHash, (bool)value);
                    return;
            }

            throw new ArgumentOutOfRangeException();
        }

        public static Dictionary<string, AnimationClip> GetClipDictionary(this Animator animator)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            Dictionary<string, AnimationClip> dict = new Dictionary<string, AnimationClip>();

            for (int i = 0; i < clips.Length; i++)
            {
                dict.Add(clips[i].name, clips[i]);
            }

            return dict;
        }


        public static string GetCurrentStateDebug(this Animator animator, int layer)
        {

#if UNITY_EDITOR
            UnityEditor.Animations.AnimatorController controller = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            if (controller == null)
            {
                UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(animator.runtimeAnimatorController);
                controller = so.FindProperty("m_Controller").objectReferenceValue as UnityEditor.Animations.AnimatorController;
                so.Dispose();
            }

            UnityEditor.Animations.AnimatorStateMachine stateMachine = controller.layers[layer].stateMachine;

            if (animator.IsInTransition(layer))
            {
                return GetStateName(stateMachine, animator.GetCurrentAnimatorStateInfo(layer).shortNameHash) + " > " + GetStateName(stateMachine, animator.GetNextAnimatorStateInfo(layer).shortNameHash);
            }

            return GetStateName(stateMachine, animator.GetCurrentAnimatorStateInfo(layer).shortNameHash);
#else
            if (animator.IsInTransition(layer))
            {
                return animator.GetCurrentAnimatorStateInfo(layer).shortNameHash + " > " + animator.GetNextAnimatorStateInfo(layer).shortNameHash;
            }

            return animator.GetCurrentAnimatorStateInfo(layer).shortNameHash.ToString();
#endif

        }

#if UNITY_EDITOR

        public static string GetStateNameDebug(this Animator animator, int shortHash, int layer)
        {
            UnityEditor.Animations.AnimatorController controller = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            if (controller == null)
            {
                UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(animator.runtimeAnimatorController);
                controller = so.FindProperty("m_Controller").objectReferenceValue as UnityEditor.Animations.AnimatorController;
                so.Dispose();
            }

            return GetStateName(controller.layers[layer].stateMachine, shortHash);
        }

        static string GetStateName(UnityEditor.Animations.AnimatorStateMachine stateMachine, int shortHash)
        {
            for (int i = 0; i < stateMachine.states.Length; i++)
            {
                if (Animator.StringToHash(stateMachine.states[i].state.name) == shortHash) return stateMachine.states[i].state.name;
            }

            for (int i = 0; i < stateMachine.stateMachines.Length; i++)
            {
                string result = GetStateName(stateMachine.stateMachines[i].stateMachine, shortHash);
                if (result != null) return result;
            }

            return null;
        }

#endif
    }

}
