using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Tennis.Models;
using System.Threading.Tasks;

namespace Tennis.Controllers
{
    public class AbboController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Abbo abbo)
        {
            //Todo: Check for less than four!
            String[] playerNames = abbo.GroupInput.Split("; ");
            foreach (string playerName in playerNames)
            {
                Player player = new() {Name = playerName};
                abbo.Group.Enqueue(player);
            }

            //Todo: Must have same length - Not good solution!
            String[] reservationDays = abbo.ReservationDayInput.Split("; ");
            String[] numberSingleGames = abbo.NumberOfSingleGamesInput.Split("; ");
            String[] numberDoubleGames = abbo.NumberOfDoubleGamesInput.Split("; ");

            //Todo: Aux-Function
            DayOfWeek weekDay = abbo.StartDate.DayOfWeek;
            int diffStartWeekDayToMonday = (7 + (weekDay - DayOfWeek.Monday)) % 7;
            DateTime tmpDate = abbo.StartDate.AddDays(-diffStartWeekDayToMonday);
            
            //Todo: Check If really correct
            do
            {
                for (int i = 0; i < reservationDays.Length; i++)
                {
                    Reservation reservation = new()
                    {
                        //Todo: Try and Catch by parse!
                        NumberSingleGames = int.Parse(numberSingleGames[i]),
                        NumberDoubleGames = int.Parse(numberDoubleGames[i]),
                        WeekDayEnum = (WeekDay) Enum.Parse(typeof(WeekDay), reservationDays[i]),
                    };

                    reservation.Date = tmpDate.AddDays((int) reservation.WeekDayEnum);

                    //Todo: Unit Tests would be nice
                    reservation.NumberOfPlayers = reservation.NumberDoubleGames > 0
                        ? reservation.NumberSingleGames * 2 +
                          (reservation.NumberDoubleGames * 4 - reservation.NumberSingleGames * 2)
                        : reservation.NumberSingleGames * 2;

                    abbo.Reservations.Add(reservation);
                }

                tmpDate = tmpDate.AddDays(7);
            } while (tmpDate <= abbo.EndDate);


            foreach (Reservation reservation in abbo.Reservations)
            {
                List<Player> players = new List<Player>();

                for (int i = 0; i < reservation.NumberOfPlayers; i++)
                {
                    Player player = abbo.Group.Dequeue();
                    players.Add(player);
                }

                //Todo Shuffle List for more randomness
                players.ForEach(p => abbo.Group.Enqueue(p));
                reservation.Players = new Queue<Player>(players);
            }

            foreach (Reservation reservation in abbo.Reservations)
            {
                for (int i = 0; i < reservation.NumberSingleGames; i++)
                {
                    Player player1 = reservation.Players.Dequeue();
                    Player player2 = reservation.Players.Dequeue();

                    GameSingle gameSingle = new()
                    {
                        Player1 = player1,
                        Player2 = player2
                    };

                    reservation.Games.Add(gameSingle);
                    reservation.Players.Enqueue(player2);
                    reservation.Players.Enqueue(player1);
                }

                for (int i = 0; i < reservation.NumberDoubleGames; i++)
                {
                    // Really not pretty
                    Player player1 = reservation.Players.Dequeue();
                    Player player2 = reservation.Players.Dequeue();
                    Player player3 = reservation.Players.Dequeue();
                    Player player4 = reservation.Players.Dequeue();

                    GameDouble gameDouble = new()
                    {
                        Team1 = (player1, player2),
                        Team2 = (player3, player4)
                    };
                    
                    reservation.Games.Add(gameDouble);
                    reservation.Players.Enqueue(player2);
                    reservation.Players.Enqueue(player1);
                    reservation.Players.Enqueue(player4);
                    reservation.Players.Enqueue(player3);
                }
            }

            foreach (Reservation reservation in abbo.Reservations)
            {
                Console.WriteLine($"{reservation.Date} ");

                foreach (Game game in reservation.Games)
                {
                    Console.WriteLine(game.ToString());
                }

                Console.WriteLine();
            }

            WriteToExcel(abbo);
            return View("Index");
        }

        public async Task WriteToExcel(Abbo abbo)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var file = new FileInfo("/Users/kristian.dubek/Desktop/Tennis/Tennis.xlsx");

            await SaveExcel(abbo, file);
        }

        private static async Task SaveExcel(Abbo abbo, FileInfo file)
        {
            if (file.Exists)
            {
                file.Delete();
            }

            using (var package = new ExcelPackage(file))
            {
                var workSheet = package.Workbook.Worksheets.Add("Tennis");

                for (int i = 0; i < abbo.Reservations.Count; i++)
                {
                    workSheet.Cells[$"A{i + 1}"].Value = abbo.Reservations[i].Date;
                    workSheet.Cells[$"A{i + 1}"].Style.Numberformat.Format = "dd.mm.yy";
                    
                    foreach (var game in abbo.Reservations[i].Games)
                    {
                        // Todo: Wird zurzeit immer überschrieben, ab in die nächste Spalte damit! 
                        workSheet.Cells[$"B{i + 1}"].Value = game.ToString();
                    }
                }
                await package.SaveAsync();
            }
        }
    }
}