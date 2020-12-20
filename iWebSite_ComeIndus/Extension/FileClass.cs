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
    public class FileClass : _BaseController { }

    [Route("Content/excel/UnivGraduation")]
    public class UnitGraduationExcel : _BaseController
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
                    Name = "UnivGraduation",
                });

                //
                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                //Get資料
                sheetData = GetUnivGraduation(sheetData, Model.Year, Model.Country);
            }

            //Return
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        //下載Excel 同年份 不同國家
        private SheetData GetUnivGraduation(SheetData sheetData, string Year = "-1", string Country = "-1")
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

            //SQL條件 國家
            string sqlCountryCondition = Country == "-1" ? "" : CountryCondition(Country);

            //SQL條件
            string sqlYearCondition = Year == "-1" ? "" : YearCondition(Year);

            //國家種類
            var sqlStr = string.Format("select [CountryNo], [CountryName] from Countries {0}", sqlCountryCondition);
            var data = _DB_GetData(sqlStr);

            #region 設定標題列

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
                    CellValue = new CellValue("國家名稱"),
                    DataType = CellValues.String,
                },
                new Cell()
                {
                    CellValue = new CellValue("科系名稱"),
                    DataType = CellValues.String
                },
                new Cell()
                {
                    CellValue = new CellValue("畢業年份"),
                    DataType = CellValues.String
                },
                new Cell()
                {
                    CellValue = new CellValue("畢業人數"),
                    DataType = CellValues.String
                }
            );        

            row.Append(new FontSize() { Val = 36 });

            //標題列 加入至Excel
            sheetData.AppendChild(row);
            
            #endregion

            //
            foreach (DataRow dataRow in data.Rows)
            {
                //國家編號
                string countryNo = dataRow.ItemArray.GetValue(0).ToString();

                //國家名稱
                string countryName = dataRow.ItemArray.GetValue(1).ToString();

                sqlStr = string.Format(
                "select [DeptName], [GraduationYear], [GraduationNumber] " +
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
                "{1} " +
                ") as c " +
                "on b.CountryDeptNo = c.CountryDeptNo", countryNo, sqlYearCondition);

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
                            CellValue = new CellValue((i + 1).ToString()),
                            DataType = CellValues.String,
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

        /// <summary>
        /// 多國條件
        /// </summary>
        /// <param name="Countries"></param>
        /// <returns></returns>
        private string CountryCondition(string Countries)
        {
            string[] Country = Countries.Split(',');

            string sqlCondition = "";

            for(int i = 0; i < Country.Length; i++)
            {
                if(i == 0)
                {
                    sqlCondition = string.Format("where CountryNo = {0} ", Country[i]);
                }
                else
                {
                    sqlCondition += string.Format("or CountryNo = {0} ", Country[i]);
                }
            }

            return sqlCondition;
        }

        /// <summary>
        /// 不同年條件
        /// </summary>
        /// <param name="Years"></param>
        /// <returns></returns>
        private string YearCondition(string Years)
        {
            string[] Year = Years.Split(',');

            string sqlCondition = "";

            for (int i = 0; i < Year.Length; i++)
            {
                if (i == 0)
                {
                    sqlCondition = string.Format("where GraduationYear = {0} ", Year[i]);
                }
                else
                {
                    sqlCondition += string.Format("or GraduationYear = {0} ", Year[i]);
                }
            }

            return sqlCondition;
        }
    }
}
