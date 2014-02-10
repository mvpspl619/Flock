'use strict';

flockApp.directive('quack', function() {
    return {
        restrict: 'E',
        replace: true,
        templateUrl: '/JSApplication/Templates/quack.html'
    };
});
