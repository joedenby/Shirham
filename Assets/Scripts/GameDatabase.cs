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


        [RuntimeInitializeOnLoadMethod]
        public static void Initialize() {
            using (var connection = new SqliteConnection(path)) {
                connection.Open();

                using (var command = connection.CreateCommand()) {
                    command.CommandText = "CREATE TABLE IF NOT EXISTS " +
                        "identity (id INT, forename VARCHAR(45), familyname VARCHAR(45), race VARCHAR(20), bio TEXT, resides VARCHAR(45))";
                    command.ExecuteNonQuery();

                    Debug.LogWarning("New rsdatabase.db was created.");
                }

                connection.Close();
            }
        }

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
    }

}
