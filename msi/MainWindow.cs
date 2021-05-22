﻿using FuzzySetLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace msi
{
    public partial class MainWindow : Form
    {
        private string ExamplesPath
        {
            get
            {
                string path = Application.StartupPath;
                int x = path.LastIndexOf("\\");
                for (int i = 0; i < 3; i++)
                {
                    path = path.Remove(path.LastIndexOf("\\"), path.Length - x);
                    x = path.LastIndexOf("\\");
                }
                path = path.Remove(path.LastIndexOf("\\"), path.Length - x);
                path += "\\Examples";
                return path;
            }
        }
        private Button SelectedCandidate = null;
        private Button SelectedJobPosition = null;
        private Button SelectedSkill = null;
        private int LastDataNr = 0;
        private Data CurrentEditedData = new();
        private Data SelectedData = null;
        private List<Data> Sets = new();
        private readonly int precision = 3;

        Func<float, float, float> norm = Norms.Lukasiewicz;
        Func<float, float, float> impl = Implications.Lukasiewicz;
        Func<Bounds[,], Bounds[,], float[,]> dist = Distances.HammingSetDistance;

        



        public MainWindow()
        {
            InitializeComponent();
            LoadDataFromPath(ExamplesPath + "\\example.json");
            LoadDataFromPath(ExamplesPath + "\\diseases.json");
            LoadDataFromPath(ExamplesPath + "\\employees.json");
        }

        private async void LoadDataFromPath(string path)
        {
            using (FileStream openStream = File.OpenRead(path))
            {
                try
                {
                    Data data = await JsonSerializer.DeserializeAsync<DataJsonClass>(openStream);
                    int x = path.LastIndexOf("\\");
                    int dot = path.IndexOf('.');
                    data.Name = path.Substring(x + 1, dot - 1 - x);
                    AddNewData(ref data);
                }
                catch
                {
                    MessageBox.Show("File was not loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AddNewData(ref Data data)
        {
            if (data == null) return;
            data.Number = LastDataNr++;
            Sets.Add(data);
            Button dataButton = new()
            {
                Name = $"data{data.Number}",
                Text = data.Name,
                Size = new Size(143, 23)
            };

            dataButton.Click += (object sender, EventArgs e) =>
            {
                Button button = (Button)sender;
                int x = int.Parse(button.Name.Substring(4));
                foreach (Data data in Sets)
                {
                    if (data.Number == x)
                        SelectedData = data;
                }
                MainWindowLoadData(SelectedData);
                CurrentEditedData = SelectedData.Clone();
                EditWindowLoadData(CurrentEditedData);
                foreach (var control in DataListLayoutPanel.Controls)
                {
                    ((Button)control).BackColor = Color.FromKnownColor(KnownColor.Control);
                }
                button.BackColor = Color.FromKnownColor(KnownColor.ActiveBorder);
            };
            DataListLayoutPanel.Controls.Add(dataButton);
        }

        private void RemoveRowFormSet(InputSet<float> set, string rowName)
        {
            List<string> jobPositions = set.RowNames.ToList();
            int index = jobPositions.IndexOf(rowName);
            jobPositions.Remove(rowName);
            set.RowNames = jobPositions.ToArray();
            int m = 0;
            float[,] numbers = new float[set.RowCount, set.ColCount];
            for (int i = 0; i < set.RowCount + 1; i++)
            {
                if (i != index)
                {
                    for (int j = 0; j < set.ColCount; j++)
                    {
                        numbers[m, j] = set.Numbers[i, j];
                    }
                    m++;
                }
            }
            set.Numbers = numbers;
        }

        private void RemoveColumnFromSet(InputSet<float> set, string columnName)
        {
            List<string> colNames = set.ColNames.ToList();
            int index = colNames.IndexOf(columnName);
            colNames.Remove(columnName);
            set.ColNames = colNames.ToArray();
            float[,] array = new float[set.RowCount, set.ColCount];
            for (int i = 0; i < set.RowCount; i++)
            {
                int m = 0;
                for (int j = 0; j < set.ColCount + 1; j++)
                {
                    if (j != index)
                    {
                        array[i, m++] = set.Numbers[i, j];
                    }
                }
            }
            set.Numbers = array;
        }        

        private void EditRowNameForSet(InputSet<float> set, string rowName, string newRowName)
        {
            List<string> rowNames = set.RowNames.ToList();
            if (rowNames.Contains(newRowName))
            {
                throw new Exception("This row name already exists");
            }
            int index = rowNames.IndexOf(rowName);
            set.RowNames[index] = newRowName;
        }    

        private void EditColumnNameForSet(InputSet<float> set, string columnName, string newColumnName)
        {
            List<string> colNames = set.ColNames.ToList();
            if (colNames.Contains(newColumnName))
            {
                throw new Exception("This column name already exists");
            }
            int index = colNames.IndexOf(columnName);
            set.ColNames[index] = newColumnName;
        }     

        private void AddRowToSet(InputSet<float> set, string newRowName)
        {
            List<string> rowNames = set.RowNames.ToList();
            if (rowNames.Contains(newRowName))
            {
                throw new Exception("This row name already exists");
            }
            rowNames.Add(newRowName);
            set.RowNames = rowNames.ToArray();
            float[,] array = new float[set.RowCount, set.ColCount];
            for (int i = 0; i < set.RowCount - 1; i++)
            {
                for (int j = 0; j < set.ColCount; j++)
                {
                    array[i, j] = set.Numbers[i, j];
                }
            }
            set.Numbers = array;
        }    

        private void AddColumnToSet(InputSet<float> set, string newColumnName)
        {
            List<string> colNames = set.ColNames.ToList();
            if (colNames.Contains(newColumnName))
            {
                throw new Exception("This column name already exists");
            }
            colNames.Add(newColumnName);
            set.ColNames = colNames.ToArray();
            float[,] array = new float[set.RowCount, set.ColCount];
            for (int i = 0; i < set.RowCount; i++)
            {
                for (int j = 0; j < set.ColCount - 1; j++)
                {
                    array[i, j] = set.Numbers[i, j];
                }
            }
            set.Numbers = array;
        }

        private string[,] BoundsToStrings(Bounds[,] bounds)
        {
            var result = new string[bounds.GetLength(0), bounds.GetLength(1)];
            for (int i = 0; i < bounds.GetLength(0); i++)
            {
                for (int j = 0; j < bounds.GetLength(1); j++)
                {
                    result[i, j] = bounds[i, j].ToString();
                }
            }

            return result;
        }

        private float[,] RoundFloats(float[,] tab)
        {
            var result = new float[tab.GetLength(0), tab.GetLength(1)];
            for (int i = 0; i < tab.GetLength(0); i++)
            {
                for (int j = 0; j < tab.GetLength(1); j++)
                {
                    result[i, j] = (float)Math.Round(tab[i, j], precision);
                }
            }

            return result;
        }

        // x < y -> -1
        // x == y -> 0
        // x > y -> 1
        private int CompareFloats(float x, float y)
        {
            int xi = (int)(x * Math.Pow(10, precision));
            int yi = (int)(y * Math.Pow(10, precision));

            if (xi < yi)
            {
                return -1;
            }
            else if (xi == yi)
            {
                return 0;
            }
            else
            {
                return 1;
            }

        }

        private string[,] ChooseBest(float[,] distances, string[] candidates)
        {
            string[,] result = new string[1, distances.GetLength(0)];

            for (int i = 0; i < distances.GetLength(0); i++)
            {
                string bests = candidates[0];
                float min = distances[i, 0];

                for (int j = 1; j < distances.GetLength(1); j++)
                {
                    int cmp = CompareFloats(min, distances[i, j]);
                    if (cmp == 1)
                    {
                        min = distances[i, j];
                        bests = candidates[j];
                    }
                    else if (cmp == 0)
                    {
                        bests += ", " + candidates[j];
                    }

                    result[0, i] = bests;
                }
            }

            return result;
        }
        private void ResizeLayoutPanelButtons(FlowLayoutPanel panel)
        {
            foreach (Button button in panel.Controls)
            {
                button.Width = panel.Width - 30;
            }
        }
    }
}
