'use strict';

flockApp.directive('simpleQuack', function() {
    return {
        restrict: 'E',
        replace: true,
        templateUrl: '/JSApplication/Templates/simpleQuack.html'
    };
});
