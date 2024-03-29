﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LatvanyossagokApplication
{
    public partial class Form1 : Form
    {
        MySqlConnection conn;

       
        public Form1()
        {
            InitializeComponent();
            conn = new MySqlConnection("Server=localhost;  Database=latvanyossagokdb; Uid=root; Pwd=;");
            conn.Open();

            VarosListazas();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        void VarosListazas()
        {
            listBoxVaros.Items.Clear();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, nev, lakossag
                                FROM varosok
                                ORDER BY id";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32("id");
                    var nev = reader.GetString("nev");
                    var lakossag = reader.GetInt32("lakossag");
                    var varos = new Varos(id, nev, lakossag);
                    listBoxVaros.Items.Add(varos);
                }
            }
        }

        void LatvanyossagListazas()
        {
            listBoxLatvanyossag.Items.Clear();
            var varos = (Varos)listBoxVaros.SelectedItem;
            var varosId = varos.Id;
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, nev, leiras, ar, varos_id
                                FROM latvanyossagok
                                WHERE varos_id = @id";
            cmd.Parameters.AddWithValue("@id", varosId);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32("id");
                    var nev = reader.GetString("nev");
                    var leiras = reader.GetString("leiras");
                    var ar = reader.GetInt32("ar");
                    var varos_id = reader.GetInt32("varos_id");
                    var latvanyossag = new Latvanyossag(id, nev, leiras, ar, varos_id);
                    listBoxLatvanyossag.Items.Add(latvanyossag);
                }
            }
        }

        private void buttonVarosHozzaad_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(textBoxVarosNev.Text) || string.IsNullOrEmpty(textBoxLakossag.Text))
            {
                MessageBox.Show("Üresen hagyta a város nevét, vagy a lakosságának a számát.");
                
            }

            else if (System.Text.RegularExpressions.Regex.IsMatch(textBoxLakossag.Text, "[^0-9]"))
            {
                MessageBox.Show("Nem számot adott meg a város lakosságának számának!");
                textBoxLakossag.Text = textBoxLakossag.Text.Remove(textBoxLakossag.Text.Length - 1);
            }
           

            else if (!System.Text.RegularExpressions.Regex.IsMatch(textBoxVarosNev.Text, "^[a-zA-Z ]"))
            {
                MessageBox.Show("Csak szöveget adhat meg a város nevének");
                textBoxVarosNev.Text.Remove(textBoxVarosNev.Text.Length - 1);
            }

            else if (textBoxLakossag.Text == "0")
            {
                MessageBox.Show("A városban nem laknak emberek?");
            }

            //sd

            else
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT INTO varosok (nev, lakossag)
                                VALUES (@nev, @lakossag)";
                cmd.Parameters.AddWithValue("@nev", textBoxVarosNev.Text);
                cmd.Parameters.AddWithValue("@lakossag", textBoxLakossag.Text);

                cmd.ExecuteNonQuery();
                VarosListazas();
            }
            
        }

        private void buttonLatvanyossagHozzaad_Click(object sender, EventArgs e)
        {


            if (string.IsNullOrEmpty(textBoxLatvanyossagNev.Text) || string.IsNullOrEmpty(textBoxLeiras.Text) || string.IsNullOrEmpty(textBoxAr.Text))
            {
                MessageBox.Show("Üresen hagyta a látványosság nevét, vagy a leírását, esetleg az árát.");
            }

           else if (System.Text.RegularExpressions.Regex.IsMatch(textBoxAr.Text, "[^0-9]"))
            
            {
              MessageBox.Show("Nem számot adott meg a látványosság árának!");
               textBoxAr.Text = textBoxAr.Text.Remove(textBoxAr.Text.Length - 1);
            }

            else if (!System.Text.RegularExpressions.Regex.IsMatch(textBoxLatvanyossagNev.Text, "^[a-zA-Z ]") )
            {
                MessageBox.Show("Csak szöveget adhat meg a látványosság nevének");
                textBoxLatvanyossagNev.Text.Remove(textBoxLatvanyossagNev.Text.Length - 1);
            }



            else
            {
                var varos = (Varos)listBoxVaros.SelectedItem;
                var id = varos.Id;
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT INTO latvanyossagok (nev, leiras, ar, varos_id)
                                VALUES (@nev, @leiras, @ar, @varos_id)";
                cmd.Parameters.AddWithValue("@nev", textBoxLatvanyossagNev.Text);
                cmd.Parameters.AddWithValue("@leiras", textBoxLeiras.Text);
                cmd.Parameters.AddWithValue("@ar", textBoxAr.Text);
                cmd.Parameters.AddWithValue("@varos_id", id);

                cmd.ExecuteNonQuery();
                LatvanyossagListazas();


            }


        }

        private void buttonVarosTorles_Click(object sender, EventArgs e)
        {
            if (listBoxVaros.SelectedIndex == -1 || listBoxVaros.Items.Count == 0)
            {
                MessageBox.Show("A listát nem töltötted fel, vagy nincs kiválasztva semmi.");
            }
            else
            {
            var varos =(Varos)listBoxVaros.SelectedItem;
            var id = varos.Id;
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM latvanyossagok WHERE varos_id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            var cmdDelete = conn.CreateCommand();
            cmdDelete.CommandText = "DELETE FROM varosok WHERE id = @id";
            cmdDelete.Parameters.AddWithValue("@id", id);
            cmdDelete.ExecuteNonQuery();
           
            VarosListazas();
            //LatvanyossagListazas();
            }
          
        }

        private void buttonLatvanyossagTorles_Click(object sender, EventArgs e)
        {
            if (listBoxLatvanyossag.SelectedIndex == -1 || listBoxLatvanyossag.Items.Count == 0)
            {
                MessageBox.Show("A listát nem töltötted fel, vagy nincs kiválasztva semmi.");
            }
            else
            {
            var latvanyossag = (Latvanyossag)listBoxLatvanyossag.SelectedItem;
            var id = latvanyossag.Id;
            var cmdDelete = conn.CreateCommand();
            cmdDelete.CommandText = "DELETE FROM latvanyossagok WHERE id = @id";
            cmdDelete.Parameters.AddWithValue("@id", id);
            cmdDelete.ExecuteNonQuery();
            //VarosListazas();
            LatvanyossagListazas();
            }
            
        }

        private void listBoxVaros_SelectedIndexChanged(object sender, EventArgs e)
        {
            LatvanyossagListazas();
        }






        private void ListBoxLatvanyossag_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private bool modositas;
        private bool kattintas;


       
        private void BtnModositas_Click(object sender, EventArgs e)
        {
            if (modositas == false)
            {
                kattintas = true;

                if (kattintas == true)
                {
                    btnModositas.Text = "Megerosites";
                    var varosok = (Varos)listBoxVaros.SelectedItem;
                    textBoxVarosNev.Text = varosok.Nev;
                    textBoxLakossag.Text = varosok.Lakossag.ToString();
                    modositas = true;

                }
            }

            else
            {

                if(modositas == true)
                {
                    var varos = (Varos)listBoxVaros.SelectedItem;
                    var id = varos.Id;
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE varosok SET nev=@nev, lakossag=@lakossag Where id=@id";
                    cmd.Parameters.AddWithValue("@nev", textBoxVarosNev.Text);
                    cmd.Parameters.AddWithValue("@lakossag", textBoxLakossag.Text);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    btnModositas.Text = "Módosítás";
                    VarosListazas();
                }
            }
        }
        
        private void BtnModositas2_Click(object sender, EventArgs e)
        {

            if (modositas == false)
            {
                kattintas = true;

                if (kattintas == true)
                {
                    btnModositas2.Text = "Megerosites";
                    var latvanyossagok = (Latvanyossag)listBoxLatvanyossag.SelectedItem;
                    textBoxLatvanyossagNev.Text = latvanyossagok.Nev;
                    textBoxLeiras.Text = latvanyossagok.Leiras;
                    textBoxAr.Text = latvanyossagok.Ar.ToString();
                    modositas = true;

                }
            }

            else
            {

                if (modositas == true)
                {
                    var varos = (Latvanyossag)listBoxLatvanyossag.SelectedItem;
                    var id = varos.Id;
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE latvanyossagok SET nev=@nev, leiras=@leiras , ar=@ar Where id=@id";
                    cmd.Parameters.AddWithValue("@nev", textBoxLatvanyossagNev.Text);
                    cmd.Parameters.AddWithValue("@leiras", textBoxLeiras.Text);
                    cmd.Parameters.AddWithValue("@ar", textBoxAr.Text);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    btnModositas2.Text = "Módosítás";
                    LatvanyossagListazas();
                }
            }

        }
        
    }
}
