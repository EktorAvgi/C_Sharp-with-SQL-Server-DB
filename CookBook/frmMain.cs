﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace CookBook
{
    public partial class frmMain : Form
    {
        SqlConnection connection;

        string connectionString;

        public frmMain()
        {
            InitializeComponent();

            connectionString = ConfigurationManager.ConnectionStrings["CookBook.Properties.Settings.CookbookConnectionString"].ConnectionString;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            PopulateRecipes();
            PopulateAllIngredients();
        }

        private void PopulateRecipes()
        {
            using (connection = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Recipe", connection))
            {
                DataTable recipeTable = new DataTable();
                adapter.Fill(recipeTable);

                lstRecipes.DisplayMember = "Name";
                lstRecipes.ValueMember = "Id";
                lstRecipes.DataSource = recipeTable;
            }
        }

        private void PopulateAllIngredients()
        {
            using (connection = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Ingredient", connection))
            {
                DataTable ingredientTable = new DataTable();
                adapter.Fill(ingredientTable);

                lstAllIngredients.DisplayMember = "Name";
                lstAllIngredients.ValueMember = "Id";
                lstAllIngredients.DataSource = ingredientTable;
            }
        }

        private void PopulateIngredients()
        {
            string query = "SELECT a.Name FROM Ingredient a " + "INNER JOIN RecipeIngredient b ON a.ID = b.IngredientId " + "WHERE b.RecipeId = @RecipeId";

            using (connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                command.Parameters.AddWithValue("@RecipeId", lstRecipes.SelectedValue);

                DataTable ingredientTable = new DataTable();
                adapter.Fill(ingredientTable);

                lstIngredients.DisplayMember = "Name";
                lstIngredients.ValueMember = "Id";
                lstIngredients.DataSource = ingredientTable;
            }
        }

        private void lstRecipes_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateIngredients();
        }

        private void btnAddRecipe_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO Recipe VALUES (@RecipeName, 80, 'blah blah')";
                
            using (connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@RecipeName", txtRecipeName.Text);

                command.ExecuteScalar();
            }

            PopulateRecipes();
        }

        private void btnUpdateRecipeName_Click(object sender, EventArgs e)
        {
            string query = "UPDATE Recipe SET Name = @RecipeName WHERE Id = @RecipeId";

            using (connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@RecipeName", txtRecipeName.Text);
                command.Parameters.AddWithValue("@RecipeId", lstRecipes.SelectedValue);

                command.ExecuteScalar();
            }

            PopulateRecipes();
        }

        private void btnAddToRecipe_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO RecipeIngredient VALUES (@RecipeId, @IngredientId)";

            using (connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@RecipeId", lstRecipes.SelectedValue);
                command.Parameters.AddWithValue("@IngredientId", lstAllIngredients.SelectedValue);
            
                command.ExecuteScalar();
            }

            PopulateRecipes();
        }
    }
}
