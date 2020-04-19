using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionshubJokAPI.Models;
using Joke = LionshubJoker.Joker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LionshubJokAPI.Services
{
    public class JokerService
    {
        private readonly ITableService _tableService;
        private readonly IGamerService _gamerService;

        private readonly DbService _context = null;
        public static Joke.PlayGame play;
        public Joke.Game game;
        public List<Joke.RoundsAndGamers> rounds;

        private List<Joke.Card> deckOfCard;

        public JokerService(ITableService tableService, IGamerService gamerService)
        {
            _tableService = tableService;
            _gamerService = gamerService;

            //play = new PlayGame(gamers, deckOfCard);
        }

        public bool GeneratePlay(string tableID)
        {
            bool res = true;
            List<Models.Gamer> modelGamers = new List<Models.Gamer>();
            Models.Table table = _tableService.Get(tableID);
            var gamers = _gamerService.GetGamersOnTable(tableID);

            var playGamers = new List<LionshubJoker.Joker.Gamer>();
            LionshubJoker.Joker.Table playTable = new LionshubJoker.Joker.Table();
            if (gamers.Count == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    playGamers.Add(new LionshubJoker.Joker.Gamer(i + 1, gamers[i].Name, playTable));
                }
                if (table.Name == Joke.GameType.Standard.ToString())
                {
                    game = new Joke.Game(Joke.GameType.Standard, playGamers);
                }
                else if (table.Name == Joke.GameType.Nines.ToString())
                {
                    game = new Joke.Game(Joke.GameType.Nines, playGamers);
                }
                else
                {
                    game = new Joke.Game(Joke.GameType.Ones, playGamers);
                }
                rounds = game.LoadGame();
                play = new Joke.PlayGame(playGamers);
                res = true;
            }
            return res;
        }

        public Joke.PlayGame StartPlay(Models.RoundsAndGamers round)
        {

            Joke.RoundsAndGamers roundsAndGamers = new Joke.RoundsAndGamers
            {
                Hand = round.handRound,
                CurrentGamer = play.Gamers.Where(p => p.Id == round.GamerID).FirstOrDefault()
            };
            play.StartRound(roundsAndGamers.Hand);
            play.CurrentGamer = roundsAndGamers.CurrentGamer;
            play.CurrentGamer.AllowCardsForTable();

            return play;
        }

        public Joke.PlayGame GetPlayState()
        {
            return play;
        }
    }
}
