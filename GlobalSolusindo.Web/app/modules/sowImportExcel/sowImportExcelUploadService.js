﻿(function () {
    'use strict';

    /**
     * @ngdoc function
     * @name app.service:dashboardService
     * @description
     * # dashboardService
     * Service of the app
     */

    angular
        .module('global-solusindo')
        .factory('SOWImportExcelUploadService', SOWImportExcelUploadService);

    SOWImportExcelUploadService.$inject = ['$state', 'HttpService', 'uiService', 'validationService'];

    function SOWImportExcelUploadService($state, http, ui, validation) {
        var self = this;
        var SOWImportExcelCtrl;

        function goToListPage() {
            $state.go('app.sowList');
        }

        self.save = function (model) {
            http.post('sow/import', model).then(function (res) {
                SOWImportExcelCtrl.uploadResults = res.data;
                if (res.success) {
                    ui.alert.success('Upload process complete.');
                } else {
                    ui.alert.error(res.message);
                    if (res.data && res.data.errors)
                        validation.serverValidation(res.data.errors);
                }
            });
        };

        self.downloadTpl = function () {
            debugger;
            http.downloadFile('sow/export', { keyword: '' }).then(function (data) {
                var contentType = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';
                var linkElement = document.createElement('a');
                try {
                    var blob = new Blob([data], { type: contentType });
                    var url = window.URL.createObjectURL(blob);

                    linkElement.setAttribute('href', url);
                    linkElement.setAttribute("download", "SOWUpload.xlsx");

                    var clickEvent = new MouseEvent("click", {
                        "view": window,
                        "bubbles": true,
                        "cancelable": false
                    });
                    linkElement.dispatchEvent(clickEvent);
                } catch (ex) {
                    console.log(ex);
                }
            });


        };
        self.init = function (ctrl) {
            SOWImportExcelCtrl = ctrl;
            angular.element('#uploadButton').on('click', function () {
                self.save(SOWImportExcelCtrl.model);
            });

            angular.element('#downloadButton').on('click', function () {
                self.downloadTpl();
            });
        };

        return self;
    }

})();