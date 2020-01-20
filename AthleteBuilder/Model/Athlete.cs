namespace AthleteBuilder.Model
{
    public class Athlete
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int BirthYear { get; set; }
        public string Club { get; set; }
        public string Sexe { get; set; }
        public bool Malus { get; set; }
        public ChampVsIndoor ChampVsIndoor { get; } = new ChampVsIndoor();
        public TourneeCross TourneeCross { get; } = new TourneeCross();
        public FinaleGruyere FinaleGruyere { get; } = new FinaleGruyere();
        public FinaleSprint FinaleSprint { get; } = new FinaleSprint();
        public ChampVsOutdoor ChampVsOutdoor { get; } = new ChampVsOutdoor();
        public ChampVsMultiple ChampVsMultiple { get; } = new ChampVsMultiple();
    }
}