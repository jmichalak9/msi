using FuzzySetLib;
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
        private void MainWindowLoadData(Data data)
        {
            ClearDataGridView(QSetDataGrid);
            ClearDataGridView(RSetDataGrid);
            if (data == null) return;
            DisplayDataGridView(data.Q, QSetDataGrid);
            DisplayDataGridView(data.R, RSetDataGrid);
        }

        private void MainPage_Enter(object sender, EventArgs e)
        {
            MainWindowLoadData(SelectedData);
        }

        private void AddNewDataButton_Click(object sender, EventArgs e)
        {
            SelectedData = null;
            CurrentEditedData = new Data();
            EditWindowLoadData(CurrentEditedData);
            TabControlMenu.SelectedTab = EditPage;
        }

        private void DeleteDataButton_Click(object sender, EventArgs e)
        {
            if (SelectedData == null)
            {
                MessageBox.Show("Choose dataset", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int index = Sets.IndexOf(SelectedData);
            Sets.Remove(SelectedData);
            DataListLayoutPanel.Controls.RemoveByKey($"data{index}");
            SelectedData = null;
            MainWindowLoadData(SelectedData);
        }

        private void EditDataButton_Click(object sender, EventArgs e)
        {
            if (SelectedData == null)
            {
                MessageBox.Show("Choose dataset", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            CurrentEditedData = SelectedData.Clone();
            EditWindowLoadData(CurrentEditedData);
            TabControlMenu.SelectedTab = EditPage;
        }

        private void SaveDataButton_Click(object sender, EventArgs e)
        {
            if (SelectedData == null)
            {
                MessageBox.Show("Choose dataset", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Stream newStream;
            SaveFileDialog saveFileDialog = new();

            string path = ExamplesPath;
            saveFileDialog.Filter = "json file (*.json*)|*.json*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.InitialDirectory = path;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((newStream = saveFileDialog.OpenFile()) != null)
                {
                    try
                    {
                        saveFileDialog.DefaultExt = "json";
                        var serivalizedScene = JsonSerializer.Serialize((DataJsonClass)SelectedData);
                        StreamWriter streamWriter = new(newStream);
                        streamWriter.Write(serivalizedScene);
                        streamWriter.Close();
                        MessageBox.Show("Save done!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show("File was not saved", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    newStream.Close();
                }
            }
        }

        private void LoadDataButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new())
            {
                string path = ExamplesPath;
                openFileDialog.Filter = "json file (*.json*)|*.json*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.InitialDirectory = path;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadDataFromPath(openFileDialog.FileName);
                }
            }
        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            if (SelectedData == null)
            {
                MessageBox.Show("Fill data table", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            float[,] Q = SelectedData.Q.Numbers; //candidates
            float[,] R = SelectedData.R.Numbers; //positions

            float[,] distances = Approximations.FuzzyRelationBasedApproximation(norm, impl, dist, Q, R,
                out Bounds[,] objectBounds, out Bounds[,] propertyBounds);

            distances = RoundFloats(distances);

            InputSet<float> distanceSet = new()
            {
                Numbers = distances,
                ColNames = SelectedData.Q.RowNames,
                RowNames = SelectedData.R.RowNames
            };

            InputSet<string> objectBundsSet = new()
            {
                Numbers = BoundsToStrings(objectBounds),
                ColNames = SelectedData.R.ColNames,
                RowNames = SelectedData.R.RowNames
            };

            InputSet<string> propertyBoundsSet = new()
            {
                Numbers = BoundsToStrings(propertyBounds),
                ColNames = SelectedData.Q.ColNames,
                RowNames = SelectedData.Q.RowNames
            };

            DisplayDataGridView(distanceSet, ThirdStep);
            DisplayDataGridView(objectBundsSet, FirstStepSet);
            DisplayDataGridView(propertyBoundsSet, SecondStep);

            InputSet<string> resultsSet = new()
            {
                Numbers = ChooseBest(distances, SelectedData.Q.RowNames),
                RowNames = new string[]
                {
                    "Best Candidates"
                },
                ColNames = SelectedData.R.RowNames
            };

            DisplayDataGridView(resultsSet, Result);
        }

        private void lukasiewiczRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button.Checked)
            {
                norm = Norms.Lukasiewicz;
                impl = Implications.Lukasiewicz;
            }
        }

        private void productRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button.Checked)
            {
                norm = Norms.Product;
                impl = Implications.GoguenGaines;
            }
        }

        private void standardRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button.Checked)
            {
                norm = Norms.Standard;
                impl = Implications.Godel;
            }
        }

        private void hammingRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button.Checked)
            {
                dist = Distances.HammingSetDistance;
            }
        }

        private void euclideanRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button.Checked)
            {
                dist = Distances.EuclideanSetDistance;
            }
        }
    }
}