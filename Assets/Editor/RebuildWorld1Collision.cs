using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
public static class RebuildWorld1Collision
{
    [MenuItem("Tools/Repair World1 Collision")]
    public static void Run()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/World1.unity");
        var ground = GameObject.Find("Ground");
        var map = ground.GetComponent<Tilemap>();
        var collider = ground.GetComponent<TilemapCollider2D>();
        var composite = ground.GetComponent<CompositeCollider2D>();
        // Refresh tile data through Unity's API to invalidate serialized physics caches.
        var bounds = map.cellBounds;
        var tiles = map.GetTilesBlock(bounds);
        map.ClearAllTiles();
        collider.ProcessTilemapChanges();
        map.SetTilesBlock(bounds, tiles);
        map.RefreshAllTiles();
        collider.ProcessTilemapChanges();
        composite.GenerateGeometry();
        Physics2D.SyncTransforms();
        foreach (float x in new[] { 8.5f, 20.5f, 42.5f, 46.5f, 63.5f, 72.5f, 84.5f, 91.5f, 118.5f })
        {
            var hit = Physics2D.Raycast(new Vector2(x, 12), Vector2.down, 20, 1 << ground.layer);
            if (hit.collider == null) throw new Exception("Ground collision missing at x=" + x);
            Debug.Log("WORLD1_COLLISION_OK x=" + x + " y=" + hit.point.y);
        }
        foreach (float x in new[] { 18.5f, 44.5f, 70.5f, 86.5f, 90.5f })
        {
            if (Physics2D.OverlapPoint(new Vector2(x, -3.5f), 1 << ground.layer) != null)
                throw new Exception("Pit blocked at x=" + x);
        }
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("WORLD1_COLLISION_SAVED paths=" + composite.pathCount);
    }
}
