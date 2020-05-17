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
using LionshubJoker.Joker;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                joker.rounds = new List<Models.RoundsAndGamers>();
                foreach (LionshubJoker.Joker.RoundsAndGamers item in rounds)
                {
                    joker.rounds.Add(new Models.RoundsAndGamers
                    {
                        GamerID = item.CurrentGamer.Id,
                        handRound = item.Hand,
                        Pulka = item.Pulka
                    });
                }
                joker.rounds.First().Aktive = true;
                //table.FourCardsAndGamersListOnTheTable = playTable._fourCardsAndGamersListOnTheTable;
                joker.play = new Joke.PlayGame(playGamers);
                joker.TableID = tableID;
                joker.Table = playTable;

                joker.play.CurrentGamer = joker.play.Gamers.First(p => p.Id == joker.rounds.First().GamerID);
                joker.ResultOfPulkas = new List<ResultOfPulka>();
                jokers.Add(joker);
                res = true;
            }
            return res;
        }

        public Joke.PlayGame StartPlay(string tableID, Models.RoundsAndGamers round)
        {
            Joker joker = jokers.Where(p => p.TableID == tableID).FirstOrDefault();
            //Models.RoundsAndGamers round = joker.rounds.LastOrDefault(p => p.Aktive == true);
            Joke.Gamer CurrentGamer = jokers.Where(p => p.TableID == tableID).FirstOrDefault().play.Gamers.Where(p => p.Id == round.GamerID).FirstOrDefault();
            joker.play.StartRound(round.handRound);
            joker.ScoresOfGamers = new Joke.ScoresOfGamers(round.handRound, joker.play.Gamers);
            joker.play.CurrentGamer = CurrentGamer;
            joker.play.CurrentGamer.AllowCardsForTable();
            joker.rounds.Where(p => p.GamerID == round.GamerID && p.handRound == round.handRound).First().Aktive = true;
            joker.CountOfCardsOnHand = Convert.ToInt16(round.handRound);
            return jokers.Where(p => p.TableID == tableID).FirstOrDefault().play;
        }

        public Joker SetTumpCard(string tableID, int cardColor)
        {
            Joker joker = jokers.FirstOrDefault(p => p.TableID == tableID);
            if (joker != null)
            {
                CardColor color = (CardColor)Enum.ToObject(typeof(CardColor), cardColor);
                joker.play.SetTrumpCardOfRound(color);
            }
            return joker;
        }
        public Joker AllowScores(string tableID, int gamerID)
        {
            Joker jok = jokers.FirstOrDefault(p => p.TableID == tableID);
            if (jok != null)
            {
                jok.ScoresOfGamers.AllowScoresForGamers(gamerID);
            }
            return jok;
        }

        public Joker TellScore(string tableID, int gamerID, int score)
        {
            Joker jok = jokers.FirstOrDefault(p => p.TableID == tableID);
            if (jok != null)
            {
                Score TellScore = (Score)Enum.ToObject(typeof(Score), score);
                jok.ScoresOfGamers.TellScore(TellScore, gamerID);
                int indexOfCurrentGamer = jok.play.Gamers.IndexOf(jok.play.CurrentGamer);
                if (indexOfCurrentGamer == jok.play.Gamers.Count - 1)
                {
                    jok.play.CurrentGamer = jok.play.Gamers[0];
                }
                else
                {
                    jok.play.CurrentGamer = jok.play.Gamers[indexOfCurrentGamer + 1];
                }
            }
            return jok;
        }

        public Joker BotTellScore(string tableID, int gamerID)
        {
            Joker jok = jokers.FirstOrDefault(p => p.TableID == tableID);
            if (jok != null)
            {
                Score TellScore = jok.play.CurrentGamer.AllowedScores.First(p => p.Allowed == true).Score;
                jok.ScoresOfGamers.TellScore(TellScore, gamerID);
                int indexOfCurrentGamer = jok.play.Gamers.IndexOf(jok.play.CurrentGamer);
                if (indexOfCurrentGamer == jok.play.Gamers.Count - 1)
                {
                    jok.play.CurrentGamer = jok.play.Gamers[0];
                }
                else
                {
                    jok.play.CurrentGamer = jok.play.Gamers[indexOfCurrentGamer + 1];
                }
            }
            return jok;
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

        public Joker SetJokerStrength(string tableID, int strengthOfCard, int cardID, int? giveANDtake = 4)
        {
            Joker joker = jokers.FirstOrDefault(p => p.TableID == tableID);

            Joke.Card card = joker.play.CurrentGamer.CardsOnHand.FirstOrDefault(p => p.CardId == cardID);
            if (card != null)
            {
                StrengthOfCard _strengthOfCard = (StrengthOfCard)Enum.ToObject(typeof(StrengthOfCard), strengthOfCard);
                CardColor _giveANDtake = (CardColor)Enum.ToObject(typeof(CardColor), giveANDtake);
                card.SetJokerStrength(_strengthOfCard, _giveANDtake);
            }
            return joker;
        }

        public Joker PutCardOnTable(int cardId, string tableID)
        {
            Joker joker = jokers.FirstOrDefault(p => p.TableID == tableID);
            Joke.Card card = joker.play.CurrentGamer.CardsOnHand.FirstOrDefault(p => p.CardId == cardId);
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
        public Joker BotPutCardOnTable(string tableID)
        {
            Joker joker = jokers.FirstOrDefault(p => p.TableID == tableID);
            Joke.Card card = joker.play.CurrentGamer.CardsOnHand.FirstOrDefault(p => p.AllowsCardOnTheTable == true);
            if (joker != null)
            {
                bool res = joker.play.CurrentGamer.PutCardAway(card);
                if (res)
                {
                    //SwitchCurrentGamer(joker);
                    return joker;
                }
            }
            return null;
        }

        public Joker SwitchCurrentGamer(string tableID)
        {
            Joker joker = jokers.FirstOrDefault(p => p.TableID == tableID);
            if (joker != null)
            {
                if (joker.Table._fourCardsAndGamersListOnTheTable._fourCardAndGamerOnTable.Count == joker.play.Gamers.Count)
                {
                    Models.RoundsAndGamers currentHand = joker.rounds.Where(p => p.Aktive == true).LastOrDefault();
                    joker.Table.TakeCardsFromTable(currentHand.handRound);
                    joker.play.CurrentGamer = joker.play.Gamers.Where(p => p.CurrentGamerAfterOneRound == true).First();
                    joker.CountOfCardsOnHand = joker.CountOfCardsOnHand - 1;
                    if (joker.CountOfCardsOnHand == 0)
                    {
                        Models.RoundsAndGamers round = joker.rounds.Where(p => p.Aktive == false).FirstOrDefault();
                        round.Aktive = true;
                        joker.play.CurrentGamer = joker.play.Gamers.Where(p => p.Id == round.GamerID).First();
                        //StartPlay(joker.TableID);
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
            return joker;
        }

        private Joker GetEndResultOfPulka(string tableID, int pulka)
        {
            Joker joker = jokers.FirstOrDefault(p => p.TableID == tableID);
            if (joker != null)
            {
                foreach (Joke.Gamer item in joker.play.Gamers)
                {
                    List<Result> result = item.Result.Where(p => p.Hand.Pulka == pulka).ToList();
                    int endResult = result.Sum(p => p.EndResult);
                    joker.ResultOfPulkas.Add(new ResultOfPulka
                    {
                        GamerID = item.Id,
                        Result = endResult,
                        Pulka = pulka
                    });
                }
            }
            return joker;
        }
    }
}
