﻿(function () {
    'use strict';
    var ctrlId = 'registerDepositTrustlyCtrl';
    APP.controller(ctrlId, ['$scope', '$q', ctrlFactory]);

    function ctrlFactory($scope, $q) {

        $scope.readFields = function() {
            var q = $q.defer();
            q.resolve();
            return q.promise;
        }
    }
})();