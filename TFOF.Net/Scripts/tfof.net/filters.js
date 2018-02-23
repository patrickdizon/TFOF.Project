'use strict';

angular.module('tfof').filter('check', function() {
    return function (input,truestatements,falsestatement) {
        if(!truestatements) {
            truestatements = ['1',1,true];
        }
        if($.inArray(input,truestatements) >= 0) {
            return '<i class="fa fa-check"></i>';
        } else {
            return '';
        }       
    }
});

angular.module('tfof').filter('compact', function () {
    return function (array) {
        if(angular.isArray(array)){
            return _.compact(array)
        } else {
            return array
        }
    };
});

angular.module('tfof').filter('startsWith', function() {
    return function (input,word,casesensitive) {
        if(!input) input= '';
        if(casesensitive) {
            return input.slice(0, word.length) == word;         
        }
        return input.toLowerCase().slice(0, word.length) == word.toLowerCase();     
    }
})


angular.module('tfof').filter('endsWith', function() {
    return function (input,word,casesensitive) {
        if(!input) input= '';
        if(casesensitive) {
            return input.slice(-word.length) == word;
        }
        
        return input.toLowerCase().slice(-word.length) == word.toLowerCase();
    }
})

angular.module('tfof').filter('firstNchars', function () {
    return function (input, count, tail) {
        if (!tail) {
            tail = '';
        }
        count = parseInt(count, 10);
        try {
            //if its an object stringify it
            input = JSON.stringify(input)
        } catch (e) {
            //do nothing its a string
        }
        return input.substr(0, count) + (tail || ' …');
    }
});

angular.module('tfof').filter('optionFilter', function () {
    return function (input, search) {
        if (!input) return input;
        if (!search) return input;
        var expected = ('' + search).toLowerCase();
        var result = {};
        angular.forEach(input, function (value, key) {
            var actual = ('' + value).toLowerCase();
            if (actual.indexOf(expected) !== -1) {
                result[key] = value;
            }
        });
        console.log(result);
        return result;
    }
});


// angular.module('tfof').filter('cut', function () {
//         return function (value, wordwise, max, tail) {
//             if (!value) return '';
//             try {
//                 //if its an object stringify it
//                 value = JSON.stringify(value)
//             } catch (e) {
//                 //do nothing its a string
//             }
//             max = parseInt(max, 10);
//             if (!max) return value;
//             if (value.length <= max) return value;

//             value = value.substr(0, max);
//             if (wordwise) {
//                 var lastspace = value.lastIndexOf(' ');
//                 if (lastspace != -1) {
//                     value = value.substr(0, lastspace);
//                 }
//             }

//             return value + (tail || ' …');
//         };
//     });
angular.module('tfof').filter('more', function() {
    return function(input, limit) {
        if (input.length <= limit+1){
            return input.join(', ')
        } else {
            var more = input.length - limit
            input.length = limit
            return input.join(', ') + ' +' + more + ' more'
        }
    }

})

angular.module('tfof').filter('equals', function() {
    return function(input, limit) {
        var return_val = true
        angular.forEach(input, function(v, i) {
            if (i > 0){
                if(v != input[i-1]) {
                    return_val = false
                }
            }
        })

        return return_val
    }

})

angular.module('tfof').filter('max', function() {
    return function (input,minval) {
        return (input > minval) ? input : minval;
    }
});


angular.module('tfof').filter('min', function() {
    return function (input,minval) {
        return (input < minval) ? input : minval;
    }
});

angular.module('tfof').filter('trim', function() {
    return function (input) {
        return $.trim(input)
    }
})

//Converts bytes into human readable sizes.
angular.module('tfof').filter('filesize', function() {
    return function (input,format) {
        if(input == 0) {return '0 ' + format;}
        if(format == 'KB') return (input/1024).toFixed(2) + ' KB';
        if(format == 'MB') return (input/1048576).toFixed(2) + ' MB';
        if(format == 'GB') return (input/1073741824).toFixed(2) + ' GB';
        if(format == 'TB') return (input/1099511627776).toFixed(2) + ' TB';
        return input + ' B';        
    }
});

//Translates coded column values into nicely formatted human readable text
//returns original value if not in the dictionary
angular.module('tfof').filter('translate', function() {
    return function (input, translations) {
        if (angular.isDefined(translations)) {
            if (translations.hasOwnProperty(input)) {
                return translations[input];
            }
        }
        return input;
    } 
});

angular.module('tfof').filter('sum', function() {
    return function (input,column) {
        var r = 0;
        angular.forEach(input,function(v,k){
            r += parseFloat(v[column])
        })
        return r;
    }
});

//Puts an array in between the specified html tag. 
angular.module('tfof').filter('array', function() {
    return function (input,display_key,separator) {
        var html = ''
        if (!angular.isDefined(separator)) {
           var separator = 'div'
        }
        angular.forEach(input,function(v,i){
            html = html + '<' + separator + '>' + v[display_key] + '</' + separator + '>'
        })
        return html
    }

});

angular.module('tfof').filter('truncatechars', function () {
    return function (input, chars, breakOnWord) {
        if (isNaN(chars)) return input;
        if (chars <= 0) return '';
        if (input && input.length > chars) {
            input = input.substring(0, chars);
            if (!breakOnWord) {
                var lastspace = input.lastIndexOf(' ');
                //get last space
                if (lastspace !== -1) {
                    input = input.substr(0, lastspace);
                }
            }else{
                while(input.charAt(input.length-1) === ' '){
                    input = input.substr(0, input.length -1);
                }
            }
            return input + '...';
        }
        return input;
    };
});

