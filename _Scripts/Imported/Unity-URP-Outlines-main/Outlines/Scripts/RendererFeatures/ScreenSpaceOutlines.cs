//using UnityEngine;
//using UnityEngine.Rendering.Universal;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.RenderGraphModule;
//using UnityEngine.Rendering.RenderGraphModule.Util;

//public class ScreenSpaceOutlines : ScriptableRendererFeature {

//    [System.Serializable]
//    public class ScreenSpaceOutlineSettings
//    {
//        [Header("General Outline Settings")]
//        public Color outlineColor = Color.black;
//        [Range(0.0f, 20.0f)]
//        public float outlineScale = 1.0f;

//        [Header("Depth Settings")]
//        [Range(0.0f, 100.0f)]
//        public float depthThreshold = 1.5f;
//        [Range(0.0f, 500.0f)]
//        public float robertsCrossMultiplier = 100.0f;

//        [Header("Normal Settings")]
//        [Range(0.0f, 1.0f)]
//        public float normalThreshold = 0.4f;

//        [Header("Depth Normal Relation Settings")]
//        [Range(0.0f, 2.0f)]
//        public float steepAngleThreshold = 0.2f;
//        [Range(0.0f, 500.0f)]
//        public float steepAngleMultiplier = 25.0f;
//    }

//    public class ScreenSpaceOutlinePass : ScriptableRenderPass
//    {
//        private readonly Material outlineMaterial;
//        private readonly LayerMask layerMask;
//        private readonly ScreenSpaceOutlineSettings settings;

//        public ScreenSpaceOutlinePass(Material material, LayerMask layerMask, ScreenSpaceOutlineSettings settings)
//        {
//            this.outlineMaterial = material;
//            this.layerMask = layerMask;
//            this.settings = settings;
//        }

//        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
//        {
//            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);
//            var resourceData = frameData.Get<UniversalResourceData>();
//            var renderingData = frameData.Get<UniversalRenderingData>();
//            var cameraColor = resourceData.activeColorTexture;
//            var cullResults = renderingData.cullResults;

//            // Configure shader properties
//            outlineMaterial.SetColor("_OutlineColor", settings.outlineColor);
//            outlineMaterial.SetFloat("_OutlineScale", settings.outlineScale);
//            outlineMaterial.SetFloat("_DepthThreshold", settings.depthThreshold);
//            outlineMaterial.SetFloat("_RobertsCrossMultiplier", settings.robertsCrossMultiplier);
//            outlineMaterial.SetFloat("_NormalThreshold", settings.normalThreshold);
//            outlineMaterial.SetFloat("_SteepAngleThreshold", settings.steepAngleThreshold);
//            outlineMaterial.SetFloat("_SteepAngleMultiplier", settings.steepAngleMultiplier);

//            // Create a temporary texture for the outline effect
//            var descriptor = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
//            var outlineTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, descriptor, "_OutlineTexture", false);

//            var drawingSettings = new DrawingSettings(shaderTagId, new SortingSettings(frameData.))
//            {
//                overrideMaterial = outlineMaterial
//            };

//            var rendererListParams = new RendererListParams(cullResults, drawingSettings, filteringSettings);
//            var rendererListHandle = renderGraph.CreateRendererList(rendererListParams);

//            // First pass: render outlines to the temporary texture
//            var blitParams1 = new RenderGraphUtils.BlitMaterialParameters(cameraColor, outlineTexture, outlineMaterial, 0);
//            renderGraph.AddBlitPass(blitParams1, "OutlinePass");

//            // Second pass: blit the result back to the camera color texture
//            var blitParams2 = new RenderGraphUtils.BlitMaterialParameters(outlineTexture, cameraColor, outlineMaterial, 1);
//            renderGraph.AddBlitPass(blitParams2, "CompositeOutline");
//        }
//    }

//    [SerializeField] private RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingSkybox;
//    [SerializeField] private LayerMask outlinesLayerMask;
//    [SerializeField] private ScreenSpaceOutlineSettings settings;

//    private Material outlineMaterial;
//    private ScreenSpaceOutlinePass outlinePass;

//    public override void Create()
//    {
//        outlineMaterial = new Material(Shader.Find("Hidden/Outlines"));
//        outlinePass = new ScreenSpaceOutlinePass(outlineMaterial, outlinesLayerMask, settings)
//        {
//            renderPassEvent = this.renderPassEvent,
//        };
//    }

//    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
//    {
//        if (outlinePass != null)
//        {
//            renderer.EnqueuePass(outlinePass);
//        }
//    }

//    protected override void Dispose(bool disposing)
//    {
//        if (disposing)
//        {
//            if (Application.isPlaying)
//                Destroy(outlineMaterial);
//            else
//                DestroyImmediate(outlineMaterial);
//        }
//    }
//}
