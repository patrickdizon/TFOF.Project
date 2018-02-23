'use strict';

angular.module('tfof', [
        'ngCookies',
        'ngResource',
        'ngSanitize',
        'ngRoute',
        'chart.js',
        'cfp.hotkeys',
        'LocalStorageModule',
        'ODataResources',
        'ui.bootstrap',
        'xeditable',
        'ui.mask',
        'ngMap',
        'cgPrompt',
          //'ui.bootstrap.datetimepicker',
        //'ui.router',
        //'ui.utils',
        //'ui.sortable',
        //'infinite-scroll',
        //'jsonFormatter',
        'angular.filter'
   ]
).run(function(editableOptions,editableThemes) {
  //editableOptions.theme = 'bs3'; // bootstrap3 theme. Can be also 'bs2', 'default'
});
