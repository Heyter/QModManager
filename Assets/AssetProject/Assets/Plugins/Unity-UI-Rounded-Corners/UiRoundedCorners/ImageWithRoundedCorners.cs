using UnityEngine;

[ExecuteInEditMode]
internal class ImageWithRoundedCorners : MonoBehaviour
{
    private static readonly int Props = Shader.PropertyToID("_WidthHeightRadius");

    public Material material = null;
    public float radius = 20;

    void OnRectTransformDimensionsChange()
    {
        Refresh();
    }

    void OnValidate()
    {
        Refresh();
    }

    internal void Refresh()
    {
        if (material == null) return;
        var rect = ((RectTransform)transform).rect;
        material.SetVector(Props, new Vector4(rect.width, rect.height, radius, 0));
    }
}