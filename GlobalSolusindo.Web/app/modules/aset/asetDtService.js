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
        .factory('asetDtService', aset);

    aset.$inject = ['DatatableService'];

    function aset(ds) {
        var self = this;

        self.init = function (ctrl) {
            var titleColumnIndex = 1;
            return ds.init("#aset", "aset/search", {
                extendRequestData: {
                    pageIndex: 1,
                    pageSize: 10
                },
                order: [titleColumnIndex, "asc"],
                columns: [
                    {
                        "orderable": false,
                        "data": "aset_pk"
                    },
                    {
                        "data": "asetId"
                    },
                    {
                        "data": "kategoriAsetName"
                    },
                    {
                        "data": "name"
                    },
                    {
                        "orderable": false,
                        "className": "text-center",
                        "render": function (data) {
                            return "<button id='show' rel='tooltip' title='Detail' data-placement='left' class='btn btn-success'><i class='fa fa-info'></i></button> " +
                                "<button id='view' rel='tooltip' title='Edit' data-placement='left' class='btn btn-warning'><i class='fas fa-pencil-alt'></i></button> " +
                                "<button id='userHistory' rel='tooltip' title='Asset History' data-placement='left' class='btn btn-success'><i class='fas fa-info'></i></button> " +
                                "<button id='delete' rel='tooltip' title='Delete' data-placement='left' class='btn btn-danger'><i class='fa fa-trash-alt'></i></button>"
                        }
                    }
                ],
                exportButtons: {
                    columns: [1, 2, 3],
                    title: "Aset"
                }
            });
        };
        return self;
    }

})();