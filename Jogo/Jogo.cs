using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public partial class Jogo : TextureRect
{
	public List<Jogador> Jogadores = new List<Jogador>();
	public List<Jogador> JogadoresMortos = new List<Jogador>();
	public override void _Ready()
	{
		foreach (Jogador jogador in Jogadores)
        {
            GD.Print($"Jogador: {jogador.NomeJogador}, Classe:{jogador.ClasseJogador.Name}, Clã: {jogador.ClasseJogador.ClaClasse}");
        }
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
