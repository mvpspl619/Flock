'use strict';

flockApp.directive('comment', function () {
    return {
        restrict: 'E',
        replace: true,
        templateUrl: '/JSApplication/Templates/comment.html'
    };
});