using Godot;
using System;
using System.Linq;
using Godot.Collections;

public partial class MeshContainer : Node3D
{

	private const int RotationSpeed = 2;

	private bool IsBenchmarkRunning { get; set; } = false;
	
	private double _xRotation = 0;
	private Array<double> _deltas = new Array<double>();

	public override void _Ready()
	{
		base._Ready();
		GetNode<XRController3D>("%RightHand").ButtonPressed += (_) =>
		{
			StartBenchmark();
		};
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is not InputEventKey eventKey) return;
		if ((eventKey.IsAction("start-benchmark") || eventKey.IsAction("godot/by_button")) && !IsBenchmarkRunning)
		{
			StartBenchmark();
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		
		if (IsBenchmarkRunning)
		{
			// I don't know if there's some internal modulo operation going on inside
			// SetRotation - Hence the separate rotation tracking
			var xInc = RotationSpeed * delta;
			_xRotation += xInc;

			SetRotation(new Vector3(
				Rotation.X + (float)xInc,
				Rotation.Y + (float)(RotationSpeed * delta),
				Rotation.Z + (float)(RotationSpeed * delta)));
			
			_deltas.Add(delta);
			if (_xRotation >= 10 * Math.PI) // 5 rotations around x axis
			{
				IsBenchmarkRunning = false;
				LogBenchmark();
			}
		}
	}

	private void StartBenchmark()
	{
		IsBenchmarkRunning = true;
		SetRotation(Vector3.Zero);
		_xRotation = 0;
		_deltas = [];
	}

	private void LogBenchmark()
	{
		var frameRates = _deltas.Select(d => 1/d).ToArray();
		var avg = frameRates.Average();
		var variance = frameRates.Select(x => Math.Pow(x - avg, 2)).Average();
		var deviation = Math.Sqrt(variance);
		GD.Print("##########   Benchmark report:   ##########");
		GD.Print($"Fps samples: [{string.Join(", ", frameRates.Select(f => f.ToString("F2")))}]");
		GD.Print($"Sample count: {frameRates.Length}");
		GD.Print($"Average: {avg}");
		GD.Print($"Minimum: {frameRates.Min()}");
		GD.Print($"Maximum: {frameRates.Max()}");
		GD.Print($"Deviation: {deviation}");
		GD.Print($"Variance: {variance}");
	}
}
