//angular.module('tfof').directive('daysOver',  [
//    'FormService', function (FormService) {
//    return {
//        restrict: 'AE',
//        scope: {
//            value: '='
//        },
//        link: function (scope, element, attributes) {
//            element.bind('keyup click change', function (event) {
//                FormService.clearAlert('#' + event.target.name + '-alert');
//                var date = Date.parse(event.target.value);
//                if (date instanceof Date && !isNaN(date.valueOf())) {
//                    date.setHours(0, 0, 0, 0);
//                    if (date.isBefore(Date.today())) {
//                        FormService.Alert('#' + event.target.name + '-alert', 'Date is in the past.', 'error');
//                    } else if (date.isAfter((Date.today()).addDays(parseInt(attributes.daysOver)))) {
//                        FormService.Alert('#' + event.target.name + '-alert', 'Date entered is more than ' + attributes.daysOver + ' days. Please confirm that this is correct.', 'warning');
//                    } 
//                } else {
//                    FormService.Alert('#' + event.target.name + '-alert', 'Not a valid date.', 'error');
//                }
//            });
                
//        }
//    }
//}]);


angular.module('tfof').directive('delay', function () {
    return {
        restrict: 'A',
        link: function(scope,element,attributes) {          
            var timer;
            element.bind('keyup', function() {              
                clearTimeout(timer);            
                timer = setTimeout(function() {
                    scope.$apply(attributes.delay);     
                }, 500);                
            });     
        }
    }

});


angular.module('tfof').directive('clearBtn', function () {
    return {
        restrict: 'A',
        link: function(scope,element,attributes) {
            if($(attributes.clearBtn).val()!= '') 
            {
                element.removeClass('hide');
            } else {
                element.addClass('hide');
            }
            $(attributes.clearBtn).bind('keyup',function(){
                if($(attributes.clearBtn).val()!= '') 
                {
                    element.removeClass('hide');
                } else {
                    element.addClass('hide');
                }
            });
            element.bind('click', function() {
                //$(attributes.clearBtn).val('');
                element.addClass('hide');
                var fn = angular.element(attributes.clearBtn).attr('ng-model') + "=''; ";
                if(angular.element(attributes.clearBtn).attr('ng-enter'))
                    fn += angular.element(attributes.clearBtn).attr('ng-enter') + "; ";
                if(angular.element(attributes.clearBtn).attr('delay'))
                    fn += angular.element(attributes.clearBtn).attr('delay') + "; ";
                scope.$apply(fn);


            });         
        }
    }
});

angular.module('tfof').directive('goto', function () {
    return {
        restrict: 'A',
        link: function(scope,element,attributes) {          
            element.bind('click', function() {
                document.location = attributes.goto;
            });
        
        }
    }
});

//angular.module('tfof').directive('previousBtn', function() {
//    return {
//        restrict: 'A',
//        link: function(scope,element,attributes) {
//            scope.$on('UpdateNavigation', function() {
//                //console.log('Prev UpdateNavigation Called');
//                element.unbind('click');
//                if(scope.filterparams.offset == 0) {
//                    element.addClass('disabled');                   
//                } else {
//                    element.removeClass('disabled');                    
//                    element.bind('click', function(){
//                        scope.$apply(attributes.previousBtn);
//                    })
//                }
//            })
//        }
//    }
//});

//angular.module('tfof').directive('nextBtn', function() {
//    return {
//        restrict: 'A',
//        link: function(scope,element,attributes) {
//            scope.$on('UpdateNavigation', function() {
//                //console.log('Next UpdateNavigation Called');
//                element.unbind('click');
//                if(scope.filterparams.recordcount == 0 || scope.filterparams.recordcount < scope.filterparams.maxresults) {
//                    element.addClass('disabled');               
//                } else {
//                    element.removeClass('disabled');                    
//                    element.bind('click', function(){
//                        scope.$apply(attributes.nextBtn);   
//                    })                  
//                }
//            })
//        }
//    }
//});

angular.module('tfof').directive('showSideMenu', function() {
    return {
        restrict: 'A',
        link: function(scope,element,attributes) {
            var origingalLeftMargin = $(attributes.showSideMenu).css('margin-left');
            element.bind('click', function(){
                if(angular.element(attributes.showSideMenu).css('margin-left') == origingalLeftMargin) {
                    angular.element(attributes.showSideMenu).animate({marginLeft: attributes.slide});
                } else {
                    angular.element(attributes.showSideMenu).animate({marginLeft: origingalLeftMargin});
                }
            });
        }
    }
});


//angular.module('tfof').directive('ngRepeatLast', function() {
//    return function(scope, element, attrs) {
//        if (scope.$last){
//            console.log('last', element)
//            //  scope.$apply(attrs.ngRepeatLast);   
//        }
//    };
//});



