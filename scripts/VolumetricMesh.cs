using Godot;
using Godot.Collections;

public partial class VolumetricMesh : MeshInstance3D
{
    [Export] public int XResolution = 1024;
    [Export] public int YResolution = 1024;
    [Export] public int DepthResolution { get; set; } = 0;
    [Export] public string SliceDirectory { get; set; } = "";

    private ImageTexture3D VolumeTexture
    {
        get
        {
            var texture = new ImageTexture3D();
            texture.Create(
                Image.Format.Rgb8,
                XResolution,
                YResolution,
                DepthResolution,
                false,
                _slices);
            return texture;
        }
    }

    private NoiseTexture3D TestTexture
    {
        get
        {
            GD.Randomize();
            var texture = new NoiseTexture3D();
            var noise = new FastNoiseLite();
            noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
            noise.SetSeed((int)GD.Randi());
            texture.SetNoise(noise);
            texture.SetDepth(64);
            texture.SetWidth(64);
            texture.SetHeight(64);
            return texture;
        }
    }

    private Array<Image> _slices;

    public override void _Ready()
    {
        _slices = [];
        for (var i = 0; i < DepthResolution; i++)
        {
            var loadPath = $"res://assets/{SliceDirectory}/slice_{i:D3}.png";
            
            var image = GD.Load<CompressedTexture2D>(loadPath).GetImage();
            if (image != null)
            {
                GD.Print($"Loaded slice {loadPath} successfully: {image.GetWidth()}x{image.GetHeight()}, {image.GetFormat()}.");
                _slices.Add(image);
            }
            else
            {
                GD.PrintErr($"Error loading slice {loadPath}");
            }
        }

        var mat = (ShaderMaterial)MaterialOverride;
        mat.SetShaderParameter("volume_texture", VolumeTexture);
    }
}
