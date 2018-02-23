'use strict';

//Basic service used for account REST endpoint
angular.module('tfof').factory('BasicService', ['$resource', '$odataresource', '$odata',
    function($resource, $odataresource, $odata) {
        var basicService = { 
            //A parameter to be passed to another basic controller
            package: null 
        }
        basicService.api = function(url) {

            return $resource(url + '/:id', { id: '@id', __request_id: Date.parse(Date()) / 1000 },
                   {
                       'delete': { method: 'DELETE', params: { id: "@id" } },
                       'update': { method: 'PUT' }
                   }
            )
        };

        basicService.setHtmlTitle = function(title) {
            if (title) {
                $('title').html(title);
            }
        }
        
        basicService.oDataFilter = function(endpoint) {
            return  { 
                count: null // count of records
                , didSearch: false
                , expand: "" // $expand in oData
                //filter object 
                /*
                  Name:  {
                       values: [],
                       comparator: equals|notequals|startsWith|notStartsWith|containsAny|notContainsAny|containsAll|notContainsAll|endsWith|notEndsWith,
                       operator: And|Or,
                       dataType: Boolean|Byte|DateTime|Decimal|Double|Guid|Int32|String|Single,
                       inputType: checkbox|date|number|text
                       selectedOptions: []
                       isRestricted: true|false
                       options: {value: '', label: ''}
                    }
                */
                , filters: {} // Filters contain keys that have array values like so { Id: [1,2,3]}
                , id: "" // the id of the filter set for storage and reference.
                , infiniteScroll: false
                , limit: 30 //$top in oData
                , load: false //performs load on page load and refresh.
                , locationSearch: false //Enables Query String Searching
                , include: [] // $select in oData
                , nextSet: 0
                , offset: 0 // $skip in oData
                , order: []
                , orderApplied: [] //the applied order by find()
                , orderLabels: {} //pretty labels of columns
                , selectall: false
                , selectedOptions: {} //contains an array the size of the number of options available. Value indicates selected state {"Fruits": ["Apple", null, "Orange", ...]}
                , showSpinner: true
                , showPagination: true
                , stats: [] //contains user defined statistical information from the query perfromed.
                , singleSearchFields: []
                , singleSearch: null
                , translations: {} //Stores coded field translations. Example: sfilters.translations["TranslationName"][a.FieldName]
                , url: angular.isDefined(endpoint) ? endpoint : null
                , useLocalStorage: true
                , viewOffset: 0
            }
        }

        basicService.oDataValue = function(value, dataType) {
            //Enforce correct value fomat for oData query.
            if (angular.isDefined(dataType) && dataType == "DateTime") {
                return new Date(value);
            } else if (angular.isDefined(dataType) && dataType == "Decimal") {
                return value.replace(",","");
            } else {
                return value;
            }
        }

        basicService.setFilter = function (filters) {
            localStorage.setItem(filters.id, JSON.stringify(filters));
        }

        basicService.removeFilter = function (filters) {
            localStorage.removeItem(filters.id);
        }

        basicService.getFilter = function (filters) {
            if(angular.isDefined(localStorage[filters.id])) {
                return JSON.parse(localStorage[filters.id])
            }
            return null;
        }

        basicService.oData = function(filters) {
            //Takes in oDataFilter
            //Prepares the oData object with the given filter
            var oDataResource = $odataresource(filters.url, {}, {}, { persistence: true, isodatav4: false });
            var oData = oDataResource.odata();
            var orPredicates = [];
            var andPredicates = [];

            //Apply selectedOptions to filters
            angular.forEach(filters.filters, function (o, k) {
                if (angular.isDefined(o.options) && _.isArray(o.options) && o.options.length > 0) {
                    o.values = _.pluck(_.where(o.options, { selected: true }), 'value');
                }
            });

            //Determine type of search: eq or range
            angular.forEach(filters.filters, function(a, k){
                //Replace __ with / to search related models.
                var p = k.replace(/__/g, '/');

                if(!angular.isArray(a.values))  a.values = [a.values];
                

                if (a.comparator.endsWith('range')) {
                    //Pass entire array to oDataFunction if range.
                    var f = basicService.oDataFunction(filters, k, p, a.values);
                    if (_.isArray(f) && f.length > 0) {
                       //if (a.comparator.startsWith('!')) {
                            //orPredicates.push(f)
                        if (a.operator == 'And') {
                            andPredicates.push(a.comparator.startsWith('!') ? $odata.Predicate.or(f) : $odata.Predicate.and(f))
                        } else {
                            orPredicates.push(a.comparator.startsWith('!') ? $odata.Predicate.or(f) : $odata.Predicate.and(f))
                        }
                        
                    }
                } else {
                    var values = basicService.compact(a.values);
                    var filterPredicates = []
                    angular.forEach(values, function(v, i){
                        var f = basicService.oDataFunction(filters, k, p, v);
                        filterPredicates.push(f);
                    });
                    if (filterPredicates.length > 0){
                        //For contains we are searching all values in the same field.
                        if (filters.filters[k].isRestricted) {
                            andPredicates.push(a.comparator.startsWith('!') ? $odata.Predicate.or(filterPredicates) : $odata.Predicate.and(filterPredicates));
                        } else {
                            if (filters.filters[k].comparator.toLowerCase().endsWith("containsall")) {
                                if (a.operator == 'And') {
                                    andPredicates.push(a.comparator.startsWith('!') ? $odata.Predicate.or(filterPredicates) : $odata.Predicate.and(filterPredicates))
                                } else {
                                    orPredicates.push(a.comparator.startsWith('!') ? $odata.Predicate.or(filterPredicates) : $odata.Predicate.and(filterPredicates))
                                }
                            } else {
                                if (a.operator == 'And') {
                                    andPredicates.push(a.comparator.startsWith('!') ? $odata.Predicate.and(filterPredicates) : $odata.Predicate.or(filterPredicates))
                                } else {
                                    orPredicates.push(a.comparator.startsWith('!') ? $odata.Predicate.and(filterPredicates) : $odata.Predicate.or(filterPredicates))
                                }
                            }
                        }
                    }
                }
            });

            if(orPredicates.length > 0) {
                oData.filter($odata.Predicate.or(orPredicates));
            }

            if(andPredicates.length > 0) {
                oData.filter($odata.Predicate.and(andPredicates));
            }
            
            filters.orderApplied = [];
            angular.forEach(filters.order, function(obj, i) {
                if(_.has(obj, 'name') && obj['name'].length > 0) {
                    var direction = "asc";
                    if(_.has(obj, 'isDescending') && obj['isDescending'] == true) {
                        direction = "desc";
                    }
                    
                    //Only include a column once. Otherwise odata will complain.
                    if (_.findWhere(filters.orderApplied, { name: obj['name']}) == undefined) {
                        filters.orderApplied.push({ name: obj['name'], isDescending: obj['isDescending'] })
                        oData.orderBy(obj['name'].replace('__', '/'), direction);
                    }
                }
            })

            if(angular.isDefined(filters.include) && _.isArray(filters.include) && filters.include.length > 0) {
                oData.select(filters.include);
            }

            if(angular.isDefined(filters.expand) && filters.expand != "") {
                angular.forEach(filters.expand.split(","), function(v, k) {
                    var expandArray = v.split("/");
                    if(expandArray.length == 2) {
                        oData.expand(expandArray[0],expandArray[1]);    
                    } else {
                        oData.expand(expandArray[0]);    
                    }
                })
            }

            if(angular.isDefined(filters.offset) &&  filters.limit != "") {
                oData.skip(filters.offset).take(filters.limit);
            }
            return oData;
    
        }

        //returns a copy. But requires to update both parameter and copy.
        basicService.compact = function (array) {
            angular.forEach(array, function (v, k) {
                if (v === 0) {
                    array[k] = '___0000000000000000000000000000000000000000000';
                }
            });
            var new_array = _.compact(array);

            //Must update both
            angular.forEach(array, function (v, k) {
                if (v == '___0000000000000000000000000000000000000000000') {
                    array[k] = 0;
                }
            });

            angular.forEach(new_array, function (v, k) {
                if (v == '___0000000000000000000000000000000000000000000') {
                    new_array[k] = 0;
                }
            });
            return new_array;
        }

        basicService.oDataFunction = function (filters, key, param, value) {

            //Change "[null]" to null
            value = (value != "[null]" ? value : null);

            //set rightSide to false when we want to perform not equals comparator
            var rightSide = (filters.filters[key].comparator.startsWith('!') ? false : true);
            var operator = (filters.filters[key].comparator.startsWith('!') ? "!=" : "==");

            if (value == null) {
                //if value is null just do a an equals search
                return new $odata.Predicate(param, operator, value);
            
            } else if (filters.filters[key].comparator.toLowerCase().endsWith("endswith")) {
                //Do endsWith
                return new $odata.Predicate(
                    new $odata.Func('endswith', param, value),
                    rightSide
                );            
            } else if (filters.filters[key].comparator.toLowerCase().endsWith("startswith")) {
                //Do startsWith
                return new $odata.Predicate(
                    new $odata.Func('startswith', param, value),
                    rightSide
                );
            } else if (filters.filters[key].comparator.toLowerCase().endsWith("containsall")) {
                return new $odata.Predicate(
                    new $odata.Func('substringof', new $odata.Value(value), new $odata.Property(param)),
                    rightSide
                );
            } else if (filters.filters[key].comparator.toLowerCase().endsWith("containsany")) {
                return new $odata.Predicate(
                    new $odata.Func('substringof', new $odata.Value(value), new $odata.Property(param)),
                    rightSide
                );
            } else if (filters.filters[key].comparator.toLowerCase().endsWith("range")) {
                //We have 2 sets of operators and it is used by 
                //determining the _rangeLookupType for each param
                var comparators = { "range": { 0: ">=", 1: "<=" }, "!range": { 0: "<", 1: ">" }, };
                if (_.isArray(value)) {
                    var predicates = [];
                    angular.forEach(value, function (dv, dk) {
                        if((dv != null && dv != '') || angular.isNumber(dv)) {
                            predicates.push(
                                new $odata.Predicate(
                                    param
                                    , comparators[filters.filters[key].comparator][dk]
                                    //Make sure we pass the enforced value format and data type
                                    , new $odata.Value(basicService.oDataValue(dv, filters.filters[key].dataType), filters.filters[key].dataType)
                                )
                            );
                        }
                    });
                    return predicates;
                }
            } else {
                if (angular.isDefined(filters.filters[key].dataType) && filters.filters[key].dataType != 'String') {
                    return new $odata.Predicate(param, operator, new $odata.Value(value, filters.filters[key].dataType));
                } else {
                     return new $odata.Predicate(param, operator, value);
                }
            }
        }
        return basicService;
    }
]);