angular.module('tfof').directive('modal', ['PageService', function (PageService) {
    return {
        restrict: 'A',
        link: function (scope, element, attributes) {
            element.bind('click', function () {
                try {
                    var p = JSON.parse(JSON.stringify(eval("(" + attributes.modal + ")")))
                } catch (e) {
                    if (!angular.isString(p)) {
                        p = { id: attributes.modal };
                    } else {
                        return;
                    }
                }

                if (angular.isUndefined(p.size)) {
                    p['size'] = 'Modal';
                }

                if (angular.isDefined(p.url) && angular.isDefined(p.id)) {
                    PageService.get(p.id, p.url, p.size, p.classnames);
                }

                if (angular.isDefined(p.backdrop)) {
                    $(p.id).modal({ backdrop: p.backdrop, keyboard: true });
                }

                if (p.id) {
                    $(p.id).modal('show');
                } else {
                    console.log('Missing id in params')
                }
                if (p.draggable) {
                    $(p.id).draggable({ handle: 'h4' });
                }
            })
        }
    }
}]);

angular.module('tfof').directive('drawer', ['PageService', function (PageService) {
    return {
        restrict: 'A',
        link: function (scope, element, attributes) {
            element.bind('click', function () {
                
                try {
                    var p = JSON.parse(JSON.stringify(eval("(" + attributes.drawer + ")")))
                } catch (e) {
                    if (!angular.isString(p)) {
                        p = { id: attributes.drawer };
                    } else {
                        return;
                    }
                }

                if (angular.isUndefined(p.position)) {
                    p['position'] = 'DrawerRight';
                }

                if (angular.isDefined(p.url) && angular.isDefined(p.id)) {
                    PageService.get(p.id, p.url, p.position, p.classnames);
                }

                // if (angular.isDefined(p.backdrop)) {
                //     $(p.id).modal({ backdrop: p.backdrop, keyboard: true });
                // }

                // if (p.id) {
                //     $(p.id).modal('show');
                // } else {
                //     console.log('Missing id in params')
                // }
                // if (p.draggable) {
                //     $(p.id).draggable({ handle: 'h4' });
                // }
            })
        }
    }
}]);


// angular.module('tfof').directive('modal', ['ModalService', function(ModalService) {
//     return {
//         restrict: 'A',
//         link: function(scope,element,attributes) {
//             element.bind('click', function(){
                
//                 try {
//                     var p = JSON.parse(JSON.stringify(eval("(" + attributes.modal + ")")))
//                 } catch (e) {
//                     if (!angular.isString(p)) {
//                         p = { id: attributes.modal };
//                     } else {
//                         console.log('haha!@@@@@@@@@@@@@@@@@@@@@@@@@@@@@')
//                         return;
//                     }
//                 }

//                 if(angular.isUndefined(p.size)) {
//                     p['size'] = 'Modal';
//                 }
//                 if(angular.isUndefined(p.classnames)) {
//                     p['classnames'] = '';
//                 }
//                 if(angular.isDefined(p.url) && angular.isDefined(p.id)) {
//                     ModalService.get(p.id, p.url, p.size, p.classnames);
//                 }

//                 if(angular.isDefined(p.backdrop)) {
//                     $(p.id).modal({backdrop: p.backdrop, keyboard: true});
//                 }
                
//                 if(p.id){
//                     $(p.id).modal('show');
//                 } else {
//                     console.log('Missing id in params')
//                 }
//                 if(p.draggable) {
//                     $(p.id).draggable({handle: 'h4'} );
//                 }
//             })
//         }
//     }
// }]);

// angular.module('tfof').directive('modalPage', function() {
//     return {
//         restrict: 'A',
//         link: function(scope,element,attributes) {
//             element.bind('click', function(){       
//                 var p = JSON.parse(JSON.stringify(eval("(" + attributes.modalPage + ")")))
//                 $.ajax({  
//                      url: p.url,
//                      dataType: 'html',  
//                      async: true,  
//                      cache: false,
//                      success: function(data) {
//                         if ($.trim(data).length > 0) {
//                             var footercontent = $(data).find('.client-modal-footer.hide').html();
//                             p['footercontent'] = (p.footercontent) ? p.footercontent: footercontent
//                             p['id'] = (p.id) ? p.id: 'app'
//                             p['bodycontent'] = data
//                             var modalw = createModal(p)
//                             $(modalw).modal('show');
//                         } else {
//                             if (modalw) {
//                                 $(modalw).modal('hide');
//                             }
//                         }
//                      }
//                 });
//             })
//         }
//     }
// });

