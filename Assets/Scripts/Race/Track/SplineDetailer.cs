using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]
public class SplineDetailer : MonoBehaviour
{
    [Range(2, 64)] public int splineDetail = 16;
    public bool mirrorTangents = true;

    [HideInInspector] public SpriteShapeController spriteShape;

    void Awake()
    {
        spriteShape = GetComponent<SpriteShapeController>();
        ApplySettings();
    }

    void OnValidate()
    {
        if (spriteShape == null) spriteShape = GetComponent<SpriteShapeController>();
        ApplySettings();
    }

    public void ApplySettings()
    {
        if (spriteShape == null) return;

        spriteShape.splineDetail = splineDetail;
        if (mirrorTangents) ApplyMirrorTangents();
    }

    public void ApplyMirrorTangents()
    {
        var spline = spriteShape.spline;
        for (int i = 0; i < spline.GetPointCount(); i++)
        {
            Vector3 left = spline.GetLeftTangent(i);
            Vector3 right = spline.GetRightTangent(i);

            float maxLen = Mathf.Max(left.magnitude, right.magnitude);
            Vector3 dir = right.normalized;
            if (dir == Vector3.zero) dir = Vector3.right;

            spline.SetLeftTangent(i, -dir * maxLen);
            spline.SetRightTangent(i, dir * maxLen);
            spline.SetTangentMode(i, ShapeTangentMode.Continuous);
        }
        spriteShape.RefreshSpriteShape();
    }
}
