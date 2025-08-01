using System;
using Godot;
using Godot.Collections;

public partial class VolumetricMesh : MeshInstance3D
{
    [Export] public int XResolution = 1024;
    [Export] public int YResolution = 1024;
    [Export] public int DepthResolution { get; set; } = 0;
    [Export] public string SliceDirectory { get; set; } = "";

    [Export] public int SamplingSteps = 256;

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

    private Array<Image> _slices;

    private ShaderMaterial Shader => (ShaderMaterial)MaterialOverride;

    private bool IsCtrlPressed { get; set; }
    private bool IsAltPressed { get; set; }
    private bool IsShiftPressed { get; set; }
    
    private float DensityThreshold { get; set; } = 0.1f;
    private float DepthScale { get; set; } = 0.5f;
    private float ModulationFactor { get; set; } = 1f;
    
    public override void _Ready()
    {
        _slices = [];
        for (var i = 0; i < DepthResolution; i++)
        {
            var loadPath = $"res://assets/{SliceDirectory}/slice_{i:D3}.png";
            
            var image = GD.Load<CompressedTexture2D>(loadPath).GetImage();
            if (image != null)
            {
                _slices.Add(image);
            }
            else
            {
                GD.PrintErr($"Error loading slice {loadPath}");
            }
        }

        Shader.SetShaderParameter("volume_texture", VolumeTexture);
        Shader.SetShaderParameter("density_threshold", DensityThreshold);
        Shader.SetShaderParameter("volume_shape", new Vector3(1f, 1f, DepthScale));
        Shader.SetShaderParameter("modulation_factor", ModulationFactor);
        Shader.SetShaderParameter("max_steps", SamplingSteps * 1.0f);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouseMotion eventMouseMotion:
                if (IsCtrlPressed)
                {
                    var rotation = new Vector3(Rotation.X, Rotation.Y, Rotation.Z);
                    rotation.Y += eventMouseMotion.Relative.X * 0.001f;
                    rotation.X += eventMouseMotion.Relative.Y * 0.001f;
                    SetRotation(rotation);
                }
                break;
            
            case InputEventKey eventKey:
                if (eventKey.IsAction("ctrl-press"))
                {
                    IsCtrlPressed = eventKey.Pressed;
                }

                if (eventKey.IsAction("alt-press"))
                {
                    IsAltPressed = eventKey.Pressed;
                }

                if (eventKey.IsAction("shift-press"))
                {
                    IsShiftPressed = eventKey.Pressed;
                }

                if (eventKey.IsAction("increase-density") && IsCtrlPressed)
                {
                    DensityThreshold = Math.Clamp(DensityThreshold + 0.01f, 0.0f, 1.0f);
                    Shader.SetShaderParameter("density_threshold", DensityThreshold);
                }

                if (eventKey.IsAction("decrease-density") && IsCtrlPressed)
                {
                    DensityThreshold = Math.Clamp(DensityThreshold - 0.01f, 0.0f, 1.0f);
                    Shader.SetShaderParameter("density_threshold", DensityThreshold);
                }

                if (eventKey.IsAction("increase-depth-scale") && IsAltPressed)
                {
                    DepthScale = Math.Clamp(DepthScale + 0.01f, 0.0f, 1.0f);
                    Shader.SetShaderParameter("volume_shape", new Vector3(1f, 1f, DepthScale));

                }

                if (eventKey.IsAction("decrease-depth-scale") && IsAltPressed)
                {
                    DepthScale = Math.Clamp(DepthScale - 0.01f, 0.0f, 1.0f);
                    Shader.SetShaderParameter("volume_shape", new Vector3(1f, 1f, DepthScale));

                }

                if (eventKey.IsAction("increase-modulation") && IsShiftPressed)
                {
                    ModulationFactor += 0.01f;
                    Shader.SetShaderParameter("modulation_factor", ModulationFactor);
                }

                if (eventKey.IsAction("decrease-modulation") && IsShiftPressed)
                {
                    ModulationFactor -= 0.01f;
                    if (ModulationFactor < 0) ModulationFactor = 0;
                    Shader.SetShaderParameter("modulation_factor", ModulationFactor);
                }
                break;
        }
    }
}
