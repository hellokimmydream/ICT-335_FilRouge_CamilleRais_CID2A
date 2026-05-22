using c335_fil_rouge_CamilleRais.Models;

namespace c335_fil_rouge_CamilleRais
{
    public partial class DetailPage : ContentPage, IQueryAttributable
    {
        // deck sur lequel on est
        private Deck? _deck;

        // indx de la carte affichée
        private int _currentIndex = 0;

        // True = on voit la réponse / false = on voit la question
        private bool _isFlipped = false;

        public DetailPage()
        {
            InitializeComponent();
        }

        // recoit deck depuis Decks.xaml ou Accueil.xaml
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("deck", out object? deckObj) && deckObj is Deck deck)
            {
                _deck = deck;
                DeckNameLabel.Text = deck.Name;

                _currentIndex = 0;
                ShowCurrentCard();
            }
        }

        // affiche toujours la carte sur la face question
        private void ShowCurrentCard()
        {
            // si pas de cartes affiche un message vide
            if (_deck == null || _deck.Cards.Count == 0)
            {
                QuestionLabel.Text = "Aucune carte";
                AnswerLabel.Text = "Ajoute-en avec ➕ sur la page Decks";
                CardCounterLabel.Text = "Carte 0 / 0";
                _isFlipped = false;
                FrontFace.IsVisible = true;
                BackFace.IsVisible = false;
                return;
            }

            // quand on a fini la pile se remet à 0
            if (_currentIndex >= _deck.Cards.Count)
            {
                _currentIndex = 0;
            }

            Card card = _deck.Cards[_currentIndex];
            QuestionLabel.Text = card.Question;
            AnswerLabel.Text = card.Answer;
            CardCounterLabel.Text = $"Carte {_currentIndex + 1} / {_deck.Cards.Count}";

            // remet toujours la face question
            _isFlipped = false;
            FrontFace.IsVisible = true;
            BackFace.IsVisible = false;
        }

        // FLIP tapez sur la carte pour voir l autre coté avec la réponse 
        private void OnFlipTapped(object sender, TappedEventArgs evenementFlip)
        {
            if (_deck == null || _deck.Cards.Count == 0) return;

            _isFlipped = !_isFlipped;
            FrontFace.IsVisible = !_isFlipped;
            BackFace.IsVisible = _isFlipped;
        }

        // btn précédent
        private void OnPreviousClicked(object sender, EventArgs evenementFlip)
        {
            if (_deck == null || _deck.Cards.Count == 0) return;

            // si on est à la 1ere on revient à la dernière
            _currentIndex = (_currentIndex - 1 + _deck.Cards.Count) % _deck.Cards.Count;
            ShowCurrentCard();
        }

        // btn suivant
        private void OnNextClicked(object sender, EventArgs evenementFlip)
        {
            if (_deck == null || _deck.Cards.Count == 0) return;

            // si on est à la dernière on revient à la 1ere
            _currentIndex = (_currentIndex + 1) % _deck.Cards.Count;
            ShowCurrentCard();
        }
    }
}