//Cant use with ng-repeat.
angular.module('tfof').directive('checkParent', function() {
    return {
        restrict: 'A',
        link: function(scope,element,attributes) {
            element.bind('click', function() {                              
                var count = 0, checkedchi = 0;
                var classname = angular.element(attributes.checkParent).attr('check-children');
                angular.forEach(angular.element(classname), function(chi, i) {
                    count++;
                    if(typeof angular.element(chi).attr('checked') !== 'undefined' && angular.element(chi).attr('checked') != false) {
                        checkedchi++;
                    }
                })              
                if(count != checkedchi) {
                    angular.element(attributes.checkParent).removeAttr('checked')
                } else {
                    angular.element(attributes.checkParent).attr('checked','true')
                }               
            })
        }
    }
})

//Cant use with ng-repeat.
angular.module('tfof').directive('checkChildren', function() {
    return {
        restrict: 'A',
        link: function(scope,element,attributes) {
            element.bind('click', function() {                              
                var fn = '';
                angular.forEach(angular.element(attributes.checkChildren),function(val) {
                    if(typeof element.attr('checked') !== 'undefined' && element.attr('checked') != false) {
                        fn += ((fn.length > 0) ? '; ' : '') + angular.element(val).attr('ng-model') + "='" + angular.element(val).attr('ng-true-value') + "'";
                    } else {
                        fn += ((fn.length > 0) ? '; ' : '') + angular.element(val).attr('ng-model') + "='" + angular.element(val).attr('ng-false-value') + "'";
                    }                   
                })  
                //console.log(fn);
                scope.$apply(fn);           
            })
        }
    }
});


//Cant use with ng-repeat.
angular.module('tfof').directive('checkAll', function() {
    return {
        restrict: 'A',
        link: function(scope,element,attributes) {
            element.bind('click', function() {              
                angular.forEach(angular.element(attributes.checkAll),function(val) {
                    if(typeof element.attr('checked') !== 'undefined' && element.attr('checked') != false) {
                        angular.element(val).attr('checked',true);                      
                    } else {
                        angular.element(val).removeAttr('checked');                     
                    }                   
                })                          
            })
        }
    }
});

// angular.module('tfof').directive('openMiniAttrEditor', function() {
//     return {
//         restrict: 'A',
//         link: function(scope,element,attributes) {
//             element.bind('click', function() {              
//                 console.log('HAHAHAH');
//             })
//         }
//     }
// });

angular.module('tfof').directive('slugify', function() {
    return {
        restrict: 'A',
        link: function(scope,element,attributes) {
            element.bind('keyup', function(){
                var separator = '-'
                if(attributes.slugify.length > 0) { separator = attributes.slugify }                
                element.val(
                    slugify(element.val(),separator)
                )
            }) 
        }
    }
});

angular.module('tfof').directive('tab_old', function() {
    return {
        restrict: 'A',
        link: function(scope,element,attributes) {
            element.bind('click', function(){
                atabs = element.parent().parent().find('a[tab]');
                if(element.parent().parent().hasClass('nav-tabs')) {
                    litabs = element.parent().parent().find('li');                  
                    angular.forEach(litabs, function(ele) {
                        angular.element(ele).removeClass('active');
                    });
                    element.parent().addClass('active');
                };
                angular.forEach(atabs, function(ele) {
                    angular.element(angular.element(ele).attr('tab')).hide();
                });
                angular.element(element.attr('tab')).show();

            })
        }
    }
});


angular.module('tfof').directive('ngTab', ['$location',function(location) {
    return {
        restrict: 'A',
        link: function(scope,element,attributes) {
            element.bind('click', function(){
                var pane = attributes.ngTab
                var panes = angular.element(element).closest('[ng-tab-panes]').attr('ng-tab-panes')
                if(!_.isUndefined(pane) && !_.isUndefined(panes)) {
                    scope.$apply("showPane({panes:'" + panes + "',pane:'" + pane + "'})")
                }
            })
        }
    }
}]);

angular.module('tfof').directive('toggle', function() {
    return {
        restrict: 'A',
        link: function(scope,element,attributes) {      
            var originalClass = element.children('i').attr('class');    
            element.bind('click', function() {
                angular.element(attributes.toggle).toggle();
                if(element.children('i').attr('alt-class')) {
                    var altClass = element.children('i').attr('alt-class');                                 
                    element.children('i').toggleClass(originalClass);
                    element.children('i').toggleClass(altClass);
                }
            });
        
        }
    }
});


angular.module('tfof').directive('toggleFilter', function() {
    return {
        restrict: 'A',
        link: function(scope, element, attributes) {
            element.bind('click', function() {
                var isOpen = element.parent().hasClass('open');
                if(attributes.toggleFilter == "") {
                    var dropdowns = element.parent().parent().find('.btn-group');
                } else {

                    var dropdowns = $(attributes.toggleFilter).children().find('.btn-group');
                }
                angular.forEach( dropdowns, function(v, k){
                    angular.element(v).removeClass('open');
                })
                element.parent().removeClass('open');
                if(!isOpen) {
                    element.parent().addClass('open');
                    element.parent().children().find('input[type="text"]').focus();
                }
            })
        }
    }
});

