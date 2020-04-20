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
        public static List<Joker> jokers = new List<Joker>();
        public Joke.Game game;

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
                var rounds = game.LoadGame();
                Joker joker = new Joker();
                joker.rounds = new List<RoundsAndGamers>();
                foreach (LionshubJoker.Joker.RoundsAndGamers item in rounds)
                {
                    joker.rounds.Add(new Models.RoundsAndGamers
                    {
                        GamerID = item.CurrentGamer.Id,
                        handRound = item.Hand
                    });
                }
                joker.play = new Joke.PlayGame(playGamers);
                joker.TableID = tableID;
                jokers.Add(joker);
                res = true;
            }
            return res;
        }

        public Joke.PlayGame StartPlay(Models.RoundsAndGamers round, string tableID)
        {

            Joke.RoundsAndGamers roundsAndGamers = new Joke.RoundsAndGamers
            {
                Hand = round.handRound,
                CurrentGamer = jokers.Where(p => p.TableID == tableID).FirstOrDefault().play.Gamers.Where(p => p.Id == round.GamerID).FirstOrDefault()
            };
            jokers.Where(p => p.TableID == tableID).FirstOrDefault().play.StartRound(roundsAndGamers.Hand);
            jokers.Where(p => p.TableID == tableID).FirstOrDefault().play.CurrentGamer = roundsAndGamers.CurrentGamer;
            jokers.Where(p => p.TableID == tableID).FirstOrDefault().play.CurrentGamer.AllowCardsForTable();

            return jokers.Where(p => p.TableID == tableID).FirstOrDefault().play;
        }

        public Joker GetPlayState(string tableID)
        {
            return jokers.Where(p => p.TableID == tableID).FirstOrDefault();
        }
        public bool RemoveJoker(string tableID)
        {
            Joker joker = jokers.Where(p => p.TableID == tableID).FirstOrDefault();
            _tableService.DeleteWithId(tableID);
            _gamerService.DeleteOnTableWithId(tableID);
            return jokers.Remove(joker);
        }

        public Joker PutCardOnTable(int cardId, string tableID)
        {

            Joker joker = jokers.Where(p => p.TableID == tableID).FirstOrDefault();
            LionshubJoker.Joker.Card card = joker.play.CurrentGamer.CardsOnHand.Where(p => p.CardId == cardId).FirstOrDefault();
            if (joker != null)
            {
                bool res = joker.play.CurrentGamer.PutCardAway(card);
                if (res)
                {
                    return joker;
                }
            }
            return null;
        }

    }
}
