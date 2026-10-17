
using UnityEngine;

public class VirusVisualBuilder : MonoBehaviour
{
    [Header("Virus Appearance")]
    [SerializeField] private float bodyRadius = 0.8f;
    [SerializeField] private int spikeCount = 24;
    [SerializeField] private float spikeLength = 0.3f;

    [Header("Colors")]
    [SerializeField] private Color bodyColor = new Color(0.22f, 0.9f, 0.04f);
    [SerializeField] private Color spikeColor = new Color(0.12f, 0.65f, 0.02f);
    [SerializeField] private Color eyeColor = Color.white;
    [SerializeField] private Color mouthColor = new Color(0.08f, 0.025f, 0.02f);

    [ContextMenu("Build Virus")]
    public void BuildVirus()
    {
        ClearChildren();

        Material bodyMat = CreateMaterial("Virus Body", bodyColor);
        Material spikeMat = CreateMaterial("Virus Spikes", spikeColor);
        Material eyeMat = CreateMaterial("Virus Eyes", eyeColor);
        Material mouthMat = CreateMaterial("Virus Mouth", mouthColor);

        // Main body
        CreateSphere(
            "Body",
            Vector3.zero,
            Vector3.one * bodyRadius * 2f,
            bodyMat
        );

        // Distribute spikes around the body
        float goldenAngle = Mathf.PI * (3f - Mathf.Sqrt(5f));

        for (int i = 0; i < spikeCount; i++)
        {
            float y = 1f - (2f * (i + 0.5f) / spikeCount);
            float ringRadius = Mathf.Sqrt(1f - y * y);
            float angle = goldenAngle * i;

            Vector3 direction = new Vector3(
                Mathf.Cos(angle) * ringRadius,
                y,
                Mathf.Sin(angle) * ringRadius
            ).normalized;

            GameObject spike = CreateCone("Spike_" + (i + 1), 8);

            spike.transform.SetParent(transform, false);
            spike.transform.localPosition = direction * bodyRadius * 0.88f;
            spike.transform.localRotation =
                Quaternion.FromToRotation(Vector3.up, direction);

            spike.transform.localScale = new Vector3(
                0.16f,
                spikeLength,
                0.16f
            );

            SetMaterial(spike, spikeMat);
        }

        // Eyes
        CreateSphere(
            "Left Eye",
            new Vector3(-0.27f, 0.18f, bodyRadius * 0.88f),
            new Vector3(0.27f, 0.34f, 0.12f),
            eyeMat
        );

        CreateSphere(
            "Right Eye",
            new Vector3(0.27f, 0.18f, bodyRadius * 0.88f),
            new Vector3(0.27f, 0.34f, 0.12f),
            eyeMat
        );

        // Dark pupils
        Material pupilMat = CreateMaterial("Dark Pupils", mouthColor);

        CreateSphere(
            "Left Pupil",
            new Vector3(-0.25f, 0.16f, bodyRadius + 0.13f),
            new Vector3(0.11f, 0.17f, 0.06f),
            pupilMat
        );

        CreateSphere(
            "Right Pupil",
            new Vector3(0.29f, 0.16f, bodyRadius + 0.13f),
            new Vector3(0.11f, 0.17f, 0.06f),
            pupilMat
        );

        // Angry mouth
        GameObject mouth = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mouth.name = "Angry Mouth";
        mouth.transform.SetParent(transform, false);
        mouth.transform.localPosition = new Vector3(
            0f,
            -0.28f,
            bodyRadius * 0.91f
        );
        mouth.transform.localScale = new Vector3(0.38f, 0.12f, 0.08f);
        SetMaterial(mouth, mouthMat);

        Debug.Log("Virus visual built successfully!");
    }

    private GameObject CreateCone(string objectName, int sides)
    {
        GameObject cone = new GameObject(objectName);

        MeshFilter meshFilter = cone.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = cone.AddComponent<MeshRenderer>();

        // One tip and a circular base
        Vector3[] vertices = new Vector3[sides + 1];
        int[] triangles = new int[sides * 3];

        vertices[0] = Vector3.up;

        for (int i = 0; i < sides; i++)
        {
            float angle = i * Mathf.PI * 2f / sides;

            vertices[i + 1] = new Vector3(
                Mathf.Cos(angle) * 0.5f,
                0f,
                Mathf.Sin(angle) * 0.5f
            );
        }

        // Build the triangular faces
        for (int i = 0; i < sides; i++)
        {
            int next = (i + 1) % sides;

            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = next + 1;
            triangles[i * 3 + 2] = i + 1;
        }

        Mesh mesh = new Mesh();
        mesh.name = objectName + " Mesh";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;

        return cone;
    }

    private GameObject CreateSphere(
        string objectName,
        Vector3 localPosition,
        Vector3 scale,
        Material material)
    {
        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        part.name = objectName;
        part.transform.SetParent(transform, false);
        part.transform.localPosition = localPosition;
        part.transform.localScale = scale;

        SetMaterial(part, material);

        return part;
    }

    private Material CreateMaterial(string materialName, Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");

        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        Material material = new Material(shader);
        material.name = materialName;

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }

        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }

        return material;
    }

    private void SetMaterial(GameObject part, Material material)
    {
        Renderer renderer = part.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }
    }

    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (Application.isPlaying)
            {
                Destroy(child);
            }
            else
            {
                DestroyImmediate(child);
            }
        }
    }
}
