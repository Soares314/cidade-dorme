using Godot;
using System;

public partial class ClassesObtidas : GridContainer
{
	public override void _Ready()
	{
		using var classesArquivo = FileAccess.Open("res://Salvamento/Classes Obtidas.txt", FileAccess.ModeFlags.Read);
		string[] listaClasses = classesArquivo.GetAsText().Split("\n");

		foreach (string classe in listaClasses)
		{
			if (string.IsNullOrWhiteSpace(classe))
				continue;

			PackedScene cenaClasse = GD.Load<PackedScene>(classe);
			Aldeão classeDaLista = cenaClasse.Instantiate<Aldeão>();

			MarginContainer margemClasse = new MarginContainer();
			margemClasse.AddThemeConstantOverride("margin_bottom", 5);
			margemClasse.AddThemeConstantOverride("margin_left", 5);
			margemClasse.AddThemeConstantOverride("margin_right", 5);
			margemClasse.AddThemeConstantOverride("margin_top", 5);

			Button botaoClasse = new Button();
			botaoClasse.CustomMinimumSize = new Vector2(100, 100);
			botaoClasse.SetMeta("cenaClasse", cenaClasse);

			AddChild(margemClasse);
			Node noDoGrid = GetChild(GetChildCount() - 1);
			noDoGrid.AddChild(botaoClasse);
			noDoGrid.GetChild(0).AddChild(classeDaLista.GerarIconeClasse());

		}
	}


}
