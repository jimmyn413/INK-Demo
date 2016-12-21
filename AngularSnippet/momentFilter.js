angular.module(APPNAME)
    .filter('moment', [
        function () {
            return function (date, method) {
                var momented = moment.utc(date);
                return momented[method].apply(momented, Array.prototype.slice.call(arguments, 2));
            };
        }
    ]);