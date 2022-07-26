﻿using System.Data;
using System.Data.SqlClient;
using server.Extensions;
using server.Models.Entity;

namespace server.Models.DAL;

public class DalUser
{
    private static bool CheckUserUnicityBy(string field, string value)
    {
        try
        {
            using SqlConnection cnn = DbConnection.GetUserConnection();
            cnn.Open();
            string strSql = @"  SELECT TOP 1 * 
                                        FROM [User] 
                                        WHERE [User].[" + field + @"] = @Value ";

            SqlCommand cmd = new SqlCommand(strSql, cnn);
            cmd.Parameters.AddWithValue("@Value", value);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return false;
                }
            }

            cnn.Close();
        }
        catch (Exception e)
        {
            throw e;
        }

        return true;
    }

 
    //add user
    public static JsonResponse AddUser(User user)
    {
        JsonResponse jsonResponse = new JsonResponse();
        try
        {
            using SqlConnection connection = DbConnection.GetUserConnection();
            if (CheckUserUnicityBy("Email", user.Email))
            {
                connection.Open();

                string sql = @" INSERT INTO [User] ( FirstName, LastName, Email, Password) OUTPUT INSERTED.Id 
                                        VALUES (@FirstName, @LastName, @Email, @Password)";

                using SqlCommand command = new SqlCommand(sql, connection);

                if (String.IsNullOrEmpty(user.FirstName))
                    command.Parameters.AddWithValue("@FirstName", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);

                if (String.IsNullOrEmpty(user.LastName))
                    command.Parameters.AddWithValue("@LastName", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@LastName", user.LastName);

                if (String.IsNullOrEmpty(user.Email))
                    command.Parameters.AddWithValue("@Email", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@Email", user.Email);

                if (String.IsNullOrEmpty(user.Password))
                    command.Parameters.AddWithValue("@Password", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@Password", user.Password);

                long id = (long) command.ExecuteScalar();

                if (id > 0)
                {
                    jsonResponse.Success = true;
                    jsonResponse.Message = id.ToString();
                }
            }
            else
            {
                jsonResponse.Success = false;
                jsonResponse.Message = "L'utilisateur existe déjà avec la même adresse e-mail";
            }

            connection.Close();
        }
        catch (Exception e)
        {
            jsonResponse.Success = false;
            jsonResponse.Message = "ERREUR INSERTION USER !!";
        }

        return jsonResponse;
    }

    //update user
    public static JsonResponse UpdateUser(User user)
    {
        JsonResponse jsonResponse = new JsonResponse();
        try
        {
            using SqlConnection connection = DbConnection.GetUserConnection();
            connection.Open();

            string sql = @" UPDATE [User] 
                                    SET 
                                        FirstName=@FirstName,
                                        LastName=@LastName,
                                        Email=@Email,
                                        Password=@Password,
                                    WHERE Id=@Id";

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;

                if (String.IsNullOrEmpty(user.FirstName))
                    command.Parameters.AddWithValue("@FirstName", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);

                if (String.IsNullOrEmpty(user.LastName))
                    command.Parameters.AddWithValue("@LastName", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@LastName", user.LastName);


                if (String.IsNullOrEmpty(user.Email))
                    command.Parameters.AddWithValue("@Email", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@Email", user.Email);

                if (String.IsNullOrEmpty(user.Password))
                    command.Parameters.AddWithValue("@Password", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@Id", user.Id);

                if (command.ExecuteNonQuery() == 1)
                {
                    jsonResponse.Success = true;
                    jsonResponse.Message = "La mise à jour de l'utilisateur est réussie !";
                }
                else
                {
                    jsonResponse.Success = false;
                    jsonResponse.Message = "Échec de la mise à jour de l'utilisateur !";
                }
            }

            connection.Close();
        }
        catch (Exception e)
        {
            jsonResponse.Success = false;
            jsonResponse.Message = e.Message;
        }

        return jsonResponse;
    }

    //get all users 
    public static List<User> GetAllUsers()
    {
        List<User> lstUser = new List<User>();
        try
        {
            using SqlConnection connection = DbConnection.GetUserConnection();
            connection.Open();
            string sql = @" SELECT * 
                                    FROM [User] 
                                    ORDER BY Id DESC";

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        User user = new User
                        {
                            Id = long.Parse(dataReader["Id"].ToString()!),
                            FirstName = dataReader["FirstName"].ToString()!,
                            LastName = dataReader["LastName"].ToString()!,
                            Email = dataReader["Email"].ToString()!,
                            Password = dataReader["Password"].ToString()!
                        };

                        lstUser.Add(user);
                    }
                }
            }

            connection.Close();
        }
        catch (Exception e)
        {
            throw e;
        }

        return lstUser;
    }

    //get user by
    public static User GetUserBy(string field, string fieldValue)
    {
        User user = new User();
        try
        {
            using SqlConnection connection = DbConnection.GetUserConnection();
            connection.Open();
            string sql = @" SELECT TOP 1 * 
                                    FROM [User] 
                                    WHERE [User].[" + field + @"]=@Value";

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@Value", fieldValue);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        user.Id = long.Parse(dataReader["Id"].ToString()!);
                        user.FirstName = dataReader["FirstName"].ToString()!;
                        user.LastName = dataReader["LastName"].ToString()!;
                        user.Email = dataReader["Email"].ToString()!;
                        user.Password = dataReader["Password"].ToString()!;
                    }
                }
            }

            connection.Close();
        }
        catch (Exception e)
        {
            throw e;
        }

        return user;
    }

    //get all users by
    public static List<User> GetAllUsersBy(string field, string value)
    {
        List<User> lstUser = new List<User>();
        try
        {
            using SqlConnection connection = DbConnection.GetUserConnection();
            connection.Open();
            string sql = @" SELECT * 
                                    FROM [User] 
                                    WHERE [User].[" + field + @"]=@Field";

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@Field", value);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        User user = new User();

                        user.Id = long.Parse(dataReader["Id"].ToString()!);
                        user.FirstName = dataReader["FirstName"].ToString();
                        user.LastName = dataReader["LastName"].ToString()!;
                        user.Email = dataReader["Email"].ToString()!;
                        user.Password = dataReader["Password"].ToString()!;
                        lstUser.Add(user);
                    }
                }
            }

            connection.Close();
        }
        catch (Exception e)
        {
            throw e;
        }

        return lstUser;
    }

    //delete user by 
    public static JsonResponse DeleteUserBy(string field, string fieldValue)
    {
        JsonResponse jsonResponse = new JsonResponse();

        try
        {
            using (SqlConnection connection = DbConnection.GetUserConnection())
            {
                connection.Open();
                string sql = @" DELETE 
                                    FROM [User] 
                                    WHERE [User].[" + field + "]=@FieldValue";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@FieldValue", fieldValue);

                    if (command.ExecuteNonQuery() == 1)
                    {
                        jsonResponse.Success = true;
                        jsonResponse.Message = "Suppression réussie !";
                    }
                    else
                    {
                        jsonResponse.Success = false;
                        jsonResponse.Message = "Échec de la suppression !";
                    }
                }

                connection.Close();
            }
        }
        catch (Exception e)
        {
            jsonResponse.Success = false;
            jsonResponse.Message = e.Message;
        }

        return jsonResponse;
    }
}