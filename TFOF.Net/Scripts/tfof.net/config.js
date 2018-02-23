'use strict';

//Setting HTML5 Location Mode
angular.module('tfof').config(['$locationProvider',
  function($locationProvider) {
    $locationProvider.hashPrefix('!');
}
]);

angular.module('tfof').config(function ($resourceProvider) {
  $resourceProvider.defaults.stripTrailingSlashes = false;
});


angular.module('tfof').config(function ($httpProvider) {
    $httpProvider.defaults.headers.post['X-CSRFToken'] = $('input[name=__RequestVerificationToken]').val();
    $httpProvider.defaults.headers.put['X-CSRFToken'] = $('input[name=__RequestVerificationToken]').val();
    $httpProvider.defaults.headers.common['X-CSRFToken'] = $('input[name=__RequestVerificationToken]').val();
    $httpProvider.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';
  }
);
