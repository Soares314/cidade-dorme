using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Jogador
{
    public string NomeJogador;
    public Node ClasseJogador;

    public Jogador(string nome, Node classe)
    {
        NomeJogador = nome;
        ClasseJogador = classe;
    }
}

public partial class MontadorJogo : ScrollContainer
{

    private HBoxContainer EspaçoAdicionarJogador;
    private TabContainer TabsClasse;
    private TabContainer ClasDisponiveis;
    private PackedScene CenaEscolhaClasse = GD.Load<PackedScene>("res://Classes Obtidas Tela/classes_obtidas_popup.tscn");
    public List<Jogador> Jogadores = new List<Jogador>();
    public List<Dictionary<PackedScene, int>> Classes = new List<Dictionary<PackedScene, int>>();

    public override void _Ready()
    {
        EspaçoAdicionarJogador = GetNode<HBoxContainer>("Abas/Aba Adicionar Jogador");
        TabsClasse = GetNode<TabContainer>("Abas/Aba Classes Possiveis");
        ClasDisponiveis = GetNode<TabContainer>("Abas/Aba Classes Possiveis");

        foreach (GridContainer claLinha in ClasDisponiveis.GetChildren())
        {
            Classes.Add(new Dictionary< PackedScene, int>());
        }

        foreach (Node classeTab in TabsClasse.GetChildren())
        {
            TextureButton botaoAdicionaClasse = classeTab.GetChild(0) as TextureButton;
            botaoAdicionaClasse.Pressed += () => OnBotãoAdicionarClassePressed(botaoAdicionaClasse);
        }
    }

    private void OnBotãoAdicionarJogadorPressed()
    {
        VBoxContainer novoPerfil = new VBoxContainer();
        novoPerfil.CustomMinimumSize = new Vector2(128, 0);
        novoPerfil.AddThemeConstantOverride("separation", -20);

        TextureRect fotoPerfil = new TextureRect();
        fotoPerfil.Texture = GD.Load<Texture2D>("res://Sprites/generic_player_pic.png");
        fotoPerfil.ExpandMode = TextureRect.ExpandModeEnum.FitHeight;
        fotoPerfil.StretchMode = TextureRect.StretchModeEnum.KeepAspect;

        TextEdit nomeEditavelPerfil = new TextEdit();
        nomeEditavelPerfil.Text = "Jogador " + (Jogadores.Count + 1);
        nomeEditavelPerfil.CustomMinimumSize = new Vector2(0, 40);
        nomeEditavelPerfil.AddThemeFontSizeOverride("font_size", 20);

        novoPerfil.AddChild(fotoPerfil);
        novoPerfil.AddChild(nomeEditavelPerfil);

        Jogadores.Add(new Jogador(nomeEditavelPerfil.Text, null));

        EspaçoAdicionarJogador.AddChild(novoPerfil);
    }

    private void OnBotãoAdicionarClassePressed(Node emissor)
    {

        ClassesObtidasPopup popupEscolhaClasse = CenaEscolhaClasse.Instantiate<ClassesObtidasPopup>();
        AddChild(popupEscolhaClasse);
        ClassesObtidas classesObtidas = popupEscolhaClasse.GetNode<ClassesObtidas>("VBoxContainer/Classes Obtidas");

        for (int i = 0; i < classesObtidas.GetChildCount(); i++)
        {
            Button adicionClasseEspecifica = classesObtidas.GetChild(i).GetChild(0) as Button;
            adicionClasseEspecifica.ButtonUp += () => OnBotãoAdicionarClasseEspButtonUp(adicionClasseEspecifica, emissor.GetParent());
        }

    }

    private void OnBotãoAdicionarClasseEspButtonUp(Node emissor, Node claClasse)
    {

        PackedScene cenaClasse = emissor.GetMeta("cenaClasse").AsGodotObject() as PackedScene;
        Aldeão classeDaLista = cenaClasse.Instantiate<Aldeão>();

        PanelContainer containerCompleto = classeDaLista.GerarIconeClasse() as PanelContainer;
        VBoxContainer iconeAdicionarClasse = containerCompleto.GetChild(0) as VBoxContainer;

        CheckButton checkButton = new CheckButton();
        checkButton.Text = "Aleatorio";
        checkButton.CustomMinimumSize = new Vector2(20, 20);
        checkButton.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
        checkButton.AddThemeColorOverride("font_color", Colors.Black);

        SpinBox quantidadeUmaClasse = new SpinBox();
        quantidadeUmaClasse.Alignment = HorizontalAlignment.Center;
        quantidadeUmaClasse.MinValue = 0;
        quantidadeUmaClasse.MaxValue = 99;
        quantidadeUmaClasse.Value = 1;
        quantidadeUmaClasse.Step = 1;
        quantidadeUmaClasse.CustomMinimumSize = new Vector2(60, 30);
        quantidadeUmaClasse.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
        quantidadeUmaClasse.ValueChanged += (double value) => OnQuantidadeClassesValueChanged(cenaClasse, claClasse, value);

        Node iconeOriginal = iconeAdicionarClasse.GetChild(0);
        Label nomeOriginal = iconeAdicionarClasse.GetChild(1) as Label;
        iconeAdicionarClasse.RemoveChild(iconeOriginal);
        iconeAdicionarClasse.RemoveChild(nomeOriginal);

        nomeOriginal.AddThemeColorOverride("font_color", Colors.Black);

        iconeAdicionarClasse.AddChild(checkButton);
        iconeAdicionarClasse.AddChild(iconeOriginal);
        iconeAdicionarClasse.AddChild(nomeOriginal);
        iconeAdicionarClasse.AddChild(quantidadeUmaClasse);

        claClasse.AddChild(containerCompleto);

        (emissor as Control).Visible = false;
    }

    private void OnQuantidadeClassesValueChanged(PackedScene classe, Node claClasse, double value)
    {
        Dictionary<PackedScene, int> claDaClasse = Classes[claClasse.GetIndex()];

        claDaClasse[classe] = (int)value;
    }

    private void OnMontarJogoPressed()
    {
        //foreach(Dictionary<PackedScene, int> classesDoCla )
    }
}
