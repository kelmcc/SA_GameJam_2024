
using JetBrains.Annotations;
using System.IO;
using UnityEditor;
using UnityEngine;

// SRC https://forum.unity.com/threads/folder-description.770660/
// https://gist.github.com/tomkail/1cb4e700e328675f2c54ea56e75b2cd0

[UsedImplicitly]
public class FolderAssetEditor : DefaultAssetInspector
{
	private const string _descriptionFilename = ".notes";
	
	private string _description;

	private string _path;
	
	public override bool IsValid(string assetPath)
	{
		return !Path.HasExtension(assetPath);
	}

	private void Init()
	{
		var path = AssetDatabase.GetAssetPath(target);
 
		if (Directory.Exists(path))
		{
			var descriptionPath = Path.Combine(path, _descriptionFilename);
			try
			{
				_description = File.ReadAllText(descriptionPath);
			}
			catch (System.Exception ea) { Error(ea); } // Print to debug error message only if there was an advanced error
		}
	}

	public override void OnInspectorGUI () {
		//	 Call to redraw every frame
		//editor.Repaint();
		//serializedObject.Update();

		if (string.IsNullOrEmpty(_path))
		{
			Init();
		}
		
		GUI.enabled = true;
		EditorGUI.BeginChangeCheck();
		
		GUILayout.Label("Add notes below:", EditorStyles.boldLabel);
		EditorGUILayout.Space(8);
 
		GUIStyle style = new (EditorStyles.textArea)
		{
			wordWrap = true
		};

		_description = EditorGUILayout.TextArea(_description, style,GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true)); // Reverted styling to default
 
		EditorGUILayout.Space(16);
 
		if (EditorGUI.EndChangeCheck())
		{
			// we get the asset path again instead of caching it in case the folder has moved since OnEnable was called
			var descriptionPath = Path.Combine(AssetDatabase.GetAssetPath(target), _descriptionFilename);
 
			if (!string.IsNullOrEmpty(_description))
			{
				try
				{
					File.SetAttributes(descriptionPath, FileAttributes.Normal);
				}
				catch (System.Exception ea) { Error(ea); } // Print to debug error message only if there was an advanced error
 
				File.WriteAllText(descriptionPath, _description);
 
				try
				{
					File.SetAttributes(descriptionPath, FileAttributes.Normal);
				}
				catch (System.Exception ea) { Error(ea); } // Print to debug error message only if there was an advanced error
			}
			else
			{
				try
				{
					File.Delete(descriptionPath);
				}
				catch (System.Exception ea) { Error(ea); } // Print to debug error message only if there was an advanced error
			}
		}
		//serializedObject.ApplyModifiedProperties();
	}
	
	void Error(System.Exception ea)
	{
		if (ea is System.IO.FileNotFoundException)
		{
			return;
		}
		Debug.LogError(ea.ToString());
	}
}