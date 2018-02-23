using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.IO;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;
using TFOF.Areas.Core.Helpers;

namespace TFOF.Areas.Core.Services
{
    public class ExportService
    {
        public void ExportToExcel(GridView gridView, string fileName)
        {
            /// <summary> 
            /// Exports the datagridview values to Excel. 
            /// </summary> 
      
            // Creating a Excel object. 
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            try
            {

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "ExportedFromDatGrid";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (int i = 0; i < gridView.Rows.Count - 1; i++)
                {
                    for (int j = 0; j < gridView.Columns.Count; j++)
                    {
                        // Excel index starts from 1,1. As first Row would have the Column headers, adding a condition check. 
                        if (cellRowIndex == 1)
                        {
                            worksheet.Cells[cellRowIndex, cellColumnIndex] = gridView.Columns[j].HeaderText;
                        }
                        else
                        {
                            worksheet.Cells[cellRowIndex, cellColumnIndex] = gridView.Rows[i].Cells[j].Text.ToString();
                        }
                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }

               
                    workbook.SaveAs(fileName);
            }
            catch
            {
                throw new System.ArgumentNullException("Could not export to Excel!");
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }

        }

        public static FileHelper ExportToCSV(string fileName, DataTable dataTable, string userId = null, string seperator = ",")
        {
            if(string.IsNullOrEmpty(userId))
            {
                userId = Guid.NewGuid().ToString();
            }
            FileHelper fileModel = new FileHelper(fileName, userId);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                sb.Append(dataTable.Columns[i]);
                if (i < dataTable.Columns.Count - 1)
                    sb.Append(seperator); 
            }
            sb.AppendLine();
            foreach (DataRow dr in dataTable.Rows)
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    sb.Append(dr[i].ToString());

                    if (i < dataTable.Columns.Count - 1)
                        sb.Append(seperator);
                }
                sb.AppendLine();
            }
            fileModel.Write(sb.ToString());
            return fileModel;
        }
    }

}