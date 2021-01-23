using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Dither : ScriptableRendererFeature
{
	class DitheringPass : ScriptableRenderPass
	{
		// Constants
		const string k_RenderInscatteringTag = "Render Inscattering Pass";

		// Things
		Material m_DitheringMaterial;
		RenderTargetIdentifier m_Source; // pointer to a Texture on the CPU
		RenderTargetHandle m_TempTexture; // pointer to a Texture on the GPU
		[Range(0f, 1f)]
		float m_Intensity;

		// Other things
		private Vector3[] frustumCorners;
		private Vector4[] vectorArray;

		// Constructor
		public DitheringPass(Material material)
		{
			m_DitheringMaterial = material;
			m_TempTexture.Init("_TempTexture");
		}

		public void SetSource(RenderTargetIdentifier source) => m_Source = source;
		public void SetIntensity(float intensity) => m_Intensity = intensity;

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer cmd = CommandBufferPool.Get(k_RenderInscatteringTag); // Name is for debugging

			RenderTextureDescriptor cameraTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor; // Get color texture parameters
			cmd.GetTemporaryRT(m_TempTexture.id, cameraTextureDescriptor, FilterMode.Point);

			m_DitheringMaterial.SetFloat("_Intensity",m_Intensity);
			Blit(cmd, m_TempTexture.Identifier(), m_Source, m_DitheringMaterial, 0);

			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}

		public override void FrameCleanup(CommandBuffer cmd)
		{
			cmd.ReleaseTemporaryRT(m_TempTexture.id);
		}
	}

	DitheringPass m_InscatteringPass;
	public RenderPassEvent re;
	public float intensity;

	public override void Create()
	{
		Material material = new Material(Shader.Find("Hidden/Dither/Dither"));
		m_InscatteringPass = new DitheringPass(material);
		m_InscatteringPass.renderPassEvent = re;
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		m_InscatteringPass.SetSource(renderer.cameraColorTarget);
		m_InscatteringPass.SetIntensity(intensity);
		renderer.EnqueuePass(m_InscatteringPass);
	}
}