angular.module('tfof').factory('RequestService', ['$rootScope','$q','$http','MessageService',
    function($rootScope,$q,$http,MessageService) {
        var requestService = {};
        requestService.broadcast = function(broadcastname) {
            $rootScope.$broadcast(broadcastname);
        };
        requestService.post = function(url,params) {
            //var deferred = $q.defer();
            url = url + '?__request_id=' + Date.parse(Date())/1000
            return $http.post(url,params)
                .success(function(data) {
                    return data
                })
                .error(function(data) {
                    return data
                    //if(angular.isDefined(data['is_authenticated'])) {
                    //    if(!data['is_authenticated']) {
                    //        var con = confirm('Your session has expired. Would you like to login again?\n\nTo continue working on this page, click Cancel and login using another tab or window.')
                    //        if(con) {
                    //            window.location.reload()
                    //        } else {
                    //            $rootScope.$broadcast('pollingStop')
                    //        }
                    //    }
                    //}
                });
        };
        
        requestService.put = function(url,params) {
            //var deferred = $q.defer();
            url = url + '?__request_id=' + Date.parse(Date())/1000
            return $http.put(url,params)
                .success(function(data) {
                    return data
                })
                .error(function(data) {
                    if(angular.isDefined(data['is_authenticated'])) {
                        if(!data['is_authenticated']) {
                            var con = confirm('Your session has expired. Would you like to login again?\n\nTo continue working on this page, click Cancel and login using another tab or window.')
                            if(con) {
                                window.location.reload()
                            } else {
                                $rootScope.$broadcast('pollingStop')
                            }
                        }
                    }
                });
        };
        
        requestService.delete = function(url,params) {
            //var deferred = $q.defer();
            url = url + '?__request_id=' + Date.parse(Date())/1000
            return $http.delete(url)
                .success(function(data) {
                    return data
                })
                .error(function(data) {
                    if(angular.isDefined(data['is_authenticated'])) {
                        if(!data['is_authenticated']) {
                            var con = confirm('Your session has expired. Would you like to login again?\n\nTo continue working on this page, click Cancel and login using another tab or window.')
                            if(con) {
                                window.location.reload()
                            } else {
                                $rootScope.$broadcast('pollingStop')
                            }
                        }
                    }
                });
        };

        requestService.get = function(url,params) {
            //var deferred = $q.defer();
            return $http.get(url+'?' + $.param(params) + '&__request_id=' + Date.parse(Date())/1000)
                .success(function(data) {
                    $('#load-p')
                    return data
                }).error(function(data) {
                    if(angular.isDefined(data['is_authenticated'])) {
                        if(!data['is_authenticated']) {
                            var con = confirm('Your session has expired. Would you like to login again?\n\nTo continue working on this page, click Cancel and login using another tab or window.')
                            if(con) {
                                window.location.reload()
                            } else {
                                $rootScope.$broadcast('pollingStop')
                            }
                        }
                    } 
                });
        };
        return requestService;
    }
]);

