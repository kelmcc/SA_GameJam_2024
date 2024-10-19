using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class UndoGroup : IDisposable
    {

        private int _groupIndex;

        public UndoGroup(string name)
        {
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName(name);
            _groupIndex = Undo.GetCurrentGroup();
        }

        public void Dispose()
        {
            Undo.CollapseUndoOperations(_groupIndex);
        }
    }

}
