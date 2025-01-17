﻿using ClosedXML.Excel;
using GlobalSolusindo.Business.Area;
using GlobalSolusindo.Business.SOW;
using GlobalSolusindo.Business.SOWStatus.Queries;
using GlobalSolusindo.Business.Operator;
using GlobalSolusindo.Business.Operator.Queries;
using GlobalSolusindo.DataAccess;
using GlobalSolusindo.Identity.KategoriJabatan.Queries;
using GlobalSolusindo.Identity.User.Queries;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web;
using GlobalSolusindo.Business.TaskList.Queries;
using GlobalSolusindo.Business.Project;
using GlobalSolusindo.Business.BTS.Queries;
using GlobalSolusindo.Business.Technology.Queries;


namespace GlobalSolusindo.Api.Models
{

    public class SOWExport
    {
        protected string _sheetName;
        protected string _fileName;


        public HttpResponseMessage Export(GlobalSolusindoDb Db, tblM_User user, string fileName, TaskListSearchFilter filter)
        {
            _fileName = fileName;
            //CREATE WORKBOOK
            var workbook = new XLWorkbook();
            DataTable SOW = new DataTable("SOWUpload"); //DataTable Name = Worksheet Name
            SOWExportDTO obj = new SOWExportDTO();
            //Setup Column Names
            foreach (var item in obj.GetType().GetProperties())
            {
                SOW.Columns.Add(item.Name);
            }
            workbook.AddWorksheet(SOW); // NO DATA = ADD Worksheet to WorkBook

            //Worksheet Properties
            var worksheet = workbook.Worksheet(1);
            worksheet.Columns().Width = 15;


            //Validation Table
            using (var ProjectQuery2 = new ProjectQuery())
            {
                //SETUP TABLE PROJECT
                DataTable validationTableProject = new DataTable();
                validationTableProject.TableName = "Project";
                //SETUP COLUMN
                LOVDTO objProject = new LOVDTO();
                foreach (var item in objProject.GetType().GetProperties())
                {
                    validationTableProject.Columns.Add(item.Name);
                }
                var dataProject = ProjectQuery2.GetProjectByPM(user.User_PK);
                DataRow drProject;
                int startcell = 2, endcell = 2;
                foreach (var item in dataProject)
                {
                    drProject = validationTableProject.NewRow();
                    drProject["Id"] = item.Project_PK;
                    drProject["Name"] = item.Project_PK + "-" + item.OperatorTitle + "-" + item.VendorTitle + "-" + item.DeliveryAreaTitle;
                    validationTableProject.Rows.Add(drProject);
                    endcell++;
                }
                var worksheetProject = workbook.AddWorksheet(validationTableProject);
                worksheet.Column(2).SetDataValidation().List(worksheetProject.Range("B" + startcell.ToString() + ":B" + endcell.ToString()), true);


                //SETUP TABLE VALIDATION BTS
                using (var BTSQuery = new BTSQuery())
                {
                    //SETUP TABLE BTS
                    DataTable validationTableBTS = new DataTable();
                    validationTableBTS.TableName = "BTS";
                    //SETUP COLUMN
                    LOVDTO objBTS = new LOVDTO();
                    foreach (var item in objBTS.GetType().GetProperties())
                    {
                        validationTableBTS.Columns.Add(item.Name);
                    }
                    var dataBTS = BTSQuery.GetQuery();
                    DataRow drBTS;
                    startcell = 2; endcell = 2;
                    foreach (var item in dataBTS)
                    {
                        drBTS = validationTableBTS.NewRow();
                        drBTS["Id"] = item.BTS_PK;
                        drBTS["Name"] = item.TowerID + "-" + item.Name;
                        validationTableBTS.Rows.Add(drBTS);
                        endcell++;
                    }
                    var worksheetBTS = workbook.AddWorksheet(validationTableBTS);
                    worksheet.Column(4).SetDataValidation().List(worksheetBTS.Range("B" + startcell.ToString() + ":B" + endcell.ToString()), true);


                    //SETUP TABLE TECHNOLOGY
                    using (var TechnologyQuery = new TechnologyQuery())
                    {
                        //SETUP TABLE Technology
                        DataTable validationTableTechnology = new DataTable();
                        validationTableTechnology.TableName = "Technology";
                        //SETUP COLUMN
                        LOVDTO objTechnology = new LOVDTO();
                        foreach (var item in objTechnology.GetType().GetProperties())
                        {
                            validationTableTechnology.Columns.Add(item.Name);
                        }
                        var dataTechnology = TechnologyQuery.GetQuery();
                        DataRow drTechnology;
                        startcell = 2; endcell = 2;
                        foreach (var item in dataTechnology)
                        {
                            drTechnology = validationTableTechnology.NewRow();
                            drTechnology["Id"] = item.Technology_PK;
                            drTechnology["Name"] = item.Title;
                            validationTableTechnology.Rows.Add(drTechnology);
                            endcell++;
                        }
                        var worksheetTechnology = workbook.AddWorksheet(validationTableTechnology);
                        worksheet.Column(5).SetDataValidation().List(worksheetTechnology.Range("B" + startcell.ToString() + ":B" + endcell.ToString()), true);
                    }


                    using (var UserQuery = new UserQuery())
                    {
                        //SETUP TABLE Teamlead
                        DataTable validationTableUser = new DataTable();
                        validationTableUser.TableName = "Teamlead";
                        //SETUP COLUMN
                        LOVDTO objUser = new LOVDTO();
                        foreach (var item in objUser.GetType().GetProperties())
                        {
                            validationTableUser.Columns.Add(item.Name);
                        }
                        var dataUser = UserQuery.GetByJabatanAndProject(1, user.User_PK);
                        DataRow drUser;
                        startcell = 2; endcell = 2;
                        foreach (var item in dataUser)
                        {
                            drUser = validationTableUser.NewRow();
                            drUser["Id"] = item.User_PK;
                            drUser["Name"] = item.Name;
                            validationTableUser.Rows.Add(drUser);
                            endcell++;
                        }
                        var worksheetUser = workbook.AddWorksheet(validationTableUser);
                        worksheet.Column(12).SetDataValidation().List(worksheetUser.Range("B" + startcell.ToString() + ":B" + endcell.ToString()), true);

                        //SETUP TABLE PLOQC
                        DataTable validationTableUserPLOQC = new DataTable();
                        validationTableUserPLOQC.TableName = "RNO";
                        //SETUP COLUMN
                        foreach (var item in objUser.GetType().GetProperties())
                        {
                            validationTableUserPLOQC.Columns.Add(item.Name);
                        }
                        var dataUserPLOQC = UserQuery.GetByJabatanAndProject(6, user.User_PK); // PLOQC JABATAN???
                        DataRow drUserPLOQC;
                        startcell = 2; endcell = 2;
                        foreach (var item in dataUserPLOQC)
                        {
                            drUserPLOQC = validationTableUserPLOQC.NewRow();
                            drUserPLOQC["Id"] = item.User_PK;
                            drUserPLOQC["Name"] = item.Name;
                            validationTableUserPLOQC.Rows.Add(drUserPLOQC);
                            endcell++;
                        }
                        var worksheetUserPLOQC = workbook.AddWorksheet(validationTableUserPLOQC);
                        worksheet.Column(13).SetDataValidation().List(worksheetUser.Range("B" + startcell.ToString() + ":B" + endcell.ToString()), true);

                        //SETUP TABLE RF
                        DataTable validationTableUserRF = new DataTable();
                        validationTableUserRF.TableName = "RF";
                        //SETUP COLUMN
                        foreach (var item in objUser.GetType().GetProperties())
                        {
                            validationTableUserRF.Columns.Add(item.Name);
                        }
                        var dataUserRF = UserQuery.GetByJabatanAndProject(5, user.User_PK);
                        DataRow drUserRF;
                        startcell = 2; endcell = 2;
                        foreach (var item in dataUserRF)
                        {
                            drUserRF = validationTableUserRF.NewRow();
                            drUserRF["Id"] = item.User_PK;
                            drUserRF["Name"] = item.Name;
                            validationTableUserRF.Rows.Add(drUserRF);
                            endcell++;
                        }
                        var worksheetUserRF = workbook.AddWorksheet(validationTableUserRF);
                        worksheet.Column(14).SetDataValidation().List(worksheetUser.Range("B" + startcell.ToString() + ":B" + endcell.ToString()), true);

                        //SETUP TABLE Rigger
                        DataTable validationTableUserRigger = new DataTable();
                        validationTableUserRigger.TableName = "Rigger";
                        //SETUP COLUMN
                        foreach (var item in objUser.GetType().GetProperties())
                        {
                            validationTableUserRigger.Columns.Add(item.Name);
                        }
                        var dataUserRigger = UserQuery.GetByJabatanAndProject(3, user.User_PK);
                        DataRow drUserRigger;
                        startcell = 2; endcell = 2;
                        foreach (var item in dataUserRigger)
                        {
                            drUserRigger = validationTableUserRigger.NewRow();
                            drUserRigger["Id"] = item.User_PK;
                            drUserRigger["Name"] = item.Name;
                            validationTableUserRigger.Rows.Add(drUserRigger);
                            endcell++;
                        }
                        var worksheetUserRigger = workbook.AddWorksheet(validationTableUserRigger);
                        worksheet.Column(15).SetDataValidation().List(worksheetUser.Range("B" + startcell.ToString() + ":B" + endcell.ToString()), true);

                        //SETUP TABLE DT
                        DataTable validationTableUserDT = new DataTable();
                        validationTableUserDT.TableName = "DriveTester";
                        //SETUP COLUMN
                        foreach (var item in objUser.GetType().GetProperties())
                        {
                            validationTableUserDT.Columns.Add(item.Name);
                        }
                        var dataUserDT = UserQuery.GetByJabatanAndProject(2, user.User_PK);
                        DataRow drUserDT;
                        startcell = 2; endcell = 2;
                        foreach (var item in dataUserDT)
                        {
                            drUserDT = validationTableUserDT.NewRow();
                            drUserDT["Id"] = item.User_PK;
                            drUserDT["Name"] = item.Name;
                            validationTableUserDT.Rows.Add(drUserDT);
                            endcell++;
                        }
                        var worksheetUserDT = workbook.AddWorksheet(validationTableUserDT);
                        worksheet.Column(16).SetDataValidation().List(worksheetUser.Range("B" + startcell.ToString() + ":B" + endcell.ToString()), true);

                    }



                }





                //SETUP TABLE TEAMLEAD, RNO, RIGGER, TESTER




            }

            MemoryStream memoryStream = GetStream(workbook);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(memoryStream.ToArray())
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue
                   ("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            response.Content.Headers.ContentDisposition =
                   new ContentDispositionHeaderValue("attachment")
                   {
                       FileName = $"{_fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx"
                   };

            return response;


        }
        private MemoryStream GetStream(XLWorkbook excelWorkbook)
        {
            MemoryStream fs = new MemoryStream();
            excelWorkbook.SaveAs(fs);
            fs.Position = 0;
            return fs;
        }
    }
}