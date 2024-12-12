using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class TCP2_Demo_PipelineMaterial : MonoBehaviour
{
    public Material material;
    public Shader birpShader;
    public Shader urpShader;

    void OnEnable()
    {
        if (material == null || birpShader == null || urpShader == null)
            return;

        RenderPipelineAsset srp = GraphicsSettings.currentRenderPipeline;
        bool isURP = srp != null && srp.GetType().ToString().Contains("Universal");
        material.shader = isURP ? urpShader : birpShader;
    }
}
