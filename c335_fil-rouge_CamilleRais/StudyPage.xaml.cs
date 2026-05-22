using System.Diagnostics;
using c335_fil_rouge_CamilleRais.Models;
// using pour utiliser les capteurs
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Devices;

namespace c335_fil_rouge_CamilleRais
{
    public partial class StudyPage : ContentPage, IQueryAttributable
    {
        // data du deck en cours
        private Deck? deck;

        // pile de cartes à voir aléatoire
        // une carte non connue retourne dans le deck
        private List<Card> remaining = new();
        private Card? currentCard;

        // resume
        // nombre d'erreurs par carte (clé = Id de la carte)
        private Dictionary<int, int> errorsPerCard = new();

        // set des cartes déjà vues au moins une fois pour  decompte total
        private HashSet<int> seenCardIds = new();

        // chronomètre du temps total de la session
        private Stopwatch sessionStopwatch = new();

        private bool isFlipped = false;

        // event ShakeDetected peut spammer pendant 1seconde
        private DateTime lastShakeTime = DateTime.MinValue;

        public StudyPage()
        {
            InitializeComponent();
        }

        // récupère le deck passé via la nav Shell
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("deck", out object? deckObj) && deckObj is Deck deckParam)
            {
                deck = deckParam;
                DeckNameLabel.Text = deck.Name;
                StartSession();
            }
        }

        // accelerometre
        // active le capteur QUE quand on arrive sur la page et coupe quand on quitte
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Accelerometer.Default.IsSupported)
            {
                if (!Accelerometer.Default.IsMonitoring)
                {
                    // s'abonner à l'événement de secouss / default n'est pas assez réactif
                    // tuto cours
                    Accelerometer.Default.ShakeDetected += AccelerometerShakeDetected;
                    Accelerometer.Default.Start(SensorSpeed.UI);
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // desabonner couper le capteur sinon il continue à tourner en fond
            // tuto cours
            if (Accelerometer.Default.IsSupported && Accelerometer.Default.IsMonitoring)
            {
                Accelerometer.Default.Stop();
                Accelerometer.Default.ShakeDetected -= AccelerometerShakeDetected;
            }

            // arrêter le chrono si l'utilisateur quitte la page d un coup
            if (sessionStopwatch.IsRunning)
                sessionStopwatch.Stop();
        }

        // observateur appelé par MAUI quand secousse est détectée
        private void AccelerometerShakeDetected(object? sender, EventArgs e)
        {
            // l'événement arrive sur un thread secondaire alors bascule sur le thread ui
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // si plus de cartes ou pas de carte courante = ignore
                if (currentCard == null) return;

                MarkCurrentCardAsUnknown();
            });
        }

        // logique pour la révision comme dans CDC
        // prépare une nouvelle session : mélange les cartes et démarre le chrono
        private void StartSession()
        {
            if (deck == null || deck.Cards.Count == 0)
            {
                QuestionLabel.Text = "Aucune carte";
                AnswerLabel.Text = "Ce deck est vide";
                KnowButton.IsEnabled = false;
                ProgressLabel.Text = "Restantes : 0 | Connues : 0";
                return;
            }

            // copie + melange aléatoire Random.Shared 
            remaining = deck.Cards.OrderBy(_ => Random.Shared.Next()).ToList();
            errorsPerCard.Clear();
            seenCardIds.Clear();
            sessionStopwatch.Reset();
            sessionStopwatch.Start();

            ShowNextCard();
        }

        // tire la carte du dessus du deck et l'affiche
        private void ShowNextCard()
        {
            if (remaining.Count == 0)
            {
                // plus de cartes = fin révision
                EndSession();
                return;
            }

            // prend la dernière de la liste (pile)
            currentCard = remaining[^1];
            remaining.RemoveAt(remaining.Count - 1);

            // marque la carte comme vue (pour le décompte)
            seenCardIds.Add(currentCard.Id);

            // affiche la question, face cachée par défaut
            QuestionLabel.Text = currentCard.Question;
            AnswerLabel.Text = currentCard.Answer;
            isFlipped = false;
            FrontFace.IsVisible = true;
            BackFace.IsVisible = false;

            UpdateProgressLabel();
        }

        private void UpdateProgressLabel()
        {
            // nombre de cartes connues à 100% = cartes vues sans aucune erreur
            int known100 = seenCardIds.Count(id => !errorsPerCard.ContainsKey(id));
            ProgressLabel.Text = $"Restantes : {remaining.Count} | Connues : {known100}";
        }

        // l'utilisateur clique "Je connais"
        private void OnKnowClicked(object sender, EventArgs e)
        {
            if (currentCard == null) return;

            // on ne remet PAS la carte dans la pile → elle est validée
            ShowNextCard();
        }

        // l'utilisateur secoue le téléphone → carte non connue
        private void MarkCurrentCardAsUnknown()
        {
            if (currentCard == null) return;

            // incrémenter le compteur d'erreurs pour cette carte
            if (errorsPerCard.ContainsKey(currentCard.Id))
                errorsPerCard[currentCard.Id]++;
            else
                errorsPerCard[currentCard.Id] = 1;

            // remettre la carte dans la pile, à une position aléatoire
            // pour qu'elle revienne plus tard et pas tout de suite
            if (remaining.Count > 0)
            {
                int insertPos = Random.Shared.Next(0, remaining.Count);
                remaining.Insert(insertPos, currentCard);
            }
            else
            {
                // s'il n'y a plus que cette carte, on la remet directement
                remaining.Add(currentCard);
            }

            ShowNextCard();
        }

        // bouton stop : on quitte et on va au résumé
        private async void OnStopClicked(object sender, EventArgs e)
        {
            // si l'utilisateur n'a vu aucune carte, juste retour arrière
            if (seenCardIds.Count == 0)
            {
                await Shell.Current.GoToAsync("..");
                return;
            }

            EndSession();
        }

        // fin de la session : on calcule les stats et on navigue vers le résumé
        private async void EndSession()
        {
            sessionStopwatch.Stop();

            // calcul des stats
            TimeSpan elapsed = sessionStopwatch.Elapsed;
            int totalCards = deck?.Cards.Count ?? 0;

            // carte connue à 100% = vue mais aucune erreur dessus
            int knownPerfectly = seenCardIds.Count(id => !errorsPerCard.ContainsKey(id));

            // pourcentage de mémorisation = parfaites / total
            double memorizationPercent = totalCards == 0
                ? 0
                : (knownPerfectly * 100.0) / totalCards;

            // carte la plus difficile = celle avec le plus d'erreurs
            string hardestCardText = "Aucune (tout connu du premier coup !)";
            if (errorsPerCard.Count > 0)
            {
                int hardestId = errorsPerCard.OrderByDescending(kv => kv.Value).First().Key;
                Card? hardest = deck?.Cards.FirstOrDefault(c => c.Id == hardestId);
                if (hardest != null)
                {
                    hardestCardText = $"« {hardest.Question} » ({errorsPerCard[hardestId]} erreurs)";
                }
            }

            // nav vers resume
            var navigationParameter = new Dictionary<string, object>
            {
                { "elapsed", elapsed },
                { "hardestCard", hardestCardText },
                { "knownPerfectly", knownPerfectly },
                { "totalCards", totalCards },
                { "memorizationPercent", memorizationPercent }
            };

            await Shell.Current.GoToAsync("studySummary", navigationParameter);
        }

        // flip
        private void OnFlipTapped(object sender, TappedEventArgs e)
        {
            if (currentCard == null) return;

            isFlipped = !isFlipped;
            FrontFace.IsVisible = !isFlipped;
            BackFace.IsVisible = isFlipped;
        }
    }
}