using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using AthleteBuilder.Model;
using CsvHelper;
using Microsoft.Win32;

namespace AthleteBuilder
{
    public partial class MainWindow : Window
    {
        public string FileName { get; private set; }
        public IList<Athlete> Athletes { get; } = new List<Athlete>();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenDialog(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|XML files (*.xml)|*.xml"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                FileName = openFileDialog.FileName;
            }

            using var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(stream);
            using var csv = new CsvReader(sr, CultureInfo.InvariantCulture);
            csv.Configuration.PrepareHeaderForMatch = (header, index) => header.ToLower();

            var record = new Record();

            var records = csv.EnumerateRecords(record);
            foreach (var r in records)
            {
                var newAthlete = false;
                Athlete currentAthlete;
                if (!Athletes.Any(p => p.FirstName == r.FirstName && p.LastName == r.LastName))
                {
                    currentAthlete = new Athlete
                    {
                        BirthYear = r.BirthYear,
                        Club = r.Club,
                        FirstName = r.FirstName,
                        LastName = r.LastName,
                        Malus = r.Malus,
                        Sexe = r.Sexe,
                    };
                    newAthlete = true;
                }
                else
                {
                    currentAthlete = Athletes.FirstOrDefault(p => p.FirstName == r.FirstName && p.LastName == r.LastName);
                }

                var rank = CalculateRank(r);

                if (currentAthlete == null)
                    throw new Exception();

                MapDisciplinePoint(r, currentAthlete, rank);

                if (newAthlete)
                    Athletes.Add(currentAthlete);
            }

            foreach (var athlete in Athletes)
            {
                //TODO : Add malus (-10 pts)
                var champVsIndoor = new List<int>
                {
                    athlete.ChampVsIndoor.FiftyMeterHurdlePoints,
                    athlete.ChampVsIndoor.FiftyMeterPoints,
                    athlete.ChampVsIndoor.HighJumpPoints,
                    athlete.ChampVsIndoor.LongJumpPoints,
                    athlete.ChampVsIndoor.PolePoints,
                    athlete.ChampVsIndoor.ShotPutPoints
                };
                athlete.ChampVsIndoor.ChampVsIndoorTotal = CalculateMax4Discipline(champVsIndoor);
                var champVsOutdoor = new List<int>
                {
                    athlete.ChampVsOutdoor.HighJumpPoints,
                    athlete.ChampVsOutdoor.PolePoints,
                    athlete.ChampVsOutdoor.LongJumpPoints,
                    athlete.ChampVsOutdoor.DiscusPoints,
                    athlete.ChampVsOutdoor.EighthyMeterPoints,
                    athlete.ChampVsOutdoor.HundredMeterHurdlePoints,
                    athlete.ChampVsOutdoor.JavelinPoints,
                    athlete.ChampVsOutdoor.ShotPutPoints,
                    athlete.ChampVsOutdoor.SixHundredMeterPoints,
                    athlete.ChampVsOutdoor.TwoThousandMeterPoints
                };
                athlete.ChampVsOutdoor.ChampVsOutdoorTotal = CalculateMax4Discipline(champVsOutdoor);
            }

            lvUsers.ItemsSource = Athletes.OrderByDescending(p => p.ChampVsIndoor.ChampVsIndoorTotal);
        }

        private static int CalculateMax4Discipline(IEnumerable<int> allPoints)
        {
            return allPoints.OrderByDescending(p => p).Take(4).Sum();
        }

