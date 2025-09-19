using Godot;
using System;
using System.Collections.Generic;

public partial class PopupJogadoresNoJogo : PopupPanel
{
	[Signal]
    public delegate void JogadorFoiSelecionadoEventHandler(int jogadorIndex);
	public Jogador JogadorDescartadoNaListagem;
	public List<Jogador> JogadoresListados = new List<Jogador>();
	public HBoxContainer EspaçoColocJogador;
	public Jogo JogoAcontecendo;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		EspaçoColocJogador = GetNode<HBoxContainer>("VBoxContainer/HBoxContainer");

		if (GetParent().GetParent() is Jogo)
		{
			JogoAcontecendo = GetParent().GetParent() as Jogo;
			JogadoresListados = JogoAcontecendo.Jogadores;
		}
			

		foreach (Jogador jogadorNaPartida in JogadoresListados)
		{
			if (jogadorNaPartida == JogadorDescartadoNaListagem)
				continue;

			GD.Print(jogadorNaPartida.NomeJogador);
			VBoxContainer novoJogador = new VBoxContainer();
			novoJogador.CustomMinimumSize = new Vector2(100, 0);
			novoJogador.AddThemeConstantOverride("separation", -20);

			TextureRect fotoJogador = new TextureRect();
			fotoJogador.Texture = GD.Load<Texture2D>("res://Sprites/generic_player_pic.png");
			fotoJogador.ExpandMode = TextureRect.ExpandModeEnum.FitHeight;
			fotoJogador.StretchMode = TextureRect.StretchModeEnum.KeepAspect;

			Label nomeJogador = new Label();
			nomeJogador.Text = jogadorNaPartida.NomeJogador;
			nomeJogador.HorizontalAlignment = HorizontalAlignment.Center;
			nomeJogador.AddThemeColorOverride("font_color", new Color(0, 0, 0));
			nomeJogador.ZIndex = 10;

			novoJogador.AddChild(fotoJogador);
			novoJogador.AddChild(nomeJogador);

			novoJogador.MouseFilter = Control.MouseFilterEnum.Stop;
			novoJogador.GuiInput += (InputEvent @event) => OnJogadorGuiInput(@event, jogadorNaPartida);

			EspaçoColocJogador.AddChild(novoJogador);
		}


	}

	private void OnJogadorGuiInput(InputEvent @event, Jogador jogadorSelecionado)
	{
		
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
			EmitSignal(SignalName.JogadorFoiSelecionado, JogadoresListados.IndexOf(jogadorSelecionado));
	}

}
