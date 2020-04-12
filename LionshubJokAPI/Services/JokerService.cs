using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionshubJokAPI.Models;
using LionshubJoker.Joker;
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
        public PlayGame play;
        public List<CardsOnRound> rounds;

        private readonly List<Card> deckOfCard;

        public JokerService(ITableService tableService, IGamerService gamerService)
        {
            _tableService = tableService;
            _gamerService = gamerService;
            DeckOfCardCreator deckOfCardCreator = new DeckOfCardCreator();
            // კარტის დასტა
            deckOfCard = new List<Card>();
            deckOfCard.AddRange(deckOfCardCreator.CreateDeckOfCards());
            //play = new PlayGame(gamers, deckOfCard);
        }

        public void GeneratePlay(string tableID)
        {

            List<Models.Gamer> modelGamers = new List<Models.Gamer>();
            Models.Table table = _tableService.Get(tableID);

            Game game = null;
            if (table.Name == GameType.Standard.ToString())
            {
                game = new Game(GameType.Standard);
                rounds = game.LoadGame();
            }
            else if (table.Name == GameType.Nines.ToString())
            {
                game = new Game(GameType.Nines);
            }
            else
            {
                game = new Game(GameType.Ones);
            }
            rounds = game.LoadGame();

            var gamers = _gamerService.GetGamersOnTable(tableID);
            LionshubJoker.Joker.Table playTable = new LionshubJoker.Joker.Table();
            var playGamers = new List<LionshubJoker.Joker.Gamer>();
            if (gamers.Count == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    playGamers.Add(new LionshubJoker.Joker.Gamer(i + 1, gamers[i].Name, playTable));
                }
                play = new PlayGame(playGamers, deckOfCard);
            }
        }

        public PlayGame StartPlay(CardsOnRound round)
        {
            play.StartRound(round);
            play.CurrentGamer.AllowCardsForTable();
            return play;
        }
    }
}
