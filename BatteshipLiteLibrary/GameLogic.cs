using BatteshipLiteLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BatteshipLiteLibrary
{
    public class GameLogic
    {
        public static int GetShotCount(PlayerInfoModel player)
        {
            int count = 0;
            foreach(var shot in player.ShotGrid)
            {
                if (shot.Status != GridSpotStatus.Empty)
                    count += 1;
            }
            return count;
        }

        public static bool IndentifyShotResult(PlayerInfoModel opponent, string row, int column)
        {
            bool isAHit = false;
            foreach (var ship in opponent.ShipLocations)
            {
                if (row.ToUpper() == ship.SpotLetter && column == ship.SpotNumber)
                {
                    isAHit = true;
                    ship.Status = GridSpotStatus.Sunk;
                }
                    
            }
            return isAHit;
        }

        public static void InitializeGrid(PlayerInfoModel model)
        {
            List<string> letters = new List<string>{ "A", "B", "C", "D", "E" };
            List<int> numbers = new List<int> { 1,2,3,4,5};

            foreach(string letter in letters)
            {
                foreach(int number in numbers)
                {
                    AddGridSpot(model, letter, number);
                }
            }
        }

        public static void MarkShotResult(PlayerInfoModel player, string row, int column, bool isAHit)
        {
            foreach (var gridSpot in player.ShotGrid)
            {
                if (row.ToUpper() == gridSpot.SpotLetter && column == gridSpot.SpotNumber)
                {
                    if (isAHit)
                        gridSpot.Status = GridSpotStatus.Hit;
                    else
                        gridSpot.Status = GridSpotStatus.Miss;
                }  
            }
            
            
        }

        public static bool PlayerStillActive(PlayerInfoModel player)
        {
            foreach(var ship in player.ShipLocations)
            {
                if (ship.Status != GridSpotStatus.Sunk)
                    return true;
            }
            return false;
        }

        public static (string row, int column) SplitShotIntoRowAndColumn(string shot)
        {
            char[] shotArray = shot.ToCharArray();

            if (shotArray.Length != 2)
                throw new ArgumentException("This is an invalid shot type", "shot");

            string row = shotArray[0].ToString();
            int column = int.Parse(shotArray[1].ToString());

            return (row, column);
        }

        public static bool StoreShip(PlayerInfoModel model, string location)
        {
            (string row, int column) = SplitShotIntoRowAndColumn(location);

            bool isValidGrid = ValidateGridLocation(model, row, column);
            bool isSpotOpen = ValidateShipLocation(model, row, column);
            if (isValidGrid && isSpotOpen)
            {
                model.ShipLocations.Add(new GridSpotModel()
                {
                    SpotLetter = row.ToUpper(),
                    SpotNumber = column,
                    Status = GridSpotStatus.Ship,
                });
                return true;
            }
            return false;

        }

        private static bool ValidateShipLocation(PlayerInfoModel model, string row, int column)
        {
            foreach(var ship in model.ShipLocations)
            {
                if (row.ToUpper() == ship.SpotLetter && column == ship.SpotNumber)
                    return false;
            }
            return true;
        }

        private static bool ValidateGridLocation(PlayerInfoModel model, string row, int column)
        {
            foreach(var grid in model.ShotGrid)
            {
                if (row.ToUpper() == grid.SpotLetter && column == grid.SpotNumber)
                    return true;
            }
            return false;
        }

        public static bool ValidateShot(PlayerInfoModel player, string row, int column)
        {
            foreach (var gridSpot in player.ShotGrid)
            {
                if (row.ToUpper() == gridSpot.SpotLetter && column == gridSpot.SpotNumber)
                {
                    if (gridSpot.Status == GridSpotStatus.Empty)
                        return true;
                }
            }
            return false;
        }

        private static void AddGridSpot(PlayerInfoModel model, string letter, int number)
        {
            GridSpotModel spot = new GridSpotModel { SpotLetter = letter, SpotNumber = number, Status = GridSpotStatus.Empty };
            model.ShotGrid.Add(spot);
        }

    }
}
