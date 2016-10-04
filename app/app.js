'use strict';

// Declare app level module which depends on views, and components
angular.module('brighterTmrwApp', [
  'ngRoute',
  'ui.router',
  'ui.materialize',
  'brighterTmrwApp.navigation',
  'brighterTmrwApp.home'
  // 'brighterTmrwApp.view1',
  // 'brighterTmrwApp.view2',
  // 'brighterTmrwApp.version'
]).
config(['$stateProvider', '$urlRouterProvider', '$locationProvider', '$routeProvider',
  function($stateProvider, $urlRouterProvider, $locationProvider, $routeProvider) {

    $stateProvider
                .state('landing', {
                    abstract: true,
                    url: "",
                    templateUrl: "navigation/navigation.html",
                    authenticate: false
                })
                .state('landing.home', {
                    url: "/home",
                    templateUrl: "home/home.html",
                    authenticate: false,
                });
                // .state('landing.expertise', {
                //     url: "/expertise",
                //     templateUrl: "landing-home/landing-home.html",
                //     authenticate: false,
                // })
                // .state('landing.foryou', {
                //     url: "/foryou",
                //     templateUrl: "landing-home/landing-home.html",
                //     authenticate: false,
                // })
                // .state('landing.about', {
                //     url: "/about",
                //     templateUrl: "landing-home/landing-home.html",
                //     authenticate: false,
                // })
                // .state('landing.contact', {
                //     url: "/contact",
                //     templateUrl: "landing-home/landing-home.html",
                //     authenticate: false,
                // })
                // .state('landing.login', {
                //     url: "/login",
                //     templateUrl: "login/login.html",
                //     authenticate: false
                // })
                // .state('landing.forgot-password', {
                //     url: "/forgot-password?password_token",
                //     templateUrl: "forgot-password/forgotpassword.html",
                //     authenticate: false
                // })
                // .state('landing.register', {
                //     url: "/register",
                //     templateUrl: "register/register.html",
                //     authenticate: false
                // })
                // .state('landing.validate', {
                //     url: "/validate?validation_token&submitted",
                //     templateUrl: "validate/validate.html",
                //     authenticate: false
                // })
                // .state('app', {
                //     abstract: true,
                //     url: "",
                //     templateUrl: "navigation/navigation.html",
                //     authenticate: true
                // })
                // .state('app.home', {
                //     url: "/home",
                //     templateUrl: "home/home.html",
                //     authenticate: true
                // })
                // .state('app.providers', {
                //     url: "/providers",
                //     templateUrl: "providers/providers.html",
                //     authenticate: true
                // })
                // .state('app.datasources', {
                //     url: "/datasources",
                //     templateUrl: "datasources/datasources.html",
                //     authenticate: true
                // });

            $urlRouterProvider.otherwise('/home');
}]);
