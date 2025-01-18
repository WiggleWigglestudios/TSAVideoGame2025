using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw default inspector fields

        Level level = (Level)target; // Get reference to the target script

        // Add a button in the Inspector
        if (GUILayout.Button("Call Example Function"))
        {
            level.recreateMesh(); // Call the function
        }
    }
}
