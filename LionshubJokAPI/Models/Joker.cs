using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionshubJoker.Joker;

namespace LionshubJokAPI.Models
{
    public class Joker
    {
        public PlayGame play { get; set; }
        public List<RoundsAndGamers> rounds { get; set; }
    }
}