angular.module('tfof').filter('truncatewords', function () {
    return function (input, words) {
        if (isNaN(words)) return input;
        if (words <= 0) return '';
        if (input) {
            var inputWords = input.split(/\s+/);
            if (inputWords.length > words) {
                input = inputWords.slice(0, words).join(' ') + '...';
            }
        }
        return input;
    };
});

angular.module('tfof').filter('alphaColumn', function() {
    return function (input) {
        var ordA = 'a'.charCodeAt(0);
        var ordZ = 'z'.charCodeAt(0);
        var len = ordZ - ordA + 1;
      
        var s = "";
        while(input >= 0) {
            s = String.fromCharCode(parseInt(input) % len + ordA) + s;
            input = Math.floor(parseInt(input) / len) - 1;
        }
        return s;
    };
})

angular.module('tfof').filter('flatAttr', function() {
    return function (input) {
        var flattattrs = []
        angular.forEach(input, function(v,k){
            flattattrs.push(k + ':' + v)
        })
        return flattattrs.join('; ');
    };
})


angular.module('tfof').filter('slugify', function() {
    return function (input,separator) {
        if(!angular.isDefined(separator)) {
            separator = '-'
        }
        return slugify(input,separator)
    };
})


angular.module('tfof').filter('adjustBackground', function() {
    return function (input) {
        var foreground = input.toLowerCase()
        if( foreground == '#fff' || foreground == 'white' || foreground == '#ffffff') {
            return '#cccccc'
        }
        return ''
    };
})

angular.module('tfof').filter('percent', ['$filter', function($filter) {
    return function (input, fractionSize) {
        if (angular.isNumber(input)) {
            return $filter('number')(input * 100, fractionSize) + '%';
        }
        return input;
    };
}])

angular.module('tfof').filter('ifnull', function () {
    return function (input, valueIfnull) {
        if(input == null || String(input).length == 0){
            return valueIfnull;
        }
        return input;
    };
})

angular.module('tfof').filter('phoneNumber', function () {
    return function (phoneNumber) {
        if (!phoneNumber) { return ''; }

        var value = phoneNumber.toString().trim().replace(/^\+/, '');

        if (value.match(/[^0-9]/)) {
            return phoneNumber;
        }

        var country, city, number;

        switch (value.length) {
            case 10: // +1PPP####### -> C (PPP) ###-####
                country = 1;
                city = value.slice(0, 3);
                number = value.slice(3);
                break;

            case 11: // +CPPP####### -> CCC (PP) ###-####
                country = value[0];
                city = value.slice(1, 4);
                number = value.slice(4);
                break;

            case 12: // +CCCPP####### -> CCC (PP) ###-####
                country = value.slice(0, 3);
                city = value.slice(3, 5);
                number = value.slice(5);
                break;

            default:
                return phoneNumber;
        }

        if (country == 1) {
            country = "";
        }

        number = number.slice(0, 3) + '-' + number.slice(3);

        return (country + " (" + city + ") " + number).trim();
    };
});

//Assigns a value to a variable in the localStorage of a browser.
angular.module('tfof').filter('localStore', function() {
    return function(variable, value){
        if(angular.isDefined(variable)) {
            if(angular.isDefined(value)) {
                if(localStorage[variable] == value) {
                    return true;
                }
                return false;
            }
            return localStorage[variable]
        }
    }
});

angular.module('tfof').filter('secondsToTime', ['$filter', function ($filter) {
    return function (seconds, minimumIncrement) {
        var format = (minimumIncrement == 'min' ? 'HH\':\'mm\'' : 'HH\':\'mm\':\'ss\'');
        ////Add another minute if there is a remainder.
        //if (minimumIncrement == 'min' && (seconds % 60) > 0) {
        //    seconds = seconds + (60 - (seconds % 60));
        //}
        return $filter('date')(new Date(1970, 0, 1).setSeconds(seconds), format);
    };
}])


angular.module('tfof').filter('encodeURI', function () {
    return function (url) {
        return encodeURI(url);
    };
})

// angular.module('tfof').filter('toColorSwatches', function() {
   
//     return function (input,separator) {
//         var swatches = []
//         var colorArray = []
//         if(!angular.isDefined(separator)) {
//             separator = ','
//         }
//         if(!angular.isArray(input)){
//             if(angular.isString(input) && input.length > 0) {
//                 colorArray = input.split(separator)
//             }
//         } else {
//             colorArray = input
//         }
        
//         if(colorArray.length > 0){
//             angular.forEach(colorArray, function(color) {
//                 if(/(^#[0-9A-F]{6}$)|(^#[0-9A-F]{3}$)/i.test(color.replace('...',''))) { 
//                     var swatch = '<b style="background: ' + color.replace('...','') +';" class="color-swatch color-swatch-' + color.replace('#','').toLowerCase() + '">&nbsp;</b>'
//                     swatches.push(swatch)
//                     if(color.indexOf('...') > -1) {
//                         swatches.push('<b class="color-swatch"><b class=" padding-top-10 fa fa-ellipsis-h"></b></b>')
//                     }
//                 } else {
//                     swatches.push('<b class="color-swatch color-swatch-empty">&times;</b>')
//                 }
//             })
//         }
//         return swatches.join(' ')
//     };
// })
