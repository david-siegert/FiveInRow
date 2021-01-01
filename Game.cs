using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FiveInRow
{
    class Game : IDisposable
    {
        private static readonly int MaxWaitTime = 10 * 60 * 1000; // 10 minutes

        private static readonly HttpClient client = new HttpClient();
        private static readonly string userId = ConfigurationManager.AppSettings.Get("userId");
        private static readonly string userToken = ConfigurationManager.AppSettings.Get("userToken");

        public string GameId { get; set; }
        public string GameToken { get; set; }

        public string StatusCode { get; set; }
        public string ActualPlayerId { get; set; }
        public string WinnerId { get; set; }
        public List<Coordinates> Coordinates { get; set; }

        public bool MyTurn
        {
            get
            {
                return ActualPlayerId == userId;
            }
        }

        public bool Victory
        {
            get
            {
                return WinnerId == userId;
            }
        }

        public Game()
        {
        }


        // these 3 methods return true if POST is succes
        private async Task<bool> StartGame()
        {
            //string json = "{  \"userToken\": \"67625002-8c4a-49a1-81d8-ce7caebba3ec\"}";

            string json = JsonConvert.SerializeObject(new { userToken });

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://piskvorky.jobs.cz/api/v1/connect", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var definition = new { gameToken = "", gameId = "", statusCode = "" };

                var result = JsonConvert.DeserializeAnonymousType(responseString, definition);

                if (result.statusCode == "201")
                {
                    GameId = result.gameId;
                    GameToken = result.gameToken;
                    return true;
                }
            }

            return false;
        }
        private async Task<bool> GetStatus()
        {
            string json = JsonConvert.SerializeObject(new { userToken = userToken, gameToken = GameToken });

            int counter = 0;

            while (counter < 50)
            {
                counter++;

                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://piskvorky.jobs.cz/api/v1/checkStatus", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var definition = new
                    {
                        statusCode = "",
                        actualPlayerId = "",
                        winnerId = "",
                        coordinates = new List<Coordinates> { }
                    };

                    var result = JsonConvert.DeserializeAnonymousType(responseString, definition);

                    StatusCode = result.statusCode;
                    ActualPlayerId = result.actualPlayerId;
                    WinnerId = result.winnerId;
                    Coordinates = result.coordinates;

                    return true;
                }
            }

            return false;
        }
        private async Task<bool> Play(int x, int y)
        {
            string json = JsonConvert.SerializeObject(new
            {
                userToken,
                gameToken = GameToken,
                positionX = x,
                positionY = y,
            });

            int counter = 0;

            while (counter < 50)
            {
                counter++;

                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://piskvorky.jobs.cz/api/v1/play", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var definition = new { statusCode = "" };

                    var result = JsonConvert.DeserializeAnonymousType(responseString, definition);

                    StatusCode = result.statusCode;

                    return true;
                }
            }

            return false;
        }


        public bool StartGameSafe()
        {
            Task<bool> startTask = StartGame();
            startTask.Wait(MaxWaitTime);
            
            if (!startTask.IsCompleted || !startTask.Result)
            {
                Console.WriteLine("Failed starting the game");
                return false;
            }
            
            return true;
        }
        public bool GetStatusSafe()
        {
            Task<bool> statusTask = GetStatus();
            statusTask.Wait(MaxWaitTime);
            
            if (!statusTask.IsCompleted || !statusTask.Result)
            {
                Console.WriteLine("Failed getting status of the game");
                return false;
            }
            
            return true;
        }
        public bool PlaySafe(XY xy)
        {
            var playTask = Play(xy.X, xy.Y);
            playTask.Wait(MaxWaitTime);

            if (!playTask.IsCompleted || !playTask.Result)
            {
                Console.WriteLine("Failed sending coordinates");
                return false;
            }

            return true;
        }

        public void Dispose()
        {

        }
    }
}
