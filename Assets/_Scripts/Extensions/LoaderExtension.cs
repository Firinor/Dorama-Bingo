#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(BuildLoader)), CanEditMultipleObjects]
public class LoaderExtension : Editor
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
#endif
