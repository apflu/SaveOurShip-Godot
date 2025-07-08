using Godot;
using System;

public partial class SlotContainer : GridContainer
{
	private static readonly int slotCount = 5;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for (int i = 0; i < slotCount; i++)
		{
			AddChild(new InventorySlot());
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
