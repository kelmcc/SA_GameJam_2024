using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TestRenderPassFeature : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        private Material _renderMaterial;
        private int _applyFogPass;

        private RTHandle _copiedColor;

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            Shader s = Shader.Find("Hidden/Andicraft/Volumetric Fog");
            _renderMaterial = CoreUtils.CreateEngineMaterial(s);
            _applyFogPass = _renderMaterial.FindPass("Apply Fog");
            var colorCopyDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            RenderingUtils.ReAllocateIfNeeded(ref _copiedColor, colorCopyDescriptor,
                name: "_VolumetricFogColorCopy");
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.isPreviewCamera ||
                renderingData.cameraData.cameraType == CameraType.Reflection)
                return;

            
            var cameraData = renderingData.cameraData;
            var source = cameraData.renderer.cameraColorTargetHandle;

            CommandBuffer cmd = CommandBufferPool.Get("Volumetric Fog");

            // Copy color to texture
            Blitter.BlitCameraTexture2D(cmd, source, _copiedColor);

            // Create half-size depth target
            // CoreUtils.SetRenderTarget(cmd, halfDepthBuffer);
            // CoreUtils.DrawFullScreen(cmd, renderMaterial, shaderPassId: copyDepthPass);
            // renderMaterial.SetTexture(HalfDepthBuffer, halfDepthBuffer);
            //
            // // Render Fog
            // CoreUtils.SetRenderTarget(cmd, fogBuffer);
            // CoreUtils.DrawFullScreen(cmd, renderMaterial, shaderPassId: renderFogPass);
            //
            // // Blur Fog
            // //renderMaterial.SetTexture("_VFogBuffer", fogBuffer);
            // Blitter.BlitCameraTexture(cmd, fogBuffer, blurBuffer, renderMaterial, blurPassH);
            // Blitter.BlitCameraTexture(cmd, blurBuffer, blurBuffer2, renderMaterial, blurPassV);
            //
            // Blitter.BlitCameraTexture(cmd, blurBuffer2, blurBuffer, renderMaterial, blurPassH);
            // Blitter.BlitCameraTexture(cmd, blurBuffer, blurBuffer2, renderMaterial, blurPassV);
            //
            // renderMaterial.SetTexture(BlurBuffer, blurBuffer2);

            // Apply Fog
            _renderMaterial.SetTexture(BlitTexture, _copiedColor);
            ConfigureTarget(cameraData.renderer.cameraColorTargetHandle);
            // CoreUtils.SetRenderTarget(cmd, cameraData.renderer.cameraColorTargetHandle);
            CoreUtils.DrawFullScreen(cmd, _renderMaterial, shaderPassId: _applyFogPass,
                colorBuffer: cameraData.renderer.cameraColorTargetHandle);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            DestroyImmediate(_renderMaterial);
        }
    }

    CustomRenderPass m_ScriptablePass;
    private static readonly int BlitTexture = Shader.PropertyToID("_BlitTexture");

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass();

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}