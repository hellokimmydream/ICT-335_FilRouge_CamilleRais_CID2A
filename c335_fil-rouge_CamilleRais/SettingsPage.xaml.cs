/******************************************************************************
** PROGRAMME  SettingsPage.xaml.cs                                           **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Camille Rais                                                  **
** Date      : 18.03.2026                                                    **
**                                                                           **
******************************************************************************/

/******************************************************************************
** DESCRIPTION                                                               **
**                                                                           **
** Page des paramètres de l'app.                                             **
**                                                                           **
******************************************************************************/

namespace c335_fil_rouge_CamilleRais
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// sauvegarde et retour
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventClick"></param>
        private async void OnSauvegarderClicked(object sender, EventArgs eventClick)
        {
            await Shell.Current.GoToAsync("..");
        }

        /// <summary>
        /// annule et retour
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventClick"></param>
        private async void OnAnnulerClicked(object sender, EventArgs eventClick)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}