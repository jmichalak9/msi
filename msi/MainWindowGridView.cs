
using System;
using System.Windows.Forms;


namespace msi
{
	public partial class MainWindow : Form
	{
		private static void DisplayDataGridView<T>(InputSet<T> set, DataGridView dataGrid)
		{
			ClearDataGridView(dataGrid);
			if (set.ColCount == 0)
				return;

			for (int i = 0; i < set.ColCount; i++)
			{
				dataGrid.Columns.Add(i.ToString(), set.ColNames[i]);
			}

			if (set.RowCount == 0)
				return;

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
		private static void ClearDataGridView(DataGridView dataGrid)
		{
			dataGrid.DataSource = null;
			dataGrid.Columns.Clear();
			dataGrid.Rows.Clear();
		}

		private void QSetEdit_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			AnyChanges = true;
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
			AnyChanges = true;
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
	}
}
