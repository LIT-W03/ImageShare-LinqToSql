using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;

namespace ImageShare.Data
{
    public class ImageShareManager
    {
        private string _connectionString;

        public ImageShareManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddImage(Image image)
        {
            using (var context = new ImageShareDataContext(_connectionString))
            {
                context.Images.InsertOnSubmit(image);
                image.DateUploaded = DateTime.Now;
                context.SubmitChanges();
            }
            //using (var connection = new SqlConnection(_connectionString))
            //{
            //    var command = connection.CreateCommand();
            //    command.CommandText = "INSERT INTO Images (FirstName, LastName, ImageFile, DateUploaded, ViewCount)" +
            //                          " VALUES (@firstName, @lastName, @imageFile, @date, 0); SELECT @@Identity";
            //    command.Parameters.AddWithValue("@firstName", image.FirstName);
            //    command.Parameters.AddWithValue("@lastName", image.LastName);
            //    command.Parameters.AddWithValue("@imageFile", image.ImageFile);
            //    command.Parameters.AddWithValue("@date", DateTime.Now);
            //    connection.Open();
            //    image.Id = (int)(decimal)command.ExecuteScalar();
            //}
        }

        public IEnumerable<Image> GetFiveMostRecent()
        {
            using (var context = new ImageShareDataContext(_connectionString))
            {
                return context.Images.OrderByDescending(i => i.DateUploaded).Take(5).ToList();
            }

            //return GetImages("SELECT Top 5 * FROM Images ORDER By DateUploaded DESC");
        }

        public IEnumerable<Image> GetFiveMostPopular()
        {
            using (var context = new ImageShareDataContext(_connectionString))
            {
                return context.Images.OrderByDescending(i => i.ViewCount).Take(5).ToList();
            }
            //return GetImages("SELECT Top 5 * FROM Images ORDER BY ViewCount DESC");
        }

        public IEnumerable<Image> GetFiveMostLikedImages()
        {

            using (var context = new ImageShareDataContext(_connectionString))
            {
                var options = new DataLoadOptions();
                options.LoadWith<Image>(i => i.ImageLikes);
                context.LoadOptions = options;
                return context.Images.OrderByDescending(i => i.ImageLikes.Count).Take(5).ToList();
            }

            //using (var connection = new SqlConnection(_connectionString))
            //{
            //    var command = connection.CreateCommand();
            //    command.CommandText = @"SELECT TOP 5 i.*, COUNT(il.ImageId) as 'LikesCount'
            //                            FROM Images i
            //                            LEFT JOIN ImageLikes il
            //                            ON i.Id = il.ImageId
            //                            GROUP BY i.Id, i.FirstName, i.LastName, i.ImageFile, i.DateUploaded, i.ViewCount
            //                            ORDER BY Count(il.ImageId) DESC";
            //    connection.Open();
            //    var reader = command.ExecuteReader();
            //    List<ImageWithLikes> images = new List<ImageWithLikes>();
            //    while (reader.Read())
            //    {
            //        Image image = ToImage(reader);
            //        ImageWithLikes imageWithLikes = new ImageWithLikes
            //        {
            //            Image = image,
            //            LikesCount = (int)reader["LikesCount"]
            //        };
            //        images.Add(imageWithLikes);
            //    }

            //    return images;
            //}
        }

        public IEnumerable<Image> GetUserLikedImages(int userId)
        {
            using (var context = new ImageShareDataContext(_connectionString))
            {

                var options = new DataLoadOptions();
                options.LoadWith<Image>(i => i.ImageLikes);
                context.LoadOptions = options;
                return context.Users.FirstOrDefault(u => u.Id == userId)
                    .ImageLikes.Select(il => il.Image).ToList();

            }

            //using (var connection = new SqlConnection(_connectionString))
            //{
            //    var command = connection.CreateCommand();
            //    command.CommandText = @"SELECT i.*, COUNT(il.ImageId) as 'LikesCount'
            //                            FROM Images i
            //                            JOIN ImageLikes il
            //                            ON i.Id = il.ImageId
            //                            WHERE il.UserId = @userId
            //                            GROUP BY i.Id, i.FirstName, i.LastName, i.ImageFile, i.DateUploaded, i.ViewCount";
            //    command.Parameters.AddWithValue("@userId", userId);
            //    connection.Open();
            //    var reader = command.ExecuteReader();
            //    List<ImageWithLikes> images = new List<ImageWithLikes>();
            //    while (reader.Read())
            //    {
            //        Image image = ToImage(reader);
            //        ImageWithLikes imageWithLikes = new ImageWithLikes
            //        {
            //            Image = image,
            //            LikesCount = (int)reader["LikesCount"]
            //        };
            //        images.Add(imageWithLikes);
            //    }

            //    return images;
            //}
        }

