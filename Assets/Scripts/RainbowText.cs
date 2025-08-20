using TMPro;
using UnityEngine;

public class RainbowText : MonoBehaviour
{
    [SerializeField] private float rainbowSpeed = 1f;
    private TextMeshProUGUI text;
    private float time;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Her frame'de yazı karakterlerini renklendir
    void Update()
    {
        if (text == null) return;
        text.ForceMeshUpdate();
        var mesh = text.mesh;
        var colors = mesh.colors32;
        int charCount = text.textInfo.characterCount;
        time += Time.unscaledDeltaTime * rainbowSpeed;

        for (int i = 0; i < charCount; i++)
        {
            if (!text.textInfo.characterInfo[i].isVisible) continue;
            float hue = Mathf.Repeat((i * 0.1f + time), 1f);
            Color32 color = Color.HSVToRGB(hue, 1f, 1f);

            int vertexIndex = text.textInfo.characterInfo[i].vertexIndex;
            for (int j = 0; j < 4; j++)
            {
                colors[vertexIndex + j] = color;
            }
        }
        mesh.colors32 = colors;
        text.canvasRenderer.SetMesh(mesh);
    }
}
