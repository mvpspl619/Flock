var flockApp = angular.module("flockApp", ["ngRoute"]).
config(function ($routeProvider) {
    $routeProvider.
        otherwise({ redirectTo: '/' });
});