angular.module('tfof').directive('ngEnter', function() {
    return function(scope, element, attrs) {
        element.bind("keydown", function(event) {
            if(event.which === 13) {
               // scope.$apply(function(){
                scope.$eval(attrs.ngEnter);
                //});
                event.preventDefault();
            }
        });
    };
});

angular.module('tfof').directive('ngSelectableText', function() {
    return function(scope, element, attrs) {
        element.bind("click", function(event) {
            angular.element(element).focus();
            angular.element(element).select();
        });
    };
});
angular.module('tfof').directive('ngEscMessages', function() {
    return function(scope, element, attrs) {
        element.bind("keyup", function(event) {
            if(event.which === 27) {
                $('.messenger').addClass('hide')
            }
        });
    };
});

angular.module('tfof').directive('showPassword', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attributes) {
            var originalClass = element.children('i').attr('class');
            element.bind('click', function () {
                angular.forEach(attributes.showPassword.split(','), function (v, k) {
                    if (angular.element(v).attr('type') == 'password') {
                        angular.element(v).attr('type', 'text')
                    } else {
                        angular.element(v).attr('type', 'password')
                    }
                })
            });
        }
    };
});

angular.module('tfof').directive('ngTooltip', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attributes) {
            element.bind('click', function () {
                $(element).tooltip();
            });
        }
    };
});

Selectize.define('enter_key_submit', function (options) {
    var self = this;

    this.onKeyDown = (function (e) {
      var original = self.onKeyDown;

      return function (e) {
        // this.items.length MIGHT change after event propagation.
        // We need the initial value as well. See next comment.
        var initialSelection = this.items.length;
        original.apply(this, arguments);

        if (e.keyCode === 13
            // Necessary because we don't want this to be triggered when an option is selected with Enter after pressing DOWN key to trigger the dropdown options
            && initialSelection && initialSelection === this.items.length
            && this.$control_input.val() === '') {
            this.loadIt = true;
          //self.trigger('submit');
        }
      };
    })();
  });


