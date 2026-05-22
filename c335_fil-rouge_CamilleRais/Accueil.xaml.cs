using c335_fil_rouge_CamilleRais.Models;
using c335_fil_rouge_CamilleRais.Services;
using System.Collections.ObjectModel;

namespace c335_fil_rouge_CamilleRais
{
    public partial class Accueil : ContentPage
    {
        private JsonDataService _dataService;
        private ObservableCollection<Deck> _decks;
        private int _nextId = 1;

        public Accueil()
        {
            InitializeComponent();
            _dataService = new JsonDataService();
            _decks = new ObservableCollection<Deck>();
            DecksCollectionView.ItemsSource = _decks;
        }

        // Recharge la liste à chaque retour sur la page
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
        }

        // tap sur un deck = ouvre direct la page etudier
        private async void OnDeckTapped(object sender, TappedEventArgs eventTap)
        {
            Deck? deck = eventTap.Parameter as Deck;
            if (deck == null) return;

            Dictionary<string, object> navigationParameter = new Dictionary<string, object>
    {
        { "deck", deck },
        { "decks", _decks }
    };

            await Shell.Current.GoToAsync("detail", navigationParameter);
        }

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
        }
    }
}