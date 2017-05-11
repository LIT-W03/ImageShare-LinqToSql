using System.Data.SqlClient;
using System.Linq;

namespace ImageShare.Data
{
    public class UserManager
    {
        private string _connectionString;

        public UserManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(string firstName, string lastName, string emailAddress, string password)
        {
            string salt = PasswordHelper.GenerateSalt();
            string hash = PasswordHelper.HashPassword(password, salt);

            using (ImageShareDataContext context = new ImageShareDataContext(_connectionString))
            {
                User user = new User
                {
                    EmailAddress = emailAddress,
                    FirstName = firstName,
                    LastName = lastName,
                    PasswordHash = hash,
                    PasswordSalt = salt
                };
                context.Users.InsertOnSubmit(user);
                context.SubmitChanges();
            }

            //using (SqlConnection connection = new SqlConnection(_connectionString))
            //{
            //    var command = connection.CreateCommand();
            //    command.CommandText = "INSERT INTO Users (FirstName, LastName, EmailAddress, PasswordHash, PasswordSalt) VALUES " +
            //                          "(@firstName, @lastName, @emailAddress, @password, @salt)";
            //    command.Parameters.AddWithValue("@firstName", firstName);
            //    command.Parameters.AddWithValue("@lastName", lastName);
            //    command.Parameters.AddWithValue("@emailAddress", emailAddress);
            //    command.Parameters.AddWithValue("@password", hash);
            //    command.Parameters.AddWithValue("@salt", salt);
            //    connection.Open();
            //    command.ExecuteNonQuery();
            //}
        }

        public User Login(string emailAddress, string password)
        {
            User user = GetUser(emailAddress);
            if (user == null)
            {
                return null;
            }

            bool isMatch = PasswordHelper.IsMatch(password, user.PasswordHash, user.PasswordSalt);
            if (isMatch) 
            {
                return user;
            }

            return null;
        }

        public User GetUser(string emailAddress)
        {
            using (ImageShareDataContext context = new ImageShareDataContext(_connectionString))
            {
                return context.Users.FirstOrDefault(u => u.EmailAddress == emailAddress);
            }
            //using (SqlConnection connection = new SqlConnection(_connectionString))
            //{
            //    var command = connection.CreateCommand();
            //    command.CommandText = "SELECT * FROM Users WHERE EmailAddress = @emailAddress";
            //    command.Parameters.AddWithValue("@emailAddress", emailAddress);
            //    connection.Open();
            //    var reader = command.ExecuteReader();
            //    if (!reader.Read())
            //    {
            //        return null;
            //    }

            //    User user = new User();
            //    user.Id = (int)reader["Id"];
            //    user.EmailAddress = (string)reader["EmailAddress"];
            //    user.FirstName = (string)reader["FirstName"];
            //    user.LastName = (string)reader["LastName"];
            //    user.PasswordHash = (string)reader["PasswordHash"];
            //    user.PasswordSalt = (string)reader["PasswordSalt"];
            //    return user;

            //}
        }

    }
}