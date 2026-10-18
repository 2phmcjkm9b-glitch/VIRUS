using System.IO;
using UnityEditor;
using UnityEngine;

public static class VirusSetupWizard
{
    private const string SpriteFolder = "Assets/Sprites";
    private const string SpritePath = SpriteFolder + "/VirusCharacter.png";

    [MenuItem("VIRUS/Create Starter Virus")]
    public static void CreateStarterVirus()
    {
        EnsureFolder("Assets", "Editor");
        EnsureFolder("Assets", "Sprites");

        if (!File.Exists(SpritePath))
        {
            byte[] png = BuildVirusPng(512);
            File.WriteAllBytes(SpritePath, png);
            AssetDatabase.ImportAsset(SpritePath, ImportAssetOptions.ForceUpdate);
        }

        TextureImporter importer = AssetImporter.GetAtPath(SpritePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.SaveAndReimport();
        }

        Sprite virusSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
        if (virusSprite == null)
        {
            EditorUtility.DisplayDialog("VIRUS", "Не удалось создать спрайт. Попробуй ещё раз и проверь Console.", "OK");
            return;
        }

        GameObject existing = GameObject.Find("VirusBody");
        GameObject virus = existing != null ? existing : new GameObject("VirusBody");
        Undo.RegisterCreatedObjectUndo(virus, "Create Virus");

        virus.transform.position = Vector3.zero;
        virus.transform.rotation = Quaternion.identity;
        virus.transform.localScale = Vector3.one;

        SpriteRenderer sr = virus.GetComponent<SpriteRenderer>();
        if (sr == null) sr = Undo.AddComponent<SpriteRenderer>(virus);
        sr.sprite = virusSprite;
        sr.color = Color.white;
        sr.sortingOrder = 10;

        Selection.activeGameObject = virus;
        EditorSceneManagerMarkDirty();
        EditorUtility.DisplayDialog("VIRUS", "Готово! В сцене создан зелёный колючий вирус. Нажми Play, чтобы увидеть его в Game.", "Отлично!");
    }

    private static void EditorSceneManagerMarkDirty()
    {
        if (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().IsValid())
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }

    private static void EnsureFolder(string parent, string child)
    {
        string path = parent + "/" + child;
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, child);
    }

    private static byte[] BuildVirusPng(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color32 clear = new Color32(0, 0, 0, 0);
        Color32 outline = new Color32(20, 70, 26, 255);
        Color32 green = new Color32(57, 230, 57, 255);
        Color32 white = new Color32(255, 255, 255, 255);
        Color32 pupil = new Color32(20, 25, 20, 255);
        Color32 mouth = new Color32(55, 18, 18, 255);
        Color32[] pixels = new Color32[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = clear;

        Vector2 c = new Vector2((size - 1) / 2f, (size - 1) / 2f);
        float scale = size / 256f;
        float radius = 76f * scale;

        // Twelve dark spikes around the body.
        for (int i = 0; i < 12; i++)
        {
            float angle = (i * 30f + 90f) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 a = c + dir * (radius * 0.72f);
            Vector2 b = c + dir * (radius + 23f * scale);
            DrawLine(pixels, size, a, b, 12f * scale, outline);
        }

        // Green circular body with a dark border.
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float d = Vector2.Distance(new Vector2(x, y), c);
            if (d <= radius) pixels[y * size + x] = d >= radius - 5f * scale ? outline : green;
        }

        // White eyes with dark pupils.
        Ellipse(pixels, size, c + new Vector2(-27, 17) * scale, 17 * scale, 22 * scale, outline);
        Ellipse(pixels, size, c + new Vector2(27, 17) * scale, 17 * scale, 22 * scale, outline);
        Ellipse(pixels, size, c + new Vector2(-27, 18) * scale, 13 * scale, 18 * scale, white);
        Ellipse(pixels, size, c + new Vector2(27, 18) * scale, 13 * scale, 18 * scale, white);
        Ellipse(pixels, size, c + new Vector2(-23, 16) * scale, 6 * scale, 10 * scale, pupil);
        Ellipse(pixels, size, c + new Vector2(31, 16) * scale, 6 * scale, 10 * scale, pupil);

        // Angry eyebrows and a frown.
        DrawLine(pixels, size, c + new Vector2(-45, 45) * scale, c + new Vector2(-12, 34) * scale, 6 * scale, outline);
        DrawLine(pixels, size, c + new Vector2(12, 34) * scale, c + new Vector2(45, 45) * scale, 6 * scale, outline);
        DrawLine(pixels, size, c + new Vector2(-20, -28) * scale, c + new Vector2(0, -36) * scale, 6 * scale, mouth);
        DrawLine(pixels, size, c + new Vector2(0, -36) * scale, c + new Vector2(20, -28) * scale, 6 * scale, mouth);

        tex.SetPixels32(pixels);
        tex.Apply();
        byte[] result = tex.EncodeToPNG();
        Object.DestroyImmediate(tex);
        return result;
    }

    private static void Ellipse(Color32[] pixels, int size, Vector2 center, float rx, float ry, Color32 color)
    {
        int minX = Mathf.Max(0, Mathf.FloorToInt(center.x - rx - 1));
        int maxX = Mathf.Min(size - 1, Mathf.CeilToInt(center.x + rx + 1));
        int minY = Mathf.Max(0, Mathf.FloorToInt(center.y - ry - 1));
        int maxY = Mathf.Min(size - 1, Mathf.CeilToInt(center.y + ry + 1));
        for (int y = minY; y <= maxY; y++)
        for (int x = minX; x <= maxX; x++)
        {
            float dx = (x - center.x) / Mathf.Max(rx, 0.001f);
            float dy = (y - center.y) / Mathf.Max(ry, 0.001f);
            if (dx * dx + dy * dy <= 1f) pixels[y * size + x] = color;
        }
    }

    private static void DrawLine(Color32[] pixels, int size, Vector2 a, Vector2 b, float thickness, Color32 color)
    {
        int minX = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(a.x, b.x) - thickness - 1));
        int maxX = Mathf.Min(size - 1, Mathf.CeilToInt(Mathf.Max(a.x, b.x) + thickness + 1));
        int minY = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(a.y, b.y) - thickness - 1));
        int maxY = Mathf.Min(size - 1, Mathf.CeilToInt(Mathf.Max(a.y, b.y) + thickness + 1));
        Vector2 ab = b - a;
        for (int y = minY; y <= maxY; y++)
        for (int x = minX; x <= maxX; x++)
        {
            Vector2 p = new Vector2(x, y);
            float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / (ab.sqrMagnitude + 0.0001f));
            if (Vector2.Distance(p, a + t * ab) <= thickness * 0.5f)
                pixels[y * size + x] = color;
        }
    }
}
