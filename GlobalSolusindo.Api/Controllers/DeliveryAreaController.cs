﻿using GlobalSolusindo.Business.DeliveryArea;
using GlobalSolusindo.Business.DeliveryArea.DML;
using GlobalSolusindo.Business.DeliveryArea.EntryForm;
using GlobalSolusindo.Business.DeliveryArea.Queries;
using GlobalSolusindo.DataAccess;
using Kairos;
using Kairos.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Transactions;
using System.Web.Http;

namespace GlobalSolusindo.Api.Controllers
{
    public class DeliveryAreaController : ApiControllerBase
    {
        private const string createRole = "DeliveryArea_Input";
        private const string updateRole = "DeliveryArea_Edit";
        private const string readRole = "DeliveryArea_ViewAll";
        private const string deleteRole = "DeliveryArea_Delete";
        private const string importRole = "DeliveryArea_Import";
        public DeliveryAreaController()
        {
        }

        [Route("deliveryArea/{id}")]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            ThrowIfUserHasNoRole(readRole);
            using (DeliveryAreaQuery deliveryAreaQuery = new DeliveryAreaQuery(Db))
            {
                var data = deliveryAreaQuery.GetByPrimaryKey(id);
                SaveLog("DeliveryArea", "Get", JsonConvert.SerializeObject(new { primaryKey = id }));
                return Ok(new SuccessResponse(data));
            }
        }

        [Route("deliveryArea/form/{id}")]
        [HttpGet]
        public IHttpActionResult GetForm(int id)
        {
            if (id > 0)
                ThrowIfUserHasNoRole(readRole);
            using (DeliveryAreaEntryDataProvider deliveryAreaEntryDataProvider = new DeliveryAreaEntryDataProvider(Db, ActiveUser, AccessControl, new DeliveryAreaQuery(Db)))
            {
                var data = deliveryAreaEntryDataProvider.Get(id);
                SaveLog("DeliveryArea", "GetForm", JsonConvert.SerializeObject(new { primaryKey = id }));
                return Ok(new SuccessResponse(data));
            }
        }

        [Route("deliveryArea/search")]
        [HttpGet]
        public IHttpActionResult Search([FromUri]DeliveryAreaSearchFilter filter)
        {
            ThrowIfUserHasNoRole(readRole);
            if (filter == null)
                throw new KairosException("Missing search filter parameter");

            using (var deliveryAreaSearch = new DeliveryAreaSearch(Db))
            {
                var data = deliveryAreaSearch.GetDataByFilter(filter);
                return Ok(new SuccessResponse(data));
            }
        }

        [Route("deliveryArea")]
        [HttpPost]
        public IHttpActionResult Create([FromBody]DeliveryAreaDTO deliveryArea)
        {
            ThrowIfUserHasNoRole(createRole);
            if (deliveryArea == null)
                throw new KairosException("Missing model parameter");

            if (deliveryArea.DeliveryArea_PK != 0)
                throw new KairosException("Post method is not allowed because the requested primary key is must be '0' (zero) .");
            using (var deliveryAreaCreateHandler = new DeliveryAreaCreateHandler(Db, ActiveUser, new DeliveryAreaValidator(), new DeliveryAreaFactory(Db, ActiveUser), new DeliveryAreaQuery(Db), AccessControl))
            {
                using (var transaction = new TransactionScope())
                {
                    var saveResult = deliveryAreaCreateHandler.Save(deliveryAreaDTO: deliveryArea, dateStamp: DateTime.Now);
                    transaction.Complete();
                    if (saveResult.Success)
                        return Ok(new SuccessResponse(saveResult.Model, saveResult.Message));
                    return Ok(new ErrorResponse(ServiceStatusCode.ValidationError, saveResult.ValidationResult, saveResult.Message));
                }
            }
        }

        [Route("deliveryArea")]
        [HttpPut]
        public IHttpActionResult Update([FromBody]DeliveryAreaDTO deliveryArea)
        {
            ThrowIfUserHasNoRole(updateRole);
            if (deliveryArea == null)
                throw new KairosException("Missing model parameter");

            if (deliveryArea.DeliveryArea_PK == 0)
                throw new KairosException("Put method is not allowed because the requested primary key is '0' (zero) .");

            using (var deliveryAreaUpdateHandler = new DeliveryAreaUpdateHandler(Db, ActiveUser, new DeliveryAreaValidator(), new DeliveryAreaFactory(Db, ActiveUser), new DeliveryAreaQuery(Db), AccessControl))
            {
                using (var transaction = new TransactionScope())
                {
                    var saveResult = deliveryAreaUpdateHandler.Save(deliveryArea, DateTime.Now);
                    transaction.Complete();
                    if (saveResult.Success)
                        return Ok(new SuccessResponse(saveResult.Model, saveResult.Message));
                    return Ok(new ErrorResponse(ServiceStatusCode.ValidationError, saveResult.ValidationResult, saveResult.Message));
                }
            }
        }

        [Route("deliveryArea")]
        [HttpDelete]
        public IHttpActionResult Delete([FromBody] List<int> ids)
        {
            ThrowIfUserHasNoRole(deleteRole);

            if (ids == null)
                throw new KairosException("Missing parameter: 'ids'");

            using (var deliveryAreaDeleteHandler = new DeliveryAreaDeleteHandler(Db, ActiveUser))
            {
                using (var transaction = new TransactionScope())
                {
                    var result = new List<DeleteResult<tblM_DeliveryArea>>();

                    foreach (var id in ids)
                    {
                        result.Add(deliveryAreaDeleteHandler.Execute(id, Base.DeleteMethod.Soft));
                    }
                    transaction.Complete();
                    return Ok(new SuccessResponse(result, DeleteMessageBuilder.BuildMessage(result)));
                }
            }
        }
    }
}
