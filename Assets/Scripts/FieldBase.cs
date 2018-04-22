using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldBase : MonoBehaviour {

    public enum FieldType
    {
        Buildable,
        NotBuildable,
        Path,
        PathStart,
        PathEnd
    }

    public FieldType fieldType;

    public Vector2 gridLocation;

    public FieldBase nextPath = null;

    public bool hasPath
    {
        get { return nextPath != null; }
    }


#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Color tmp = UnityEditor.Handles.color;
        UnityEditor.Handles.color = Color.blue;
        switch(fieldType)
        {
            case FieldType.Buildable:
                UnityEditor.Handles.Label(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), string.Format("({0},{1}){2}{3}", (int)gridLocation.x, (int)gridLocation.y, System.Environment.NewLine, "Buildable"));
                break;
            case FieldType.NotBuildable:
                UnityEditor.Handles.Label(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), string.Format("({0},{1}){2}{3}", (int)gridLocation.x, (int)gridLocation.y, System.Environment.NewLine, "NotBuildable"));
                break;
            case FieldType.Path:
                UnityEditor.Handles.Label(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), string.Format("({0},{1}){2}{3}", (int)gridLocation.x, (int)gridLocation.y, System.Environment.NewLine, "Path"));
                break;
            case FieldType.PathStart:
                UnityEditor.Handles.Label(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), "PathStart");
                break;
            case FieldType.PathEnd:
                UnityEditor.Handles.Label(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), "PathEnd");
                break;
        }
        UnityEditor.Handles.color = tmp;

    }
#endif

}
