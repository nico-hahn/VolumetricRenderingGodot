using Godot;

public partial class Root : Node3D
{
	private XRInterface _xrInterface;

	public override void _Ready()
	{
		_xrInterface = XRServer.FindInterface("OpenXR");
		if (_xrInterface != null && _xrInterface.IsInitialized())
		{
			GD.Print("OpenXR is initialized");

			// Turn off v-sync!
			DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);

			// Change our main viewport to output to the HMD
			GetViewport().UseXR = true;

			// Must be set to use passthrough
			_xrInterface.EnvironmentBlendMode = XRInterface.EnvironmentBlendModeEnum.AlphaBlend;
		}
		else
		{
			GD.Print("OpenXR not initialized, please check if your headset is connected");
		}
	}
}
