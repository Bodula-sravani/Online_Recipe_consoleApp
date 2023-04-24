﻿using Microsoft.Data.SqlClient;

namespace OnlineRecipes
{
    internal class OnlineRecipesClass
    {
        public string connectionString;
        SqlConnection connection;

        public OnlineRecipesClass()
        {
            this.connectionString ="Data Source=DESKTOP-8C83AOT;Initial Catalog=Online_recipe;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            this.connection = new SqlConnection(connectionString);
        }

        public void addUser(string username, string password)
        {
            // Add new user during registration using stored procedure
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("insertUSer", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("id", username);
                command.Parameters.AddWithValue("passsword", password);
                command.ExecuteNonQuery();
                connection.Close();

            }
            catch (SqlException e)
            {
                Console.WriteLine("error: " + e.Message);
            }
        }

        public bool validateUser(string username,string password) 
        {
            // To validate the user credentials by first checking if user exists at all
            connection.Open();
            SqlCommand countCommand = new SqlCommand($"select count(*) from users where userId ='{username}'", connection);
            int count = Convert.ToInt32(countCommand.ExecuteScalar());
            connection.Close();
            if (count == 0) return false;
            try
            {
                connection.Open();
                string query = $"Select * from users where userId='{username}'";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                string confirmPassword = "";
                while (reader.Read())
                {
                    confirmPassword = reader["password"].ToString();
                }
                if (confirmPassword.Equals(password))
                {
                    reader.Close();
                    connection.Close();
                    return true;
                }
               
            }
            catch(SqlException e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            return false;
        
        }

        public int listUserRecipes(string username)
        {
            // TO get list of particular user recipes
            connection.Open();
            SqlCommand countCommand = new SqlCommand($"select count(*) from recipes where userId ='{username}'",connection);
            int count = Convert.ToInt32(countCommand.ExecuteScalar());
            connection.Close();
            if (count == 0) return count;
            try
            {
                connection.Open();
                string query = $"select * from recipes where userId='{username}'";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                Console.WriteLine();
                Console.WriteLine("Recipes uploaded by you");
                Console.WriteLine("Id  Name     Ingredients         CookingTime     Instructions        createdDate     category        ");
                while (reader.Read())
                {
                    for (int j = 0; j < reader.FieldCount-1; j++)
                    {
                        Console.Write(reader[j] + " ");
                    }
                    Console.WriteLine();
                }
                reader.Close();
                connection.Close();

            }
            catch (SqlException e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            return count;
        }



        public bool addRecipe(string username)
        {
            // Add a new recipe by a user
            Recipe recipe = new Recipe();
            Console.WriteLine();
            Console.WriteLine("Enter name of recipe");
            recipe.name = Console.ReadLine().Trim();
            Console.WriteLine("enter ingredients");
            recipe.ingredients = Console.ReadLine().Trim();
            Console.WriteLine("Enter cooking time in hrs");
            recipe.cookingTime = Console.ReadLine().Trim();
            Console.WriteLine("Enter instructions");
            recipe.instructions = Console.ReadLine().Trim();
            recipe.createddate = DateTime.Now;
            Console.WriteLine("Enter cateogry: breakfast,lunch,dinner,snack");
            recipe.category= Console.ReadLine().Trim();
            recipe.userId = username;

            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("insertRecipe", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("userId", username);
                command.Parameters.AddWithValue("name", recipe.name);
                command.Parameters.AddWithValue("ingredients", recipe.ingredients);
                command.Parameters.AddWithValue("cookingTime", recipe.cookingTime);
                command.Parameters.AddWithValue("instruction", recipe.instructions);
                command.Parameters.AddWithValue("date1", recipe.createddate);
                command.Parameters.AddWithValue("category", recipe.category);
                command.ExecuteNonQuery();
                connection.Close();
                return true;
            }
            catch(SqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
        }

        public bool deleteRecipe(string username)
        {
            // User deletes one of his/her own recipes
            try
            {
                Console.WriteLine("Enter id of recipe to be deleted from list");
                int id = Convert.ToInt32(Console.ReadLine());
                connection.Open();
                SqlCommand command = new SqlCommand("deleterecipe", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("id", id);
                command.Parameters.AddWithValue("userId", username);
                command.ExecuteNonQuery();
                connection.Close();
                return true;

            }
            catch(SqlException e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            return false;
        }


        public int searchRecipe(int filter)
        {
            // Search for recipe based on filter by name or by category or just lists all available
            string query = "";
            string countquery = "";
            if (filter == 1)
            {
                Console.WriteLine();
                Console.WriteLine("Choose category breakfast,lunch,dinner,snack");
                string category = Console.ReadLine().Trim();
                query = $"select * from recipes where category='{category}'";
                countquery = $"select count(*) from recipes where category='{category}'";
            }
            else if(filter==2)
            {
                Console.WriteLine();
                Console.WriteLine("Enter name of recipe");
                string name = Console.ReadLine().Trim();
                query = $"select * from recipes where name like '%{name}%'";
                countquery = $"select count(*) from recipes where name like '%{name}%'";
            }
            else
            {
                query = "select * from recipes";
                countquery = "select count(*) from recipes";
            }
            connection.Open();
            SqlCommand countCommand = new SqlCommand(query, connection);
            int count = Convert.ToInt32(countCommand.ExecuteScalar());
            connection.Close();
            if(count==0) return count;
            try
            {
                connection.Open();   
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                Console.WriteLine();
                Console.WriteLine("List of Recipes");
                Console.WriteLine("Id  Name     Ingredients         CookingTime     Instructions        createdDate     category        ");
                while (reader.Read())
                {
                    for (int j = 0; j < reader.FieldCount - 1; j++)
                    {
                        Console.Write(reader[j] + " ");
                    }
                    Console.WriteLine();
                }
                reader.Close();
                connection.Close();

            }
            catch (SqlException e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            return count;
        }
        
        static void Main(string[] args)
        {
            OnlineRecipesClass app = new OnlineRecipesClass();
            int quitApp = 0;  // used to quit the app
            do
            {
                Console.WriteLine(".....WELCOME TO ONLINE RECIPE PORTAL......");
                Console.WriteLine();
                Console.WriteLine("Press 1 to quit the app");
                Console.WriteLine("Press 0 to  homepage");    
                quitApp = Convert.ToInt32(Console.ReadLine());
                switch(quitApp)
                {
                    case 0:
                        Console.WriteLine();
                        Console.WriteLine("Enter 1 for newUser 0 for existing User");
                        int userOption = Convert.ToInt32(Console.ReadLine());  // to store register or login option
                        if (userOption == 1)
                        {
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine("Please enter unique userId");
                            string newuserId = Console.ReadLine();
                            Console.WriteLine();
                            Console.WriteLine("Please enter your password");
                            string newpassword = Console.ReadLine();
                            app.addUser(newuserId, newpassword);
                        }
                        Console.WriteLine();
                        Console.WriteLine(".....LOGIN PAGE.......");
                        Console.WriteLine("Please enter your userId ");
                        string userId = Console.ReadLine();
                        Console.WriteLine();
                        Console.WriteLine("Please enter your password");
                        string password = Console.ReadLine();
                        if (app.validateUser(userId, password))
                        {
                            user currentUser = new user();   // Storing current user to manipulate throughout his/her login session
                            currentUser.userId = userId;
                            currentUser.password = password;
                            Console.WriteLine();
                            Console.WriteLine();
                            int userChoice = 0;
                            do
                            {
                                // This is userPage and options available
                                Console.WriteLine("choose from following sections");
                                Console.WriteLine("Press 0 to logout");
                                Console.WriteLine("Press 1 to open your recipes section");
                                Console.WriteLine("Press 2 to open your favourite recipes section");
                                Console.WriteLine("Press 3 to search all recipes in app");
                                Console.WriteLine();
                                userChoice = Convert.ToInt32(Console.ReadLine());
                                switch (userChoice)
                                {
                                    case 0:
                                        Console.WriteLine();
                                        Console.WriteLine("....Logging out....");
                                        break;
                                    case 1:
                                        Console.WriteLine();
                                        Console.WriteLine("....Your Recipes section.....");
                                        int recipeChoice = 0;
                                        do
                                        {
                                            // This is user recipe section and options avaibale
                                            int count = app.listUserRecipes(currentUser.userId);
                                            Console.WriteLine();
                                            if (count == 0) Console.WriteLine("You have not uploded any recipes");
                                            Console.WriteLine();
                                            Console.WriteLine("Press 1 to upload another recipe");
                                            Console.WriteLine("press 2 to delete a recipe");
                                            Console.WriteLine("Press 0 to back to userpage");
                                            recipeChoice = Convert.ToInt32(Console.ReadLine());
                                            switch (recipeChoice)
                                            {
                                                case 0:
                                                    Console.WriteLine();
                                                    Console.WriteLine("....Redircting to User pager....");
                                                    break;
                                                case 1:
                                                    Console.WriteLine();
                                                    if (app.addRecipe(currentUser.userId)) Console.WriteLine("....Recipe Successfully added....");
                                                    break;
                                                case 2:
                                                    Console.WriteLine();
                                                    if (app.deleteRecipe(currentUser.userId)) Console.WriteLine("....Recipe Successfully deleted....");
                                                    break;
                                            }

                                        } while (recipeChoice != 0);

                                        break;
                                    //case 2:
                                    //    Console.WriteLine();
                                    //    Console.WriteLine("....Your fav recipes section....");
                                    //    int favChoice = 0;
                                    //    do
                                    //    {
                                    //        // This is favourite recipes section and options avaiable
                                    //        int count = app.listUserfavRecipes(currentUser.userId);
                                    //        Console.WriteLine();
                                    //        if (count == 0) Console.WriteLine("You have no fav recipes");
                                    //        Console.WriteLine();
                                    //        Console.WriteLine("Press 1 to add a recipe to favrouties");
                                    //        Console.WriteLine("press 2 to delete a a favourite");
                                    //        Console.WriteLine("Press 0 to back to userpage");
                                    //        favChoice = Convert.ToInt32(Console.ReadLine());
                                    //        switch (favChoice)
                                    //        {
                                    //            case 0:
                                    //                Console.WriteLine();
                                    //                Console.WriteLine("....Redircting to User pager....");
                                    //                break;
                                    //            case 1:
                                    //                Console.WriteLine();
                                    //                if (app.addFavRecipe(currentUser.userId)) Console.WriteLine("....Fav Recipe Successfully added....");
                                    //                break;
                                    //            case 2:
                                    //                Console.WriteLine();
                                    //                if (app.deleteFavRecipe(currentUser.userId)) Console.WriteLine("....Fav Recipe Successfully deleted....");
                                    //                break;
                                    //        }

                                    //    } while(favChoice != 0);
                                    //    break;
                                    case 3:
                                        Console.WriteLine();
                                        Console.WriteLine("....Search page....");
                                        int searchFilter = 0;
                                        do
                                        {
                                            // This is search page 
                                            Console.WriteLine();
                                            Console.WriteLine("Press 1 to filter by category");
                                            Console.WriteLine("press 2 to search by name of recipe");
                                            Console.WriteLine("press 3 to search by no filter");
                                            Console.WriteLine("Press 0 to back to userpage");
                                            searchFilter= Convert.ToInt32(Console.ReadLine());
                                            if (searchFilter != 0)
                                            {
                                                int count = app.searchRecipe(searchFilter);
                                                Console.WriteLine();
                                                if (count == 0) Console.WriteLine("No  recipes Avaialble ");
                                            }
                                        } while(searchFilter!=0);
                                        break;


                                }

                            } while (userChoice != 0);



                        }
                        else
                        {
                            // If the credetails are wrong 
                            Console.WriteLine();
                            Console.WriteLine("....Wrong credentials....");
                            Console.WriteLine("....Returing to home page....");
                        }
                        break;
                    case 1:
                        // Quit app case
                        Console.WriteLine();
                        Console.WriteLine("....Quitting the app....");
                        break;
                }
            } while (quitApp != 1);
            


        }
    }
}