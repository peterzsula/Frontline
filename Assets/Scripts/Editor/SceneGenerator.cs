using UnityEngine;
using UnityEditor;
using Frontline.GameManagement;

namespace Frontline.EditorTools
{
    /// <summary>
    /// AI-driven tool to generate a complete, basic playable scene for the Frontline game.
    /// It automatically clears previously generated objects before creating a new scene setup,
    /// making it a one-click solution.
    /// </summary>
    public class SceneGenerator
    {
        private const string PrefabFolderPath = "Assets/Prefabs";

        [MenuItem("AI Tools/Setup Basic Scene")]
        public static void SetupBasicScene()
        {
            // --- 0. Automatically Clean the Scene ---
            ClearGeneratedScene();

            // --- 1. Create the Ground Plane (The "Map") ---
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(20, 1, 20); // 200m x 200m map
            Material groundMat = new Material(Shader.Find("Standard"));
            groundMat.color = new Color(0.3f, 0.4f, 0.2f); // Grassy green
            ground.GetComponent<Renderer>().material = groundMat;

            // --- 2. Create Spawn Points ---
            GameObject spawnPointRoot = new GameObject("SpawnPoints");
            Transform[] teamASpawns = new Transform[5];
            Transform[] teamBSpawns = new Transform[5];

            for (int i = 0; i < 5; i++)
            {
                GameObject spawnA = new GameObject($"TeamA_Spawn_{i}");
                spawnA.transform.position = new Vector3(-80 + (i * 10), 0, 0);
                spawnA.transform.SetParent(spawnPointRoot.transform);
                teamASpawns[i] = spawnA.transform;

                GameObject spawnB = new GameObject($"TeamB_Spawn_{i}");
                spawnB.transform.position = new Vector3(80 - (i * 10), 0, 0);
                spawnB.transform.rotation = Quaternion.Euler(0, 180, 0);
                spawnB.transform.SetParent(spawnPointRoot.transform);
                teamBSpawns[i] = spawnB.transform;
            }
            Debug.Log("AI generated spawn points for both teams.");

            // --- 3. Create and Configure the GameManager ---
            GameObject gmObject = new GameObject("GameManager");
            GameManager gameManager = gmObject.AddComponent<GameManager>();

            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabFolderPath}/PlayerTank.prefab");
            GameObject aiPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabFolderPath}/AITank.prefab");

            if (playerPrefab == null || aiPrefab == null)
            {
                EditorUtility.DisplayDialog("Error",
                    $"Could not find 'PlayerTank.prefab' or 'AITank.prefab' in the '{PrefabFolderPath}' folder.\n\n" +
                    "Please make sure your prefabs are named correctly and placed in that folder before running this tool.",
                    "OK");
                Object.DestroyImmediate(ground);
                Object.DestroyImmediate(spawnPointRoot);
                Object.DestroyImmediate(gmObject);
                return;
            }

            SerializedObject so = new SerializedObject(gameManager);
            so.FindProperty("playerTankPrefab").objectReferenceValue = playerPrefab;
            so.FindProperty("aiTankPrefab").objectReferenceValue = aiPrefab;

            var teamAProp = so.FindProperty("teamASpawnPoints");
            teamAProp.arraySize = teamASpawns.Length;
            for (int i = 0; i < teamASpawns.Length; i++)
            {
                teamAProp.GetArrayElementAtIndex(i).objectReferenceValue = teamASpawns[i];
            }

            var teamBProp = so.FindProperty("teamBSpawnPoints");
            teamBProp.arraySize = teamBSpawns.Length;
            for (int i = 0; i < teamBSpawns.Length; i++)
            {
                teamBProp.GetArrayElementAtIndex(i).objectReferenceValue = teamBSpawns[i];
            }

            so.ApplyModifiedProperties();
            Debug.Log("AI configured GameManager and assigned all prefabs and spawn points.");

            // --- 4. Create and Configure the Camera (Robust Version) ---
            GameObject camGO = new GameObject("MainCamera");
            Camera mainCamera = camGO.AddComponent<Camera>();
            camGO.tag = "MainCamera";

            mainCamera.transform.position = new Vector3(0, 20, -30);
            mainCamera.transform.rotation = Quaternion.Euler(30, 0, 0);
            mainCamera.gameObject.AddComponent<CameraController>();
            Debug.Log("AI has created and configured the main camera.");

            EditorUtility.DisplayDialog("AI Scene Setup Complete",
                "Successfully cleared the old setup and generated a new, complete test scene.\n\nYou can now press Play!",
                "Perfect!");
        }

        /// <summary>
        /// Finds and removes all objects that were created by the scene generator.
        /// </summary>
        private static void ClearGeneratedScene()
        {
            string[] objectNamesToDelete = { "Ground", "SpawnPoints", "GameManager", "MainCamera" };
            foreach (var objName in objectNamesToDelete)
            {
                GameObject obj = GameObject.Find(objName);
                if (obj != null)
                {
                    Object.DestroyImmediate(obj);
                }
            }
            Debug.Log("AI automatically cleared previously generated scene objects.");
        }
    }
}