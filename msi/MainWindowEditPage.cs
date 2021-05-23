
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace msi
{
	public partial class MainWindow : Form
	{
		private bool AnyChanges = false;
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

		private void ClearEditWindowPanels()
		{
			JobPositionLayoutPanel.Controls.Clear();
			CandidateLayoutPanel.Controls.Clear();
			SkillLayoutPanel.Controls.Clear();
		}

		private void EditPage_Enter(object sender, EventArgs e)
		{
			if (SelectedData != null)
			{
				CurrentEditedData = SelectedData.Clone();
			}
			else
			{
				CurrentEditedData = new Data
				{
					Name = "NewData"
				};
			}
			EditWindowLoadData(CurrentEditedData);
			AnyChanges = false;
		}

		private void EditPage_Leave(object sender, EventArgs e)
		{
			if (AnyChanges)
			{
				DialogResult dialogResult = MessageBox.Show("Do you want to save?", "Save data", MessageBoxButtons.YesNo);
				if (dialogResult == DialogResult.Yes)
				{
					SaveData();
				}
			}
		}

		private void SaveData()
		{
			if (string.IsNullOrEmpty(CurrentEditedData.Name))
			{
				MessageBox.Show("Data name can not be empty. Data name is now \"EditedData\".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				CurrentEditedData.Name = "EditedData";
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
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			SaveData();
			TabControlMenu.SelectedTab = MainPage;
		}

		private void NameEditTextBox_TextChanged(object sender, EventArgs e)
		{
			AnyChanges = true;
			CurrentEditedData.Name = ((TextBox)sender).Text;
		}

		#region FillLayoutPanel
		private void FillJobPositionPanel(Data data)
		{
			foreach (string rowname in data.R.RowNames)
			{
				Button newButton = new()
				{
					Name = $"jobposition{rowname}",
					Text = rowname,
					Size = new Size(JobPositionLayoutPanel.Width - 30, 40)
				};

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
				Button newButton = new()
				{
					Name = $"candidate{rowname}",
					Text = rowname,
					Size = new Size(CandidateLayoutPanel.Width - 30, 40)
				};

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
				Button newButton = new()
				{
					Name = $"skill{colname}",
					Text = colname,
					Size = new Size(SkillLayoutPanel.Width - 30, 40)
				};

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
		#endregion

		#region DeleteElementFromLayoutPanel
		private void DeleteCandidateButton_Click(object sender, EventArgs e)
		{
			AnyChanges = true;
			if (SelectedCandidate == null)
			{
				MessageBox.Show("Select candidate", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			string text = SelectedCandidate.Text;
			RemoveRowFormSet(CurrentEditedData.Q, text);
			EditWindowLoadData(CurrentEditedData);
		}

		private void DeleteJobPositionButton_Click(object sender, EventArgs e)
		{
			AnyChanges = true;
			if (SelectedJobPosition == null)
			{
				MessageBox.Show("Select candidate", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			string text = SelectedJobPosition.Text;
			RemoveRowFormSet(CurrentEditedData.R, text);
			EditWindowLoadData(CurrentEditedData);
		}

		private void DeleteSkillButton_Click(object sender, EventArgs e)
		{
			AnyChanges = true;
			if (SelectedSkill == null)
			{
				MessageBox.Show("Select candidate", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			string text = SelectedSkill.Text;
			RemoveColumnFromSet(CurrentEditedData.Q, text);
			RemoveColumnFromSet(CurrentEditedData.R, text);
			EditWindowLoadData(CurrentEditedData);
		}
		#endregion

		#region EditElementInLayoutPanel
		private void EditCandidateButton_Click(object sender, EventArgs e)
		{
			AnyChanges = true;
			if (SelectedCandidate == null)
			{
				MessageBox.Show("Select candidate", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (string.IsNullOrEmpty(CandidateTextBox.Text))
			{
				MessageBox.Show("Text is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			try
			{
				EditRowNameForSet(CurrentEditedData.Q, SelectedCandidate.Text, CandidateTextBox.Text);
			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			CandidateTextBox.Text = "";
			EditWindowLoadData(CurrentEditedData);
		}

		private void EditJobPositionButton_Click(object sender, EventArgs e)
		{
			AnyChanges = true;
			if (SelectedJobPosition == null)
			{
				MessageBox.Show("Select candidate", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (string.IsNullOrEmpty(JobPositionTextBox.Text))
			{
				MessageBox.Show("Text is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			try
			{
				EditRowNameForSet(CurrentEditedData.R, SelectedJobPosition.Text, JobPositionTextBox.Text);
			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			JobPositionTextBox.Text = "";
			EditWindowLoadData(CurrentEditedData);
		}

		private void EditSkillButton_Click(object sender, EventArgs e)
		{
			AnyChanges = true;
			if (SelectedSkill == null)
			{
				MessageBox.Show("Select candidate", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (string.IsNullOrEmpty(SkillTextBox.Text))
			{
				MessageBox.Show("Text is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			try
			{
				EditColumnNameForSet(CurrentEditedData.R, SelectedSkill.Text, SkillTextBox.Text);
				EditColumnNameForSet(CurrentEditedData.Q, SelectedSkill.Text, SkillTextBox.Text);
			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			SkillTextBox.Text = "";
			EditWindowLoadData(CurrentEditedData);
		}
		#endregion

		#region AddElementToLayoutPanel
		private void AddCandidateButton_Click(object sender, EventArgs e)
		{
			AnyChanges = true;
			if (string.IsNullOrEmpty(CandidateTextBox.Text))
			{
				MessageBox.Show("Text is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			try
			{
				AddRowToSet(CurrentEditedData.Q, CandidateTextBox.Text);
			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			CandidateTextBox.Text = "";
			EditWindowLoadData(CurrentEditedData);
		}

		private void AddJobPositionButton_Click(object sender, EventArgs e)
		{
			AnyChanges = true;
			if (string.IsNullOrEmpty(JobPositionTextBox.Text))
			{
				MessageBox.Show("Text is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			try
			{
				AddRowToSet(CurrentEditedData.R, JobPositionTextBox.Text);
			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			JobPositionTextBox.Text = "";
			EditWindowLoadData(CurrentEditedData);
		}

		private void AddSkillButton_Click(object sender, EventArgs e)
		{
			AnyChanges = true;
			if (string.IsNullOrEmpty(SkillTextBox.Text))
			{
				MessageBox.Show("Text is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			try
			{
				AddColumnToSet(CurrentEditedData.R, SkillTextBox.Text);
				AddColumnToSet(CurrentEditedData.Q, SkillTextBox.Text);
			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			SkillTextBox.Text = "";
			EditWindowLoadData(CurrentEditedData);
		}
		#endregion

		#region LayoutPanelsResizing
		private void CandidateLayoutPanel_Resize(object sender, EventArgs e)
		{
			ResizeLayoutPanelButtons((FlowLayoutPanel)sender);
		}

		private void JobPositionLayoutPanel_Resize(object sender, EventArgs e)
		{
			ResizeLayoutPanelButtons((FlowLayoutPanel)sender);
		}

		private void SkillLayoutPanel_Resize(object sender, EventArgs e)
		{
			ResizeLayoutPanelButtons((FlowLayoutPanel)sender);
		}
		#endregion
	}
}
