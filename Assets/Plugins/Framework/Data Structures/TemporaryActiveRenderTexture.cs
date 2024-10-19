using System;
using UnityEngine;

namespace Framework
{
    public class TemporaryActiveRenderTexture : IDisposable
    {
        private RenderTexture _originalRenderTexture;

        public TemporaryActiveRenderTexture(RenderTexture renderTexture)
        {
            _originalRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;
        }

        public void Dispose()
        {
            RenderTexture.active = _originalRenderTexture;
        }
    }
}
