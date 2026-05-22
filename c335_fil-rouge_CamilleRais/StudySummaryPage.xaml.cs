namespace c335_fil_rouge_CamilleRais
{
    public partial class StudySummaryPage : ContentPage, IQueryAttributable
    {
        public StudySummaryPage()
        {
            InitializeComponent();
        }

        // récupère les stats envoyées par StudyPage
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // temps passé
            if (query.TryGetValue("elapsed", out var elapsedObj) && elapsedObj is TimeSpan elapsed)
            {
                // format mm:ss ou hh:mm:ss si plus d une heure
                // calcul le temps passé sur le quizz
                if (elapsed.TotalHours >= 1)
                {
                    TimeLabel.Text = $"{(int)elapsed.TotalHours}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
                }
                else
                {
                    TimeLabel.Text = $"{elapsed.Minutes}:{elapsed.Seconds:D2}";
                }
            }

            // calcul les carte la plus difficile
            if (query.TryGetValue("hardestCard", out var hardestObj) && hardestObj is string hardest)
            {
                HardestCardLabel.Text = hardest;
            }

            // calcul cartes connues a 100%
            int knownPerfectly = 0;
            int totalCards = 0;

            if (query.TryGetValue("knownPerfectly", out var knownObj) && knownObj is int known)
            {
                knownPerfectly = known;
            }

            if (query.TryGetValue("totalCards", out var totalObj) && totalObj is int tot)
            {
                totalCards = tot;
            }

            KnownLabel.Text = $"{knownPerfectly} / {totalCards}";

            // %
            if (query.TryGetValue("memorizationPercent", out var pctObj) && pctObj is double pct)
            {
                PercentLabel.Text = $"{pct:F0} %";
            }
        }

        // btn retour : on revient à la liste des decks
        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//decks");
        }
    }
}