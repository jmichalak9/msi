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
        private Data CurrentEditedData = new Data();
        private Data SelectedData = null;
        private List<Data> Sets = new List<Data>();
        private void DisplayDataGridView<T>(InputSet<T> set, DataGridView dataGrid)
        {
            ClearDataGridView(dataGrid);
            if (set.ColCount == 0) return;
            for (int i = 0; i < set.ColCount; i++)
            {
                dataGrid.Columns.Add(i.ToString(), set.ColNames[i]);
            }
            if (set.RowCount == 0) return;
            dataGrid.Rows.Add(set.RowCount);
            for (int i = 0; i < set.RowCount; i++)
            {
                dataGrid.Rows[i].HeaderCell.Value = set.RowNames[i];
                for (int j = 0; j < set.ColCount; j++)
                {
                    dataGrid.Rows[i].Cells[j].Value = set.Numbers[i, j];
                }
            }
        }

        private void MainWindowLoadData(Data data)
        {
            ClearDataGridView(QSetDataGrid);
            ClearDataGridView(RSetDataGrid);
            if (data == null) return;
            DisplayDataGridView(data.Q, QSetDataGrid);
            DisplayDataGridView(data.R, RSetDataGrid);
        }

        private void ClearDataGridView(DataGridView dataGrid)
        {
            dataGrid.DataSource = null;
            dataGrid.Columns.Clear();
            dataGrid.Rows.Clear();
        }

        private void EditWindowLoadData(Data data)
        {
            ClearDataGridView(QSetEdit);
            ClearDataGridView(RSetEdit);
            ClearEditWindowPanels();
            if (data == null) return;
            NameEditTextBox.Text = data.Name;
            DisplayDataGridView(data.Q, QSetEdit);
            DisplayDataGridView(data.R, RSetEdit);
            FillJobPositionPanel(data);
            FillCandidatePanel(data);
            FillSkillPanel(data);
        }

        private void FillJobPositionPanel(Data data)
        {
            foreach (string rowname in data.R.RowNames)
            {
                Button newButton = new Button();
                newButton.Name = $"jobposition{rowname}";
                newButton.Text = rowname;
                newButton.Size = new Size(90, 23);
                newButton.Click += (object sender, EventArgs e) =>
                {
                    SelectedJobPosition = (Button)sender;
                    foreach (Button button in JobPositionLayoutPanel.Controls)
                    {
                        button.BackColor = Color.FromKnownColor(KnownColor.Control);

                    }
                    SelectedJobPosition.BackColor = Color.FromKnownColor(KnownColor.ActiveBorder);
                };
                JobPositionLayoutPanel.Controls.Add(newButton);
            }
        }

        private void FillCandidatePanel(Data data)
        {
            foreach (string rowname in data.Q.RowNames)
            {
                Button newButton = new Button();
                newButton.Name = $"candidate{rowname}";
                newButton.Text = rowname;
                newButton.Size = new Size(90, 23);
                newButton.Click += (object sender, EventArgs e) =>
                {
                    SelectedCandidate = (Button)sender;
                    foreach (Button button in CandidateLayoutPanel.Controls)
                    {
                        button.BackColor = Color.FromKnownColor(KnownColor.Control);

                    }
                    SelectedCandidate.BackColor = Color.FromKnownColor(KnownColor.ActiveBorder);
                };
                CandidateLayoutPanel.Controls.Add(newButton);
            }
        }

        private void FillSkillPanel(Data data)
        {
            foreach (string colname in data.Q.ColNames)
            {
                Button newButton = new Button();
                newButton.Name = $"skill{colname}";
                newButton.Text = colname;
                newButton.Size = new Size(90, 23);
                newButton.Click += (object sender, EventArgs e) =>
                {
                    SelectedSkill = (Button)sender;
                    foreach (Button button in SkillLayoutPanel.Controls)
                    {
                        button.BackColor = Color.FromKnownColor(KnownColor.Control);

                    }
                    SelectedSkill.BackColor = Color.FromKnownColor(KnownColor.ActiveBorder);
                };
                SkillLayoutPanel.Controls.Add(newButton);
            }
        }

        private void ClearEditWindowPanels()
        {
            JobPositionLayoutPanel.Controls.Clear();
            CandidateLayoutPanel.Controls.Clear();
            SkillLayoutPanel.Controls.Clear();
        }

        public MainWindow()
        {
            InitializeComponent();
            LoadDataFromPath(ExamplesPath + "\\Example");
            LoadDataFromPath(ExamplesPath + "\\Medyczny");
            LoadDataFromPath(ExamplesPath + "\\Przyklad1");
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
                MessageBox.Show("Choose dataset");
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
                MessageBox.Show("Choose dataset");
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
                MessageBox.Show("Choose dataset");
                return;
            }
            Stream newStream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            string path = ExamplesPath;
            saveFileDialog.Filter = "all files (*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.InitialDirectory = path;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((newStream = saveFileDialog.OpenFile()) != null)
                {
                    try
                    {
                        var serivalizedScene = JsonSerializer.Serialize((DataJsonClass)SelectedData);
                        StreamWriter streamWriter = new StreamWriter(newStream);
                        streamWriter.Write(serivalizedScene);
                        streamWriter.Close();
                        MessageBox.Show("Zapisano pomyślnie");
                    }
                    catch
                    {
                        MessageBox.Show("Nie udało się zapisać");
                        return;
                    }
                    newStream.Close();
                }
            }
        }

        private void LoadDataButton_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                string path = ExamplesPath;
                openFileDialog.Filter = "all files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.InitialDirectory = path;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadDataFromPath(openFileDialog.FileName);
                }
            }
        }


        private async void LoadDataFromPath(string path)
        {
            using (FileStream openStream = File.OpenRead(path))
            {
                try
                {
                    Data data = await JsonSerializer.DeserializeAsync<DataJsonClass>(openStream);
                    int x = path.LastIndexOf("\\");
                    data.Name = path.Substring(x + 1);
                    AddNewData(ref data);
                }
                catch
                {
                    MessageBox.Show("File was not loaded");
                }
            }
        }

        private void AddNewData(ref Data data)
        {
            if (data == null) return;
            data.Number = LastDataNr++;
            Sets.Add(data);
            Button dataButton = new Button();
            dataButton.Name = $"data{data.Number}";
            dataButton.Text = data.Name;
            dataButton.Size = new Size(143, 23);
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

        private void QSetEdit_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGrid = (DataGridView)sender;
            var oldData = CurrentEditedData.Q.Numbers[dataGrid.CurrentCell.RowIndex, dataGrid.CurrentCell.ColumnIndex];
            try
            {
                float value = float.Parse((string)dataGrid.CurrentCell.Value);

                if (value >= 0 && value <= 1)
                {
                    CurrentEditedData.Q.Numbers[dataGrid.CurrentCell.RowIndex, dataGrid.CurrentCell.ColumnIndex] = value;
                }
                else
                {
                    CurrentEditedData.Q.Numbers[dataGrid.CurrentCell.RowIndex, dataGrid.CurrentCell.ColumnIndex] = oldData;
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void RSetEdit_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGrid = (DataGridView)sender;
            var oldData = CurrentEditedData.R.Numbers[dataGrid.CurrentCell.RowIndex, dataGrid.CurrentCell.ColumnIndex];
            try
            {
                float value = float.Parse((string)dataGrid.CurrentCell.Value);
                if (value >= 0 && value <= 1)
                {
                    CurrentEditedData.R.Numbers[dataGrid.CurrentCell.RowIndex, dataGrid.CurrentCell.ColumnIndex] = value;
                }
                else
                {
                    CurrentEditedData.R.Numbers[dataGrid.CurrentCell.RowIndex, dataGrid.CurrentCell.ColumnIndex] = oldData;
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentEditedData.Name))
            {
                MessageBox.Show("Data name can not be empty");
                return;
            }
            if (SelectedData == null)
            {
                SelectedData = CurrentEditedData.Clone();
                AddNewData(ref SelectedData);
            }
            else
            {
                SelectedData = Sets[Sets.IndexOf(SelectedData)] = CurrentEditedData.Clone();
                ((Button)DataListLayoutPanel.Controls.Find($"data{SelectedData.Number}", false).First()).Text = SelectedData.Name;
            }
            MainWindowLoadData(SelectedData);
            TabControlMenu.SelectedTab = MainPage;
        }

        private void NameEditTextBox_TextChanged(object sender, EventArgs e)
        {
            CurrentEditedData.Name = ((TextBox)sender).Text;
        }

        private void EditPage_Enter(object sender, EventArgs e)
        {
            if (SelectedData != null)
            {
                CurrentEditedData = SelectedData.Clone();
            }
            else
            {
                CurrentEditedData = new Data();
            }
            EditWindowLoadData(CurrentEditedData);
        }

        private void DeleteCandidateButton_Click(object sender, EventArgs e)
        {
            if (SelectedCandidate == null)
            {
                MessageBox.Show("Select candidate");
                return;
            }
            Button button = (Button)sender;
            string text = SelectedCandidate.Text;
            RemoveRowFormSet(CurrentEditedData.Q, text);
            EditWindowLoadData(CurrentEditedData);
        }

        private void DeleteJobPositionButton_Click(object sender, EventArgs e)
        {
            if (SelectedJobPosition == null)
            {
                MessageBox.Show("Select candidate");
                return;
            }
            Button button = (Button)sender;
            string text = SelectedJobPosition.Text;
            RemoveRowFormSet(CurrentEditedData.R, text);
            EditWindowLoadData(CurrentEditedData);
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

        private void DeleteSkillButton_Click(object sender, EventArgs e)
        {
            if (SelectedSkill == null)
            {
                MessageBox.Show("Select candidate");
                return;
            }
            Button button = (Button)sender;
            string text = SelectedSkill.Text;
            RemoveColumnFromSet(CurrentEditedData.Q, text);
            RemoveColumnFromSet(CurrentEditedData.R, text);
            EditWindowLoadData(CurrentEditedData);
        }

        private void RemoveColumnFromSet(InputSet<float> set, string columnName)
        {
            List<string> colNames = set.ColNames.ToList();
            int index = colNames.IndexOf(columnName);
            colNames.Remove(columnName);
            set.ColNames = colNames.ToArray();
            int m = 0;
            float[,] array = new float[set.RowCount, set.ColCount];
            for (int i = 0; i < set.RowCount; i++)
            {
                m = 0;
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

        private void EditCandidateButton_Click(object sender, EventArgs e)
        {
            if (SelectedCandidate == null)
            {
                MessageBox.Show("Select candidate");
                return;
            }
            if (string.IsNullOrEmpty(CandidateTextBox.Text))
            {
                MessageBox.Show("Text is empty");
                return;
            }
            Button button = (Button)sender;
            try
            {
                EditRowNameForSet(CurrentEditedData.Q, SelectedCandidate.Text, CandidateTextBox.Text);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            CandidateTextBox.Text = "";
            EditWindowLoadData(CurrentEditedData);
        }

        private void EditJobPositionButton_Click(object sender, EventArgs e)
        {
            if (SelectedJobPosition == null)
            {
                MessageBox.Show("Select candidate");
                return;
            }
            if (string.IsNullOrEmpty(JobPositionTextBox.Text))
            {
                MessageBox.Show("Text is empty");
                return;
            }
            Button button = (Button)sender;
            try
            {
                EditRowNameForSet(CurrentEditedData.R, SelectedJobPosition.Text, JobPositionTextBox.Text);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            JobPositionTextBox.Text = "";
            EditWindowLoadData(CurrentEditedData);
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

        private void EditSkillButton_Click(object sender, EventArgs e)
        {
            if (SelectedSkill == null)
            {
                MessageBox.Show("Select candidate");
                return;
            }
            if (string.IsNullOrEmpty(SkillTextBox.Text))
            {
                MessageBox.Show("Text is empty");
                return;
            }
            Button button = (Button)sender;
            try
            {
                EditColumnNameForSet(CurrentEditedData.R, SelectedSkill.Text, SkillTextBox.Text);
                EditColumnNameForSet(CurrentEditedData.Q, SelectedSkill.Text, SkillTextBox.Text);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            SkillTextBox.Text = "";
            EditWindowLoadData(CurrentEditedData);
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

        private void AddCandidateButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CandidateTextBox.Text))
            {
                MessageBox.Show("Text is empty");
                return;
            }
            try
            {
                AddRowToSet(CurrentEditedData.Q, CandidateTextBox.Text);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            CandidateTextBox.Text = "";
            EditWindowLoadData(CurrentEditedData);
        }

        private void AddJobPositionButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(JobPositionTextBox.Text))
            {
                MessageBox.Show("Text is empty");
                return;
            }
            try
            {
                AddRowToSet(CurrentEditedData.R, JobPositionTextBox.Text);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            JobPositionTextBox.Text = "";
            EditWindowLoadData(CurrentEditedData);
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

        private void AddSkillButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SkillTextBox.Text))
            {
                MessageBox.Show("Text is empty");
                return;
            }
            try
            {
                AddColumnToSet(CurrentEditedData.R, SkillTextBox.Text);
                AddColumnToSet(CurrentEditedData.Q, SkillTextBox.Text);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            SkillTextBox.Text = "";
            EditWindowLoadData(CurrentEditedData);
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
            for(int i=0;i<bounds.GetLength(0);i++)
            {
                for(int j=0;j<bounds.GetLength(1);j++)
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
                    result[i, j] = (float)Math.Round(tab[i, j], 3);
                }
            }

            return result;
        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            if (SelectedData == null)
            {
                MessageBox.Show("Fill data table");
                return;
            }

            float[,] Q = SelectedData.Q.Numbers; //kandydaci
            float[,] R = SelectedData.R.Numbers; //stanowiska

            Bounds[,] objectBounds, propertyBounds;

            float[,] result = Approximations.FuzzyRelationBasedApproximation(Norms.Lukasiewicz, Implications.Lukasiewicz,
                Distances.EuclideanSetDistance, Q, R, out objectBounds, out propertyBounds);

            result = RoundFloats(result);

            InputSet<float> resultSet = new InputSet<float>
            {
                Numbers = result,
                ColNames = SelectedData.Q.RowNames,
                RowNames = SelectedData.R.RowNames
            };

            InputSet<string> objectBundsSet = new InputSet<string>
            {
                Numbers = BoundsToStrings(objectBounds),
                ColNames = SelectedData.R.ColNames,
                RowNames = SelectedData.R.RowNames
            };

            InputSet<string> propertyBoundsSet = new InputSet<string>
            {
                Numbers = BoundsToStrings(propertyBounds),
                ColNames = SelectedData.Q.ColNames,
                RowNames = SelectedData.Q.RowNames
            };

            DisplayDataGridView(resultSet, ThirdStep);
            DisplayDataGridView(objectBundsSet, FirstStepSet);
            DisplayDataGridView(propertyBoundsSet, SecondStep);
        }
    }
}
