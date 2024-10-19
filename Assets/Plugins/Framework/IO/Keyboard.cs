using System;
using UnityEngine;

namespace Framework
{
    public enum ModifierKey
    {
        Shift,
        Control,
        Alt,
        Command,
        ControlOrCommand,
        Windows
    }

    /// <summary>
    /// Static wrapper for keyboard input.
    /// </summary>
    public static class Keyboard
    {
        /// <summary>
        /// Whether or not any key is currently held down.
        /// </summary>
        public static bool AnyKey => Input.anyKey;

        /// <summary>
        /// Whether or not any key was pressed since the last frame.
        /// </summary>
        public static bool AnyKeyDown => Input.anyKeyDown;

        /// <summary>
        /// Checks whether or not a key is currently pressed.
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key is currently pressed</returns>
        public static bool GetKey(KeyCode key)
        {
            return Input.GetKey(key);
        }

        /// <summary>
        /// Checks whether or not a key was pressed since the last frame.
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key was pressed since the last frame</returns>
        public static bool GetKeyDown(KeyCode key)
        {
            return Input.GetKeyDown(key);
        }

        /// <summary>
        /// Checks whether or not a key was released since the last frame.
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key was released since the last frame</returns>
        public static bool GetKeyUp(KeyCode key)
        {
            return Input.GetKeyUp(key);
        }

        public static bool GetKey(params KeyCode[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (Input.GetKey(keys[i])) return true;
            }

            return false;
        }

        public static bool GetKeyDown(params KeyCode[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (Input.GetKeyDown(keys[i])) return true;
            }

            return false;
        }

        public static bool GetKeyUp(params KeyCode[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (Input.GetKeyUp(keys[i])) return true;
            }

            return false;
        }

        public static bool GetEditorKey(KeyCode key)
        {
            if (!Runtime.IsEditor) return false;
            return Input.GetKey(key);
        }

        /// <summary>
        /// Checks whether or not a key was pressed since the last frame.
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key was pressed since the last frame</returns>
        public static bool GetEditorKeyDown(KeyCode key)
        {
            if (!Runtime.IsEditor) return false;
            return Input.GetKeyDown(key);
        }

        /// <summary>
        /// Checks whether or not a key was released since the last frame.
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key was released since the last frame</returns>
        public static bool GetEditorKeyUp(KeyCode key)
        {
            if (!Runtime.IsEditor) return false;
            return Input.GetKeyUp(key);
        }

        public static bool GetEditorKey(params KeyCode[] keys)
        {
            if (!Runtime.IsEditor) return false;

            for (int i = 0; i < keys.Length; i++)
            {
                if (Input.GetKey(keys[i])) return true;
            }

            return false;
        }

        public static bool GetEditorKeyDown(params KeyCode[] keys)
        {
            if (!Runtime.IsEditor) return false;

            for (int i = 0; i < keys.Length; i++)
            {
                if (Input.GetKeyDown(keys[i])) return true;
            }

            return false;
        }

        public static bool GetEditorKeyUp(params KeyCode[] keys)
        {
            if (!Runtime.IsEditor) return false;

            for (int i = 0; i < keys.Length; i++)
            {
                if (Input.GetKeyUp(keys[i])) return true;
            }

            return false;
        }

        public static bool GetEditorCombinationDown(ModifierKey modifier, KeyCode key)
        {
            return Runtime.IsEditor && GetModifier(modifier) && GetKeyDown(key);
        }

        public static bool GetCombination(ModifierKey modifier, KeyCode key)
        {
            return GetModifier(modifier) && GetKey(key);
        }

        public static bool GetCombinationDown(ModifierKey modifier, KeyCode key)
        {
            return GetModifier(modifier) && GetKeyDown(key);
        }

        public static bool GetCombinationUp(ModifierKey modifier, KeyCode key)
        {
            return (GetModifier(modifier) && GetKeyUp(key)) || (GetModifierUp(modifier) && GetKey(key));
        }

