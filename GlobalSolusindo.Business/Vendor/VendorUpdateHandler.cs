﻿using GlobalSolusindo.Base;
using GlobalSolusindo.Business.Vendor.EntryForm;
using GlobalSolusindo.DataAccess;
using GlobalSolusindo.Identity;
using Kairos.Data;
using System;

namespace GlobalSolusindo.Business.Vendor
{
    public class VendorUpdateHandler : UpdateOperation
    {
        private VendorValidator vendorValidator;
        private VendorFactory vendorFactory;
        private VendorQuery vendorQuery;
        private VendorEntryDataProvider vendorEntryDataProvider;

        public VendorUpdateHandler(GlobalSolusindoDb db, tblM_User user, VendorValidator vendorValidator, VendorFactory vendorFactory, VendorQuery vendorQuery, AccessControl accessControl) : base(db, user)
        {
            this.vendorValidator = vendorValidator;
            this.vendorFactory = vendorFactory;
            this.vendorQuery = vendorQuery;
            this.vendorEntryDataProvider = new VendorEntryDataProvider(db, user, accessControl, vendorQuery);
        }

        private void Initialize(VendorValidator vendorValidator, VendorFactory vendorFactory)
        {
            this.vendorValidator = vendorValidator;
            this.vendorFactory = vendorFactory;
        }

        public void UpdateVendor(VendorDTO vendorDTO, DateTime dateStamp)
        {
            if (vendorDTO == null)
                throw new ArgumentNullException("Vendor model is null.");
            tblM_Vendor vendor = vendorFactory.CreateFromDbAndUpdateFromDTO(vendorDTO, dateStamp);
        }

        public SaveResult<VendorEntryModel> Save(VendorDTO vendorDTO, DateTime dateStamp)
        {
            ModelValidationResult validationResult = vendorValidator.Validate(vendorDTO);
            bool success = false;
            VendorEntryModel model = null;

            if (validationResult.IsValid)
            {
                success = true;
                UpdateVendor(vendorDTO, dateStamp);
                Db.SaveChanges();
                model = vendorEntryDataProvider.Get(vendorDTO.Vendor_PK);
            }

            return new SaveResult<VendorEntryModel>
            {
                Success = success,
                Message = validationResult.IsValid ? "Data successfully updated." : "Validation error occured.",
                Model = model,
                ValidationResult = validationResult
            };
        }
    }
}