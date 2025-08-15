using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class HitboxEditorWindow : EditorWindow {
	private int frame = 0;
	private int animationFrame = 0;
	private bool showHitboxes = true;
	private MoveData move;
	[MenuItem("Fighting Game/Hitbox Editor")]
    public static void StartWindow() {
		HitboxEditorWindow window = GetWindow<HitboxEditorWindow>();
		window.titleContent = new GUIContent("Hitbox Editor");
	}
	public void OnGUI() {
		move = (MoveData)EditorGUILayout.ObjectField("Hitbox Data", move, typeof(MoveData), false);
		if (move == null) return;
		showHitboxes = EditorGUILayout.Toggle("Show Hitboxes", showHitboxes);
		int lastFrame = frame;
		frame = EditorGUILayout.IntSlider("Frame", frame, 1, move.TotalFrames());
		if (lastFrame != frame) SceneView.RepaintAll();

		EditorGUILayout.BeginHorizontal();
		SerializedObject moveSO = new SerializedObject(move);
		SerializedProperty frames = moveSO.FindProperty("frameData");
		if (GUILayout.Button(new GUIContent("Add Frame"))) {
			frames.InsertArrayElementAtIndex(frame);
			frame += 1;
		}
		if (GUILayout.Button(new GUIContent("Remove Frame"))) {
			frames.DeleteArrayElementAtIndex(frame - 1);
			frame = Mathf.Max(frame - 1, 1);
		}
		EditorGUILayout.EndHorizontal();
		if (GUILayout.Button(new GUIContent("Add Collider"))) {
			SerializedProperty hitboxes = frames.GetArrayElementAtIndex(frame - 1).FindPropertyRelative("hitboxes");
			hitboxes.InsertArrayElementAtIndex(hitboxes.arraySize);
			hitboxes.GetArrayElementAtIndex(hitboxes.arraySize - 1).rectValue = new Rect(0f, 0f, 1f, 1f);
			SceneView.RepaintAll();
		}
		moveSO.ApplyModifiedProperties();
	}
	private void OnEnable() {
		SceneView.duringSceneGui += OnSceneGUI;
	}
	private void OnDisable() {
		SceneView.duringSceneGui -= OnSceneGUI;
	}
	private void OnSceneGUI(SceneView view) {
		if (!showHitboxes) return;
		if (move == null) return;
		SerializedObject moveSO = new SerializedObject(move);
		SerializedProperty hitboxesPerFrame = moveSO.FindProperty("frameData");
		if (hitboxesPerFrame.arraySize == 0) return;
		SerializedProperty hitboxes = hitboxesPerFrame.GetArrayElementAtIndex(frame - 1).FindPropertyRelative("hitboxes");
		if (hitboxes == null) return;
		for (int i = 0; i < hitboxes.arraySize; i++) {
			SerializedProperty hitboxSerialized = hitboxes.GetArrayElementAtIndex(i);
			Rect hitbox = hitboxSerialized.rectValue;
			Vector2[] corners = GetCorners(hitbox);
			for (int j = 0; j < corners.Length; j++) {
				Vector2 newCorner = Handles.FreeMoveHandle(corners[j], 0.05f, Vector3.zero, Handles.RectangleHandleCap);
				if (corners[j] != newCorner) {
					var oppositeCorner = corners[(j + 2) % 4];
					hitbox.size = (newCorner - oppositeCorner).Abs();
					hitbox.center = (newCorner + oppositeCorner) / 2f;
				}
			}
			hitbox.center = Handles.FreeMoveHandle(hitbox.center, 0.05f, Vector3.zero, Handles.RectangleHandleCap);
			Handles.DrawSolidRectangleWithOutline(hitbox, new Color(0f, 1f, 0f, 0.2f), Color.green);
			hitboxSerialized.rectValue = hitbox;
		}
		moveSO.ApplyModifiedProperties();
	}
	private Vector2[] GetCorners(Rect rect) {
		Vector2[] corners = new Vector2[4];
		Vector2 halfSize = rect.size / 2f;
		corners[0] = rect.center - halfSize;
		corners[1] = new Vector2(rect.center.x + halfSize.x, rect.center.y - halfSize.y);
		corners[2] = rect.center + halfSize;
		corners[3] = new Vector2(rect.center.x - halfSize.x, rect.center.y + halfSize.y);
		return corners;
	}
}