        public Image GetImage(int id)
        {

            using (var context = new ImageShareDataContext(_connectionString))
            {
                return context.Images.FirstOrDefault(i => i.Id == id);
            }

            //using (var connection = new SqlConnection(_connectionString))
            //{
            //    var command = connection.CreateCommand();
            //    command.CommandText = "SELECT * FROM Images WHERE Id = @id";
            //    command.Parameters.AddWithValue("@id", id);
            //    connection.Open();
            //    var reader = command.ExecuteReader();
            //    reader.Read();
            //    return ToImage(reader);
            //}
        }

        public void IncrementCount(int imageId)
        {

            using (var context = new ImageShareDataContext(_connectionString))
            {
                context.ExecuteCommand("UPDATE Images SET ViewCount = ViewCount + 1 WHERE id = {0}", imageId);
            }

            //using (var connection = new SqlConnection(_connectionString))
            //{
            //    var command = connection.CreateCommand();
            //    command.CommandText = "UPDATE Images SET ViewCount = ViewCount + 1 WHERE id = @id";
            //    command.Parameters.AddWithValue("@id", imageId);
            //    connection.Open();
            //    command.ExecuteNonQuery();
            //}
        }

        public void AddImageLike(int userId, int imageId)
        {
            using (var context = new ImageShareDataContext(_connectionString))
            {
                var imageLike = new ImageLike
                {
                    UserId = userId,
                    ImageId = imageId
                };
                context.ImageLikes.InsertOnSubmit(imageLike);
                context.SubmitChanges();
            }

            //using (var connection = new SqlConnection(_connectionString))
            //{
            //    var command = connection.CreateCommand();
            //    command.CommandText = "INSERT INTO ImageLikes (UserId, ImageId) VALUES (@userId, @imageId)";
            //    command.Parameters.AddWithValue("@userId", userId);
            //    command.Parameters.AddWithValue("@imageId", imageId);
            //    connection.Open();
            //    command.ExecuteNonQuery();
            //}
        }

        public int GetLikesCount(int imageId)
        {
            using (var context = new ImageShareDataContext(_connectionString))
            {
                return context.ImageLikes.Count(i => i.ImageId == imageId);
            }
            //using (var connection = new SqlConnection(_connectionString))
            //{
            //    var command = connection.CreateCommand();
            //    command.CommandText = "SELECT ISNULL(Count(*),0) FROM ImageLikes WHERE ImageId = @imageId";
            //    command.Parameters.AddWithValue("@imageId", imageId);
            //    connection.Open();
            //    return (int)command.ExecuteScalar();
            //}
        }

        public bool HasUserLiked(string emailAddress, int imageId)
        {
            using (var context = new ImageShareDataContext(_connectionString))
            {
                return context.Users
                    .FirstOrDefault(u => u.EmailAddress == emailAddress)
                    .ImageLikes
                    .Any(il => il.ImageId == imageId);
            }

            //using (var connection = new SqlConnection(_connectionString))
            //{
            //    var command = connection.CreateCommand();
            //    command.CommandText = "SELECT ISNULL(Count(*), 0) FROM ImageLikes il " +
            //                          "JOIN Users u ON il.userId = u.id " +
            //                          "WHERE u.emailAddress = @emailAddress AND il.imageId = @imageId";
            //    command.Parameters.AddWithValue("@emailAddress", emailAddress);
            //    command.Parameters.AddWithValue("@imageId", imageId);
            //    connection.Open();
            //    return ((int)command.ExecuteScalar()) == 1;
            //}
        }



        private IEnumerable<Image> GetImages(string query)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = query;
                connection.Open();
                List<Image> images = new List<Image>();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    images.Add(ToImage(reader));
                }

                return images;
            }
        }

        private Image ToImage(SqlDataReader reader)
        {
            Image image = new Image();
            image.Id = (int)reader["Id"];
            image.FirstName = (string)reader["FirstName"];
            image.LastName = (string)reader["LastName"];
            image.ViewCount = (int)reader["ViewCount"];
            image.DateUploaded = (DateTime)reader["DateUploaded"];
            image.ImageFile = (string)reader["ImageFile"];
            return image;
        }
    }
}