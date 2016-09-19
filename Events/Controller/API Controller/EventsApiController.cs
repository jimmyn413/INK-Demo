using Sabio.Data;
using Sabio.Web.Domain;
using Sabio.Web.Models;
using Sabio.Web.Models.Requests;
using Sabio.Web.Models.Responses;
using Sabio.Web.Services;
using Sabio.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sabio.Web.Controllers.Api
{
    [RoutePrefix("api/events")]
    public class EventsApiController : ApiController
    {
        private ILocationService _locationService { get; set; }
        public EventsApiController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [Route("discover"), HttpGet]
        public HttpResponseMessage Get([FromUri] EventsGetRequest GetEvents)
        {

            List<Events> eventsData = EventsService.Get(GetEvents);

            ItemsResponse<Events> response = new ItemsResponse<Events>();

            response.Items = eventsData;


            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route, HttpGet]
        [Authorize]
        public HttpResponseMessage GetByUser()
        {
            string UserId = UserService.GetCurrentUserId();

            List<Events> eventsData = EventsService.GetByUser(UserId);

            ItemsResponse<Events> response = new ItemsResponse<Domain.Events>();

            response.Items = eventsData;


            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("{Id:int}"), HttpGet]
        public HttpResponseMessage GetById(int Id)
        {
            Events eventsData = EventsService.GetById(Id);

            User eventCreator = UserProfileServices.GetFullUserById(eventsData.UserId);

            eventsData.Organizer = eventCreator.UserMini;

            ItemResponse<Events> response = new ItemResponse<Events>();

            response.Item = eventsData;


            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route, HttpPost]
        [Authorize]
        public HttpResponseMessage Post(EventsRequest model)
        {

            if (model == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Request payload was null");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            model.UserId = UserService.GetCurrentUserId();

            //post new event basic info
            int blogId = EventsService.Post(model);

            ItemResponse<int> response = new ItemResponse<int>();

            response.Item = blogId;

            if (model.Location != null && model.Location.Lat != null)
            {
                //post new event location
                int locId = _locationService.Post(model.Location);

                //post to event info and event location mapping table
                EventLocationRequest evLoc = new EventLocationRequest();

                evLoc.EventId = blogId;
                evLoc.LocationId = locId;

                EventLocationService.Post(evLoc);
            }


            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("{id:int}"), HttpPut]
        [Authorize]
        public HttpResponseMessage Put(int Id, EventsUpdateRequest model)
        {
            if (model == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Request payload was null");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            model.UserId = UserService.GetCurrentUserId();

            EventsService.Put(Id, model);

            LocationServices _locationServices = new LocationServices();

            _locationServices.Put(model.Location.Id, model.Location);

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        [Route("{id:int}"), HttpDelete]
        [Authorize]
        public void Delete(int Id)
        {
            EventsService.Delete(Id);
        }

        [Route("media/{id:int}"), HttpPut]
        [Authorize]
        public void UpdateMedia(int Id, EventsRequest model)
        {
            EventsService.UpdateMedia(Id, model);
        }

        [Route("tag/{slug}"), HttpGet]
        public HttpResponseMessage GetEventsByTagSlug(string slug)
        {
            List<Events> eventsData = EventsService.GetByTagSlug(slug);

            ItemsResponse<Events> response = new ItemsResponse<Events>();

            response.Items = eventsData;

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("tag"), HttpGet]
        public HttpResponseMessage TagsWithEvents()
        {
            List<TagsCount> eventsData = EventsService.TagsThatHaveEvents();

            ItemsResponse<TagsCount> response = new ItemsResponse<TagsCount>();

            response.Items = eventsData;

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("{lat:decimal}/{lon:decimal}/{distance:decimal}"), HttpGet]
        public HttpResponseMessage EventsByLocation(decimal lat, decimal lon, decimal distance)
        {
            List<Events> eventsData = EventsService.EventsByLocation(lat, lon, distance);

            ItemsResponse<Events> response = new ItemsResponse<Events>();

            response.Items = eventsData;

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }
    }
}