        public static bool GetCombination(ModifierKey modifier, ModifierKey secondModifier, KeyCode key)
        {
            return GetModifier(modifier) && GetModifier(secondModifier) && GetKey(key);
        }

        public static bool GetCombinationDown(ModifierKey modifier, ModifierKey secondModifier, KeyCode key)
        {
            return GetModifier(modifier) && GetModifier(secondModifier) && GetKeyDown(key);
        }

        public static bool GetCombinationUp(ModifierKey modifier, ModifierKey secondModifier, KeyCode key)
        {
            return (GetModifier(modifier) && GetModifier(secondModifier) && GetKeyUp(key)) || (GetModifierUp(modifier) && GetModifier(secondModifier) && GetKey(key)) || (GetModifier(modifier) && GetModifierUp(secondModifier) && GetKey(key));
        }

        public static bool GetAnyModifier()
        {
            int numModifiers = EnumUtils.GetCount<ModifierKey>();
            for (int i = 0; i < numModifiers; i++)
            {
                if (GetModifier((ModifierKey)i)) return true;
            }

            return false;
        }

        public static bool GetAnyModifierDown()
        {
            int numModifiers = EnumUtils.GetCount<ModifierKey>();
            for (int i = 0; i < numModifiers; i++)
            {
                if (GetModifierDown((ModifierKey)i)) return true;
            }

            return false;
        }


        public static bool GetAnyModifierUp()
        {
            int numModifiers = EnumUtils.GetCount<ModifierKey>();
            for (int i = 0; i < numModifiers; i++)
            {
                if (GetModifierUp((ModifierKey)i)) return true;
            }

            return false;
        }

        public static bool GetModifier(ModifierKey modifier)
        {
            switch (modifier)
            {
                case ModifierKey.Control: return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                case ModifierKey.Shift: return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                case ModifierKey.Alt: return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
                case ModifierKey.Command: return Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
                case ModifierKey.ControlOrCommand: return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
                case ModifierKey.Windows: return Input.GetKey(KeyCode.LeftWindows) || Input.GetKey(KeyCode.RightWindows);
                default: throw new ArgumentOutOfRangeException(nameof(modifier), modifier, null);
            }
        }

        public static bool GetModifierDown(ModifierKey modifier)
        {
            switch (modifier)
            {
                case ModifierKey.Control: return Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);
                case ModifierKey.Shift: return Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
                case ModifierKey.Alt: return Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt);
                case ModifierKey.Command: return Input.GetKeyDown(KeyCode.LeftCommand) || Input.GetKeyDown(KeyCode.RightCommand);
                case ModifierKey.ControlOrCommand: return Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.LeftCommand) || Input.GetKeyDown(KeyCode.RightCommand);
                case ModifierKey.Windows: return Input.GetKeyDown(KeyCode.LeftWindows) || Input.GetKeyDown(KeyCode.RightWindows);
                default: throw new ArgumentOutOfRangeException(nameof(modifier), modifier, null);
            }
        }

        public static bool GetModifierUp(ModifierKey modifier)
        {
            switch (modifier)
            {
                case ModifierKey.Control: return Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl);
                case ModifierKey.Shift: return Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift);
                case ModifierKey.Alt: return Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt);
                case ModifierKey.Command: return Input.GetKeyUp(KeyCode.LeftCommand) || Input.GetKeyUp(KeyCode.RightCommand);
                case ModifierKey.ControlOrCommand: return Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl) || Input.GetKeyUp(KeyCode.LeftCommand) || Input.GetKeyUp(KeyCode.RightCommand);
                case ModifierKey.Windows: return Input.GetKeyUp(KeyCode.LeftWindows) || Input.GetKeyUp(KeyCode.RightWindows);
                default: throw new ArgumentOutOfRangeException(nameof(modifier), modifier, null);
            }
        }
    }
}