var flockApp = angular.module("flockApp", ["ngRoute", "infinite-scroll"]).
config(function ($routeProvider) {
    $routeProvider.
        otherwise({ redirectTo: '/' });
});





