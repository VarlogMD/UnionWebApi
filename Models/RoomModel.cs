using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySqlConnector;

namespace UnionWebApi
{
    public class Room
    {
        public int room_id { get; set; }
        public string name { get; set; }


        internal Database Db { get; set; }

        public Room()
        {
        }

        internal Room(Database db)
        {
            Db = db;
        }

        public async Task<List<Room>> GetAllAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM  rooms ;";
            return await ReturnAllAsync(await cmd.ExecuteReaderAsync());
        }

        public async Task<Room> FindOneAsync(int room_id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM  rooms  WHERE  room_id  = @room_id";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@room_id",
                DbType = DbType.Int32,
                Value = room_id,
            });
            var result = await ReturnAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }


        public async Task<int> InsertAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO  rooms  ( name ) VALUES (@name);";
            BindParams(cmd);
            try
            {
                await cmd.ExecuteNonQueryAsync();
                int user_id = (int) cmd.LastInsertedId;
                return user_id; 
            }
            catch (System.Exception)
            {   
                return 0;
            } 
        }

        public async Task UpdateAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"UPDATE  rooms  SET  name  = @name WHERE  room_id  = @room_id;";
            BindParams(cmd);
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM  rooms  WHERE  room_id  = @room_id;";
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task<List<Room>> ReturnAllAsync(DbDataReader reader)
        {
            var posts = new List<Room>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new Room(Db)
                    {
                        room_id = reader.GetInt32(0),
                        name = reader.GetString(1),
                    };
                    posts.Add(post);
                }
            }
            return posts;
        }

        private void BindId(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@room_id",
                DbType = DbType.Int32,
                Value = room_id,
            });
        }

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@name",
                DbType = DbType.String,
                Value = name,
            });
        }
    }
}