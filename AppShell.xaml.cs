namespace HelloMaui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute("RaceSelection", typeof(Views.HeroCreation.RaceSelectionPage));
        // Регистрация шагов вкладок чтобы позволить прямой переход с параметром draftId
        Routing.RegisterRoute("HeroStepRace", typeof(Views.HeroCreation.RaceSelectionPage));
        Routing.RegisterRoute("HeroStepBackground", typeof(Views.HeroCreation.BackgroundSelectionPage));
        Routing.RegisterRoute("HeroStepProfOrSurv", typeof(Views.HeroCreation.ProfessionOrSurvivalPage));
        Routing.RegisterRoute("HeroStepHobbies", typeof(Views.HeroCreation.HobbySelectionPage));
        Routing.RegisterRoute("HeroStepAttributes", typeof(Views.HeroCreation.AttributesPage));
	}
}
