using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using iWebSite_ComeIndus.Areas.Content.Models;
using iWebSite_ComeIndus.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace iWebSite_ComeIndus.Extension
{
    [Route("Content/excel/UnivGraduation")]
    public class FileClass : _BaseController
    {
        [HttpGet]
        public FileStreamResult Get(UnivGraduationModel Model)
        {
            var memoryStream = new MemoryStream();

            using (var document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
            {
                //Excel
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                //Excel 分頁(Sheet)
                var sheets = workbookPart.Workbook.AppendChild(new Sheets());

                sheets.Append(new Sheet()
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "UnivGraduation"
                });

                //
                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                //Get資料
                sheetData = GetUnivGraduation(sheetData, Model.Year);
            }

            //Return
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        //下載Excel 同年份 不同國家
        private SheetData GetUnivGraduation(SheetData sheetData, string year)
        {
            //標題列
            var row = new Row();

            //編號標題列
            row.Append(
                    new Cell()
                    {
                        CellValue = new CellValue("流水號"),
                        DataType = CellValues.String
                    }
                );

            //國家種類
            var sqlStr = string.Format("select [CountryNo], [CountryName] from Countries");
            var data = _DB_GetData(sqlStr);

            #region 預備程式碼
            //讀取標題列 自動讀取
            //for (int i = 0; i < countryGradData.Columns.Count; i++)
            //{
            //    row.Append(
            //        new Cell()
            //        {
            //            CellValue = new CellValue(countryGradData.Columns[i].ColumnName),
            //            DataType = CellValues.String
            //        }
            //    );
            //}
            #endregion

            //設定標題列
            row.Append(
                new Cell()
                {
                    CellValue = new CellValue("CountryName"),
                    DataType = CellValues.String
                },
                new Cell()
                {
                    CellValue = new CellValue("DeptName"),
                    DataType = CellValues.String
                },
                new Cell()
                {
                    CellValue = new CellValue("GraduationNumber"),
                    DataType = CellValues.String
                },
                new Cell()
                {
                    CellValue = new CellValue("GraduationYear"),
                    DataType = CellValues.String
                }
            );

            //標題列 加入至Excel
            sheetData.AppendChild(row);

            //
            foreach (DataRow dataRow in data.Rows)
            {
                //國家編號
                string countryNo = dataRow.ItemArray.GetValue(0).ToString();

                //國家名稱
                string countryName = dataRow.ItemArray.GetValue(1).ToString();

                sqlStr = string.Format(
                "select [DeptName], [GraduationNumber], [GraduationYear] " +
                "from[dbo].[Department] as a " +
                "inner join( " +
                "select[CountryDeptNo],[CountryNo], [DeptNo] " +
                "from[dbo].[CountryDepartment] " +
                "where CountryNo = {0} " +
                ") as b " +
                "on b.DeptNo = a.DeptNo " +
                "inner join( " +
                "select * " +
                "from[dbo].[Graduation] " +
                "where GraduationYear = {1} " +
                ") as c " +
                "on b.CountryDeptNo = c.CountryDeptNo", countryNo, year);

                var countryGradData = _DB_GetData(sqlStr);

                //讀取DB資料(直向)
                for (int i = 0; i < countryGradData.Rows.Count; i++)
                {
                    //New row
                    row = new Row();

                    //流水號
                    row.Append(
                        new Cell()
                        {
                            CellValue = new CellValue(i + 1),
                            DataType = CellValues.Number
                        },
                        new Cell()
                        {
                            CellValue = new CellValue(countryName),
                            DataType = CellValues.String
                        }
                    );

                    //資料(橫向)
                    for (int x = 0; x < countryGradData.Rows[i].ItemArray.Length; x++)
                    {
                        row.Append(
                            new Cell()
                            {
                                CellValue = new CellValue(countryGradData.Rows[i].ItemArray.GetValue(x).ToString()),
                                DataType = CellValues.String
                            }
                        );
                    }

                    //Append
                    sheetData.AppendChild(row);
                }
            }

            //Return
            return sheetData;
        }
    }
}
