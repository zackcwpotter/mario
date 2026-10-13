using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Tools > Build Main Menu Layout
// Lays out the Main Menu scene like the SMB title screen: sky, clouds, hill,
// shrub, ground, Mario, the title logo, and the existing buttons/high score.
// Safe to run again: it removes what it created last time before rebuilding.
public static class MainMenuLayoutBuilder
{
    const string MiscSheet = "Assets/Sprites/misc-3.gif";
    const string MarioSheet = "Assets/Sprites/mario.png";
    const string LogoPath = "Assets/Sprites/title_logo.png";

    const string BackgroundRootName = "Menu Background";
    const string LogoName = "Title Logo";

    static readonly Color SkyColor = new Color(0.3607843f, 0.5803922f, 0.9882353f);

    [MenuItem("Tools/Build Main Menu Layout")]
    public static void Build()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != "Main Menu")
        {
            EditorUtility.DisplayDialog("Build Main Menu Layout",
                "Open the 'Main Menu' scene first.", "OK");
            return;
        }

        PrepareLogoImporter();

        Camera cam = Camera.main;
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (cam == null || canvas == null)
        {
            Debug.LogError("Main Menu needs a Main Camera and a Canvas.");
            return;
        }

        BuildBackground(cam);
        BuildUI(canvas);

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Main Menu layout built. Save the scene (Ctrl+S) to keep it.");
    }

    // ---------- world sprites ----------

    static void BuildBackground(Camera cam)
    {
        Undo.RecordObject(cam, "Build Main Menu Layout");
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = SkyColor;

        GameObject old = GameObject.Find(BackgroundRootName);
        if (old != null)
            Undo.DestroyObjectImmediate(old);

        GameObject root = new GameObject(BackgroundRootName);
        Undo.RegisterCreatedObjectUndo(root, "Build Main Menu Layout");

        Vector3 c = cam.transform.position;
        float halfHeight = cam.orthographicSize;
        float bottom = c.y - halfHeight;
        float groundHeight = 2f;
        float groundTop = bottom + groundHeight;

        // ground: tiled brick floor across the whole screen
        SpriteRenderer ground = AddSprite(root, "Ground", MiscSheet, "brownFloor", Vector2.zero, 1);
        ground.drawMode = SpriteDrawMode.Tiled;
        ground.size = new Vector2(40f, groundHeight);
        ground.transform.position = new Vector3(c.x, bottom + groundHeight / 2f, 0f);

        PlaceOnGround(AddSprite(root, "Hill", MiscSheet, "big_hill", Vector2.zero, 0), c.x - 6f, groundTop);
        PlaceOnGround(AddSprite(root, "Shrub", MiscSheet, "1shrub", Vector2.zero, 0), c.x + 5.5f, groundTop);
        PlaceOnGround(AddSprite(root, "Mario", MarioSheet, "mario_6", Vector2.zero, 2), c.x - 4f, groundTop);

        AddSprite(root, "Cloud Left", MiscSheet, "1cloud", new Vector2(c.x - 6.5f, c.y + 2.5f), 0);
        AddSprite(root, "Cloud Right", MiscSheet, "1cloud", new Vector2(c.x + 6.5f, c.y + 3.2f), 0);
    }

    static SpriteRenderer AddSprite(GameObject parent, string name, string sheet, string spriteName,
                                    Vector2 position, int order)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.transform.position = new Vector3(position.x, position.y, 0f);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = LoadSprite(sheet, spriteName);
        sr.sortingOrder = order;
        return sr;
    }

    static void PlaceOnGround(SpriteRenderer sr, float x, float groundTop)
    {
        float y = groundTop;
        if (sr.sprite != null)
            y += sr.sprite.bounds.extents.y - sr.sprite.bounds.center.y;
        sr.transform.position = new Vector3(x, y, 0f);
    }

    static Sprite LoadSprite(string sheet, string spriteName)
    {
        Sprite sprite = AssetDatabase.LoadAllAssetsAtPath(sheet)
            .OfType<Sprite>()
            .FirstOrDefault(s => s.name == spriteName);

        if (sprite == null)
            Debug.LogWarning($"Sprite '{spriteName}' not found in {sheet}");
        return sprite;
    }

    // ---------- UI ----------

    static void BuildUI(Canvas canvas)
    {
        // scale the UI with the window, so the layout looks the same at any resolution
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            Undo.RecordObject(scaler, "Build Main Menu Layout");
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }

        // title logo
        Transform oldLogo = canvas.transform.Find(LogoName);
        if (oldLogo != null)
            Undo.DestroyObjectImmediate(oldLogo.gameObject);

        GameObject logo = new GameObject(LogoName, typeof(RectTransform), typeof(Image));
        Undo.RegisterCreatedObjectUndo(logo, "Build Main Menu Layout");
        logo.transform.SetParent(canvas.transform, false);
        logo.transform.SetAsFirstSibling();

        Image logoImage = logo.GetComponent<Image>();
        logoImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(LogoPath);
        logoImage.preserveAspect = true;
        logoImage.raycastTarget = false;
        SetRect(logo.GetComponent<RectTransform>(), new Vector2(0, 220), new Vector2(174 * 4.5f, 86 * 4.5f));

        // existing menu items, matched by name so MainMenu's references stay intact
        LayoutButton(canvas, "Start Game Button", -120);
        LayoutButton(canvas, "Reset High Score Button", -200);
        LayoutHighScore(canvas, -310);
    }

    static void LayoutButton(Canvas canvas, string name, float y)
    {
        Transform button = FindDeep(canvas.transform, name);
        if (button == null)
        {
            Debug.LogWarning($"'{name}' not found in the Canvas");
            return;
        }

        RectTransform rect = (RectTransform)button;
        Undo.RecordObject(rect, "Build Main Menu Layout");
        SetRect(rect, new Vector2(0, y), new Vector2(700, 70));

        // hide the default white button background, keep it clickable
        Image bg = button.GetComponent<Image>();
        if (bg != null)
        {
            Undo.RecordObject(bg, "Build Main Menu Layout");
            bg.color = new Color(1, 1, 1, 0);
        }

        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
        {
            Undo.RecordObject(label, "Build Main Menu Layout");
            label.fontSize = 40;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.fontStyle = FontStyles.UpperCase;
            label.textWrappingMode = TextWrappingModes.NoWrap;
        }

        // mushroom cursor just left of the text
        Transform cursor = button.Find("Cursor");
        if (cursor != null)
        {
            Undo.RecordObject(cursor, "Build Main Menu Layout");
            SetRect((RectTransform)cursor, new Vector2(-390, 0), new Vector2(50, 50));
        }
    }

    static void LayoutHighScore(Canvas canvas, float y)
    {
        Transform highScore = FindDeep(canvas.transform, "High Score");
        if (highScore == null)
        {
            Debug.LogWarning("'High Score' not found in the Canvas");
            return;
        }

        Undo.RecordObject(highScore, "Build Main Menu Layout");
        SetRect((RectTransform)highScore, new Vector2(0, y), new Vector2(900, 60));

        TMP_Text text = highScore.GetComponent<TMP_Text>();
        if (text != null)
        {
            Undo.RecordObject(text, "Build Main Menu Layout");
            text.fontSize = 40;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            text.fontStyle = FontStyles.UpperCase;
        }
    }

    static void SetRect(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    static Transform FindDeep(Transform parent, string name)
    {
        return parent.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == name);
    }

    // make sure the cut-out logo imports as a crisp pixel-art sprite
    static void PrepareLogoImporter()
    {
        TextureImporter importer = AssetImporter.GetAtPath(LogoPath) as TextureImporter;
        if (importer == null)
            return;

        bool changed = importer.textureType != TextureImporterType.Sprite
                       || importer.filterMode != FilterMode.Point
                       || importer.textureCompression != TextureImporterCompression.Uncompressed;
        if (!changed)
            return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
    }
}
