sabio.services.events = sabio.services.events || {};

sabio.services.events.discover = function (payload, onSuccess, onError) {

    var url = "/api/events/discover";
    var settings = {
        cache: false
    , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
    , dataType: "json"
    , data: payload
    , success: onSuccess
    , error: onError
    , type: "GET"
    };

    $.ajax(url, settings);
};



sabio.services.events.get = function (onSuccess, onError) {

    var url = "/api/events";
    var settings = {
        cache: false
    , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
    , dataType: "json"
    , success: onSuccess
    , error: onError
    , type: "GET"
    };

    $.ajax(url, settings);
};

sabio.services.events.insertNewEvent = function (payload, onSuccess, onError) {

    var url = "/api/Events";
    var settings = {
        cache: false
    , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
    , dataType: "json"
    , data: payload
    , success: onSuccess
    , error: onError
    , type: "Post"
    };

    $.ajax(url, settings);
};

sabio.services.events.delete = function (id, onSuccess, onError) {

    var url = "/api/events/" + id;
    var settings = {
        cache: false
    , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
    , dataType: "json"
    , success: onSuccess
    , error: onError
    , type: "DELETE"
    };

    $.ajax(url, settings);
};

sabio.services.events.updateEvent = function (id, data, onSuccess, onError) {

    var url = "/api/events/" + id;
    var settings = {
        cache: false
    , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
    , data: data
    , dataType: "json"
    , success: onSuccess
    , error: onError
    , type: "PUT"
    };

    $.ajax(url, settings);
};

sabio.services.events.getById = function (id, onSuccess, onError) {

    console.log("sabio.services.events.getById has been called");

    var url = "/api/events/" + id;
    var settings = {
        cache: false
    , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
    , dataType: "json"
    , success: onSuccess
    , error: onError
    , type: "GET"
    };

    $.ajax(url, settings);
}
sabio.services.events.updateEventMedia = function (id, data, onSuccess, onError) {

    var url = "/api/events/media/" + id;
    var settings = {
        cache: false
    , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
    , data: data
    , dataType: "json"
    , success: onSuccess
    , error: onError
    , type: "PUT"
    };

    $.ajax(url, settings);
};

sabio.services.events.getAttendeesByEvent = function (eventId, rsvpStatus, onSuccess, onError) {

    console.log("sabio.services.events.getAttendeesByEvent has been called");

    var url = "/api/eventinvites/attendees/" + eventId + "/" + rsvpStatus;
    var settings = {
        cache: false
    , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
    , dataType: "json"
    , success: onSuccess
    , error: onError
    , type: "GET"
    };

    $.ajax(url, settings);
}

sabio.services.events.getEventsByTagSlug = function (tagslug, onSuccess, onError) {

    var url = "/api/events/tag/" + tagslug;
    var settings = {
        cache: false
    , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
    , dataType: "json"
    , success: onSuccess
    , error: onError
    , type: "GET"
    };

    $.ajax(url, settings);
};

sabio.services.events.tagsWithEvents = function (onSuccess, onError) {

    var url = "/api/events/tag";
    var settings = {
        cache: false
    , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
    , dataType: "json"
    , success: onSuccess
    , error: onError
    , type: "GET"
    };

    $.ajax(url, settings);
};

sabio.services.events.eventsByLocation = function (lat, lon, distance, onSuccess, onError) {

    var url = "/api/events/" + lat + "/" + lon +"/"+ distance;
    var settings = {
        cache: false
    , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
    , dataType: "json"
    , success: onSuccess
    , error: onError
    , type: "GET"
    };

    $.ajax(url, settings);
};