angular.module('tfof').directive('ngSelectized', [
    'BasicService' ,'MessageService' ,'$timeout'
    ,function(
        BasicService
        ,MessageService
        ,$timeout
    ) {
    return {
        restrict: 'AE',
        link: function(scope,element,attrs) {
            return $timeout(function() {
                if (_.isUndefined(attrs.ngSelectized) || attrs.ngSelectized.length == 0) {
                    $(element).selectize({
                        plugins: [],
                        create: (attrs.create == "True" ? true : false),
                        createOnBlur: (attrs.create == "True" ? true : false)
                    });
                } else {
                    if(!angular.isDefined(attrs.labelFields)) {
                        alert('Selectize Error LabelFields parameter required');
                    } else {
                        $(element).selectize({
                            plugins: [],
                            create: false,
                            valueField: attrs.valueField,
                            labelField: attrs.labelFields.split(',')[0],
                            placeholder: attrs.placeholder,
                            loadThrottle: 1000,
                            highlight: false,
                            persist: false,
                            openOnFocus: true,
                            onType: function(e) {
                                //console.log(e)
                            },
                            
                            render: {
                                item: function(item, escape) {

                                    var labels = [];
                                   
                                    angular.forEach(
                                        _.filter(attrs.labelFields.split(','),function(w) {return w.slice(0,1) != '-' ? w: false})
                                        , function(v, i) {
                                            labels.push(item[v]);
                                    });

                                    return '<div class="selected-value">' +
                                        (item[attrs.valueField] ? '<span class="id">' + escape(item[attrs.valueField]) + '</span>' : '') +
                                        '<span class="name">' + escape(_.compact(labels).join(' ').trim()) + '</span>' +
                                    '</div>';
                                },
                                option: function(item, escape) {
                                    var texts = [];
                                    var metaLine = '';
                                    console.log(attrs.create)
                                    angular.forEach(attrs.labelFields.split(','), function(v, i){
                                        if(v.slice(0,1) == '-') {
                                            //Put in secondary areas
                                            v = v.replace('-','');
                                            if(angular.isDefined(item[v])) {
                                                metaLine += '<li>' + splitUpperCase(v).join(' ') 
                                                + ': <span class="value">' + item[v] + '</span></li>';
                                            }
                                        } else {
                                            texts.push(item[v] != null ? escape(item[v]) : null);    
                                        }
                                    });
                                
                                    
                                    var options = '<div>' +
                                        '<span class="title">' +
                                            '<span class="name">' + _.compact(texts).join(' ') + '</span>' +
                                        '</span><ul class="meta">' + metaLine + '</ul>'
                                    '</div>';
                                    return options;
                                }
                            },
                            score: function(search) {
                                return function(item) {
                                    return 1
                                };
                            },

                            load: function(searchTerm, callback) {
                                var self = this;
                        
                                if(searchTerm == null || searchTerm.trim() == "") return;
                                
                                searchTerm = [searchTerm];

                                //Set the top limit
                                var oDataFilter = BasicService.oDataFilter(attrs.ngSelectized);
                                if(angular.isDefined(attrs.top)) 
                                    oDataFilter.limit = attrs.top;
                                
                                //console.log(searchTerm);
                                //Set the parameters to search on.
                                if(angular.isDefined(attrs.filters)) {
                                    angular.forEach(attrs.filters.split(','), function(p){
                                        //Or operation is 100% needed for 2 Property lookup with 1 Parameter.
                                        var comp =(angular.isDefined(attrs.searchType) && attrs.searchType == "equals") ? "equals" : "startsWith";
                                        oDataFilter.filters[p] = { values: searchTerm, comparator: comp, operator: 'or', dataType: "String" };
                                        //oDataFilter.include.push(p);
                                        //Change the operator to or to be able to look into multiple fields
                                        // oDataFilter.operator = 'or';
                                        ////console.log(attrs.searchType)
                                        //if(angular.isDefined(attrs.searchType) && attrs.searchType == "equals") {
                                        //    oDataFilter.quals.push(p)
                                        //} else {
                                        //    oDataFilter.startsWith.push(p)
                                        //}
                                        console.log(oDataFilter)
                                    });
                                }

                                if (angular.isDefined(attrs.restrictBy)) {
                                    angular.forEach(attrs.restrictBy.split(','), function(v, k){
                                        restrictArray = v.split(':');
                                        console.log(restrictArray, restrictArray.length)
                                        if(restrictArray.length == 2) {
                                            //oDataFilter.filters[restrictArray[0]] = restrictArray[1];
                                            oDataFilter.filters[restrictArray[0]] = {
                                                values: [restrictArray[1]],
                                                dataType: "String",
                                                comparator: "equals",
                                                operator: "and",
                                                inputType: "String",
                                                selectedOptions: [],
                                                isRestricted: true
                                            }
                                            //oDataFilter.include.push(restrictArray[0]);
                                            //oDataFilter.equals.push(restrictArray[0]);
                                        }
                                    })
                                }
                                //Set the parameters to be returned
                                if(angular.isDefined(attrs.labelFields)) {
                                    oDataFilter.include = attrs.labelFields.replace(/-/g,'').split(",");
                                }                             
                                    
                                //Set the order by
                                if(angular.isDefined(attrs.orderBy)) {
                                    angular.forEach(attrs.orderBy.split(','), function(p){
                                        oDataFilter.order.push(
                                            {   name: p.replace('-','')
                                                , isDescending: (p.slice(0,1) == '-' ? true : false)
                                            }
                                        )
                                    });
                                }
                                BasicService.oData(oDataFilter).single(function(result) {
                                    //Success
                                    angular.forEach(self.options, function(o,k){
                                        if(self.getValue().indexOf(k) == -1){
                                            delete self.options[k]
                                        }
                                    })
                                    if(result.Items.length == 0) {
                                        MessageService.flashError('Could not find \'' + searchTerm[0].replace("|","") + '\'. Please try again.')
                                    }
                                    if(result.Next > 0){
                                        MessageService.flashInfo('There are ' + (result.Count - attrs.top) +  ' more "' + searchTerm[0].replace("|","") + '". Keep typing for a shorter set.')
                                    }
                                    callback(result.Items)
                                }, function() {
                                    MessageService.flashError("An error occured. Please contact a system administrator.");
                                });
                            }
                        }).on('change', function(event){
                            // if (params.changeLocation, ) {
                            //     window.location.href = element.context.value;
                            // }
                            if(angular.isDefined(attrs.ngModal)) {
                                scope.$apply(applyChange);    
                            }
                        });
                        
                        function applyChange() {
                            try{
                                attrs.ngModel.$setValue(element.context.value);
                            }catch(e){
                                //attrs.value = element.context.value;
                            }
                        }
                    }
                }
            })
        }
    };
}]);


angular.module('tfof').directive('localStore', ['$timeout', function ($timeout) {
    return {
        restrict: 'AE',
        link: function(scope, element, attributes, controller) {
            element.bind('click', function() {
                if(angular.isDefined(attributes.localStoreSet)) {
                    localStorage.setItem(attributes.localStore, attributes.localStoreSet);    
                }
                if(angular.isDefined(attributes.localStoreUnset)) {
                    localStorage.removeItem(attributes.localStore);
                }
                if(angular.isDefined(attributes.localStoreToggle)) {
                    if(angular.isDefined(localStorage[attributes.localStore]) || 
                        localStorage[attributes.localStore] == attributes.localStoreToggle) {
                        localStorage.removeItem(attributes.localStore);       
                    } else {
                        localStorage.setItem(attributes.localStore, attributes.localStoreToggle);       
                    }
                }
                
                scope[attributes.localStore] = localStorage[attributes.localStore];
                //controller.$render();
                scope.$apply();
            });

          
            scope[attributes.localStore] = localStorage[attributes.localStore];
            //controller.$render();
            //}, 0);  //Calling a scoped method
        }
    }
}]);

