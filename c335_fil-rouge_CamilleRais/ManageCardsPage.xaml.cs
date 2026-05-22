using c335_fil_rouge_CamilleRais.Models;
using c335_fil_rouge_CamilleRais.Services;
using System.Collections.ObjectModel;

namespace c335_fil_rouge_CamilleRais
{
    public partial class ManageCardsPage : ContentPage, IQueryAttributable
    {
        // deck dont on gère les cartes
        private Deck? _deck;

        // service pour sauvegarder dans le JSON
        private JsonDataService _dataService;

        // liste complète des decks (pour la sauvegarde)
        private ObservableCollection<Deck>? _decks;

        // liste affichée dans la CollectionView
        private ObservableCollection<Card> _cards;

        // pour donner des Id uniques aux nouvelles cartes
        private int _nextCardId = 1;

        public ManageCardsPage()
        {
            InitializeComponent();
            _dataService = new JsonDataService();
            _cards = new ObservableCollection<Card>();
            CardsCollectionView.ItemsSource = _cards;
        }

        // Réception des paramètres depuis Decks.xaml
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // recup le deck
            if (query.TryGetValue("deck", out object? deckObj) && deckObj is Deck deck)
            {
                _deck = deck;
                DeckNameLabel.Text = $"Cartes de : {deck.Name}";

                // affiche cartes existantes
                _cards.Clear();
                foreach (Card card in _deck.Cards)
                {
                    _cards.Add(card);
                }

                // calcule prochain Id de carte
                if (_deck.Cards.Any())
                {
                    _nextCardId = _deck.Cards.Max(c => c.Id) + 1;
                }
                else
                {
                    _nextCardId = 1;
                }
            }

            // recup la liste complète des decks
            if (query.TryGetValue("decks", out object? decksObj)
                && decksObj is ObservableCollection<Deck> decks)
            {
                _decks = decks;
            }
        }

        // CREATE add new cartes
        private async void OnAddCardClicked(object sender, EventArgs e)
        {
            if (_deck == null) return;

            string? question = NewQuestionEntry.Text?.Trim();
            string? answer = NewAnswerEntry.Text?.Trim();

            if (string.IsNullOrEmpty(question) || string.IsNullOrEmpty(answer))
            {
                await DisplayAlert("Erreur", "Question et réponse obligatoires", "OK");
                return;
            }

            // crée la nouvelle carte
            Card newCard = new Card
            {
                Id = _nextCardId++,
                Question = question,
                Answer = answer
            };

            // ajoute au deck ET à la liste affichée
            _deck.Cards.Add(newCard);
            _cards.Add(newCard);
            _deck.CardCount = _deck.Cards.Count;

            // sauvegarde
            await SaveAsync();

            // vide les champ
            NewQuestionEntry.Text = string.Empty;
            NewAnswerEntry.Text = string.Empty;
        }

        // DELETE
        private async void OnDeleteCardClicked(object sender, EventArgs e)
        {
            Button? button = sender as Button;
            Card? card = button?.CommandParameter as Card;

            if (card == null || _deck == null) return;

            bool confirm = await DisplayAlert(
                "Confirmation",
                $"Supprimer cette carte ?\n\nQ: {card.Question}",
                "Supprimer",
                "Annuler"
            );

            if (!confirm) return;

            _deck.Cards.Remove(card);
            _cards.Remove(card);
            _deck.CardCount = _deck.Cards.Count;

            await SaveAsync();
        }

        // UPDATE
        private async void OnEditCardClicked(object sender, EventArgs e)
        {
            Button? button = sender as Button;
            Card? card = button?.CommandParameter as Card;

            if (card == null || _deck == null) return;

            // demande la nouvelle question avec l'ancienne pré-remplie
            string newQuestion = await DisplayPromptAsync(
                "Modifier la question",
                "Nouvelle question :",
                "OK",
                "Annuler",
                initialValue: card.Question
            );

            // si l'utilisateur annule
            if (string.IsNullOrWhiteSpace(newQuestion)) return;

            // demande la nouvelle réponse
            string newAnswer = await DisplayPromptAsync(
                "Modifier la réponse",
                "Nouvelle réponse :",
                "OK",
                "Annuler",
                initialValue: card.Answer
            );

            if (string.IsNullOrWhiteSpace(newAnswer)) return;

            // met à jour la carte
            card.Question = newQuestion.Trim();
            card.Answer = newAnswer.Trim();

            // sauvegarde
            await SaveAsync();

            // rafraîchit l'affichage
            // retire et remet pour forcer la mise à jour
            int index = _cards.IndexOf(card);
            _cards.RemoveAt(index);
            _cards.Insert(index, card);
        }

        // sauvegarde tout dans le JSON
        private async Task SaveAsync()
        {
            if (_decks != null)
            {
                await _dataService.SaveDecksAsync(_decks.ToList());
            }
        }
    }
}