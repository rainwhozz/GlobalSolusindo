﻿using GlobalSolusindo.Api.Models;
using GlobalSolusindo.Business.PO;
using GlobalSolusindo.Business.PO.DML;
using GlobalSolusindo.Business.PO.Queries;
using GlobalSolusindo.DataAccess.PO.EntryForm;
using GlobalSolusindo.Identity.User;
using GlobalSolusindo.Identity.User.DML;
using GlobalSolusindo.Identity.User.EntryForm;
using GlobalSolusindo.Identity.User.Queries;
using Kairos;
using Kairos.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Transactions;
using System.Web.Http;

namespace GlobalSolusindo.Api.Controllers
{
    public class POController : ApiControllerBase
    {
        public POController()
        {
        }

        [Route("po/{id}")]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            //string accessType = "PO_ViewAll";
            //ThrowIfUserHasNoRole(accessType);
            using (POQuery poQuery = new POQuery(Db))
            {
                var data = poQuery.GetByPrimaryKey(id);
                SaveLog("PO", "Get", JsonConvert.SerializeObject(new { primaryKey = id }));
                return Ok(new SuccessResponse(data));
            }
        }

        [Route("po/form/{id}")]
        [HttpGet]
        public IHttpActionResult GetForm(int id)
        {
            //string accessType = "PO_ViewAll";
            //if (id > 0)
            //    ThrowIfUserHasNoRole(accessType);
            using (POEntryDataProvider poEntryDataProvider = new POEntryDataProvider(Db, ActiveUser, new POQuery(Db)))
            {
                var data = poEntryDataProvider.Get(id);
                SaveLog("PO", "GetForm", JsonConvert.SerializeObject(new { primaryKey = id }));
                return Ok(new SuccessResponse(data));
            }
        }

        [Route("po/search")]
        [HttpGet]
        public IHttpActionResult Search([FromUri]POSearchFilter filter)
        {
            //string accessType = "PO_ViewAll";
            //ThrowIfUserHasNoRole(accessType);
            if (filter == null)
                throw new KairosException("Missing search filter parameter");

            using (var poSearch = new POSearch(Db))
            {
                var data = poSearch.GetDataByFilter(filter);
                return Ok(new SuccessResponse(data));
            }
        }
      
        

        [Route("po")]
        [HttpDelete]
        public IHttpActionResult Delete([FromBody] List<int> ids)
        {
            if (ids == null)
                throw new KairosException("Missing parameter: 'ids'");

            string accessType = "";
            ThrowIfUserHasNoRole(accessType);

            using (var poDeleteHandler = new PODeleteHandler(Db, ActiveUser))
            {
                using (var transaction = new TransactionScope())
                {
                    var result = new List<DeleteResult<int>>();

                    foreach (var id in ids)
                    {
                        result.Add(poDeleteHandler.Execute(id, Base.DeleteMethod.Soft));
                    }
                    transaction.Complete();
                    return Ok(new SuccessResponse(result, DeleteMessageBuilder.BuildMessage(result)));
                }
            }
        }

        

        [Route("po/import")]
        [HttpPost]
        public IHttpActionResult Import([FromBody]POImportDTO poImportDTO)
        {
            string accessType = "";
            ThrowIfUserHasNoRole(accessType);
            if (poImportDTO == null)
                throw new KairosException("Missing model parameter");
            var importResult = new POImportExcelHandler(Db, ActiveUser, new POValidator(), new POFactory(Db, ActiveUser), new POQuery(Db)).ExecuteImport(poImportDTO, DateTime.Now);
            return Ok(new SuccessResponse(importResult));
        }

        [Route("po/export")]
        [HttpPost]
        public HttpResponseMessage Export([FromBody]POSearchFilter filter)
        {
            //string accessType = "PO_ViewAll";
            //ThrowIfUserHasNoRole(accessType);
            if (filter == null)
                throw new KairosException("Missing search filter parameter");

            using (var poSearch = new POSearch(Db))
            {
                var data = poSearch.GetDataByFilter(filter);
                AbstractDataExportBridge expor = new AbstractDataExportBridge();
                //expor.WriteData<PODTO>(data.Records);
                return expor.Export<PODTO>(data.Records, "PoUpload", "POUpload");
            }
        }
    }
}