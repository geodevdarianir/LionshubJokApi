using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace LionshubJokAPI.Models
{
    public class Table
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("Status")]
        public bool Status { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; }
    }
}
