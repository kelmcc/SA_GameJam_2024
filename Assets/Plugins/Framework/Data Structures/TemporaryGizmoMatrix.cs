using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class TemporaryGizmoMatrix : IDisposable
    {
        private Matrix4x4 _originalMatrix;

        public TemporaryGizmoMatrix(Matrix4x4 matrix)
        {
            _originalMatrix = Gizmos.matrix;
            Gizmos.matrix = matrix;
        }

        public void Dispose()
        {
            Gizmos.matrix = _originalMatrix;
        }
    }

}
