using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionshubJoker.Joker;
using Microsoft.AspNetCore.Mvc;

namespace LionshubJokAPI.Services
{
    public class JokerService
    {
        private readonly PlayGame play;
        private readonly List<Card> deckOfCard;
        public JokerService()
        {
            DeckOfCardCreator deckOfCardCreator = new DeckOfCardCreator();
            // კარტის დასტა
            deckOfCard = new List<Card>();
            deckOfCard.AddRange(deckOfCardCreator.CreateDeckOfCards());
            //play = new PlayGame(gamers, deckOfCard);
        }

        public void StartPlay(List<Models.Gamer> gamers)
        {
            List<Gamer> players = new List<Gamer>();
            foreach (Models.Gamer gamer in gamers)
            {
                RedirectToActionResult r = new RedirectToActionResult("Get", "LionshubApi/Tables", gamer.Id);

                //players.Add(new Gamer(gamer.Id, gamer.Name));
            }
        }
    }
}
