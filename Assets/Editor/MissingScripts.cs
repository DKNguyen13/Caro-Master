using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MissingScripts : EditorWindow
{
    #region Enum
    private enum ScanMode { Scene, Prefabs }
    #endregion

    private ScanMode scanMode = ScanMode.Scene;

    private List<GameObject> listMissing = new();
    private Dictionary<GameObject, int> missingCount = new();
    private Dictionary<GameObject, bool> selectedObjects = new();

    private Vector2 scrollPos;
    private string[] prefabGuids;
    private int scanIndex;
    private bool isScanning;

    [MenuItem("Tools/Missing Scripts")]
    public static void ShowWindow()
    {
        GetWindow<MissingScripts>("Missing scripts");
    }

    private void OnGUI()
    {
        GUILayout.Label("Missing Scripts", EditorStyles.boldLabel);

        scanMode = (ScanMode)EditorGUILayout.EnumPopup("Scane Mode:", scanMode);

        if (!isScanning && GUILayout.Button("Scan"))
        {
            if (scanMode == ScanMode.Scene)
            {
                ScanScene();
            }
            else if (scanMode == ScanMode.Prefabs)
            {
                ScanPrefabs();
            }
        }

        if (isScanning)
        {
            GUILayout.Label($"Scanning... {scanIndex}/{prefabGuids.Length}", EditorStyles.helpBox);
            if (GUILayout.Button("Cancel"))
            {
                CancelScan();
            }
        }

        GUILayout.Space(10);

        if (listMissing.Count > 0 && !isScanning)
        {
            GUILayout.Label($"Found {listMissing.Count} GameObjects/Prefabs with missing scripts", EditorStyles.helpBox);

            scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Height(350));
            List<GameObject> toRemove = new List<GameObject>();

            foreach (var go in listMissing)
            {
                GUILayout.BeginHorizontal();

                bool isSelected = selectedObjects.ContainsKey(go) && selectedObjects[go];
                selectedObjects[go] = GUILayout.Toggle(isSelected, "", GUILayout.Width(20));

                GUILayout.Label(go.name, GUILayout.Width(200));
                GUILayout.Label($"Missing: {missingCount[go]}", GUILayout.Width(100));

                if (GUILayout.Button("Ping", GUILayout.Width(50)))
                {
                    EditorGUIUtility.PingObject(go);
                    Selection.activeObject = go;
                }

                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    toRemove.Add(go);
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            GUILayout.Space(10);
            if (GUILayout.Button("Remove Selected"))
            {
                foreach (var go in new List<GameObject>(selectedObjects.Keys))
                {
                    if (selectedObjects[go])
                    {
                        RemoveMissing(go);
                    }
                }
            }

            if (GUILayout.Button("Remove All"))
            {
                foreach (var go in new List<GameObject>(listMissing))
                {
                    RemoveMissing(go);
                }
            }

            foreach (var go in toRemove)
            {
                RemoveMissing(go);
            }
        }
        else if (!isScanning)
        {
            GUILayout.Label("No missing scripts found.", EditorStyles.helpBox);
        }
    }

    private void ScanScene()
    {
        listMissing.Clear();
        missingCount.Clear();
        selectedObjects.Clear();

        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
        {
            int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
            if (count > 0)
            {
                listMissing.Add(go);
                missingCount[go] = count;
                selectedObjects[go] = false;
            }
        }

        Debug.Log($"[CleanerPro] Found {listMissing.Count} objects with missing scripts in scene.");
    }

    private void ScanPrefabs()
    {
        listMissing.Clear();
        missingCount.Clear();
        selectedObjects.Clear();

        prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        scanIndex = 0;
        isScanning = true;

        EditorApplication.update += ContinueProjectScan;
    }

    private void ContinueProjectScan()
    {
        int batchSize = 20;
        for (int i = 0; i < batchSize && scanIndex < prefabGuids.Length; i++, scanIndex++)
        {
            string guid = prefabGuids[scanIndex];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null)
            {
                int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(prefab);
                if (count > 0)
                {
                    listMissing.Add(prefab);
                    missingCount[prefab] = count;
                    selectedObjects[prefab] = false;
                }
            }
        }

        float progress = (float)scanIndex / prefabGuids.Length;
        EditorUtility.DisplayProgressBar("Scanning Prefabs", $"{scanIndex}/{prefabGuids.Length}", progress);

        if (scanIndex >= prefabGuids.Length)
            FinishScan();
    }

    private void FinishScan()
    {
        isScanning = false;
        EditorApplication.update -= ContinueProjectScan;
        EditorUtility.ClearProgressBar();

        Debug.Log($"[CleanerPro] Found {listMissing.Count} prefabs with missing scripts in project.");
        Repaint();
    }

    private void CancelScan()
    {
        isScanning = false;
        EditorApplication.update -= ContinueProjectScan;
        EditorUtility.ClearProgressBar();
        Debug.Log("[CleanerPro] Scan cancelled.");
        Repaint();
    }

    private void RemoveMissing(GameObject go)
    {
        Undo.RegisterFullObjectHierarchyUndo(go, "Remove Missing Scripts");
        int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
        if (removed > 0)
        {
            Debug.Log($"Removed {removed} missing scripts from {go.name}");
            EditorUtility.SetDirty(go);

            listMissing.Remove(go);
            missingCount.Remove(go);
            selectedObjects.Remove(go);
        }
    }
}