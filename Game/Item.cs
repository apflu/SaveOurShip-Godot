using Godot;
using System;

public partial class Item : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//GetNode<TextureRect>("TextureRect").Texture = ResourceLoader.Load("res://");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