        private int CalculateRank(Record r)
        {
            return r.Discipline switch
            {
                201 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.ChampVsIndoor.FiftyMeterPoints > 0),
                202 => Athletes.Count(p =>
                    p.BirthYear == r.BirthYear && p.ChampVsIndoor.FiftyMeterHurdlePoints > 0),
                203 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.ChampVsIndoor.HighJumpPoints > 0),
                204 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.ChampVsIndoor.LongJumpPoints > 0),
                205 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.ChampVsIndoor.ShotPutPoints > 0),
                206 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.ChampVsIndoor.PolePoints > 0),
                100 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.ChampVsMultiple.ChampVsMultipleTotal > 0),
                300 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.TourneeCross.TourneeCrossTotal > 0),
                400 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.FinaleSprint.FinaleSprintTotal > 0),
                500 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.FinaleGruyere.FinaleGruyereTotal > 0),
                601 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.ChampVsOutdoor.EighthyMeterPoints > 0),
                602 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.ChampVsOutdoor.HundredMeterHurdlePoints > 0),
                603 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.ChampVsOutdoor.SixHundredMeterPoints > 0),
                604 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.ChampVsOutdoor.TwoThousandMeterPoints > 0),
                605 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.ChampVsOutdoor.HighJumpPoints > 0),
                606 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.ChampVsOutdoor.PolePoints > 0),
                607 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.ChampVsOutdoor.LongJumpPoints > 0),
                608 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.ChampVsOutdoor.ShotPutPoints > 0),
                609 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.ChampVsOutdoor.JavelinPoints > 0),
                610 => Athletes.Count(p => p.BirthYear == r.BirthYear && p.ChampVsOutdoor.DiscusPoints > 0),
                _ => 0
            };
        }

        private static void MapDisciplinePoint(Record record, Athlete currentAthlete, int rank)
        {
            switch (record.Discipline)
            {
                case 100:
                    currentAthlete.ChampVsMultiple.ChampVsMultipleTotal = currentAthlete.ChampVsMultiple.MaxPoints - rank;
                    break;
                case 201:
                    currentAthlete.ChampVsIndoor.FiftyMeterPoints = currentAthlete.ChampVsIndoor.MaxPoints - rank;
                    break;
                case 202:
                    currentAthlete.ChampVsIndoor.FiftyMeterHurdlePoints = currentAthlete.ChampVsIndoor.MaxPoints - rank;
                    break;
                case 203:
                    currentAthlete.ChampVsIndoor.HighJumpPoints = currentAthlete.ChampVsIndoor.MaxPoints - rank;
                    break;
                case 204:
                    currentAthlete.ChampVsIndoor.LongJumpPoints = currentAthlete.ChampVsIndoor.MaxPoints - rank;
                    break;
                case 205:
                    currentAthlete.ChampVsIndoor.ShotPutPoints = currentAthlete.ChampVsIndoor.MaxPoints - rank;
                    break;
                case 206:
                    currentAthlete.ChampVsIndoor.PolePoints = currentAthlete.ChampVsIndoor.MaxPoints - rank;
                    break;
                case 300:
                    currentAthlete.TourneeCross.TourneeCrossTotal = currentAthlete.TourneeCross.MaxPoints - rank;
                    break;
                case 400:
                    currentAthlete.FinaleSprint.FinaleSprintTotal = currentAthlete.FinaleSprint.MaxPoints - rank;
                    break;
                case 500:
                    currentAthlete.FinaleGruyere.FinaleGruyereTotal = currentAthlete.ChampVsIndoor.MaxPoints - rank;
                    break;
                case 601:
                    currentAthlete.ChampVsOutdoor.EighthyMeterPoints = currentAthlete.ChampVsIndoor.MaxPoints - rank;
                    break;
                case 602:
                    currentAthlete.ChampVsOutdoor.HundredMeterHurdlePoints = currentAthlete.ChampVsIndoor.MaxPoints - rank;
                    break;
                case 603:
                    currentAthlete.ChampVsOutdoor.SixHundredMeterPoints = currentAthlete.ChampVsIndoor.MaxPoints - rank;
                    break;
                case 604:
                    currentAthlete.ChampVsOutdoor.TwoThousandMeterPoints = currentAthlete.ChampVsIndoor.MaxPoints - rank;
                    break;
                case 605:
                    currentAthlete.ChampVsOutdoor.HighJumpPoints = currentAthlete.ChampVsIndoor.MaxPoints - rank;
                    break;
                case 606:
                    currentAthlete.ChampVsOutdoor.PolePoints = currentAthlete.ChampVsIndoor.MaxPoints - rank;
                    break;
                case 607:
                    currentAthlete.ChampVsOutdoor.LongJumpPoints = currentAthlete.ChampVsIndoor.MaxPoints - rank;
                    break;
                case 608:
                    currentAthlete.ChampVsOutdoor.ShotPutPoints = currentAthlete.ChampVsIndoor.MaxPoints - rank;
                    break;
                case 609:
                    currentAthlete.ChampVsOutdoor.JavelinPoints = currentAthlete.ChampVsIndoor.MaxPoints - rank;
                    break;
                case 610:
                    currentAthlete.ChampVsOutdoor.DiscusPoints = currentAthlete.ChampVsIndoor.MaxPoints - rank;
                    break;
            }
        }

        private void ExportCsv(object sender, RoutedEventArgs e)
        {
            using var writer = new StreamWriter(@"C:\temp\athlete.csv");
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(Athletes);
            writer.Flush();
            MessageBox.Show("Exported as CSV");
        }
    }
}
