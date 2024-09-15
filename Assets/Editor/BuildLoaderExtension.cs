using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildLoader)), CanEditMultipleObjects]
public class BuildLoaderExtension : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        BuildLoader buildLoader = (BuildLoader)target;
        
        if(GUILayout.Button("Download to Build"))
        {
            
            buildLoader.DownloadFiles();
        }
    }
}
