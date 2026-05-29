/******************************************************************************
** PROGRAMME  AppShell.xaml.cs                                               **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Camille Rais                                                  **
** Date      : 18.03.2026                                                    **
**                                                                           **
******************************************************************************/

/******************************************************************************
** DESCRIPTION                                                               **
**                                                                           **
** Gère la navigation de l'app.                                              **
** Enregistre les routes vers les différentes pages.                         **
**                                                                           **
******************************************************************************/

namespace c335_fil_rouge_CamilleRais
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // routes pour la navigation
            // ne se fait pas automatiquement pour les pages
            // qui ne sont pas déclarées dans AppShell.xaml
            Routing.RegisterRoute("settings", typeof(SettingsPage));
            Routing.RegisterRoute("detail", typeof(DetailPage));
            Routing.RegisterRoute("EditDeck", typeof(EditDeckPage));
            Routing.RegisterRoute("manageCards", typeof(ManageCardsPage));
            Routing.RegisterRoute("studySummary", typeof(StudySummaryPage));

            // nouvelles routes lors de mise en place des capteurs
            Routing.RegisterRoute("study", typeof(StudyPage));
            Routing.RegisterRoute("studySummary", typeof(StudySummaryPage));

        }
    }
}