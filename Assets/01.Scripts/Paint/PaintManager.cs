using UnityEngine;
using UnityEngine.Rendering;

public class PaintManager : Singleton<PaintManager>
{
    // ���Ǹ� ���� ���� �� �ڵ��Դϴ�. �ּ����� ������ ������ҽ��ϴ�.

    public Shader texturePaint;
    public Shader extendIslands;

    // ���̴� �Ӽ� ID�� ĳ���Ͽ� ������
    private int prepareUVID = Shader.PropertyToID("_PrepareUV");
    private int positionID = Shader.PropertyToID("_PainterPosition");
    private int hardnessID = Shader.PropertyToID("_Hardness");
    private int strengthID = Shader.PropertyToID("_Strength");
    private int radiusID = Shader.PropertyToID("_Radius");
    private int colorID = Shader.PropertyToID("_PainterColor");
    private int textureID = Shader.PropertyToID("_MainTex");
    private int uvOffsetID = Shader.PropertyToID("_OffsetUV");
    private int uvIslandsID = Shader.PropertyToID("_UVIslands");

    private Material paintMaterial;
    private Material extendMaterial;

    private CommandBuffer command;

    private void Awake()
    {
        // ���̴��κ��� ���ο� ���׸��� ����
        paintMaterial = new Material(texturePaint);
        extendMaterial = new Material(extendIslands);

        // ���ο� ��� ���� ����
        command = new CommandBuffer();
    }

    public void initTextures(Paintable paintable)
    {
        // Paintable ��ü���� ���� �ؽ�ó�� ������
        RenderTexture mask = paintable.getMask();
        RenderTexture uvIslands = paintable.getUVIslands();
        RenderTexture extend = paintable.getExtend();
        RenderTexture support = paintable.getSupport();
        Renderer rend = paintable.getRenderer();

        // ��� ���ۿ� ���� Ÿ���� ����
        command.SetRenderTarget(mask);
        command.SetRenderTarget(extend);
        command.SetRenderTarget(support);

        // ����Ʈ ���׸��� �غ� ���� �÷��� ����
        paintMaterial.SetFloat(prepareUVID, 1);
        command.SetRenderTarget(uvIslands);
        command.DrawRenderer(rend, paintMaterial, 0);

        // ��� ���� ���� �� �ʱ�ȭ
        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }

    public void paint(Paintable paintable, Vector3 pos, float radius = 1f, float hardness = .5f, float strength = .5f, Color? color = null)
    {
        // Paintable ��ü���� ���� �ؽ�ó�� ������
        RenderTexture mask = paintable.getMask();
        RenderTexture uvIslands = paintable.getUVIslands();
        RenderTexture extend = paintable.getExtend();
        RenderTexture support = paintable.getSupport();
        Renderer rend = paintable.getRenderer();

        // ����Ʈ ���׸����� ���̴� �Ӽ��� ����
        paintMaterial.SetFloat(prepareUVID, 0);
        paintMaterial.SetVector(positionID, pos);
        paintMaterial.SetFloat(hardnessID, hardness);
        paintMaterial.SetFloat(strengthID, strength);
        paintMaterial.SetFloat(radiusID, radius);
        paintMaterial.SetTexture(textureID, support);
        paintMaterial.SetColor(colorID, color ?? Color.red);

        extendMaterial.SetFloat(uvOffsetID, paintable.extendsIslandOffset);
        extendMaterial.SetTexture(uvIslandsID, uvIslands);

        // ��� ���ۿ� ���� Ÿ�� �� ��ο� ȣ���� �߰�
        command.SetRenderTarget(mask);
        command.DrawRenderer(rend, paintMaterial, 0);

        // ����ũ�� support �ؽ�ó�� ����
        command.SetRenderTarget(support);
        command.Blit(mask, support);

        // ����ũ�� Ȯ�� �ؽ�ó�� �����ϰ� Ȯ�� ���׸����� ���
        command.SetRenderTarget(extend);
        command.Blit(mask, extend, extendMaterial);

        // ��� ���� ���� �� �ʱ�ȭ
        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }
}