angular.module('tfof').factory('PageService', ['$rootScope','$q','$http','MessageService',
    function($rootScope,$q,$http,MessageService) {
        var pageService = {};
        
        pageService.get = function(id, url, layout, classnames) {
            //Remove modal if it exists
            var div = document.getElementById(id.replace('#',''));
            if(div != null) {
                div.parentNode.removeChild(div);
            } 
            if(angular.isUndefined(layout)) {
                layout = "Modal";
            }
            if(angular.isUndefined(classnames)) {
                classnames = "";
            }
            //Load modal and insert into new element in the body
            console.log(url);
            $.ajax({  
                url: url + (url.indexOf('?') == -1 ? '?' : '&' ) + 'Layout=' + layout,
                dataType: 'html',  
                async: false,  
                cache: false,
                success: function(data) {
                    //var divelement = document.createElement('div');
                    data = data.replace(/{id}/, id.replace('#','')).replace('ng-cloak','').replace('{classnames}', classnames);
                    //$(divelement).html(data)
                    $('body').append(data);
                }
            });
        };

        return pageService;
    }   
]);

//Message service for UI alerts messaging
angular.module('tfof').factory('FormService', 
    function($rootScope) {
        var formService = { 
            ids: []
        };

        formService.clearAlert = function (id) {
            angular.element(id).html('');
            return formService;
        };

        formService.Alert = function (id, message, type) {
            var html = '<div class="' + type + '">' + message + '</div>';
            angular.element(id).html(html);
            return formService;
        };

        return formService;
    }
);

