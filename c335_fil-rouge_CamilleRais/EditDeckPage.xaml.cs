/******************************************************************************
** PROGRAMME  EditDeckPage.xaml.cs                                           **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Camille Rais                                                  **
** Date      : 18.03.2026                                                    **
**                                                                           **
******************************************************************************/

/******************************************************************************
** DESCRIPTION                                                               **
**                                                                           **
** Page pour modifier le nom d'un deck.                                      **
**                                                                           **
******************************************************************************/

using c335_fil_rouge_CamilleRais.Models;
using c335_fil_rouge_CamilleRais.Services;
using System.Collections.ObjectModel;

namespace c335_fil_rouge_CamilleRais
{
    public partial class EditDeckPage : ContentPage, IQueryAttributable
    {
        private Deck? _deck;
        private JsonDataService? _dataService;
        private ObservableCollection<Deck>? _decks;

        public EditDeckPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Receive navigation parameters
        /// </summary>
        /// <param name="query"></param>
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("deck", out object? deckObj) && deckObj is Deck deck)
            {
                _deck = deck;

                // Initialize fields
                NameEntry.Text = deck.Name;
            }

            if (query.TryGetValue("dataService", out object? serviceObj)
                && serviceObj is JsonDataService service)
            {
                _dataService = service;
            }

            if (query.TryGetValue("decks", out object? decksObj)
                && decksObj is ObservableCollection<Deck> decks)
            {
                _decks = decks;
            }
        }

        /// <summary>
        /// sauvegarde le nouveau nom puis retour
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventEdit"></param>
        private async void OnSaveClicked(object sender, EventArgs eventEdit)
        {
            if (_deck == null || _dataService == null || _decks == null) return;

            string? newName = NameEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(newName))
            {
                await DisplayAlert("Erreur", "Le nom ne peut pas être vide", "OK");
                return;
            }

            // Update deck
            _deck.Name = newName;

            // Save immediately to JSON
            await _dataService.SaveDecksAsync(_decks.ToList());

            await Shell.Current.GoToAsync("..");
        }

        /// <summary>
        /// annule et retour
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventEdit"></param>
        private async void OnCancelClicked(object sender, EventArgs eventEdit)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}