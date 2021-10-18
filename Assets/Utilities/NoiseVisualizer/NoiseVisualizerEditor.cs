using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NoiseVisualizer))]
public class NoiseVisualizerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NoiseVisualizer noiseVisualizer = (NoiseVisualizer) target;

        if (GUILayout.Button("Display Noise"))
        {
            noiseVisualizer.DisplayNoise();
        }
    }
}
