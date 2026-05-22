namespace c335_fil_rouge_CamilleRais.Models
{
    public class Deck
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int CardCount { get; set; }

        // NOUVEAU : la liste des cartes du deck
        public List<Card> Cards { get; set; } = new List<Card>();

        public Deck()
        {
            CreatedDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Name} ({CardCount} cartes)";
        }
    }
}