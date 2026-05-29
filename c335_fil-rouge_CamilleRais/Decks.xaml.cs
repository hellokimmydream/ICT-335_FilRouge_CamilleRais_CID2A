/******************************************************************************
** PROGRAMME  Decks.xaml.cs                                                  **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Camille Rais                                                  **
** Date      : 18.03.2026                                                    **
**                                                                           **
******************************************************************************/

/******************************************************************************
** DESCRIPTION                                                               **
**                                                                           **
** Page qui liste tous mes decks.                                            **
** Permet d'ajouter, modifier, supprimer un deck : CRUD.                     **
** Donne aussi accès à la gestion des cartes et au mode étudier.             **
**                                                                           **
******************************************************************************/

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

        /// <summary>
        /// constructeur de la page
        /// </summary>
        public Decks()
        {
            InitializeComponent();
            _dataService = new JsonDataService();
            _decks = new ObservableCollection<Deck>();
            DecksCollectionView.ItemsSource = _decks;
            LoadDecks();
        }

        /// <summary>
        /// Recharge decks depuis JSON à chaque retour sur la page
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadDecks();
        }

        /// <summary>
        /// charge la liste des decks depuis le JSON
        /// </summary>
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

        /// <summary>
        /// CREATE rapide depuis entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventDeck"></param>
        private async void OnAddDeckClicked(object sender, EventArgs eventDeck)
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

        /// <summary>
        /// UPDATE : navvers page de détail
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventDeck"></param>
        private async void OnEditDeckClicked(object sender, EventArgs eventDeck)
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

        /// <summary>
        /// DELETE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventDeck"></param>
        private async void OnDeleteDeckClicked(object sender, EventArgs eventDeck)
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
        }

        /// <summary>
        /// naviguer vers la page pour étudier des cartes avec flip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventDeck"></param>
        private async void OnStudyDeckClicked(object sender, EventArgs eventDeck)
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

        /// <summary>
        /// nav vers mode d'apprentissage avec secousse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventDeck"></param>
        private async void OnLearnDeckClicked(object sender, EventArgs eventDeck)
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

        /// <summary>
        /// ouvre la page gestion des cartes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventDeck"></param>
        private async void OnManageCardsClicked(object sender, EventArgs eventDeck)
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