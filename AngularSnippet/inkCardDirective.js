//card directive
(function () {
    "use strict";

    angular.module(APPNAME)
        .directive('contact', myFunction);
    
    function myFunction(){
            return {
                restrict: 'E',
                scope: {
                    cardTemp: '=class',
                    person: '=ngModel'
                },

                link: function ($scope) {
                    $scope.flip = null;
                    $scope.rotateCard = function () {
                        $scope.flip = 'hover manual-flip';
                    };
                    $scope.unRotateCard = function () {
                        $scope.flip = null;
                    }
                    $scope.$watch('cardTemp', function (cardTemp) {
                        $scope.newUrl = '/Scripts/sabio/directives/inkCard/templates/' + cardTemp + '.html';
                    });

                },
                templateUrl: $scope.newUrl
            };
        };
})();