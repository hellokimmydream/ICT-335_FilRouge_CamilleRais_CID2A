/******************************************************************************
** PROGRAMME  DetailPage.xaml.cs                                             **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Camille Rais                                                  **
** Date      : 18.03.2026                                                    **
**                                                                           **
******************************************************************************/

/******************************************************************************
** DESCRIPTION                                                               **
**                                                                           **
** Page qui affiche les cartes d'un deck une par une.                        **
** On peut taper sur la carte pour voir la réponse (flip).                   **
** Boutons précédent / suivant pour passer d'une carte à l'autre.            **
**                                                                           **
******************************************************************************/

using c335_fil_rouge_CamilleRais.Models;

namespace c335_fil_rouge_CamilleRais
{
    /// <summary>
    /// page qui affiche les cartes d'un deck
    /// </summary>
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

        /// <summary>
        /// recoit deck depuis Decks.xaml ou Accueil.xaml
        /// </summary>
        /// <param name="query"></param>
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

        /// <summary>
        /// affiche toujours la carte sur la face question
        /// </summary>
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

        /// <summary>
        /// FLIP tapez sur la carte pour voir l autre coté avec la réponse 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="evenementFlip"></param>
        private void OnFlipTapped(object sender, TappedEventArgs evenementFlip)
        {
            if (_deck == null || _deck.Cards.Count == 0) return;

            _isFlipped = !_isFlipped;
            FrontFace.IsVisible = !_isFlipped;
            BackFace.IsVisible = _isFlipped;
        }

        /// <summary>
        /// btn précédent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="evenementFlip"></param>
        private void OnPreviousClicked(object sender, EventArgs evenementFlip)
        {
            if (_deck == null || _deck.Cards.Count == 0) return;

            // si on est à la 1ere on revient à la dernière
            _currentIndex = (_currentIndex - 1 + _deck.Cards.Count) % _deck.Cards.Count;
            ShowCurrentCard();
        }

        /// <summary>
        /// btn suivant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="evenementFlip"></param>
        private void OnNextClicked(object sender, EventArgs evenementFlip)
        {
            if (_deck == null || _deck.Cards.Count == 0) return;

            // si on est à la dernière on revient à la 1ere
            _currentIndex = (_currentIndex + 1) % _deck.Cards.Count;
            ShowCurrentCard();
        }
    }
}