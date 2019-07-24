using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Tests.Serialization.Attributes;
using MongoDB.Driver;

namespace yxx
{
    class Program
    {
        static void Main(string[] args)
        {

            MongoClient tMongoClient = new MongoClient();
            IMongoDatabase tMongoDataBase = tMongoClient.GetDatabase("Db1");
            IMongoCollection<BsonAttributeTests.Test> tMongoCollection = tMongoDataBase.GetCollection<BsonAttributeTests.Test>("Collection1");

            BsonAttributeTests.Test tTest1 = new BsonAttributeTests.Test();
            tMongoCollection.InsertOne(tTest1);

            BsonAttributeTests.Test tTest2 = new BsonAttributeTests.Test();
            BsonDocument tBsonDocument = new BsonDocument();

            tMongoCollection.Find((s) =>
                a = 10,
                a > 10
                


            );
            tMongoCollection.FindAsync((s) => true);
        }
    }
}
