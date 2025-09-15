using Godot;
using System;

public partial class Jogo : TextureRect
{
	public override void _Ready()
	{
		GD.Print(GetChild(0).GetChild(0));
	}

	public override void _Process(double delta)
	{
		PanelContainer ultimaCartaDaRodada = GetChild(0).GetChild(0) as PanelContainer;

		if ((GetChild(0).GetChild(0) as PanelContainer).Visible == false)
		{
			Texture = GD.Load<CompressedTexture2D>("res://Sprites/background_gameday.jpg");
		}

		/*foreach (Node carta in GetChildren())
		{
			PanelContainer cartaDesvirada = carta.GetChild(0) as PanelContainer;
			TextureButton cartaVirada = carta.GetChild(1) as TextureButton;
			cartaDesvirada.Visible = true;
			cartaVirada.Visible = true;
		}*/

	}
}