angular.module('tfof').directive('ngValidateType', function () {
    return {
        restrict: 'AE',
        link: function (scope, element, attributes) {
            element.bind('keyup onblur', function () {
                console.log(element.val());

                console.log(attributes.ngValidateType);
            });
        }
    }
});

angular.module('tfof').directive('ngNestedSortable', ['BasicService','$timeout',function(BasicService,$timeout) {
    return {
        restrict: 'AE',
        link: function(scope,element,attributes) {

            $().ready(function(){
                var ns = $(element).nestedSortable({
                    forcePlaceholderSize: true,
                    handle: 'div',
                    helper: 'clone',
                    items: 'li',
                    opacity: .6,
                    placeholder: 'placeholder',
                    revert: 250,
                    tabSize: 25,
                    tolerance: 'pointer',
                    toleranceElement: '',
                    maxLevels: 2,
                    isTree: false,
                    expandOnHover: 700,
                    startCollapsed: false,
                    change: function(){
                        //console.log('Relocated item');
                    }
            });

            $('.expandEditor').attr('title','Click to show/hide item editor');
            $('.disclose').attr('title','Click to show/hide children');
            $('.deleteMenu').attr('title', 'Click to delete item.');

            $('.disclose').on('click', function() {
                $(this).closest('li').toggleClass('mjs-nestedSortable-collapsed').toggleClass('mjs-nestedSortable-expanded');
                $(this).toggleClass('ui-icon-plusthick').toggleClass('ui-icon-minusthick');
            });

            $('.expandEditor, .itemTitle').click(function(){
                var id = $(this).attr('data-id');
                $('#menuEdit'+id).toggle();
                $(this).toggleClass('ui-icon-triangle-1-n').toggleClass('ui-icon-triangle-1-s');
            });

            $('.deleteMenu').click(function(){
                var id = $(this).attr('data-id');
                $('#menuItem_'+id).remove();
            });

            $('#serialize').click(function(){
                //serialized = $(element).nestedSortable('serialize');
                //console.log(serialized)
                //$('#serializeOutput').text(serialized+'\n\n');
                serialized = []
                angular.forEach(element.children('li'), function(li,k){
                    children = []
                    if($(li).has('ol')) {
                        c = seriz($(li).children('ol').children('li'))
                    }
                    
                    serialized.push({i:k,id:$(li).attr('data-id'),children: c})
                    
                })
                
                //console.log(serialized)
                //console.log(serialized.length)
            })

            $('#toHierarchy').click(function(e){
                hiered = $(element).nestedSortable('toHierarchy', {startDepthCount: 0});
                hiered = dump(hiered);
                //console.log(hiered)
                //(typeof($('#toHierarchyOutput')[0].textContent) != 'undefined') ?
                //$('#toHierarchyOutput')[0].textContent = hiered : $('#toHierarchyOutput')[0].innerText = hiered;
            })

            $('#toArray').click(function(e){
                arraied = $(element).nestedSortable('toArray', {startDepthCount: 0});
                arraied = dump(arraied);
                //console.log(arraied)
                //(typeof($('#toArrayOutput')[0].textContent) != 'undefined') ?
                //$('#toArrayOutput')[0].textContent = arraied : $('#toArrayOutput')[0].innerText = arraied;
            });

            var seriz = function(lis) {
                children = []
                angular.forEach(lis, function(li,k){
                    children.push({i:k,id:$(li).attr('data-id')})
                })
                return children
            }
        });
        }
    };
}]);

// workarounds for timezone bugs in datepicker widget
angular.module('tfof').directive('datepickerLocaldate', ['$parse', function ($parse)
{
    var directive =
        {
            restrict: 'A',
            require: ['ngModel'],
            link: link
        };
    return directive;

    function link(scope, element, attributes, controllers)
    {
        var ngModelController = controllers[0];

        // formatter controls how the model value is formatted for display
        ngModelController.$formatters.push(function (s)
        {
            try
            {
                // convert string to Date object
                if ((typeof s === "string") && (s.length > 0))
                {
                    // add time equal to the timezone offset, so datepicker input will display correctly
                    var d = new Date(s);
                    if ((d.getTimezoneOffset() > 0) && (((d.getHours()*60) + d.getMinutes()) != d.getTimezoneOffset()))
                        d.setMinutes(d.getMinutes() + d.getTimezoneOffset());
                }
                else
                {
                    var d = new Date();
                    d.setHours(0,0,0,0);
                }

                // update view
                if (!ngModelController.$viewValue)
                    ngModelController.$setViewValue(d.toISOString());
                return d;
            }
            catch(err)
            {
                // date may not be set yet
            }
            return s;
        });

        // parser changes how view values are saved in the model
        // need to remove timezone offset added by formatter
        ngModelController.$parsers.push(function (d)
        {
            if (!(d instanceof Date))
                return d;

            // add time equal to the timezone offset, so datepicker widget will display correctly
            if ((d.getTimezoneOffset() > 0) && (((d.getHours()*60) + d.getMinutes()) != d.getTimezoneOffset()))
                d.setMinutes(d.getMinutes() + d.getTimezoneOffset());
            return d;
        });
    }
}]);




