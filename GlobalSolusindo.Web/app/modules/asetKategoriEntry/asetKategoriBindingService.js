(function () {
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
        .factory('AsetKategoriBindingService', AsetKategoriBindingService);

    AsetKategoriBindingService.$inject = ['HttpService', '$state'];

    function AsetKategoriBindingService(http, $state) {
        var self = this;
        var controller = {};

        self.applyBinding = function (id) {
            return http.get('asetKategori/form/' + id);
        };

        self.init = function (ctrl) {
            controller = ctrl;
            var id = ctrl.stateParam.id;
            return new Promise(function (resolve, reject) {
                self.applyBinding(id).then(function (res) {
                    controller.model = res.data.model;
                    controller.formControls = res.data.formControls;
                    resolve(res);
                });
            });
        };

        return self;
    }

})();