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
                joker.rounds.First().Aktive = true;
                //table.FourCardsAndGamersListOnTheTable = playTable._fourCardsAndGamersListOnTheTable;
                joker.play = new Joke.PlayGame(playGamers);
                joker.TableID = tableID;
                joker.Table = playTable;
                jokers.Add(joker);
                res = true;
            }
            return res;
        }

        public Joke.PlayGame StartPlay(string tableID)
        {
            //Joke.RoundsAndGamers roundsAndGamers = new Joke.RoundsAndGamers
            //{
            //    Hand = round.handRound,
            //    CurrentGamer = jokers.Where(p => p.TableID == tableID).FirstOrDefault().play.Gamers.Where(p => p.Id == round.GamerID).FirstOrDefault()
            //};
            Joker joker = jokers.Where(p => p.TableID == tableID).FirstOrDefault();
            //RoundsAndGamers round = joker.rounds.Where(p => p.handRound == Joke.CardsOnRound.One).FirstOrDefault();
            //round.Aktive = true;
            RoundsAndGamers roun = joker.rounds.LastOrDefault(p => p.Aktive == true);
            Joke.Gamer CurrentGamer = jokers.Where(p => p.TableID == tableID).FirstOrDefault().play.Gamers.Where(p => p.Id == roun.GamerID).FirstOrDefault();
            joker.play.StartRound(roun.handRound);
            joker.play.CurrentGamer = CurrentGamer;
            joker.play.CurrentGamer.AllowCardsForTable();
            joker.rounds.Where(p => p.GamerID == roun.GamerID && p.handRound == roun.handRound).First().Aktive = true;
            joker.CountOfCardsOnHand = Convert.ToInt16(roun.handRound);
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

        public List<Joker> GetCurrentGames()
        {
            return jokers;
        }

        public Joker PutCardOnTable(int cardId, string tableID)
        {
            Joker joker = jokers.Where(p => p.TableID == tableID).FirstOrDefault();
            Joke.Card card = joker.play.CurrentGamer.CardsOnHand.Where(p => p.CardId == cardId).FirstOrDefault();
            if (joker != null)
            {
                bool res = joker.play.CurrentGamer.PutCardAway(card);
                if (res)
                {
                    SwitchCurrentGamer(joker);
                    return joker;
                }
            }
            return null;
        }

        private void SwitchCurrentGamer(Joker joker)
        {
            if (joker.Table._fourCardsAndGamersListOnTheTable._fourCardAndGamerOnTable.Count == joker.play.Gamers.Count)
            {
                RoundsAndGamers currentHand = joker.rounds.Where(p => p.Aktive == true).LastOrDefault();
                joker.Table.TakeCardsFromTable(currentHand.handRound);
                joker.play.CurrentGamer = joker.play.Gamers.Where(p => p.CurrentGamerAfterOneRound == true).First();
                joker.CountOfCardsOnHand = joker.CountOfCardsOnHand - 1;
                if (joker.CountOfCardsOnHand == 0)
                {
                    RoundsAndGamers round = joker.rounds.Where(p => p.Aktive == false).FirstOrDefault();
                    round.Aktive = true;
                    joker.play.CurrentGamer = joker.play.Gamers.Where(p => p.Id == round.GamerID).First();
                    StartPlay(joker.TableID);
                }
            }
            else
            {
                int indexOfCurrentGamer = joker.play.Gamers.IndexOf(joker.play.CurrentGamer);
                if (indexOfCurrentGamer == joker.play.Gamers.Count - 1)
                {
                    joker.play.CurrentGamer = joker.play.Gamers[0];
                }
                else
                {
                    joker.play.CurrentGamer = joker.play.Gamers[indexOfCurrentGamer + 1];
                }
            }
            joker.play.CurrentGamer.AllowCardsForTable();
        }
    }
}