// function createModal(p) {
//     if(p.redraw) {
//         if ($('#' + p.id).length > 0) {
//             $('#' + p.id).modal('hide');
//             $('#' + p.id).remove();
//         }
//     }

//     var modal = $('#' + p.id);
//     if($(modal).length == 0) {

//         var divelement = document.createElement('div');
//         var modaltemplate = '<div class="modal ' + p.classname + '" id="' + p.id + '" tabindex="-1" role="dialog" aria-labelledby="' + p.id + '_label" aria-hidden="true">' +
//                     '<div class="modal-dialog"><div class="modal-content"><div class="modal-header">' + 
//                     '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' + 
//                     '<h4 class="modal-title" id="' + p.id + '_label">' + p.title + '</h4>' + '</div>' ;
        
//         if(p.bodycontent) {
//             modaltemplate = modaltemplate + '<div class="modal-body">' + p.bodycontent + '</div>';
//         } else {
//             //console.log(p.bodycontent)
//             modaltemplate = modaltemplate + '<div class="modal-body"></div>';
//         }
        
    
//         if (p.footercontent) {
//             modaltemplate = modaltemplate + '<div class="modal-footer">' + p.footercontent + '</div>';               
//         }
//         else {  
//             if(p.footer == 'show') {
//                 modaltemplate = modaltemplate + '<div class="modal-footer"></div>';
//             }
//         }
//         modaltemplate  = modaltemplate + '</div></div></div>';
            
//         $(divelement).html(modaltemplate);
//         $('body').append($(divelement).html());
//     }

//     modal = $('#' + p.id);
//     if (p.backdrop) {
//         $(modal).modal({backdrop: p.backdrop});
//     } else {
//         $(modal).modal({backdrop: false});
//     }
    
    
//     if (p.show) {
//         $(modal).modal('show');
//     }
    
//     if(_.isUndefined(p.draggable) || p.draggable) {
//         $(modal).draggable({handle:'.modal-title',cursor:'move'} );
//         $(modal).children('.modal-title').css({cursor:'move'})
//     }
    
//     return modal;                    
// }   



$('html').click(function(){
    $('.dropdown-menu').parent('.open').removeClass('open')
})

$('.dropdown-menu').parent().click(function(event){
    var toggler = $(this).has('[data-toggle="dropdown"]');
    if (!toggler.length) {
        event.stopPropagation();
    }
})


angular.element(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip()
});

$(function () {
    $('[data-toggle="popover"]').popover()
});


// function dump(arr,level) {
//     var dumped_text = "";
//     if(!level) level = 0;

//     //The padding given at the beginning of the line.
//     var level_padding = "";
//     for(var j=0;j<level+1;j++) level_padding += "    ";

//     if(typeof(arr) == 'object') { //Array/Hashes/Objects
//         for(var item in arr) {
//             var value = arr[item];

//             if(typeof(value) == 'object') { //If it is an array,
//                 dumped_text += level_padding + "'" + item + "' ...\n";
//                 dumped_text += dump(value,level+1);
//             } else {
//                 dumped_text += level_padding + "'" + item + "' => \"" + value + "\"\n";
//             }
//         }
//     } else { //Strings/Chars/Numbers etc.
//         dumped_text = "===>"+arr+"<===("+typeof(arr)+")";
//     }
//     return dumped_text;
// }


function slugify(input,separator) {
    //console.log(separator)
    var regex = '[^a-z0-9' + separator + ']'
    var multiseparator  = separator+'+'
    return input.toLowerCase().replace(/-+/g,separator).replace(/\s+/g,separator).replace(new RegExp(multiseparator,'gi'),separator).replace(new RegExp(regex, 'gi'), '')
}


function splitUpperCase(str) {
    return str.split(/(?=[A-Z])/);
}

if (!String.prototype.startsWith) {
    String.prototype.startsWith = function (searchString, position) {
        position = position || 0;
        return this.substr(position, searchString.length) === searchString;
    };
}

