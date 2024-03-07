using BatteshipLiteLibrary;
using BatteshipLiteLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipLite
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Welcoming();
            PlayerInfoModel activePlayer = CreatePlayer("Player 1");
            PlayerInfoModel opponent = CreatePlayer("Player 2");
            PlayerInfoModel winner = null;

            do
            {
                //Display grid
                DisplayShotGrid(activePlayer);
                //Ask active player for a shot
                //Determine if it is a valid shot
                //Determine  shot results
                RecordPlayerShot(activePlayer, opponent);
                //Determine if the game is over
                bool doesGameContinue = GameLogic.PlayerStillActive(opponent);
                //If yes , set active player as the winner
                //else , swap positions (activeplayer to opponent)
                if(doesGameContinue)
                {
                    //Using Tuple;
                    (activePlayer, opponent) = (opponent, activePlayer);
                }
                else
                {
                     winner = activePlayer;
                    
                }
            } while (winner == null);
            IdentifyWinner(winner);

            Console.ReadLine();
        }

        private static void IdentifyWinner(PlayerInfoModel winner)
        {
            Console.WriteLine($"Congratulations to {winner.UserName} for winning!");
            //How many shots thewinner took from opponent;
            Console.WriteLine($"{winner.UserName} took {GameLogic.GetShotCount(winner)} shots");
        }

        private static void RecordPlayerShot(PlayerInfoModel activePlayer, PlayerInfoModel opponent)
        {
            //Asks for a shot (we ask for "B2")
            //Determine what row and column that is - split apart
            //Determine if that is a valid shot
            //Go back to the beggining if its not a valid shot
            bool isValidShot = false;
            string row = "";
            int column = 0;
            do
            {
                string shot = AskForShot(activePlayer);
                try
                {
                    (row, column) = GameLogic.SplitShotIntoRowAndColumn(shot);
                    isValidShot = GameLogic.ValidateShot(activePlayer, row, column);
                }
                catch (Exception ex)
                {
                    
                    isValidShot = false;
                }

                if(!isValidShot )
                    Console.WriteLine("Invalid shot location. Please try again");

            } while (!isValidShot);
            //Determine shot results
            //Record results
            bool isAHit = GameLogic.IndentifyShotResult(opponent,row,column);
            GameLogic.MarkShotResult(activePlayer, row, column , isAHit);
            DisplayShotResults(row,column, isAHit);
        }

        private static void DisplayShotResults(string row, int column, bool isAHit)
        {
            if (isAHit)
                Console.WriteLine($"{row}{column}, is a Hit!");
            else
                Console.WriteLine($"{row}{column}, is a Miss");
            Console.WriteLine();
        }

        private static string AskForShot(PlayerInfoModel player)
        {
            Console.Write($"{player.UserName}, Please enter your shot selection: ");
            string shot = Console.ReadLine();
            return shot;
        }

        private static void DisplayShotGrid(PlayerInfoModel activePlayer)
        {
            string currentRow = activePlayer.ShotGrid[0].SpotLetter;
            foreach(var gridSpot in activePlayer.ShotGrid)
            {
                if(gridSpot.SpotLetter != currentRow)
                {
                    Console.WriteLine();
                    currentRow = gridSpot.SpotLetter;
                }
                if(gridSpot.Status == GridSpotStatus.Empty)
                {
                    Console.Write($" {gridSpot.SpotLetter}{gridSpot.SpotNumber} ");
                }
                else if(gridSpot.Status == GridSpotStatus.Hit)
                {
                    Console.Write(" X ");
                }
                else if(gridSpot.Status == GridSpotStatus.Miss)
                {
                    Console.Write(" O ");
                }
                else
                {
                    Console.Write(" ? ");
                }
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void Welcoming()
        {
            Console.WriteLine("Welcome to the BattleshipLite APP");
            Console.WriteLine("created by Pedro Wang");
            Console.WriteLine();
        }
        private static PlayerInfoModel CreatePlayer(string playerTitle)
        {
            Console.WriteLine($"Player information for {playerTitle}");
            PlayerInfoModel output = new PlayerInfoModel();
            //Ask for the username
            output.UserName = AskForTheUsersName();
            //Load up the shot grid
            GameLogic.InitializeGrid(output);
            //Ask the user for the 5 ship placements
            PlaceShips(output);
            //Clear
            Console.Clear();
            return output;
        }
        private static string AskForTheUsersName()
        {
            Console.Write("What is your name: ");
            string name = Console.ReadLine();
            return name;
        }
        private static void PlaceShips(PlayerInfoModel model)
        {
            do
            {
                Console.Write($"Where do you want to place the ship number {model.ShipLocations.Count +1}: ");
                string location = Console.ReadLine();

                bool isValidOutput = false;

                try
                {
                    isValidOutput = GameLogic.StoreShip(model, location);
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);
                    isValidOutput = false;
                }

                if (!isValidOutput)
                    Console.WriteLine("That was not a valid location. Please try again.");

            } while (model.ShipLocations.Count < 5);
        }
    }
}
