using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
internal class ToolbarFPSSlider : BaseToolbarElement {
	[SerializeField] int minFPS = 1;
	[SerializeField] int maxFPS = 120;
	[SerializeField] int defaultFPS = 60;

    int selectedFramerate;

	public override string NameInList => "[Slider] FPS";

	public override void Init() {
		selectedFramerate = defaultFPS;
	}

	public ToolbarFPSSlider(int minFPS = 1, int maxFPS = 120, int defaultFPS = 60) : base(200) {
		this.minFPS = minFPS;
		this.maxFPS = maxFPS;
		this.defaultFPS = defaultFPS;
    }

	protected override void OnDrawInList(Rect position) {
		position.width = 70.0f;
		EditorGUI.LabelField(position, "Min FPS");

		position.x += position.width + FieldSizeSpace;
		position.width = 50.0f;
		minFPS = Mathf.RoundToInt(EditorGUI.IntField(position, "", minFPS));

		position.x += position.width + FieldSizeSpace;
		position.width = 70.0f;
		EditorGUI.LabelField(position, "Max FPS");

		position.x += position.width + FieldSizeSpace;
		position.width = 50.0f;
		maxFPS = Mathf.RoundToInt(EditorGUI.IntField(position, "", maxFPS));

        position.x += position.width + FieldSizeSpace;
        position.width = 70.0f;
        EditorGUI.LabelField(position, "Default FPS");

        position.x += position.width + FieldSizeSpace;
        position.width = 50.0f;
        defaultFPS = Mathf.RoundToInt(EditorGUI.IntField(position, "", defaultFPS));
    }

	protected override void OnDrawInToolbar() {
		EditorGUILayout.LabelField("FPS", GUILayout.Width(30));
		defaultFPS = EditorGUILayout.IntField(defaultFPS, GUILayout.Width(30f));
		selectedFramerate = EditorGUILayout.IntSlider("", selectedFramerate, minFPS, maxFPS, GUILayout.Width(WidthInToolbar - 30.0f));
		if (EditorApplication.isPlaying && selectedFramerate != Application.targetFrameRate)
			Application.targetFrameRate = selectedFramerate;
	}
}
