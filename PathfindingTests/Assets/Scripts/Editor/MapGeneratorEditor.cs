namespace Editor
{
    using UnityEditor;
    using UnityEngine;

// Define the custom editor for the MapGenerator class
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default inspector options
            DrawDefaultInspector();

            // Reference to the MapGenerator script
            MapGenerator mapGenerator = (MapGenerator)target;

            // Create a button in the inspector
            if (GUILayout.Button("Generate Map"))
            {
                // Call the GenerateMap function when the button is clicked
                mapGenerator.GenerateMap();
            }
        }
    }

}