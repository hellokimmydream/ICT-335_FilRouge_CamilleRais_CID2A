/******************************************************************************
** PROGRAMME  ProfilePage.xaml.cs                                            **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Camille Rais                                                  **
** Date      : 18.03.2026                                                    **
**                                                                           **
******************************************************************************/

/******************************************************************************
** DESCRIPTION                                                               **
**                                                                           **
** Page profil avec un bouton retour vers l'accueil.                         **
**                                                                           **
******************************************************************************/

namespace c335_fil_rouge_CamilleRais
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// bouton retour vers l'accueil
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ecouterObject"></param>
        private async void OnRetourClicked(object sender, EventArgs ecouterObject)
        {
            await Shell.Current.GoToAsync("//Accueil");
        }
    }

}