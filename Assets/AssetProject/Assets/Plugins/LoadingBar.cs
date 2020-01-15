using QModManager.Utility;
using UnityEngine;

[ExecuteInEditMode]
public class LoadingBar : MonoBehaviour
{
    public float start = 398.15f;
    public float end = -298.15f;
    public float amount = 0.5f;

    void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        ((RectTransform)transform).SetEdge(RectTransform.Edge.Right, start + amount * (-start + end));
    }
}
