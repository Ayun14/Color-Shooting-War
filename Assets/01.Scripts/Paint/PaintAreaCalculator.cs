using UnityEngine;

public class PaintAreaCalculator : MonoBehaviour
{
    [SerializeField] private PaintManager paintManager;

    // �� �޼���� ������ ĥ���� ������ ������ ����մϴ�.
    public float CalculatePaintCoverage(Paintable paintable)
    {
        // Paintable ��ü���� ����ũ �ؽ�ó�� �����ɴϴ�.
        RenderTexture mask = paintable.getMask();

        // ����ũ �ؽ�ó�� ���� �� �ְ� ����ϴ�.
        RenderTexture.active = mask;

        // RenderTexture���� �ȼ��� �б� ���� ���ο� Texture2D�� �����մϴ�.
        Texture2D readableTexture = new Texture2D(mask.width, mask.height, TextureFormat.RGB24, false);
        readableTexture.ReadPixels(new Rect(0, 0, mask.width, mask.height), 0, 0);
        readableTexture.Apply();

        RenderTexture.active = null;

        // ���� �� �ִ� �ؽ�ó���� �ȼ� �����͸� �����ɴϴ�.
        Color[] pixels = readableTexture.GetPixels();

        // ĥ���� �ȼ��� ���ϴ�.
        int paintedPixelCount = 0;
        foreach (Color pixel in pixels)
        {
            // ĥ���� ������ �������� �ʴٰ� �����մϴ�.
            if (pixel.a > 0)
            {
                paintedPixelCount++;
            }
        }

        // �� �ȼ� ���� ����մϴ�.
        int totalPixels = pixels.Length;

        // ĥ���� ������ ������ ����մϴ�.
        float coverage = (float)paintedPixelCount / totalPixels * 100;

        // ����
        Destroy(readableTexture);

        return coverage;
    }
}
