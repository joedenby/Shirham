using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;
using System;
using System.Linq;
namespace GameDatabase {

    public static class RSSchema
    {
        private static readonly string path = "URI=file:rsdatabase.db";

        public static void AddIdentity(Identity identity) {
            using (var connection = new SqliteConnection(path)) {
                connection.Open();

                using (var command = connection.CreateCommand()) {
                    command.CommandText = $"DELETE FROM identity where ID = '{identity.id}'";
                    command.ExecuteNonQuery();

                    command.CommandText = $"INSERT INTO identity {identity.Insert}";
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public static Identity[] QueryIdentity(string query) {
            List<Identity> identities = new List<Identity>();

            using (var connection = new SqliteConnection(path)) {
                connection.Open();

                using (var command = connection.CreateCommand()) {
                    command.CommandText = query;

                    using (IDataReader reader = command.ExecuteReader()) {

                        while (reader.Read()) {
                            var id = int.Parse(reader["id"].ToString());
                            var race = Enum.Parse(typeof(Identity.Race), reader["race"].ToString());

                            identities.Add(new Identity(id, reader["forename"].ToString(), reader["familyname"].ToString(), (Identity.Race)race, reader["bio"].ToString(), reader["resides"].ToString()));
                        }

                        reader.Close();
                    }
                }
            }

            return identities.ToArray();
        }

        public static void AddRelationship(Relationship relationship) {
            using (var connection = new SqliteConnection(path))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"DELETE FROM relationship where relationshipID = '{relationship.relationshipID}';";
                    command.ExecuteNonQuery();

                    command.CommandText = $"INSERT INTO relationship {relationship.Insert}";
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public static Relationship[] QueryRelationship(string query) {
            List<Relationship> identities = new List<Relationship>();

            using (var connection = new SqliteConnection(path))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    using (IDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            int self = int.Parse(reader["selfID"].ToString());
                            int other = int.Parse(reader["otherID"].ToString());
                            int score = int.Parse(reader["score"].ToString());

                            identities.Add(new Relationship(reader["relationshipID"].ToString(), self, other, score));
                        }

                        reader.Close();
                    }
                }
            }

            return identities.ToArray();
        }

        public class Identity {
            public readonly int id;
            public readonly string forename;
            public readonly string familyname;
            public readonly Race race;
            public enum Race
            {
                Human, Orc, Elf
            }
            public readonly string bio;
            public readonly string resides;
            public string Insert => $"(id, forename, familyname, race, bio, resides) " +
                    $"VALUES ({id}, '{forename}', '{familyname}', '{race}', '{bio}', '{resides}')";

            public Identity(int id, string forename, string familyname, Race race, string bio, string resides) {
                this.id = id;
                this.forename = forename;
                this.familyname = familyname;
                this.race = race;
                this.bio = bio;
                this.resides = resides;
            }

            public static Identity Get(int id) {
                Identity[] result = QueryIdentity($"SELECT * FROM identity WHERE id = '{id}'");
                return result.FirstOrDefault();
            }

            public override string ToString()
            {
                return $"id: {id} \n" +
                       $"forname: {forename} \n" +
                       $"familyname: {familyname} \n" +
                       $"race: {race} \n" +
                       $"bio: {bio} \n" +
                       $"resides: {resides}";
            }
        }
        public class Relationship { 
            public readonly string relationshipID;
            public readonly int selfID;
            public readonly int otherID;
            [Range(-10, 10)]
            public readonly int score;
            public string Insert => $"(relationshipID, selfID, otherID, score) " +
            $"VALUES ('{relationshipID}', {selfID}, {otherID}, {score})";

            public Relationship(string relationshipID, int selfID, int otherID, int score) { 
                this.relationshipID = relationshipID;
                this.selfID = selfID;
                this.otherID = otherID;
                this.score = score;
            }

            public override string ToString()
            {
                return $"ID: {relationshipID} \n" +
                    $"selfID: {selfID} \n" +
                    $"otherID: {otherID} \n" +
                    $"score: {score}";
            }
        }
    }

}
