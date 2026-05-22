namespace c335_fil_rouge_CamilleRais {
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        private async void OnRetourClicked(object sender, EventArgs ecouterObject)
        {
            await Shell.Current.GoToAsync("//Accueil");
        }
    }

}