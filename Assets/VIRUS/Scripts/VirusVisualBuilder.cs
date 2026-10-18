
using UnityEngine;

public class VirusVisualBuilder : MonoBehaviour
{
    [Header("Virus Appearance")]
    [SerializeField] private float bodyRadius = 0.8f;
    [SerializeField] private int spikeCount = 24;
    [SerializeField] private float spikeLength = 0.3f;

    [Header("Face Position")]
    [SerializeField] private float faceHeight = 0.55f;
    [SerializeField] private float faceForward = 0.48f;

    [Tooltip("Additional face tilt relative to the camera.")]
    [SerializeField] private float faceTilt = -15f;

    [Header("Colors")]
    [SerializeField] private Color bodyColor = new Color(0.22f, 0.9f, 0.04f);
    [SerializeField] private Color spikeColor = new Color(0.12f, 0.65f, 0.02f);
    [SerializeField] private Color eyeColor = Color.white;
    [SerializeField] private Color pupilColor = new Color(0.06f, 0.025f, 0.015f);
    [SerializeField] private Color mouthColor = new Color(0.08f, 0.025f, 0.02f);
    [SerializeField] private Color browColor = new Color(0.08f, 0.35f, 0.015f);

    private Camera mainCamera;
    private Transform faceTransform;

    private void Awake()
    {
        FindCamera();
    }

    private void LateUpdate()
    {
        if (faceTransform == null)
            return;

        if (mainCamera == null)
            FindCamera();

        if (mainCamera == null)
            return;

        Vector3 directionToCamera =
            mainCamera.transform.position - faceTransform.position;

        if (directionToCamera.sqrMagnitude < 0.0001f)
            return;

        Quaternion cameraFacingRotation = Quaternion.LookRotation(
            directionToCamera.normalized,
            mainCamera.transform.up
        );

        faceTransform.rotation =
            cameraFacingRotation * Quaternion.Euler(faceTilt, 0f, 0f);
    }

    private void FindCamera()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
            mainCamera = FindAnyObjectByType<Camera>();
    }

    [ContextMenu("Build Virus")]
    public void BuildVirus()
    {
        faceTransform = null;
        ClearChildren();

        Material bodyMat = CreateMaterial("Virus Body", bodyColor);
        Material spikeMat = CreateMaterial("Virus Spikes", spikeColor);
        Material eyeMat = CreateMaterial("Virus Eyes", eyeColor);
        Material pupilMat = CreateMaterial("Virus Pupils", pupilColor);
        Material mouthMat = CreateMaterial("Virus Mouth", mouthColor);
        Material browMat = CreateMaterial("Virus Brows", browColor);

        CreateSphere(
            "Body",
            transform,
            Vector3.zero,
            Vector3.one * bodyRadius * 2f,
            bodyMat
        );

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
            spike.transform.localScale =
                new Vector3(0.16f, spikeLength, 0.16f);

            SetMaterial(spike, spikeMat);
        }

        GameObject face = new GameObject("Face");
        face.transform.SetParent(transform, false);

        face.transform.localPosition = new Vector3(
            0f,
            faceHeight,
            faceForward
        );

        face.transform.localRotation = Quaternion.identity;
        faceTransform = face.transform;

        // Eyes sit closer to the body.
        CreateSphere(
            "Left Eye",
            face.transform,
            new Vector3(-0.25f, 0.13f, 0.02f),
            new Vector3(0.29f, 0.34f, 0.09f),
            eyeMat
        );

        CreateSphere(
            "Right Eye",
            face.transform,
            new Vector3(0.25f, 0.13f, 0.02f),
            new Vector3(0.29f, 0.34f, 0.09f),
            eyeMat
        );

        // Pupils are smaller and less protruding.
        CreateSphere(
            "Left Pupil",
            face.transform,
            new Vector3(-0.22f, 0.12f, 0.07f),
            new Vector3(0.12f, 0.18f, 0.045f),
            pupilMat
        );

        CreateSphere(
            "Right Pupil",
            face.transform,
            new Vector3(0.28f, 0.12f, 0.07f),
            new Vector3(0.12f, 0.18f, 0.045f),
            pupilMat
        );

        GameObject leftBrow = CreateSphere(
            "Left Angry Brow",
            face.transform,
            new Vector3(-0.25f, 0.36f, 0.07f),
            new Vector3(0.34f, 0.085f, 0.06f),
            browMat
        );

        leftBrow.transform.localRotation =
            Quaternion.Euler(0f, 0f, -18f);

        GameObject rightBrow = CreateSphere(
            "Right Angry Brow",
            face.transform,
            new Vector3(0.25f, 0.36f, 0.07f),
            new Vector3(0.34f, 0.085f, 0.06f),
            browMat
        );

        rightBrow.transform.localRotation =
            Quaternion.Euler(0f, 0f, 18f);

        CreateSphere(
            "Angry Mouth",
            face.transform,
            new Vector3(0f, -0.19f, 0.07f),
            new Vector3(0.34f, 0.12f, 0.05f),
            mouthMat
        );

        Debug.Log("Virus visual built successfully!");
    }

    private GameObject CreateCone(string objectName, int sides)
    {
        GameObject cone = new GameObject(objectName);

        MeshFilter meshFilter = cone.AddComponent<MeshFilter>();
        cone.AddComponent<MeshRenderer>();

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
        Transform parent,
        Vector3 localPosition,
        Vector3 scale,
        Material material)
    {
        GameObject part =
            GameObject.CreatePrimitive(PrimitiveType.Sphere);

        part.name = objectName;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = localPosition;
        part.transform.localScale = scale;

        SetMaterial(part, material);

        return part;
    }

    private Material CreateMaterial(string materialName, Color color)
    {
        Shader shader =
            Shader.Find("Universal Render Pipeline/Lit");

        if (shader == null)
            shader = Shader.Find("Standard");

        if (shader == null)
        {
            Debug.LogError(
                "Could not find a supported shader for " + materialName
            );

            return null;
        }

        Material material = new Material(shader);
        material.name = materialName;

        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", color);

        if (material.HasProperty("_Color"))
            material.SetColor("_Color", color);

        return material;
    }

    private void SetMaterial(GameObject part, Material material)
    {
        if (material == null)
            return;

        Renderer renderer = part.GetComponent<Renderer>();

        if (renderer != null)
            renderer.sharedMaterial = material;
    }

    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }
    }
}
