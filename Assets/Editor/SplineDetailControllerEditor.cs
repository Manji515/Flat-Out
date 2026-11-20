#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineDetailer))]
public class SplineDetailControllerEditor : Editor
{
    void OnSceneGUI()
    {
        var controller = (SplineDetailer)target;
        if (controller.mirrorTangents && controller.spriteShape != null)
        {
            controller.ApplyMirrorTangents();
        }
    }
}
#endif
