using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

public class Jogador
{
    public string NomeJogador;
    public Aldeão ClasseJogador;

    public Jogador(string nome, Aldeão classe)
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
    private Button BotaoMontarJogo;
    private PackedScene CenaEscolhaClasse = GD.Load<PackedScene>("res://Classes Obtidas Tela/classes_obtidas_popup.tscn");
    public PackedScene JogoVaiMontar = GD.Load<PackedScene>("res://Jogo/jogo.tscn");
    private List<Jogador> Jogadores = new List<Jogador>();
    public List<Jogador> JogadoresMortos = new List<Jogador>();
    public List<Dictionary<PackedScene, int>> Classes = new List<Dictionary<PackedScene, int>>();
    bool BotaoMontarEstaConectado = false;

    public override void _Ready()
    {
        EspaçoAdicionarJogador = GetNode<HBoxContainer>("Abas/Aba Adicionar Jogador");
        TabsClasse = GetNode<TabContainer>("Abas/Aba Classes Possiveis");
        ClasDisponiveis = GetNode<TabContainer>("Abas/Aba Classes Possiveis");
        BotaoMontarJogo = GetNode<Button>("Abas/Montar Jogo");

        foreach (GridContainer claLinha in ClasDisponiveis.GetChildren())
        {
            Classes.Add(new Dictionary<PackedScene, int>());
        }

        foreach (Node classeTab in TabsClasse.GetChildren())
        {
            TextureButton botaoAdicionaClasse = classeTab.GetChild(0) as TextureButton;
            botaoAdicionaClasse.Pressed += () => OnBotãoAdicionarClassePressed(botaoAdicionaClasse);
        }
    }

    public int QuantosElementosListaDicionario(List<Dictionary<PackedScene, int>> Lista)
    {
        int QuantElementosDicionario = 0;
        foreach (Dictionary<PackedScene, int> cla in Lista)
        {
            foreach (KeyValuePair<PackedScene, int> classeNoCla in cla)
            {
                QuantElementosDicionario += classeNoCla.Value;
            }
        }

        return QuantElementosDicionario;
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
        quantidadeUmaClasse.Value = 0;
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

    public override void _Process(double delta)
    {
        bool podeMontar = Jogadores.Count > 0 && Classes.Any(clasDaClasse => clasDaClasse.Count != 0) && Jogadores.Count <= QuantosElementosListaDicionario(Classes);
        //GD.Print(podeMontar);
        if (podeMontar && !BotaoMontarEstaConectado)
        {
            BotaoMontarEstaConectado = true;
            BotaoMontarJogo.ButtonUp += OnMontarJogoPressed;
            BotaoMontarJogo.AddThemeStyleboxOverride("normal", GD.Load<StyleBox>("res://Resources/StyleBoxes/ClicavelEstilizado.tres"));
        }
        else
            if (!podeMontar && BotaoMontarEstaConectado)
        {
            BotaoMontarEstaConectado = false;
            BotaoMontarJogo.ButtonUp -= OnMontarJogoPressed;
            BotaoMontarJogo.RemoveThemeStyleboxOverride("normal");
        }
        
    }

    public void AtribuaClasseAleatoriaParaCadaJogador()
    {
        int quanClasseClaTotal = QuantosElementosListaDicionario(Classes);
        int quanClasseClaAux = 0;
        Dictionary<PackedScene, int> classesDoCla;

        while (quanClasseClaTotal > 0 && Jogadores.Any(i => i.ClasseJogador == null))
        {
            while (true)
            {
                int numClaAlea = -1;
                if (Classes.Count > 0)
                {
                    numClaAlea = (int)(GD.Randi() % Classes.Count);
                    //GD.Print($"Numero aleatorio do Clã: {numClaAlea}, Tipos de Classes no clã: {Classes[numClaAlea].Count}");
                    if (Classes[numClaAlea].Count > 0)
                    {
                        classesDoCla = Classes[numClaAlea];
                        break;
                    }
                    else
                    {
                        Classes.RemoveAt(numClaAlea);
                    }
                }
            }

            int quanClasseCla = 0;
            foreach (KeyValuePair<PackedScene, int> classeNoCla in classesDoCla)
            {
                quanClasseCla += classeNoCla.Value;
            }

            int numClasseAlea = (int)(GD.Randi() % quanClasseCla) + 1;
            //GD.Print($"Quantidade Classes no cla: {quanClasseCla}, Numero aleatorio da classe: {numClasseAlea}");

            foreach (KeyValuePair<PackedScene, int> classeNoCla in classesDoCla)
            {
                quanClasseClaAux += classeNoCla.Value;
                if (numClasseAlea <= quanClasseClaAux)
                {
                    classesDoCla[classeNoCla.Key] = classesDoCla[classeNoCla.Key] - 1;
                    quanClasseClaTotal--;
                    quanClasseCla--;
                    //GD.Print($"Quantidade dessa classe no clã {classesDoCla[classeNoCla.Key]}");
                    if (classesDoCla[classeNoCla.Key] == 0)
                        classesDoCla.Remove(classeNoCla.Key);

                    Aldeão novaClasseInstanciada = classeNoCla.Key.Instantiate<Aldeão>();
                    novaClasseInstanciada.ClaClasse = ClasDisponiveis.GetChild(Classes.IndexOf(classesDoCla)).Name;

                    while (true)
                    {
                        int numJogadorAlea = (int)(GD.Randi() % Jogadores.Count);
                        //GD.Print($"Quantidade DE jogadores: {Jogadores.Count}, Numero aleatorio do jogador: {numJogadorAlea}\n");
                        if (Jogadores[numJogadorAlea].ClasseJogador == null)
                        {
                            Jogadores[numJogadorAlea].ClasseJogador = novaClasseInstanciada;
                            Jogadores[numJogadorAlea].ClasseJogador.JogadorUsuario = Jogadores[numJogadorAlea];
                            break;
                        }
                    }

                    quanClasseClaAux = 0;
                    break;
                }
            }
        }

        foreach (Jogador jogador in Jogadores)
        {
            //GD.Print($"Jogador: {jogador.NomeJogador}, Classe:{jogador.ClasseJogador.Name}, Clã: {jogador.ClasseJogador.ClaClasse}");
        }
    }

    private void OnMontarJogoPressed()
    {
        AtribuaClasseAleatoriaParaCadaJogador();

        Jogo jogoaMontar = JogoVaiMontar.Instantiate<Jogo>();

        foreach (Jogador jogador in Jogadores)
        {
            jogoaMontar.Jogadores.Add(jogador);
            jogoaMontar.AddChild(jogador.ClasseJogador);
        }
        GetTree().Root.AddChild(jogoaMontar);
        GetTree().CurrentScene.QueueFree();
        GetTree().CurrentScene = jogoaMontar;

    }
}
