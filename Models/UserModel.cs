using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySqlConnector;

namespace UnionWebApi

{
    public class User
    {
        public int user_id { get; set; }
        public string login { get; set; }
        public string pass { get; set; }
        public string email { get; set; }

        internal Database Db { get; set; }

        public User()
        {
        }

        internal User(Database db)
        {
            Db = db;
        }

        public async Task<List<User>> GetAllAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM  users ;";
            return await ReturnAllAsync(await cmd.ExecuteReaderAsync());
        }

        public async Task<User> FindOneAsync(int user_id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM  users  WHERE  user_id  = @user_id";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@user_id",
                DbType = DbType.Int32,
                Value = user_id,
            });
            var result = await ReturnAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }


        public async Task DeleteAllAsync()
        {
            using var txn = await Db.Connection.BeginTransactionAsync();
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM  users ";
            await cmd.ExecuteNonQueryAsync();
            await txn.CommitAsync();
        }
    

        public async Task<int> InsertAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO  users  ( login,  pass, email ) VALUES (@login, @pass, @email);";
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
            cmd.CommandText = @"UPDATE  users  SET  login  = @login,  pass  = @pass, email = @email WHERE  user_id  = @user_id;";
            BindParams(cmd);
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM  Users  WHERE  user_id  = @user_id;";
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task<List<User>> ReturnAllAsync(DbDataReader reader)
        {
            var posts = new List<User>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new User(Db)
                    {
                        user_id = reader.GetInt32(0),
                        login = reader.GetString(1),
                        pass = reader.GetString(2),
                        email = reader.GetString(3)
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
                ParameterName = "@user_id",
                DbType = DbType.Int32,
                Value = user_id,
            });
        }

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@login",
                DbType = DbType.String,
                Value = login,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@pass",
                DbType = DbType.String,
                Value = pass,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@email",
                DbType = DbType.String,
                Value = email,
            });

        }
    }
}