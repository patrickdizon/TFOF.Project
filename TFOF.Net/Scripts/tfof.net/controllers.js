//AngularJS controller

'use strict';

angular.module('tfof').controller(
    'BasicController', 
    [
        '$cookies',
        '$filter', 
        '$location',
        '$parse',
        '$rootScope',
        '$scope',
        //'$stateParams',
        '$odataresource',
        '$odata',
        '$timeout', 
        '$sce',
        '$resource',
        '$anchorScroll',
        'BasicService',
        'MessageService',
        'FormService',
        'RequestService',
        'hotkeys',
    function (
        $cookies,
        $filter, 
        $location,
        $parse,
        $rootScope,
        $scope,
        //$stateParams,
        $odataresource,
        $odata,
        $timeout, 
        $sce,
        $resource,
        $anchorScroll,
        BasicService,
        MessageService,
        FormService,
        RequestService,
        HotKeys
    ) 
{
    $scope.controllerName = "BasicController";
    
    $scope.basic = {}
    //The var BASIC is set in each template.
    if(angular.isDefined(window.BASIC)) {
       $scope.basic = BASIC
    }
    
    //models used for UI visibility toggling
    $scope.unsaved = false
    $scope.updating = false
    $scope.loading = false
    $scope.deleteMode = false
    //Used by sub controllers for their polling.
    //Set here for BasicService broadcasts of PollStart, PollStop
    $scope.polling = false

    

    $scope.filters = BasicService.oDataFilter();

    $scope.limitOptions = [
        {value: 15, text: '15'},
        {value: 30, text: '30'},
        {value: 100, text: '100'},
    ]

    //One selected Item
    $scope.selectedItem;
    //List of selectedItems items
    $scope.selectedItems = []
    $scope.selectOptions = {} 

    $scope.init = function(controllerName) {
        $scope.controllerName = (angular.isDefined(controllerName) && controllerName != '') ? controllerName : $scope.controllerName;  
        window[$scope.controllerName] = $scope;
    }

    $scope.scopeModel = function(modelName, value) {
        $scope[modelName] = value
        //console.log(modelName,value)
    }

    $scope.back = function(h) {
        if(angular.isUndefined(h)) {
            var h = -1
        }
        window.history.go(h)
    }

    $scope.flashErrors = function(errors) {
        if(angular.isObject(errors)) {
            angular.forEach(errors, function(value,key) {
                var m = '<b>' + key.toUpperCase().replace(/_/g,' ') + ':</b> ' + errors[key]
                if (m.length > 0) {
                    MessageService.flash(m.replace("``","<i>empty</i>"),'error')    
                }
            })
        } else {
            MessageService.flash('An error occurred while trying perform an update.' + 
                ' Please refresh your browser and try again.' +
                ' If you have unsaved data, please copy it to the clipboard and paste it after refreshing.' +
                ' If this error persists, please contact your system administrator.','error')
        }
    }
        

    $scope.create = function (url, basic, displayMessage, callback) {
        $scope.updating = true
        var Record = BasicService.api(url, { id: "@id" })
        var record = new Record(basic);
        record.$save( function (createSuccessResult) {
            if (angular.isDefined(callback)) {
                $scope.callback(callback, false);
            }
            MessageService.clear().flash(displayMessage, 'success');
        },
        function (createErrorResult) {
            MessageService.flash(createErrorResult.statusText, 'error')
        });
    }

    $scope.update = function(url, recordId, basic, callback) {
        $scope.updating = true
        $scope.spinner(true)
        var Record = BasicService.api(url);
        Record.update({ id: recordId }, basic, function (updateSuccessResult) {
            if (angular.isDefined(callback)) {
                $scope.callback(callback, false)
            }
            MessageService.clear().flash("Record Updated!", "success");
            $scope.spinner(false)
        }, function (updateErrorResult) {
            MessageService.flash(updateErrorResult.statusText, 'error')
            $scope.spinner(false)
        });
    }

    $scope.delete = function (url, id, callback) {
        var Record = BasicService.api(url);
        Record.delete({ id: id }, function (deleteSuccessResult) {
            //Delete Success
            if(angular.isDefined(callback)) {
                callback()
            }
            MessageService.clear().flash("Record deleted!","success")
        }, function(deleteRrrorResult) {
            //Delete Error
            MessageService.flash("There was an error in deleting the record.","error")
        });
    }

    $scope.updateAll = function(url,alertField){
        var updated = 0
        $rootScope.$broadcast('ClearMessages')
        if(angular.isDefined($scope.basics) && $scope.basics.length > 0) {
            angular.forEach($scope.basics, function(basic) {
                /*TODO */
                var record_url = url.replace('<pk>',basic['id']).replace('<kind>',basic['kind'])
                $timeout(function() {
                    $scope.update(basic,record_url,alertField)
                },300)
            })    
        }
        $timeout(function() {
            $scope.find()
        },3000)
    }

    $scope.transmit = function(controllerName, name, obj) {
        BasicService.package = { 
            "controllerName": controllerName
            ,"name" : angular.isDefined(name) ? name : "object"
            ,"obj" : obj
        }
        $rootScope.$broadcast("transmit");
    }

    

    $scope.changeComparator = function (name) {
        if ($scope.filters.filters[name].comparator.endsWith('range') && $scope.filters.filters[name].values != null && $scope.filters.filters[name].values.length != 2) {
            while($scope.filters.filters[name].values.length < 2) {
                $scope.filters.filters[name].values.push(null);
            }
            while ($scope.filters.filters[name].values.length > 2) {
                $scope.filters.filters[name].values.pop();
            }

        }
    }
    $scope.find = function(kwargs,callback) {
        if(angular.isDefined(kwargs)) {
            //var bylocation = true
            //if(kwargs.hasOwnProperty('_locationSearch')) {
            //$scope.filters.locationSearch = false
            //}        

            //var bykwarg = true
            if (kwargs.hasOwnProperty('init')) {
                if ($scope.filters.useLocalStorage) {
                    var id = $scope.filters.id;
                    var f = BasicService.getFilter($scope.filters);
                    if (angular.isDefined($location.search().s) &&
                            $location.search().s == $scope.filters.id) {
                        if (f != null && angular.isDefined(f.filters)) {
                            $scope.filters = f;
                            $scope.filters.id = id;
                            //Format data to input type
                            angular.forEach($scope.filters.filters, function (f, k) {
                                if (angular.isDefined(f.values) && _.isArray(f.values) && angular.isDefined(f.inputType)) {
                                    f.values = $scope.setFilter(f.values, f.inputType);
                                }
                            })
                        }
                    }
                }
                if ($scope.filters.load == false) {
                    return false;
                }
            }

            if (angular.isDefined(kwargs.offset)) {
                $scope.filters.offset = kwargs.offset;
            }
        }

      
        $scope.loading = true;
        BasicService.oData($scope.filters).single(function(basics) {
            //Success
            $scope.digestBasic(basics);
            if(angular.isDefined(callback)) {
                $scope.callback(callback)
            }
            $scope.loading = false;
            $scope.filters.didSearch = true;
            $scope.filters.load = true;
            if($scope.selectedItem != null) {
                $scope.selectRow($scope.selectedItem);
            }
            
            if ($scope.filters.useLocalStorage) {
                BasicService.setFilter($scope.filters);
                $location.search('s', $scope.filters.id);
            }
            $scope.$broadcast('findSuccess', basics);

        }, function(response) {
            //Error
            var errorMessage = 'An error occurred while performing a query.'
            if (angular.isDefined(MessageService.httpStatuses[response.status])) {
                errorMessage = MessageService.httpStatuses[response.status]
            }
            MessageService.clear().flash(errorMessage,'error')
            $scope.loading = false;
            $scope.$broadcast('findError')
        });
    };

    $scope.reFind = function() {
        $scope.find()
    }
    
   
    $scope.digestBasic = function (basics) {
        if (angular.isDefined(basics.Messages)) {
            angular.forEach(basics.Messages, function (v, k) {
                MessageService.clear();
                MessageService.flash(v.Message, v.Type.toLowerCase());
            })
            return;
        }
        $scope.filters.count = angular.isDefined(basics.Count) ? basics.Count : $scope.filters.count;
        $scope.filters.stats = angular.isDefined(basics.Stats) ? basics.Stats : $scope.filters.stats;
        $scope.filters.nextSet = angular.isDefined(basics.Next) ? basics.Next : 30;
        basics = angular.isDefined(basics.Items) ? basics.Items : basics;

        if (!$scope.filters.infiniteScroll || !angular.isDefined($scope.basics)) {
            $scope.basics = basics;
        } else {
            angular.forEach(basics, function(b) {
                $scope.basics.push(b)
            })
        }
        $scope.filters.viewOffset = parseInt($scope.filters.offset);
        $scope.EnableHotKeys();
    }

    $scope.findOne = function(url,id,modelName,callback) {
        var urlString = url + (angular.isDefined(id) ? '/' + id : '')
        $scope.loading = true;
        BasicService.api(urlString).get({}, 
            function(basic) {
                if(angular.isDefined(modelName)) {
                    $scope.scopeModel(modelName, basic);
                } else {
                   $scope.basic = basic;
                }
                
                if(angular.isDefined(callback)) {
                        $scope.callback(callback)
                }
                $scope.loading = false;
            }
        );
    };


    $scope.formatFilter = function (value, inputType) {
        //For formatting labels search filters
        if(inputType == 'date') {
            return $filter('date')(value, 'shortDate');
        } 
        return value;
    }

    $scope.showFilterLabel = function(value){
        //Decide if label should be shown. Compact handles strings and arrays.
        if(_.compact(value).length > 0) {
            return false;
        }
        return true;
    }

    $scope.filterLabel = function (field, inputType) {
        var label = [];
        var delimiter = ($scope.filters.filters[field].comparator.endsWith('range') ? '-' : ', ');
        if (angular.isString($scope.filters.filters[field].values)) $scope.filters.filters[field].values = [$scope.filters.filters[field].values];

        angular.forEach($scope.filters.filters[field].values, function (v, k) {
            if (_.isArray($scope.filters.filters[field].options) && angular.isDefined(_.findWhere($scope.filters.filters[field].options, {value: v }))) {
                label.push(_.findWhere($scope.filters.filters[field].options, { value: v })['label']);
            } else {
                label.push($scope.formatFilter(v, inputType));
            }
        })
        label = BasicService.compact(label);
        return ($scope.filters.filters[field].comparator.startsWith('!') && label.length > 0 ? "&ne;" : "") + label.join(delimiter)
    }

    $scope.initFilter = function (id, url, top, doLoad, expand, include, useLocalStorage) {
        $scope.filters.limit = top;
        $scope.filters.id = id;
        $scope.filters.load = doLoad;
        $scope.filters.url = url;
        $scope.filters.expand = expand;
        $scope.filters.include = include;
        $scope.filters.useLocalStorage = useLocalStorage;
    }

    $scope.addFilter = function (name, values, dataType, comparator, operator, inputType, isRestricted) {
        $scope.filters.filters[name] = {
            values: $scope.setFilter(values, inputType),
            dataType: dataType,
            comparator: comparator,
            operator: operator,
            inputType: inputType,
            selectedOptions: [],
            isRestricted: isRestricted,
            options: []
        };
    }

    $scope.setFilter = function (value, inputType) {
        if (inputType == 'date' && _.isArray(value)) {
            angular.forEach(value, function (v, k) {
                if (!_.isNull(v) && v.trim().length > 0) {
                    //short date attempt mm/dd/yyyy
                    if (Date.parse(v) != null) {
                        value[k] = Date.parse(v);
                    }
                        //isodate attempt yyyy-mm-ddT00:00:00.0000Z
                    else if (Date.parse(new Date(v)) != null) {
                        value[k] = Date.parse(new Date(v));
                    }
                }
            });
        }
        return value;
    }


    $scope.removeFilter = function (key, ifEmpty) {

        BasicService.removeFilter($scope.filters.id);
        $scope.filters.offset = 0;
        if (angular.isDefined(ifEmpty) && ifEmpty && $scope.filters.filters[key].values.length == 0) {
            $scope.filters.filters[key].values = [''];
            if (_.isArray($scope.filters.filters[key].options)) {
                angular.forEach($scope.filters.filters[key].options, function (o, k) {
                    o.selected = false;
                });
            }
            $scope.changeComparator(key);
            $scope.filters.didSearch = false;
            return;
        }

        if (angular.isUndefined(ifEmpty)) {
            if (key == '*') {
                
                angular.forEach($scope.filters.filters, function (v, k) {
                    if ($scope.filters.filters[k].isRestricted == false) {
                        $scope.filters.filters[k].values = [''];
                        if (_.isArray($scope.filters.filters[key].options)) {
                            angular.forEach($scope.filters.filters[key].options, function (o, k) {
                                o.selected = false;
                            });
                        }
                        $scope.changeComparator(k);
                    }
                })
            } else {
                $scope.filters.filters[key].values = [''];
                if (_.isArray($scope.filters.filters[key].options)) {
                    angular.forEach($scope.filters.filters[key].options, function (o, k) {
                        o.selected = false;
                    });
                }
                $scope.changeComparator(key);
            }
            $scope.filters.didSearch = false;
        }
    }

    //bulk assigns filter values.
    //Values must be entered one line per value
    $scope.setFilterValues = function (filterName, values) {
        var valuelist = values.split('\n');
        $scope.filters.filters[filterName].values = valuelist;
    }


    $scope.translate = function(value, key) {
        var translation = $scope.filters.translations[key][value];
        if(angular.isDefined(translation)) {
            return translation;
        }
        return value;
    }

    $scope.spinner = function(on,message, notimeout) {
        //Spinner is outside of any controllers
        //so we control its visibility by adding/remove
        //the hide class
        //We can also assign a custom message and set 
        //$scope.loading = true|false so that controllers
        //know the state

        if($scope.filters.showSpinner == true) { 
            if(angular.isDefined(message)) {
                angular.element('.spinner-message').html(message)
            } else {
                angular.element('.spinner-message').html('Loading...')
            }
            if (on) {
                angular.element(".spinner-overlay").removeClass('hide');
                $scope.loading = true
                if (!angular.isDefined(notimeout)) {
                    $timeout(function () {
                        $scope.spinner(false)
                    }, 10000)
                }
            } else {
                angular.element(".spinner-overlay").addClass('hide');
                $scope.loading = false
            }

        };

    }


    //Goes to the next set of records
    $scope.next = function() { 
        $scope.filters.offset = ($scope.basics.length == $scope.filters.limit)
            ? parseInt($scope.filters.offset) + parseInt($scope.filters.limit)
            : parseInt($scope.filters.offset)
        $scope.find($scope.filters);
    };


    //Goes back to the previous set of records
    $scope.previous = function() { 
        var p = parseInt($scope.filters.offset) - parseInt($scope.filters.limit)
        $scope.filters.offset = p > 0 ? p : 0;
        $scope.find($scope.filters);
    };

    //Displays the current set of records off of the total number of results.
    $scope.pageInfo = function() {
        var from = 0;
        var to = 0;
        if($scope.filters.count > 0) {
            from = $scope.filters.offset + 1;
            to = _.min([$scope.filters.offset + $scope.filters.limit, $scope.filters.count]);
        }
        if(angular.isDefined($scope.filters.count) && $scope.filters.count != null) {
            return $filter('number')(from) + "-" + $filter('number')(to) + " of " + $filter('number')($scope.filters.count);
        } 
        return "";
    }

    //Paginate and only show pages for 9000 records 300 pgs. Beyond that would be too much.
    $scope.paginate = function() {
        if(angular.isDefined($scope.filters.count) && $scope.filters.count != null) {
            var pages = ($scope.min($scope.filters.count,9000))/$scope.filters.limit
            if(pages % 1 == 0) {
                //remove empty last page if exact
                pages = pages-1;
            }
            return pages;
        }
        return 0;
    }

    //Show page control if its greater than 3 pages 
    $scope.showPagination = function() {
        if($scope.filters.showPagination && $scope.paginate() > 3) {
            return true;
        }
        return false;
    }

    //Current page number to display on the control
    $scope.pageNumber = function() {
        if ($scope.filters.offset == 0) {
            return 1
        } 
        var page = $scope.filters.offset/$scope.filters.limit
        return page+1
    }

    //Goes to the page using the selected page number
    $scope.pageSelect = function(page) {       
        $scope.filters.offset=(page*$scope.filters.limit); 
        $scope.find($scope.filters)
    }

    $scope.range = function(start, end) {
        var result = [];
        if (angular.isUndefined(start)){ start = 1 }
        if (angular.isUndefined(end)){ end = 1 }

        for (var i = start; i <= end; i++) {
            result.push(i);
        }
        return result;
    };
    
    //Compiles selectedItems items from the list into the selected array
    //IMPORTANT: Use ng-change for checkboxes
    $scope.select = function() {
        var log = [];
        angular.forEach($scope.basics, function(b){
            if(b.selectedItems == true) { log.push(b.id) };
        });
        $scope.selectedItems = log;      
    }


    //Selects all items in the list. 
    //IMPORTANT: Use ng-change for checkboxes
    $scope.selectAll = function() {
        angular.forEach($scope.basics, function(v,k){
            if($scope.filters.selectall) {
                $scope.basics[k].selectedItems = true;
            } else {
                $scope.basics[k].selectedItems = false;
            }
        });
        $scope.select();
    }

    //Selects by index Id and assigns it to basic.
    $scope.selectRowByIncrement = function(increment) {
        var previousItem = 0;
        if($scope.selectedItem == null) {
            $scope.selectedItem = 0;
        } else {
            previousItem = $scope.selectedItem;
            $scope.selectedItem += increment;
        }
        if($scope.selectedItem < 0) $scope.selectedItem = 0;
        
        if($scope.basics.length > 0 && angular.isDefined($scope.basics[$scope.selectedItem])) {
            $scope.selectRow($scope.selectedItem)
           
        } else {
            //Otherwise return to the previous index to prevent increments beyond the number of basic items.
            $scope.selectedItem = previousItem
        }
    }

    $scope.selectRow = function(index) {
        $scope.selectedItem = index;
        $scope.basic = $scope.basics[index];
    }

    $scope.setVar = function(varName,value) {
        $scope.basic[varName] = value
    }

    /*
    For dynamically populating select fields. 
    Used in conjunction with selectized, xeditable
    */
    
    $scope.showOptionValue = function(forModel,modelVal) {
        var options = $scope.selectOptions[forModel]
        var selected = $filter('filter')(options, {value: modelVal});
        return (modelVal && selected.length) ? selected[0].label : 'empty';
    }

    $scope.showChoiceValue = function(choices,modelVal) {
        var display = ''
        angular.forEach(choices,function(c){
            if(c[0]==modelVal){display = c[1]}
        })
        return display
    }

    $scope.choicesToOpts = function(choices,name) {
        var list = []
        if(angular.isArray(choices)) {
            if(angular.isDefined(name) 
                && $scope.selectOptions.hasOwnProperty(name)
                && $scope.selectOptions[name].length == choices.length
            ){
                return $scope.selectOptions[name]
            }
     
            angular.forEach(choices,function(v){
                list.push({value:v[0],label:v[1]})
            })
            if(angular.isDefined(name)) {
                $scope.selectOptions[name] = list
            }
        }
        return list
    }

    $scope.choicesToObject = function(choices,name) {

        if(angular.isDefined(name) && $scope.selectOptions.hasOwnProperty(name)){
            return $scope.selectOptions[name]
        }
        var obj = {}
        angular.forEach(choices,function(v){
            obj[v[0]] = v[1]
        })
        if(angular.isDefined(name)) {
            $scope.selectOptions[name] = obj
        }
        return obj
    }

    $scope.callback = function(callback, timeout) {
        if(angular.isDefined(timeout)) {
            $timeout(function(){
                $scope.$apply(callback)
            })
        } else {
            $scope.$apply(callback)
        }
    }

    $scope.isEmpty = function (obj) {
        return angular.equals({},obj); 
    };

    $scope.toggleClass = function (selector,className) {
        angular.element(selector).toggleClass(className)
    }
    
    $scope.toggleValue = function (model,val1,val2) {
        if($scope[model] == val1) {
            $scope[model] = val2
        } else {
            $scope[model] = val1
        }
    }

    $scope.min = function(int1, int2) {
        return Math.min(int1,int2)
    }
    
    $scope.call = function(url, callback) {
        BasicService.api(url).get({},function(result) {
            if(angular.isDefined(callback)) {
               callback()
            }
            $scope.spinner(false);
        });
    }

    $scope.callGet = function (url, callback) {
        $scope.call(url, callback)
    }

    $scope.callPost = function(url, params, message, callback) {
        RequestService.post(url,params).success(function(result) {
            if(angular.isDefined(result.Messages)) {
                angular.forEach(result.Messages, function (v, k) {
                    if (angular.isDefined(v.Model)) {
                        params = v.Model;
                    }
                    MessageService.clear();
                    MessageService.flash(v.Message, v.Type.toLowerCase());
                })
            } else {
                if (angular.isDefined(message)) {
                    MessageService.clear().flashSuccess(message)
                }
            }


            if(angular.isDefined(callback)) {
                callback()
            }
            $scope.spinner(false);
        }).error(function(errorData) {
            MessageService.clear().flashError("An error occurred. Please try again later or contact a system administrator.");
            $scope.spinner(false);
        });   
    }

    $scope.callPut = function (url, params, message, callback) {
        $scope["ModelFromService"] = "";
        RequestService.put(url, params).success(function (result) {
            if (angular.isDefined(result.Messages)) {
                angular.forEach(result.Messages, function (v, k) {
                    if (angular.isDefined(v.Model)) {
                        //params = v.Model;
                        $scope["ModelFromService"] = v.Model;
                    }
                    MessageService.clear();
                    MessageService.flash(v.Message, v.Type.toLowerCase());
                })
                if (angular.isDefined(message)) {
                    MessageService.clear().flashSuccess(message)
                }
            } else {
                if (angular.isDefined(message)) {
                    MessageService.clear().flashSuccess(message)
                }
            }
            if (angular.isDefined(callback)) {
                callback(result);
            }
            $scope.spinner(false);
        }).error(function () {
            MessageService.clear().flashError("An error occurred. Please try again later or contact a system administrator.");
            $scope.spinner(false)
        });
    }

    $scope.callDelete = function(url, message, callback) {
        RequestService.delete(url).success(function(result) {
            if(angular.isDefined(result.Messages)) {
                angular.forEach(result.Messages, function(v, k) {
                    MessageService.clear();
                    MessageService.flash(v.Message, v.Type.toLowerCase());
                })
            } else {
                if (angular.isDefined(message)) {
                    MessageService.clear().flashSuccess(message)
                }
            }
            if(angular.isDefined(callback)) {
                callback()
            }

            $scope.spinner(false);
        }).error(function() {
            MessageService.clear().flashError("An error occurred. Please try again later or contact a system administrator.");
            $scope.spinner(false);
        });   
    }


    $scope.jsonParse = function(modelName,string) {
        //jsonParse requires a ng-if wrap for async calls.
        //and to prevent angular infdig
        //Example
        //ng-if="basic.content"
        //ng-init="jsonParse('mymodel',basic.content)"
        //ng-repeat="m in mymodel"

        if(angular.isDefined(string)) {
            $scope[modelName] = JSON.parse(string)
        } 
    }

    $scope.slugify = function(input, separator) {
        return slugify(input, separator)
    }

    $scope.jsonStringyfy = function(modelName, indent) {
        if(angular.isDefined(indent)) {
            return JSON.stringify($scope[modelName],null,indent)
        } else {
            return JSON.stringify($scope[modelName])
        }
    }

    $scope.trustAsHtml = function(string) {
        if (string == null) {string = '';}
        return $sce.trustAsHtml(String(string));
    };

    $scope.redirect = function(url) {
        window.location = url;
    }

    $scope.EnableHotKeys = function(string){
        ///HOTKEYS///
        HotKeys.add({
            combo: 'right',
            description: 'Goes to the next ' + $scope.filters.nextSet + ' records.',
            callback: function() {
                if(!$scope.loading) {
                    $scope.next();
                }
            }
        });

        HotKeys.add({
            combo: 'left',
            description: 'Goes to the previous ' + $scope.filters.limit + ' records.',
            callback: function() {
                if(!$scope.loading) {
                    $scope.previous();
                }
            }
        });

        HotKeys.add({
            combo: 'down',
            description: 'Selects a row in the list.',
            callback: function() {
                $scope.selectRowByIncrement(1)
            }
        });

        HotKeys.add({
            combo: 'up',
            description: 'Selects a row in the list.',
            callback: function() {
                $scope.selectRowByIncrement(-1)
            }
        });


    }

    $scope.$on('warnUnsavedOn',function() {
        $scope.unsaved = true
    })

    $scope.$on('warnUnsavedOff',function() {
        $scope.unsaved = false
    })

    $scope.$on('pollingStart',function() {
        $scope.polling = true
    })

    $scope.$on('pollingStop',function() {
        $scope.polling = false
    })    

    $scope.$on('spinnerShow',function() {
        $scope.spinner(true)
    })

    $scope.$on('spinnerHide',function() {
        $scope.spinner(false)
    })

    $scope.$on('transmit', function() {
        if(BasicService.package != null) {
            //alias
            var pkg = BasicService.package;
            //receivers
            pkg['receivers'] = angular.isDefined(pkg.receivers) ? pkg.receivers : [];
            if(angular.isDefined(pkg.controllerName)
                && pkg.controllerName.split(',').indexOf($scope.controllerName) !== -1) 
            {
                if(angular.isDefined(pkg.name) && angular.isDefined(pkg.obj)) {
                    $scope[pkg.name] = {};
                    $timeout(function() {
                        $scope[pkg.name] = pkg.obj;
                        pkg.receivers.push($scope.controllerName);    
                    }, 10);
                    
                }
            }
            if(pkg.receivers.length == pkg.controllerName.split(',')) {
                BasicService.package = null;
            }
        }

    })

    window.onbeforeunload = function (event) {
        var message = ''
        if($scope.unsaved) {
            message = 'You have unsaved changes!';
        }

        if(message.length > 0) { 
            if (typeof event == 'undefined') {
                event = window.event;
            }
            if (event) {
                event.returnValue = message;
            }
            return message;
        }
    }

    // $scope.search = function(elementId,url,q){
    //     BasicService.api(what).query(q,function(result) {
    //         $scope.options[elementId] = result
    //     })

    // }   

    

    /*$scope.download = function(url,param_name) {
        if ($scope.selectedItems.length == 0) {
            alert("Please select one or more items.")
        } else {
            url += '?' + param_name + '=' + $scope.selectedItems.join()
            window.open(url)
        }
    }*/

    /*$scope.listMove = function(list,fromIndex,toIndex,warn) {
         var element = list[fromIndex];
        list.splice(fromIndex, 1);
        list.splice(toIndex, 0, element);
        if(angular.isDefined(warn) && warn) { 
            $scope.$broadcast('warnUnsavedOn')
        }
    }


    $scope.listInsert = function(which,item,index,warn) {
        if(!angular.isArray(which)) {
            var list = $scope.getBasicParam($scope.basic,which)
            if(angular.isUndefined(list)){
                list = []
            }
            list.splice(index, 0, item);
            $scope.setBasicParam($scope.basic,which,list)
       
        } else {
            var list = which
            list.splice(index, 0, item);
        }
        
         if(angular.isDefined(warn) && warn) { 
            $scope.$broadcast('warnUnsavedOn')
        }
    }*/

    /*$scope.listAppend = function(which,item,warn) {
        //var newitem = ''
        //angular.copy(item,newitem)
        if(!angular.isArray(which)) {
            var list = $scope.getBasicParam($scope.basic,which)
            if(angular.isUndefined(list) | !angular.isArray(list)){
                list = []
                //console.log('undefinded','not a list')
            }
            
            list.push(item)
            $scope.setBasicParam($scope.basic,which,list)
        
        } else {
            var list = which
            if(angular.isArray(item)){
                angular.forEach(item, function(i){
                    list.push(i);        
                })
            } else {
                list.push(item);
            }
            
        }
        
        if(angular.isDefined(warn) && warn) { 
            $scope.$broadcast('warnUnsavedOn')
        }
    }*/

    $scope.arrayRemove = function(list,index,warn) {
        list.splice(index,1)
        if(angular.isDefined(warn) && warn) {
             $scope.$broadcast('warnUnsavedOn')
        }    
    }

    /*$scope.dictUpdate = function(dict,key,value,warn) {
        if((value.charAt(0) == '{' || value.charAt(0) == '[') &&
            (value.charAt(value.length-1) == '}' || value.charAt(value.length-1) == ']')){
            value = eval("(" + value + ")");
        }
        
        dict[key] = value
        if(angular.isDefined(warn) && warn) {
            $scope.$broadcast('warnUnsavedOn')
            //The line below doesnt work for some reason beacause warn can be false
            //$scope.unsaved = warn
        }
    }
*/
    /*$scope.dictRemove = function(dict,key,warn) {
        delete dict[key]
        if(angular.isDefined(warn) && warn) {
            $scope.$broadcast('warnUnsavedOn')
            //The line below doesnt work for some reason beacause warn can be false
            //$scope.unsaved = warn
        }    
    }*/

   

    // $scope.showPane = function(kwargs){
    //     var params = $location.search()
    //     if(angular.isDefined(kwargs) && Object.keys(kwargs).length > 0) {
    //         params = kwargs
    //     }
    //     if(params.hasOwnProperty('panes') && params.hasOwnProperty('pane')) {
    //         angular.element(params.panes).removeClass('active')
    //         angular.element(params.pane).addClass('active')
    //         $location.search({panes:params.panes,pane:params.pane})
    //         angular.element("[ng-tab-panes='" + params.panes + "']").children().removeClass('active')
    //         angular.element("a[ng-tab='" + params.pane + "']").parent('li').addClass('active')
    //         angular.element("a[ng-tab='" + params.pane + "']").addClass('active')
    //        return true
    //     }
    //     return false
    // }

    /*$scope.saveOrderOld = function(url,method,ul,callback) {
        var lis = angular.element(ul).children('li[data-id]')
        var params = {}
        var ids = []
        angular.forEach(lis,function(ele){  
            ids.push(angular.element(ele).attr('data-id'))
        })
        params[method] = ids.join()
        BasicService.api(url).get(params,function() {
            if(angular.isDefined(callback)) {
                $scope.callback(callback)
            }
        });

    
    }*/

    

    //api function for typeahead
    /*$scope.getTypeahead = function(q, cls) {
        return RequestService.get('/search/json/', {q: q, cls: cls}).then(function(response) {
            if (response.data[cls])
                return response.data[cls]
            return [];
        });
    };*/

    // $scope.selectTypeAhead = function($item, d, key) {
    //     d[key] = $item.pk;
    // }

    //used for datetimepicker widget
    /*$scope.clickDropdown = function(selector) {
        if (angular.element(selector).parents(".open").length == 1) {
            angular.element(selector).click();
        }
    }*/

    

    // $scope.toggleBasicValue = function(basicParam, val1, val2) {
    //     if(basicParam == val1) {
    //         basicParam = val2
    //     } else {
    //         basicParam = val1
    //     }
    // }


}]);


