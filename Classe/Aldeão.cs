using Godot;
using System;
using System.Collections.Generic;

public partial class Aldeão : Node
{
	[Export]
	public CompressedTexture2D IconeClasse = GD.Load<CompressedTexture2D>("res://Classe/Icones/peasant_icon.png");
	[Export]
	public string DescricaoClasse = "Aldeões não possuem efeitos adicionais. Sua principal forma de se proteger é votando pelo clã rival suspeito.";
	public Jogador JogadorUsuario;
	public string Classe;
	public string ClaClasse;
	public Jogo ControladorJogo;
	public List<Jogador> JogadoresNoJogo;
	public PackedScene CenaEscolhaJogador = GD.Load<PackedScene>("res://Jogadores no Jogo Popup/popup_jogadores_no_jogo.tscn");
	private TextureButton CartaVirada;
	private VBoxContainer AcoesClasse;
	private Label ClaNaCarta;
	public override void _Ready()
	{
		CartaVirada = GetNode<TextureButton>("Carta Virada");
		AcoesClasse = GetNode<VBoxContainer>("Carta Desvirada/Margem da Carta/Sumário da Carta/Descrição da Carta/Ações da Carta");
		ClaNaCarta = GetNode<Label>("Carta Desvirada/Margem da Carta/Sumário da Carta/Clã da Carta");

		if (GetParent() is Jogo)
		{
			ControladorJogo = GetParent() as Jogo;
			JogadoresNoJogo = ControladorJogo.Jogadores;
		}


		foreach (MarginContainer Acao in AcoesClasse.GetChildren())
		{
			if (Acao.GetChild(0) is Button)
			{
				Button acao = Acao.GetChild(0) as Button;
				acao.ButtonUp += () => OnAcaoButtonUp(acao);
			}
		}

	}

	public Node GerarIconeClasse()
	{
		PanelContainer fundoClasse = new PanelContainer();
		fundoClasse.CustomMinimumSize = new Vector2(100, 100); 
		fundoClasse.Size = new Vector2(100, 100); // Força o tamanho exato
		fundoClasse.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter; // Não expande horizontalmente
		fundoClasse.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter; // Não expande verticalmente
		fundoClasse.MouseFilter = Control.MouseFilterEnum.Ignore;
		fundoClasse.AddThemeStyleboxOverride("panel", GD.Load<StyleBox>("res://Resources/StyleBoxes/BotaoClasseStyle.tres"));

		VBoxContainer dadosClasse = new VBoxContainer();
		dadosClasse.AddThemeConstantOverride("separation", 0);

		TextureRect iconeClasse = new TextureRect();
		iconeClasse.Texture = IconeClasse;
		iconeClasse.ExpandMode = TextureRect.ExpandModeEnum.FitWidthProportional;
		iconeClasse.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
		iconeClasse.CustomMinimumSize = new Vector2(60, 60); // Tamanho fixo para o ícone
		iconeClasse.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter; // Centraliza horizontalmente

		Label nomeClasse = new Label();
		nomeClasse.Text = Name;
		nomeClasse.HorizontalAlignment = HorizontalAlignment.Center;
		nomeClasse.VerticalAlignment = VerticalAlignment.Center;
		nomeClasse.CustomMinimumSize = new Vector2(0, 30); // Altura mínima fixa

		dadosClasse.AddChild(iconeClasse);
		dadosClasse.AddChild(nomeClasse);

		fundoClasse.AddChild(dadosClasse);

		return fundoClasse;
	}

	private void OnCartaViradaButtonUp()
	{
		CartaVirada.Visible = false;
	}
	private void PassarTurno()
	{
		(GetChild(0) as Control).Visible = false;
		(GetChild(1) as Control).Visible = false;
	}

	private void MatarJogador(Jogador assassino, Jogador vitima)
	{
		GD.Print($"Assassino: {assassino.NomeJogador}");
		GD.Print($"Vitima: {vitima.NomeJogador}");
		JogadoresNoJogo.Remove(vitima);
	}

	private void OnAcaoButtonUp(Button acaoApertada)
	{
		switch (acaoApertada.Name)
		{
			case "Passar Turno":
				PassarTurno();
				break;

			case "Matar Jogador":
				PopupJogadoresNoJogo popupEscolhaJogador = CenaEscolhaJogador.Instantiate<PopupJogadoresNoJogo>();
				popupEscolhaJogador.JogadorDescartadoNaListagem = JogadorUsuario;
        		AddChild(popupEscolhaJogador);

				popupEscolhaJogador.JogadorFoiSelecionado += (int jogadorSelecionadoIndex) =>
				{
					MatarJogador(JogadorUsuario, JogadoresNoJogo[jogadorSelecionadoIndex]);
					popupEscolhaJogador.QueueFree();
				};
				break;
		}
	}
}