if (!String.prototype.endsWith) {
    String.prototype.endsWith = function (searchString, position) {
        var subjectString = this.toString();
        if (typeof position !== 'number' || !isFinite(position) || Math.floor(position) !== position || position > subjectString.length) {
            position = subjectString.length;
        }
        position -= searchString.length;
        var lastIndex = subjectString.lastIndexOf(searchString, position);
        return lastIndex !== -1 && lastIndex === position;
    };
}
//angular.module('tfof').directive('modalChange', function () {
//    return {
//        restrict: 'A',
//        link: function (scope, element, attributes) {
//            element.bind('change', function () {
//                try {
//                    var p = JSON.parse(JSON.stringify(eval("(" + attributes.modalChange + ")")))
//                } catch (e) {
//                    if (!angular.isString(p)) {
//                        p = { id: attributes.modalChange };
//                    } else {
//                        return;
//                    }
//                }
//                if (p.backdrop) {
//                    $(p.id).modal({ backdrop: p.backdrop, keyboard: true });
//                }

//                if (p.id) {
//                    $(p.id).modal('show');
//                } else {
//                    console.log('Missing id in params')
//                }
//                if (p.draggable) {

//                    $(p.id).draggable({ handle: 'h4' });
//                }
//            })
//        }
//    }
//});

//angular.module('tfof').directive('modalChange', function() {
//    return {
//        restrict: 'A',
//        link: function(scope,element,attributes) {
//            element.bind('change', function(){
//                try {
//                    var p = JSON.parse(JSON.stringify(eval("(" + attributes.modal + ")")))
//                } catch (e) {
//                    if (!angular.isString(p)) {
//                        p = { id: attributes.modal };
//                    } else {
//                        return;
//                    }
//                }

//                if(angular.isDefined(p.url) && angular.isDefined(p.id)) {
                    
//                    //Remove modal if it exists
//                    var div = document.getElementById(p.id.replace('#',''));
//                    if(div != null) {
//                        div.parentNode.removeChild(div);
//                       } 

//                    //Load modal and insert into new 
//                    $.ajax({  
//                        url: p.url + '?Layout=Modal',
//                        dataType: 'html',  
//                        async: false,  
//                        cache: false,
//                        success: function(data) {
//                            //var divelement = document.createElement('div');
//                            data = data.replace('{id}', p.id.replace('#','')).replace('ng-cloak','');
//                            //$(divelement).html(data)
//                            $('body').append(data);
//                        }
//                    });
//                }

//                if(angular.isDefined(p.backdrop)) {
//                    $(p.id).modal({backdrop: p.backdrop, keyboard: true});
//                }
                
//                if(p.id){
//                    $(p.id).modal('show');
//                } else {
//                    console.log('Missing id in params')
//                }
//                if(p.draggable) {
//                    $(p.id).draggable({handle: 'h4'} );
//                }
//            })
//        }
//    }
//});






angular.module('tfof').directive('dataselector', function () {
    return {
        restrict: 'A',
        require: ['ngModel', 'select'], //Require ng-model and select in order to restrict this to be used with anything else
        link: function (scope, elm, attrs, ctrl) {
            //get configured data to be selected, made this configurable in order to be able to use it with any other data attribs
            var dataProp = attrs.dataselector,
                ngModel = ctrl[0];
            elm.on('change', handleChange);//Register change listener

            scope.$on('$destroy', function () {
                elm.off('change', handleChange);
            });

            function handleChange() {
                //get the value
                var value = this.value;
                //get the dataset value
                var dataVal = JSON.parse(elm[0].querySelector('option[value="' + this.value + '"]').dataset[dataProp]);
                //reset ngmodel view value
                ngModel.$setViewValue(dataVal);
                ngModel.$render();
                //set element value so that it selects appropriate item
                elm.val(value);
            }
        }
    }
});

$(document).ready(function () {
    angular.forEach(angular.element('.fixed-header'), function (f, k) {
        var tw = 0;
        console.log(angular.element(f).find('[data-freeze-width]'));
        angular.forEach(angular.element(f).find('[data-freeze-width]'), function (o, k) {
            var ref = angular.element(o).data('freeze-width');
            var w = angular.element(ref).width();
            var pr = parseInt(angular.element(ref).css('padding-right').replace('px', ''));
            var pl = parseInt(angular.element(ref).css('padding-left').replace('px', ''));
            var br = parseInt(angular.element(ref).css('border-right-width').replace('px', ''));
            var bl = parseInt(angular.element(ref).css('border-left-width').replace('px', ''));
            var mr = parseInt(angular.element(ref).css('margin-right').replace('px', ''));
            var ml = parseInt(angular.element(ref).css('margin-left').replace('px', ''));
            console.log(angular.element(o).text(), w, pr, pl, br, bl, mr, ml);
            tw += (w + pr + pl + br + bl + mr + ml);
            angular.element(o).css('width', (w + pr + pl + br + bl + mr + ml));
        });
        angular.element(f).css('width',tw);
    });
    //angular.element('[data-freeze-width]').css('width', angular.element('[data-freeze-width]').data('freeze-width'));
    //console.log(angular.element('[data-freeze-width]'));
})