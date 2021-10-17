using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Chroma.Rendering.ScreenEffects
{
    public class ScreenFadePass : ScriptableRenderPass
    {
        private FadeSettings settings = null;


        public ScreenFadePass(FadeSettings newSettings)
        {
            settings = newSettings;
            renderPassEvent = newSettings.renderPassEvent;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Set command to store instructions
            CommandBuffer command = CommandBufferPool.Get(settings.profilerTag);

            // Get the locations of our textures
            RenderTargetIdentifier source = BuiltinRenderTextureType.CameraTarget;
            RenderTargetIdentifier destination = BuiltinRenderTextureType.CurrentActive;

            // Copy texture info with added material
            command.SetSinglePassStereo(SinglePassStereoMode.Instancing);
            command.Blit(source, destination, settings.runTimeMaterial);
            command.SetSinglePassStereo(SinglePassStereoMode.None);

            context.ExecuteCommandBuffer(command);

            // Release command
            CommandBufferPool.Release(command);
        }
    }
}