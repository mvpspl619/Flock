'use strict';

flockApp.directive('userLike', function () {
    return {
        restrict: 'E',
        replace: true,
        templateUrl: '/JSApplication/Templates/userLike.html'
    };
});