angular.module('tfof').controller('MessageController',
    ['$rootScope',
    '$scope', 
    //'$stateParams',
    '$location',
    '$timeout',
    '$interval',
    'MessageService',
    'BasicService',
    'hotkeys',
    function (
        $rootScope, 
        $scope, 
        //$stateParams, 
        $location, 
        $timeout, 
        $interval, 
        MessageService, 
        BasicService, 
        HotKeys
    ) {
    $scope.messages = MessageService.messages;
    $scope.groups = MessageService.types;
    $scope.show = false
    $scope.pinned = false
    $scope.hover = {
        pinned : false,
        closer : false,
        countdown: true 
    }
    
    var stop;

    $scope.flash = function(message, type) {
        //Sometimes you just want to flash the messages via controllers
        //instead of internal app messages
        if(angular.isDefined(message) && angular.isDefined(type)) {
            MessageService.flash(message,type)
        }
        $rootScope.$broadcast('FlashMessages')
    }

    $scope.close = function(type) {
        MessageService.clear(type)
    }
    
    $scope.getMessageClass = function(message) {
        return MessageService.types[message.type];
    }

    $scope.hasMessages = function() {
        var msgCount = $scope.messages['success'].length +
        $scope.messages['info'].length +
        $scope.messages['warning'].length +
        $scope.messages['error'].length +
        $scope.messages['warning'].length +
        $scope.messages['debug'].length 
        var has =  true ? msgCount > 0 : false
        return has
    };


    $scope.pinHover = function(hover,reset) {
        if(angular.isDefined(reset)) {
            $scope.hover = {
                pinned : false,
                closer : false,
                countdown: true 
            }
            return
        }

        if(hover){
            if($scope.pinned) {
                $scope.hover.pinned = false
                $scope.hover.closer = true
                $scope.hover.countdown = false
            } else {
                $scope.hover.pinned = true
                $scope.hover.closer = false
                $scope.hover.countdown = false
            }
        } else {
            if($scope.pinned) {
                $scope.hover.pinned = true
                $scope.hover.closer = false
                $scope.hover.countdown = false
            } else {
                $scope.hover.pinned = false
                $scope.hover.closer = false
                $scope.hover.countdown = true
            }
        }
    }
    
    $scope.pinMessages = function(permanent) {
        if(angular.isDefined(stop)){
            $interval.cancel(stop)
            stop = undefined
        }

        if ($scope.pinned) {
            angular.element('.closer>i.fa-thumb-tack').addClass('fa-rotate-90')
            $scope.$broadcast('ClearMessages')
            $scope.pinned = false
            $scope.pinHover(true,true)
            return
        }

        if(angular.isDefined(permanent) && permanent) {
            angular.element('.closer>i.fa-thumb-tack').removeClass('fa-rotate-90')
            $scope.pinned = true
        }
    }
    
    $scope.$on('FlashMessages', function(){
        //console.log("Receiving Broadcast");
        $scope.pinned = false
        $('.messenger').removeClass('hide')
        $scope.messages = MessageService.messages;
        
        //reest pin hover
        $scope.pinHover(true,true)
        
        //pin message for another 10 seconds
        $scope.pinMessages(false)

        //create countdown for hiding message
        $scope.countdown = MessageService.timer;
        stop = $interval(function(){ 
            $scope.countdown -= 1
            if($scope.countdown == 0){
                $('.messenger').addClass('hide')
            }
          }, 1000, $scope.countdown);
    })

    $scope.$on('ClearMessages', function(){
        MessageService.clear()
        $('.messenger').addClass('hide')
    })

    HotKeys.add({
        combo: 'ctrl+f',
        description: 'Search for customers.',
        callback: function() {
            $('#SearchModal').modal('show');
            setTimeout(function() {
                $('#SearchField').focus();
            }, 100);
        }
    });

    window.mscope = $scope

}]);

