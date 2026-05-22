using c335_fil_rouge_CamilleRais.Models;
using c335_fil_rouge_CamilleRais.Services;
using System.Collections.ObjectModel;

namespace c335_fil_rouge_CamilleRais
{
    public partial class Decks : ContentPage
    {
        private JsonDataService _dataService;
        private ObservableCollection<Deck> _decks;
        private int _nextId = 1;

        public Decks()
        {
            InitializeComponent();
            _dataService = new JsonDataService();
            _decks = new ObservableCollection<Deck>();
            DecksCollectionView.ItemsSource = _decks;
            LoadDecks();
        }

        // Recharge decks depuis JSON à chaque retour sur la page
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadDecks();
        }

        private async void LoadDecks()
        {
            List<Deck> loadedDecks = await _dataService.LoadDecksAsync();

            _decks.Clear();
            foreach (Deck deck in loadedDecks)
            {
                _decks.Add(deck);
            }

            // calcule le prochain ID
            if (_decks.Any())
            {
                _nextId = _decks.Max(d => d.Id) + 1;
            }
            else
            {
                _nextId = 1;
            }

            //UpdateInfo($"{_decks.Count} deck(s) chargé(s)");
        }

        //private void UpdateInfo(string message)
        //{
        //    InfoLabel.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
        //}

        // CREATE rapide depuis entry
        private async void OnAddDeckClicked(object sender, EventArgs e)
        {
            string? name = NewDeckEntry.Text?.Trim();

            if (string.IsNullOrEmpty(name))
            {
                await DisplayAlert("Erreur", "Veuillez entrer un nom", "OK");
                return;
            }

            Deck newDeck = new Deck
            {
                Id = _nextId++,
                Name = name,
                CardCount = 0
            };

            _decks.Add(newDeck);
            await _dataService.SaveDecksAsync(_decks.ToList());

            NewDeckEntry.Text = string.Empty;
            //UpdateInfo($"Ajouté : {name}");
        }

        // UPDATE : navvers page de détail
        private async void OnEditDeckClicked(object sender, EventArgs e)
        {
            Button? button = sender as Button;
            Deck? deck = button?.CommandParameter as Deck;

            if (deck == null) return;

            Dictionary<string, object> navigationParameter = new Dictionary<string, object>
            {
                { "deck", deck },
                { "dataService", _dataService },
                { "decks", _decks }
            };
            await Shell.Current.GoToAsync("EditDeck", navigationParameter);
        }

        // DELETE
        private async void OnDeleteDeckClicked(object sender, EventArgs e)
        {
            Button? button = sender as Button;
            Deck? deck = button?.CommandParameter as Deck;

            if (deck == null) return;

            bool confirm = await DisplayAlert(
                "Confirmation",
                $"Voulez-vous vraiment supprimer '{deck.Name}' ?",
                "Supprimer",
                "Annuler"
            );

            if (!confirm) return;

            _decks.Remove(deck);
            await _dataService.SaveDecksAsync(_decks.ToList());

            //UpdateInfo($"Supprimé : {deck.Name}");
        }

        // naviguer vers la page pour étudier des cartes avec flip
        private async void OnStudyDeckClicked(object sender, EventArgs e)
        {
            Button? button = sender as Button;
            Deck? deck = button?.CommandParameter as Deck;

            if (deck == null) return;

            Dictionary<string, object> navigationParameter = new Dictionary<string, object>
            {
                { "deck", deck },
                { "decks", _decks }
            };

            await Shell.Current.GoToAsync("detail", navigationParameter);
        }

        // nav vers mode d'apprentissage avec secousse
        private async void OnLearnDeckClicked(object sender, EventArgs e)
        {
            Button? button = sender as Button;
            Deck? deck = button?.CommandParameter as Deck;

            if (deck == null) return;

            if (deck.Cards.Count == 0)
            {
                await DisplayAlert("Deck vide",
                    "Ajoute des cartes avant de lancer l'apprentissage.", "OK");
                return;
            }

            Dictionary<string, object> navigationParameter = new Dictionary<string, object>
    {
        { "deck", deck }
    };

            await Shell.Current.GoToAsync("study", navigationParameter);
        }

        // ouvre la page gestion des cartes
        private async void OnManageCardsClicked(object sender, EventArgs e)
        {
            Button? button = sender as Button;
            Deck? deck = button?.CommandParameter as Deck;

            if (deck == null) return;

            Dictionary<string, object> navigationParameter = new Dictionary<string, object>
    {
        { "deck", deck },
        { "decks", _decks }
    };

            await Shell.Current.GoToAsync("manageCards", navigationParameter);
        }
    }
}