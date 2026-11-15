using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEditor.Tilemaps;

public static class MG6Build
{
    private static Material _material;
    private const string Generated = "Assets/Pantry";

    public static void Prepare()
    {
        Directory.CreateDirectory(Generated);
        Directory.CreateDirectory(Generated + "/Tiles");
        AssetDatabase.Refresh();
        EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
        foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            if (root.name == "Pantry Environment" || root.name == "Pantry HUD") UnityEngine.Object.DestroyImmediate(root);

        _material = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"));
        string materialPath = Generated + "/PantrySprite.mat";
        if (AssetDatabase.LoadAssetAtPath<Material>(materialPath) == null)
            AssetDatabase.CreateAsset(_material, materialPath);
        else
        {
            UnityEngine.Object.DestroyImmediate(_material);
            _material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        }

        var gridRoot = new GameObject("Pantry Environment", typeof(Grid));
        var wallpaper = MakeMap(gridRoot.transform, "Wallpaper", "Untagged", -10, false);
        var ground = MakeMap(gridRoot.transform, "Ground", "Ground", 0, true);
        var platforms = MakeMap(gridRoot.transform, "Platforms", "Platform", 1, true);
        var details = MakeMap(gridRoot.transform, "Shelf trim", "Untagged", 2, false);
        Tile wall = MakeTile("Wall_2_48x48", 4, "Wallpaper");
        Tile wood = MakeTile("Floor_1_48x48", 12, "Warm floorboards");
        Tile shelf = MakeTile("Floor_1_48x48", 27, "Shelf boards");
        Tile trim = MakeTile("Wall_2_48x48", 0, "Shelf edge");
        wallpaper.color = new Color(0.25f, 0.30f, 0.42f);
        for (int x = -17; x <= 16; x++)
            for (int y = -8; y <= 10; y++) wallpaper.SetTile(new Vector3Int(x, y, 0), wall);
        for (int x = -17; x <= 16; x++)
        {
            ground.SetTile(new Vector3Int(x, -7, 0), wood);
            ground.SetTile(new Vector3Int(x, -8, 0), wood);
        }
        Shelf(platforms, details, shelf, trim, -14, -9, -4);
        Shelf(platforms, details, shelf, trim, -10, -5, -1);
        Shelf(platforms, details, shelf, trim, -5, 1, 2);
        Shelf(platforms, details, shelf, trim, 2, 7, 5);
        Shelf(platforms, details, shelf, trim, 5, 10, -1);
        Shelf(platforms, details, shelf, trim, 10, 14, 2);
        Shelf(platforms, details, shelf, trim, 10, 14, -4);
        for (int y = -6; y < 9; y++)
        {
            ground.SetTile(new Vector3Int(-17, y, 0), wood);
            ground.SetTile(new Vector3Int(16, y, 0), wood);
        }

        Decor(gridRoot.transform, "Pantry clock", 16, -0.5f, 6.3f, 1.8f);
        Decor(gridRoot.transform, "Books left", 19, -8.0f, 0.9f, 1.7f);
        Decor(gridRoot.transform, "Books right", 19, 12.0f, 4.0f, 1.7f);
        Decor(gridRoot.transform, "Houseplant", 22, -13.0f, -2.0f, 1.7f);
        Decor(gridRoot.transform, "Houseplant right", 23, 8.6f, 0.7f, 1.5f);
        Decor(gridRoot.transform, "Lamp", 17, 5.0f, 6.8f, 1.5f);
        Decor(gridRoot.transform, "Pantry cupboard", 10, -6.5f, -4.8f, 2.0f);
        Decor(gridRoot.transform, "Pantry cupboard right", 10, 12.0f, -2.3f, 1.4f);
        MakePalette(new[] { wall, wood, shelf, trim, MakeTile("Items_1_48x48", 16, "Clock"), MakeTile("Items_1_48x48", 19, "Books") });

        GameObject cat = GameObject.Find("Cat");
        cat.tag = "Untagged";
        cat.transform.position = new Vector3(-12, -5.1f, 0);
        var catBody = cat.GetComponent<Rigidbody2D>();
        catBody.gravityScale = 1.7f;
        catBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        catBody.interpolation = RigidbodyInterpolation2D.Interpolate;
        catBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        var catData = new SerializedObject(cat.GetComponent<Cat>());
        catData.FindProperty("_jump").floatValue = 10.5f;
        catData.FindProperty("_speed").floatValue = 6.0f;
        catData.FindProperty("_attackCooldown").floatValue = 0.65f;
        catData.ApplyModifiedPropertiesWithoutUndo();
        GameObject rat = GameObject.Find("Rat_Mother");
        rat.transform.position = new Vector3(2.2f, -4.8f, 0);
        var ratData = new SerializedObject(rat.GetComponent<Rat>());
        ratData.FindProperty("_spawnAmount").intValue = 5;
        ratData.ApplyModifiedPropertiesWithoutUndo();
        var ratBody = rat.GetComponent<Rigidbody2D>();
        ratBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        ratBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        foreach (var sr in UnityEngine.Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None)) sr.sharedMaterial = _material;
        var baby = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/RatBaby.prefab");
        var babyRoot = PrefabUtility.LoadPrefabContents(AssetDatabase.GetAssetPath(baby));
        babyRoot.GetComponent<SpriteRenderer>().sharedMaterial = _material;
        PrefabUtility.SaveAsPrefabAsset(babyRoot, AssetDatabase.GetAssetPath(baby));
        PrefabUtility.UnloadPrefabContents(babyRoot);
        var explosionRoot = PrefabUtility.LoadPrefabContents("Assets/Prefabs/Explosion.prefab");
        explosionRoot.GetComponent<SpriteRenderer>().sharedMaterial = _material;
        PrefabUtility.SaveAsPrefabAsset(explosionRoot, "Assets/Prefabs/Explosion.prefab");
        PrefabUtility.UnloadPrefabContents(explosionRoot);