angular.module('tfof').controller(
    'StatsController', 
    [
        '$cookies',
        '$filter', 
        '$location',
        '$parse',
        '$rootScope',
        '$scope',
        //'$stateParams',
        '$odataresource',
        '$odata',
        '$timeout', 
        '$sce',
        'BasicService',
        'MessageService',
        'FormService',
        'RequestService',
    function (
        $cookies,
        $filter, 
        $location,
        $parse,
        $rootScope,
        $scope,
        //$stateParams,
        $odataresource,
        $odata,
        $timeout, 
        $sce,
        BasicService,
        MessageService,
        FormService,
        RequestService
        ) 
{  
    $scope.stat_filters = BasicService.oDataFilter();
    $scope.stats = [];

    $scope.getStats = function() {
      
        BasicService.oData($scope.stat_filters).query(function(stats) {
           
            if (!angular.isDefined($scope.stats)) {
                $scope.stats = stats;
            } else {
                $scope.stats = [];
                angular.forEach(stats, function (b) {
                    $scope.stats.push(b)
                })
            }

        }, function () {
            //Error
            //ToDo, bring up message service
            //MessageService.flash('An error occurred while performing a query.','error')
            //$scope.spinner(false)
        });
    }

    $scope.$on('findSuccess', function () {
        var statUrl = angular.copy($scope.stat_filters.statUrl);
        $scope.stat_filters = angular.copy($scope.filters);
        $scope.stat_filters.url = statUrl;
        $scope.stat_filters.statUrl = statUrl; //Copy again for re-render
        $scope.stat_filters.orderApplied = [];
        $scope.stat_filters.limit = null;
        $scope.stat_filters.include = '';
        $scope.stat_filters.expand = '';
        if($scope.filters.offset == 0) {
            $scope.getStats();
        }
    })
    window['scope_stats' + $scope.controllername] = $scope
}]);