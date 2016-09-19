using Sabio.Web.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Sabio.Data;
using Sabio.Web.Models.Requests;
using Sabio.Web.Enums;

namespace Sabio.Web.Services
{
    public class EventsService : BaseService
    {
        public static int Post(EventsRequest model)
        {
            int outputId = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Events_Insert"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@UserId", model.UserId);
                   paramCollection.AddWithValue("@Title", model.Title);
                   paramCollection.AddWithValue("@Description", model.Description);
                   paramCollection.AddWithValue("@Start", model.Start);
                   paramCollection.AddWithValue("@End", model.End);
                   paramCollection.AddWithValue("@EventType", model.EventType);
                   paramCollection.AddWithValue("@IsPublic", model.IsPublic);
                   paramCollection.AddWithValue("@ExternalEventId", model.ExternalEventId);

                   SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                   p.Direction = System.Data.ParameterDirection.Output;

                   paramCollection.Add(p);



                   // adding new paramerter
                   SqlParameter s = new SqlParameter("@TagsId", SqlDbType.Structured);
                   if (model.Tags != null && model.Tags.Any())
                   {
                       s.Value = new IntIdTable(model.Tags);
                   }
                   paramCollection.Add(s);




               }, returnParameters: delegate (SqlParameterCollection param)
               {
                   int.TryParse(param["@Id"].Value.ToString(), out outputId);
               }
               );

            //creating SystemEvents post for new Public Events for recent activity feed
            if (model.IsPublic)
            {
                SystemEventsRequest newModel = new SystemEventsRequest();
                newModel.ActorUserId = model.UserId;
                newModel.EventType = SystemEventTypes.NewEvent;
                newModel.TargetId = outputId;

                SystemEventsService.Post(newModel);
            }

            return outputId;
        }

        public static void Put(int Id, EventsUpdateRequest model)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Events_Update"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Id", model.Id);
                   paramCollection.AddWithValue("@UserId", model.UserId);
                   paramCollection.AddWithValue("@Title", model.Title);
                   paramCollection.AddWithValue("@Description", model.Description);
                   paramCollection.AddWithValue("@Start", model.Start);
                   paramCollection.AddWithValue("@End", model.End);
                   paramCollection.AddWithValue("@EventType", model.EventType);
                   paramCollection.AddWithValue("@IsPublic", model.IsPublic);

                   // adding new parameter
                   SqlParameter s = new SqlParameter("@TagsId", SqlDbType.Structured);
                   if (model.Tags != null && model.Tags.Any())
                   {
                       s.Value = new IntIdTable(model.Tags);
                   }
                   paramCollection.Add(s);


               }, returnParameters: delegate (SqlParameterCollection param)
               {
               }
               );
        }

        public static List<Events> Get(EventsGetRequest model)
        {
            List<Events> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Events_SearchEpic"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@PageSize", model.PageSize);
                   paramCollection.AddWithValue("@PageNumber", model.PageNumber);
                   paramCollection.AddWithValue("@lat", model.Lat);
                   paramCollection.AddWithValue("@lon", model.Lon);
                   paramCollection.AddWithValue("@distance", model.Distance);
                   paramCollection.AddWithValue("@SearchString", model.SearchString);

               }, map: delegate (IDataReader reader, short set) //executes after stored proc
               {
                   Events p = new Events();
                   int startingIndex = 0; //startingOrdinal

                   p.Id = reader.GetSafeInt32(startingIndex++);
                   p.UserId = reader.GetSafeString(startingIndex++);
                   p.EventType = reader.GetSafeInt32(startingIndex++);
                   p.IsPublic = reader.GetSafeBool(startingIndex++);
                   p.Title = reader.GetString(startingIndex++);
                   p.Description = reader.GetSafeString(startingIndex++);
                   p.CountYes = reader.GetSafeInt32(startingIndex++);
                   p.CountNo = reader.GetSafeInt32(startingIndex++);
                   p.CountMaybe = reader.GetSafeInt32(startingIndex++);
                   p.CreateDate = reader.GetSafeDateTime(startingIndex++);
                   p.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
                   p.Start = reader.GetSafeDateTime(startingIndex++);
                   p.End = reader.GetSafeDateTime(startingIndex++);
                   p.MediaId = reader.GetSafeInt32(startingIndex++);

                   Medias m = new Medias();

                   m.MediasTableId = reader.GetSafeInt32(startingIndex++);
                   m.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                   m.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
                   m.MediaType = reader.GetSafeEnum<MediaType>(startingIndex++);
                   m.ContentType = reader.GetSafeString(startingIndex++);
                   m.UserId = reader.GetSafeString(startingIndex++);
                   m.FilePath = reader.GetSafeString(startingIndex++);

                   if(m.MediasTableId > 0)
                   p.Media = m;

                   UserMini u = new UserMini();

                   u.profileName = reader.GetSafeString(startingIndex++);
                   u.profileLastName = reader.GetSafeString(startingIndex++);
                   u.UserId = reader.GetSafeString(startingIndex++);

                   Medias n = new Medias();

                   n.MediasTableId = reader.GetSafeInt32(startingIndex++);
                   n.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                   n.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
                   n.MediaType = reader.GetSafeEnum<MediaType>(startingIndex++);
                   n.ContentType = reader.GetSafeString(startingIndex++);
                   n.UserId = reader.GetSafeString(startingIndex++);
                   n.FilePath = reader.GetSafeString(startingIndex++);

                   if(n.MediasTableId > 0)
                   u.Avatar = n;

                   p.Organizer = u;

                   Location l = new Location();
                   
                   l.Id = reader.GetSafeInt32(startingIndex++);
                   l.Latitude = reader.GetSafeDecimal(startingIndex++);
                   l.Longitude = reader.GetSafeDecimal(startingIndex++);
                   l.Address = reader.GetSafeString(startingIndex++);
                   l.City = reader.GetSafeString(startingIndex++);
                   l.State = reader.GetSafeString(startingIndex++);
                   l.ZipCode = reader.GetSafeString(startingIndex++);

                   p.Location = l;


                   if (list == null)
                   {
                       list = new List<Events>();
                   }
                   list.Add(p);
               }
               );
            return DecorateEvents(list);
        }

        public static List<Events> GetByUser(string UserId)
        {
            List<Events> list = null;
            List<EventTags> tagsList = null;
            DataProvider.ExecuteCmd(GetConnection, "dbo.Events_SelectEventWithMedia"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@UserId", UserId);
               }, map: delegate (IDataReader reader, short set) //executes after stored proc
               {
                   Events p = new Events();
                   EventTags myTags = new EventTags();

                   int startingIndex = 0; //startingOrdinal

                   if (set == 0)
                   {
                       p.Id = reader.GetInt32(startingIndex++);
                       p.UserId = reader.GetString(startingIndex++);
                       p.Title = reader.GetString(startingIndex++);
                       p.Description = reader.GetSafeString(startingIndex++);
                       p.CreateDate = reader.GetDateTime(startingIndex++);
                       p.ModifiedDate = reader.GetDateTime(startingIndex++);
                       p.Start = reader.GetDateTime(startingIndex++);
                       p.End = reader.GetDateTime(startingIndex++);
                       p.EventType = reader.GetInt32(startingIndex++);
                       p.IsPublic = reader.GetBoolean(startingIndex++);

                       Medias m = new Medias();

                       m.MediasTableId = reader.GetSafeInt32(startingIndex++);
                       m.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                       m.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
                       m.MediaType = reader.GetSafeEnum<MediaType>(startingIndex++);
                       m.ContentType = reader.GetSafeString(startingIndex++);
                       m.UserId = reader.GetSafeString(startingIndex++);
                       m.FilePath = reader.GetSafeString(startingIndex++);

                       p.Media = m;
                   }

                   if (set == 1)
                   {

                       myTags.EventId = reader.GetSafeInt32(startingIndex++);
                       myTags.Id = reader.GetSafeInt32(startingIndex++);
                       myTags.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                       myTags.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
                       myTags.TagName = reader.GetSafeString(startingIndex++);
                       myTags.TagSlug = reader.GetSafeString(startingIndex++);
                       myTags.ParentTagId = reader.GetSafeInt32(startingIndex++);

                       tagsList.Add(myTags);
                   }
                   if (tagsList == null)
                   {
                       tagsList = new List<EventTags>();
                   }
                   if (list == null)
                   {
                       list = new List<Events>();
                   }
                   list.Add(p);
               }
               );
            if (tagsList != null)
            {
                foreach (Events p in list)
                {
                    List<Tags> evtTags = new List<Tags>();
                    foreach (EventTags tag in tagsList)
                    {
                        if (p.Id == tag.EventId)
                        {
                            Tags newTag = new Tags();
                            newTag.Id = tag.Id;
                            newTag.CreatedDate = tag.CreatedDate;
                            newTag.ModifiedDate = tag.ModifiedDate;
                            newTag.TagName = tag.TagName;
                            newTag.TagSlug = tag.TagSlug;
                            newTag.ParentTagId = tag.ParentTagId;
                            evtTags.Add(newTag);
                        }
                    }
                    p.Tags = evtTags;
                }
            }
            return list;
        }

        public static Events GetById(int id)
        {
            Events p = null;


            DataProvider.ExecuteCmd(GetConnection, "dbo.Events_SelectById"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Id", id);
               }, map: delegate (IDataReader reader, short set)
               {

                   int startingIndex = 0; //startingOrdinal

                   //if (set == 0)
                   //{
                       p = new Events();

                       p.Id = reader.GetInt32(startingIndex++);
                       p.UserId = reader.GetString(startingIndex++);
                       p.Title = reader.GetString(startingIndex++);
                       p.Description = reader.GetSafeString(startingIndex++);
                       p.CreateDate = reader.GetDateTime(startingIndex++);
                       p.ModifiedDate = reader.GetDateTime(startingIndex++);
                       p.Start = reader.GetDateTime(startingIndex++);
                       p.End = reader.GetSafeDateTime(startingIndex++);
                       p.EventType = reader.GetSafeInt32(startingIndex++);
                       p.IsPublic = reader.GetSafeBool(startingIndex++);
                       p.MediaId = reader.GetSafeInt32(startingIndex++);
                       p.CountYes = reader.GetSafeInt32(startingIndex++);
                       p.CountNo = reader.GetSafeInt32(startingIndex++);
                       p.CountMaybe = reader.GetSafeInt32(startingIndex++);

                       Medias m = new Medias();

                       m.MediasTableId = reader.GetSafeInt32(startingIndex++);
                       m.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                       m.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
                       m.MediaType = reader.GetSafeEnum<MediaType>(startingIndex++);
                       m.ContentType = reader.GetSafeString(startingIndex++);
                       m.UserId = reader.GetSafeString(startingIndex++);
                       m.FilePath = reader.GetSafeString(startingIndex++);

                       p.Media = m;
                   //   tags commented out - function pass to decorate function where tags will be added
                   //    p.Tags = new List<Tags>();
                   //}
                   //else if (set == 1)
                   //{
                   //    Tags myTags = new Tags();

                   //    myTags.Id = reader.GetSafeInt32(startingIndex++);
                   //    myTags.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                   //    myTags.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
                   //    myTags.TagName = reader.GetSafeString(startingIndex++);
                   //    myTags.TagSlug = reader.GetSafeString(startingIndex++);
                   //    myTags.ParentTagId = reader.GetSafeInt32(startingIndex++);

                   //    p.Tags.Add(myTags);
                   //}
               }
               );

            return DecorateEvent(p);
        }

        public static void Delete(int Id)
        {

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Events_Delete"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Id", Id);

               }, returnParameters: delegate (SqlParameterCollection param)
               {
               }
               );
        }

        public static void UpdateMedia(int Id, EventsRequest model)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Events_UpdateMediaOnly"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Id", Id);
                   paramCollection.AddWithValue("@MediaId", model.MediaId);

               }, returnParameters: delegate (SqlParameterCollection param)
               {
               }
               );
        }

        public static List<Events> EventsSearch(string SearchString, int PageNumber = 1, int PageSize = 5)
        {
            List<Events> list = null;
            DataProvider.ExecuteCmd(GetConnection, "dbo.Events_EventSearch"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@PageNumber", PageNumber);
                   paramCollection.AddWithValue("@PageSize", PageSize);
                   paramCollection.AddWithValue("@Search", SearchString);
               }, map: delegate (IDataReader reader, short set)
               {

                   Events e = new Events();
                   int startingIndex = 0;
                   e.Id = reader.GetSafeInt32(startingIndex++);
                   e.UserId = reader.GetSafeString(startingIndex++);
                   e.EventType = reader.GetSafeInt32(startingIndex++);
                   e.IsPublic = reader.GetSafeBool(startingIndex++);
                   e.Title = reader.GetSafeString(startingIndex++);
                   e.Description = reader.GetSafeString(startingIndex++);
                   e.CreateDate = reader.GetSafeDateTime(startingIndex++);
                   e.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
                   e.Start = reader.GetSafeDateTime(startingIndex++);
                   e.End = reader.GetSafeDateTime(startingIndex++);
                   if (list == null)
                   {
                       list = new List<Events>();
                   }
                   list.Add(e);

               }
               );
            return list;
        }


        public static List<Events> EventsByLocation(decimal lat, decimal lon, decimal distance)
        {
            List<Events> list = null;
            DataProvider.ExecuteCmd(GetConnection, "dbo.Events_EventSearchLocationRadius"
                   , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                   {
                       paramCollection.AddWithValue("@lat", lat);
                       paramCollection.AddWithValue("@lon", lon);
                       paramCollection.AddWithValue("@distance", distance);
                   }, map: delegate (IDataReader reader, short set)
                   {

                       //public int Id { get; set; }
                       //public string UserId { get; set; }
                       //public int EventType { get; set; }
                       //public bool IsPublic { get; set; }
                       //public string Title { get; set; }
                       //public string Description { get; set; }
                       //public int MediaId { get; set; }
                       //public int CountYes { get; set; }
                       //public int CountNo { get; set; }
                       //public int CountMaybe { get; set; }
                       //public DateTime CreateDate { get; set; }
                       //public DateTime ModifiedDate { get; set; }
                       //public DateTime Start { get; set; }
                       //public DateTime End { get; set; }
                       //public Location Location { get; set; }

                       Events e = new Events();
                       int startingIndex = 0;
                       e.Id = reader.GetSafeInt32(startingIndex++);
                       e.UserId = reader.GetSafeString(startingIndex++);
                       e.EventType = reader.GetSafeInt32(startingIndex++);
                       e.IsPublic = reader.GetSafeBool(startingIndex++);
                       e.Title = reader.GetSafeString(startingIndex++);
                       e.Description = reader.GetSafeString(startingIndex++);
                       e.CountYes = reader.GetSafeInt32(startingIndex++);
                       e.CountNo = reader.GetSafeInt32(startingIndex++);
                       e.CountMaybe = reader.GetSafeInt32(startingIndex++);
                       e.CreateDate = reader.GetSafeDateTime(startingIndex++);
                       e.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
                       e.Start = reader.GetSafeDateTime(startingIndex++);
                       e.End = reader.GetSafeDateTime(startingIndex++);
                       e.MediaId = reader.GetSafeInt32(startingIndex++);

                       //public int Id { get; set; }
                       //public Decimal? Latitude { get; set; }
                       //public Decimal? Longitude { get; set; }
                       //public string Address { get; set; }
                       //public string City { get; set; }
                       //public string State { get; set; }
                       //public string ZipCode { get; set; }

                       Location l = new Location();
                       l.Id = reader.GetSafeInt32(startingIndex++);
                       l.Latitude = reader.GetSafeDecimal(startingIndex++);
                       l.Longitude = reader.GetSafeDecimal(startingIndex++);
                       l.Address = reader.GetSafeString(startingIndex++);
                       l.City = reader.GetSafeString(startingIndex++);
                       l.State = reader.GetSafeString(startingIndex++);
                       l.ZipCode = reader.GetSafeString(startingIndex++);

                       e.Location = l;

                       if (list == null)
                       {
                           list = new List<Events>();
                       }
                       list.Add(e);

                   }
                   );
            return DecorateEvents(list);
        }



        public static List<Events> GetByTagSlug(string slug)
        {
            List<Events> list = null;
            DataProvider.ExecuteCmd(GetConnection, "dbo.Tags_SelectEventsByTag"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@TagSlug", slug);
               }, map: delegate (IDataReader reader, short set)
               {
                   int startingIndex = 0; //startingOrdinal
                   Events p = new Events();

                   p.Id = reader.GetInt32(startingIndex++);
                   p.UserId = reader.GetString(startingIndex++);
                   p.EventType = reader.GetSafeInt32(startingIndex++);
                   p.IsPublic = reader.GetSafeBool(startingIndex++);
                   p.Title = reader.GetString(startingIndex++);
                   p.Description = reader.GetSafeString(startingIndex++);
                   p.CreateDate = reader.GetDateTime(startingIndex++);
                   p.ModifiedDate = reader.GetDateTime(startingIndex++);
                   p.Start = reader.GetDateTime(startingIndex++);
                   p.End = reader.GetSafeDateTime(startingIndex++);
                   p.MediaId = reader.GetSafeInt32(startingIndex++);
                   p.CountYes = reader.GetSafeInt32(startingIndex++);
                   p.CountNo = reader.GetSafeInt32(startingIndex++);
                   p.CountMaybe = reader.GetSafeInt32(startingIndex++);

                   Medias m = new Medias();

                   m.MediasTableId = reader.GetSafeInt32(startingIndex++);
                   m.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                   m.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
                   m.MediaType = reader.GetSafeEnum<MediaType>(startingIndex++);
                   m.ContentType = reader.GetSafeString(startingIndex++);
                   m.UserId = reader.GetSafeString(startingIndex++);
                   m.FilePath = reader.GetSafeString(startingIndex++);

                   p.Media = m;

                   UserMini u = new UserMini();

                   u.profileName = reader.GetSafeString(startingIndex++);
                   u.profileLastName = reader.GetSafeString(startingIndex++);

                   p.Organizer = u;

                   if (list == null)
                   {
                       list = new List<Events>();
                   }
                   list.Add(p);
               }
               );
            return DecorateEvents(list);  //list of events that have a certain tag
        }

        public static Events DecorateEvent(Events events)
        {
            List<Events> newList = new List<Events>(); //create list to use existing decorate function
            newList.Add(events);
            newList = DecorateEvents(newList);
            return newList[0];  //grabbing index 0 of list because only grabbing a single result
        }

        //DECORATE EVENTS ---LOCATION----TAGS-----ATTENDEES
        public static List<Events> DecorateEvents(List<Events> events)
        {
            if (events == null)
                return events;

            List<int> evtId = new List<int>();

            foreach (Events evt in events)  //getting just the eventID of the list of events from above
            {
                evtId.Add(evt.Id);
            }

            List<EventLocation> locList = null;
            List<EventTags> tagList = null;
            List<EventAttendee> attendList = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Location_EventTagLocationDecorate"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   SqlParameter s = new SqlParameter("@EventIds", SqlDbType.Structured);
                   if (evtId != null && evtId.Any())
                   {
                       s.Value = new IntIdTable(evtId);
                   }
                   paramCollection.Add(s);

               }, map: delegate (IDataReader reader, short set) //executes after stored proc
               {
                   
                   int startingIndex = 0; //startingOrdinal

                   if (set == 0)
                   {
                       EventLocation p = new EventLocation();

                       p.EventId = reader.GetSafeInt32(startingIndex++);
                       p.Id = reader.GetSafeInt32(startingIndex++);
                       p.Latitude = reader.GetSafeDecimal(startingIndex++);
                       p.Longitude = reader.GetSafeDecimal(startingIndex++);
                       p.Address = reader.GetSafeString(startingIndex++);
                       p.City = reader.GetSafeString(startingIndex++);
                       p.State = reader.GetSafeString(startingIndex++);
                       p.ZipCode = reader.GetSafeString(startingIndex++);
                       if (locList == null)
                       {
                           locList = new List<EventLocation>();
                       }
                       locList.Add(p);
                   }

                   if (set == 1)
                   {
                       EventTags t = new EventTags();

                       t.EventId = reader.GetSafeInt32(startingIndex++);
                       t.Id = reader.GetSafeInt32(startingIndex++);
                       t.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                       t.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
                       t.TagName = reader.GetSafeString(startingIndex++);
                       t.TagSlug = reader.GetSafeString(startingIndex++);
                       t.ParentTagId = reader.GetSafeInt32(startingIndex++);

                       if (tagList == null)
                       {
                           tagList = new List<EventTags>();
                       }
                       tagList.Add(t);
                   }

                   if (set == 2)
                   {
                       EventAttendee ea = new EventAttendee();

                       ea.EventId = reader.GetSafeInt32(startingIndex++);
                       ea.profileName = reader.GetSafeString(startingIndex++);
                       ea.profileLastName = reader.GetSafeString(startingIndex++);
                       ea.UserId = reader.GetSafeString(startingIndex++);

                       Medias m = new Medias();

                       m.MediasTableId = reader.GetSafeInt32(startingIndex++);
                       m.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                       m.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
                       m.MediaType = reader.GetSafeEnum<MediaType>(startingIndex++);
                       m.ContentType = reader.GetSafeString(startingIndex++);
                       m.UserId = reader.GetSafeString(startingIndex++);
                       m.FilePath = reader.GetSafeString(startingIndex++);
                       
                       ea.Avatar = m;

                       if (attendList == null)
                       {
                           attendList = new List<EventAttendee>();
                       }
                       attendList.Add(ea);
                   }

               }
               );


            if (locList != null)  //matching locations in locationList to event in eventsList
            {
                foreach (EventLocation loc in locList)
                {
                    foreach (Events evt in events)
                    {
                        if (loc.EventId == evt.Id)
                        {
                            evt.Location = loc;
                            break;
                        }
                    }
                }
            }

            foreach (Events evt in events)
            {
                if (tagList != null)
                {
                    List<Tags> evtTags = new List<Tags>();
                    foreach (EventTags tag in tagList)
                    {
                        if (tag.EventId == evt.Id)
                        {
                            evtTags.Add(tag);
                        }
                    }
                    evt.Tags = evtTags;
                }
                if (attendList != null)
                {
                    List<UserMini> evtAttendee = new List<UserMini>();
                    foreach (EventAttendee evtatt in attendList)
                    {
                        if (evtatt.EventId == evt.Id)
                        {
                            evtAttendee.Add(evtatt);
                        }
                    }
                    evt.Attendees = evtAttendee;
                }
            }

            return events;
        }
        //END DECORATE EVENTS




        public static List<TagsCount> TagsThatHaveEvents()
        {
            List<TagsCount> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.EventTags_TagsWithEvents"
               , inputParamMapper: null//executes before stored proc
               , map: delegate (IDataReader reader, short set) //executes after stored proc
               {
                   TagsCount p = new TagsCount();
                   int startingIndex = 0; //startingOrdinal

                   p.Id = reader.GetInt32(startingIndex++);
                   p.TagName = reader.GetString(startingIndex++);
                   p.TagSlug = reader.GetString(startingIndex++);
                   p.Count = reader.GetInt32(startingIndex++);

                   if (list == null)
                   {
                       list = new List<TagsCount>();
                   }
                   list.Add(p);
               }
               );
            return list;
        }
    }
}