        var camera = Camera.main;
        camera.transform.SetParent(null, true);
        camera.transform.localScale = Vector3.one;
        camera.transform.position = new Vector3(0, 1, -10);
        camera.orthographicSize = 9.6f;
        camera.backgroundColor = new Color(0.035f, 0.05f, 0.085f);
        var hud = new GameObject("Pantry HUD").AddComponent<PantryGame>();
        hud.cat = cat.transform;
        hud.mother = rat.GetComponent<Rat>();
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/Main.unity", true) };
        PlayerSettings.companyName = "Qiliang Wan";
        PlayerSettings.productName = "Midnight Pantry";
        PlayerSettings.defaultWebScreenWidth = 960;
        PlayerSettings.defaultWebScreenHeight = 540;
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
        PlayerSettings.WebGL.decompressionFallback = false;
        PlayerSettings.WebGL.template = "APPLICATION:Default";
        PlayerSettings.runInBackground = true;
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        Debug.Log("MG6_PREPARE_COMPLETE");
    }

    private static Tilemap MakeMap(Transform parent, string name, string tag, int sorting, bool collide)
    {
        var go = new GameObject(name, typeof(Tilemap), typeof(TilemapRenderer));
        go.transform.SetParent(parent);
        go.tag = tag;
        go.GetComponent<TilemapRenderer>().sortingOrder = sorting;
        go.GetComponent<TilemapRenderer>().sharedMaterial = _material;
        if (collide) go.AddComponent<TilemapCollider2D>();
        return go.GetComponent<Tilemap>();
    }

    private static Tile MakeTile(string sheet, int index, string title)
    {
        string path = Generated + "/Tiles/" + title + ".asset";
        var tile = AssetDatabase.LoadAssetAtPath<Tile>(path);
        if (tile == null)
        {
            tile = ScriptableObject.CreateInstance<Tile>();
            AssetDatabase.CreateAsset(tile, path);
        }
        tile.sprite = Sprite(sheet, index);
        tile.colliderType = Tile.ColliderType.Grid;
        EditorUtility.SetDirty(tile);
        return tile;
    }

    private static Sprite Sprite(string sheet, int index)
    {
        return AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/environment/" + sheet + ".png")
            .OfType<Sprite>().First(x => x.name == sheet + "_" + index);
    }

    private static void Shelf(Tilemap platforms, Tilemap details, Tile wood, Tile edge, int start, int end, int y)
    {
        for (int x = start; x <= end; x++)
        {
            platforms.SetTile(new Vector3Int(x, y, 0), wood);
            details.SetTile(new Vector3Int(x, y, 0), edge);
        }
    }

    private static void Decor(Transform parent, string name, int index, float x, float y, float size)
    {
        var go = new GameObject(name, typeof(SpriteRenderer));
        go.transform.SetParent(parent);
        go.transform.position = new Vector3(x, y, 0);
        go.transform.localScale = Vector3.one * size;
        var sr = go.GetComponent<SpriteRenderer>();
        sr.sprite = Sprite("Items_1_48x48", index);
        sr.sharedMaterial = _material;
        sr.sortingOrder = -2;
        sr.color = new Color(0.8f, 0.83f, 0.9f);
    }

    private static void MakePalette(Tile[] tiles)
    {
        string path = Generated + "/Pantry Palette.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
        {
            if (!AssetDatabase.LoadAllAssetsAtPath(path).OfType<GridPalette>().Any())
            {
                var palette = ScriptableObject.CreateInstance<GridPalette>();
                palette.name = "Palette Settings";
                palette.cellSizing = GridPalette.CellSizing.Automatic;
                AssetDatabase.AddObjectToAsset(palette, path);
            }
            return;
        }
        var go = new GameObject("Pantry Palette", typeof(Grid));
        var map = MakeMap(go.transform, "Tiles", "Untagged", 0, false);
        for (int i = 0; i < tiles.Length; i++) map.SetTile(new Vector3Int(i, 0, 0), tiles[i]);
        PrefabUtility.SaveAsPrefabAsset(go, path);
        UnityEngine.Object.DestroyImmediate(go);
        var settings = ScriptableObject.CreateInstance<GridPalette>();
        settings.name = "Palette Settings";
        settings.cellSizing = GridPalette.CellSizing.Automatic;
        AssetDatabase.AddObjectToAsset(settings, path);
    }

    public static void BuildWeb()
    {
        string output = Environment.GetEnvironmentVariable("MG6_WEB_OUTPUT");
        if (string.IsNullOrEmpty(output)) output = "Builds/Web";
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/Main.unity" },
            locationPathName = output,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        });
        Debug.Log("MG6_BUILD_RESULT=" + report.summary.result);
        if (report.summary.result != BuildResult.Succeeded) EditorApplication.Exit(1);
    }
}
