/**
 * Created by brandon on 9/14/15.
 */

'use strict';

angular.module('brighterTmrwApp.navigation', ['ngRoute'])
    .controller('NavigationCtrl', ['$scope', '$state',
        function($scope, $state) {

            // $scope.$on('$viewContentLoaded', function(event) {
            //     var stateName = $state.current.name;
            //     var offsety = 0;
            //
            //     switch(stateName){
            //         case "landing.expertise":
            //             offsety = $("#expertise").offset().top;
            //             break;
            //         case "landing.foryou":
            //             offsety = $("#foryou").offset().top;
            //             break;
            //         case "landing.about":
            //             offsety = $("#about").offset().top;
            //             break;
            //         case "landing.contact":
            //             offsety = $("#contact").offset().top;
            //             break;
            //     }
            //
            //     $('html, body').animate({
            //         scrollTop: offsety
            //     }, 1000);
            //
            // });

        }]);
