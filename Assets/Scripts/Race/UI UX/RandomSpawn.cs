using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RandomBackgroundSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] prefabs;
    public PolygonCollider2D spawnArea;
    public int spawnCount = 20;
    public Vector2 randomScaleRange = new Vector2(0.5f, 2f);
    public float minDistanceBetween = 0.5f;

    [Header("Color Settings")]
    public Color[] possibleColors;

    [Header("Collision Filter")]
    public LayerMask blockedLayers;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    // Dictionary untuk menyimpan grup menurut index warna
    private Dictionary<int, Transform> colorGroups = new Dictionary<int, Transform>();


    // ================
    //  GROUPING
    // ================
    Transform GetOrCreateColorGroup(int index)
    {
        if (colorGroups.ContainsKey(index))
            return colorGroups[index];

        GameObject newGroup = new GameObject("Group " + index);
        newGroup.transform.SetParent(transform);

        colorGroups[index] = newGroup.transform;
        return newGroup.transform;
    }


    // ================
    //  SPAWNER
    // ================
    public void SpawnRandomObjects()
    {
        // Pilih prefab acak
if (prefabs == null || prefabs.Length == 0)
{
    Debug.LogWarning("Tidak ada prefab di daftar!");
    return;
}



        ClearSpawnedObjects();

        int spawned = 0;
        int safetyLimit = spawnCount * 50;
        int attempts = 0;

        while (spawned < spawnCount && attempts < safetyLimit)
        {
            attempts++;
            Vector2 spawnPos = GetRandomPointInPolygon(spawnArea);

            // Cek layer yang diblokir
            Collider2D hit = Physics2D.OverlapCircle(spawnPos, 0.1f, blockedLayers);
            if (hit != null)
                continue;

            // Cek jarak antar objek
            bool tooClose = false;
            foreach (GameObject existing in spawnedObjects)
            {
                if (existing == null) continue;
                float dist = Vector2.Distance(spawnPos, existing.transform.position);
                if (dist < minDistanceBetween)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose)
                continue;

            // Spawn
            GameObject chosenPrefab = prefabs[Random.Range(0, prefabs.Length)];
GameObject obj = Instantiate(chosenPrefab, spawnPos, Quaternion.identity);

            // Skala acak
            float randomScale = Random.Range(randomScaleRange.x, randomScaleRange.y);
            obj.transform.localScale = Vector3.one * randomScale;

            // Warna acak
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null && possibleColors.Length > 0)
            {
                int colorIndex = Random.Range(0, possibleColors.Length);
                sr.color = possibleColors[colorIndex];

                // Masukkan berdasarkan index warna
                Transform colorParent = GetOrCreateColorGroup(colorIndex);
                obj.transform.SetParent(colorParent);

            }
            else
            {
                obj.transform.SetParent(transform);
            }

            spawnedObjects.Add(obj);
            spawned++;
        }

        Debug.Log($"Spawned {spawned} objek di area {spawnArea.name}");
    }


    // ================
    //  CLEAR
    // ================
    public void ClearSpawnedObjects()
    {
        foreach (Transform t in colorGroups.Values)
        {
            if (t != null)
                DestroyImmediate(t.gameObject);
        }
        colorGroups.Clear();

        // Hapus grup warna
        foreach (Transform t in colorGroups.Values)
        {
            if (t != null)
                DestroyImmediate(t.gameObject);
        }
        colorGroups.Clear();

        Debug.Log("Semua objek hasil spawn telah dihapus.");
    }


    // ================
    //  RANDOM POINT
    // ================
    Vector2 GetRandomPointInPolygon(PolygonCollider2D col)
    {
        Bounds bounds = col.bounds;
        Vector2 point;
        int safety = 0;

        do
        {
            point = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );
            safety++;
            if (safety > 500) break;

        } while (!col.OverlapPoint(point));

        return point;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RandomBackgroundSpawner))]
public class RandomBackgroundSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RandomBackgroundSpawner spawner = (RandomBackgroundSpawner)target;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("=== Spawn Controls ===", EditorStyles.boldLabel);

        if (GUILayout.Button("Spawn"))
        {
            spawner.SpawnRandomObjects();
        }

        if (GUILayout.Button("Clear Spawned"))
        {
            spawner.ClearSpawnedObjects();
        }
    }
}
#endif