//Message service for UI alerts messaging
angular.module('tfof').factory('MessageService', 
    function ($timeout, $rootScope) {
        if(isUndefined(window.EXISTING_MESSAGES)) {
            window.EXISTING_MESSAGES = {
                debug: [],
                success: [],
                info: [],
                warning: [],
                error: []
            };
            
        }
        var messageService = {
            timer : 10, //inseconds
            messages: {
                success: window.EXISTING_MESSAGES['success'],
                info: window.EXISTING_MESSAGES['info'],
                warning: window.EXISTING_MESSAGES['warning'],
                error: window.EXISTING_MESSAGES['error'],
                debug: window.EXISTING_MESSAGES['debug']
            },
            types: {
                success:'alert-success',
                info: 'alert-info',
                warning: 'alert-warning',
                error: 'alert-danger',
                debug: 'alert-warning'
            },
            fadeout: {
                success: true,
                info: true,
                warning: false,
                error: false,
                debug: false
            },
            httpStatuses: {
                400: "Bad Request",
                401 : "Your session has expired. Please log in and try again.",//Unauthorized",
                402 : "Payment Required",
                403 : "Insufficient access. Please contact your manager if access is required.", //Forbidden
                404 : "The data you are requesting was not found.", //Not Found",
                405 : "Method Not Allowed",
                406 : "Not Acceptable",
                407 : "Proxy Authentication Required",
                408 : "Your request did not complete in time. Please try again.", //Request Timeout",
                409 : "Conflict",
                410 : "Gone",
                411 : "Length Required",
                412 : "Precondition Failed",
                413 : "Request Entity Too Large",
                414 : "Request-URI Too Long",
                415 : "Unsupported Media Type",
                416 : "Requested Range Not Satisfiable",
                417 : "Expectation Failed",
            }
        };

        //clear all messages
        messageService.clear = function(type) {
            if(!_.isUndefined(type)){
                messageService.messages[type] = []
            } else {
                messageService.messages =  {
                    success: [],
                    info: [],
                    warning: [],
                    error: [],
                    debug: []
                }
            }
            return messageService;
        };

        //success method
        messageService.flashSuccess = function (message) {
            messageService.flash(message, "success")
        }
        //error method
        messageService.flashError = function (message) {
            messageService.flash(message, "error")
        }
        //info method
        messageService.flashInfo = function (message) {
            messageService.flash(message, "info")
        }
        //warning method
        messageService.flashWarning = function (message) {
            messageService.flash(message, "warning")
        }
        //debug method
        messageService.flashDebug = function (message) {
            messageService.flash(message, "debug")
        }
        //add new message
        messageService.flash = function(message,type,timer) {
            if(angular.isUndefined(type)) type = "info";
            if (angular.isUndefined(timer)) timer = 10;

            var newMessage = {
                message: message,
                type: messageService.types[type],
                fadeout: messageService.fadeout[type]
            };

            if(messageService.messages[type].length==0) {
                messageService.messages[type].push(newMessage)
            } else {
                var exists = false
                angular.forEach(messageService.messages[type], function(v){
                    var curm = v.message + '-----' + v.type;
                    var newm = message + '-----' + messageService.types[type];
                    if(curm == newm) { exists = true}
                });
                if(!exists) messageService.messages[type].push(newMessage);
            }

            messageService.timer = timer;
            $rootScope.$broadcast('FlashMessages');
            return messageService;
        };

        return messageService;
    }
);

angular.module('tfof').factory('ModalService', ['$rootScope', '$q', '$http', 'MessageService',
    function ($rootScope, $q, $http, MessageService) {
        var modalService = {};

        modalService.get = function (id, url,size) {
            //Remove modal if it exists
            var div = document.getElementById(id.replace('#', ''));
            if (div != null) {
                div.parentNode.removeChild(div);
            }
            //Load modal and insert into new 
            if (angular.isUndefined(size)) {
               size = 'Modal'
            }
            $.ajax({
                url: url + (url.indexOf('?') > -1 ? '&Layout=': '?Layout=') + size,
                dataType: 'html',
                async: false,
                cache: false,
                success: function (data) {
                    //var divelement = document.createElement('div');
                    data = data.replace('{id}', id.replace('#', '')).replace('ng-cloak', '');
                    //$(divelement).html(data)
                    $('body').append(data);
                }
            });
        };
        return modalService;
    }
]);