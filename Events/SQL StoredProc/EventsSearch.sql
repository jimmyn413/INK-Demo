USE [C20]
GO
/****** Object:  StoredProcedure [dbo].[Events_Search]    Script Date: 9/19/2016 2:08:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROC [dbo].[Events_SearchEpic]

						@PageNumber int = 1,
						@PageSize int = 48,
						@lat decimal (9,6) = null,
						@lon decimal (9,6) = null,
						@distance decimal (9,6) = null,
						@SearchString nvarchar(128) = null

as


/*

			DECLARE		@PageNumber int = 1,
						@PageSize int = 48,
						@lat decimal (9,6) = null,
						@lon decimal (9,6) = null,
						@distance decimal (9,6) = null,
						@SearchString nvarchar(128) = 'my event'

			EXECUTE		[dbo].[Events_SearchEpic]
						@PageNumber,
						@PageSize,
						@lat,
						@lon,
						@distance,
						@SearchString

*/


BEGIN

				IF @lat IS NOT NULL

				BEGIN

							DECLARE 
								@maxLat decimal (9,6), 
								@minLat decimal (9,6), 
								@maxLon decimal (9,6), 
								@minLon decimal (9,6)

							DECLARE 
								@R decimal

							SET @R = 3959 -- Earth Radius in Miles
							SET @maxLat = @lat + DEGREES(@distance/@R)
							SET @minLat = @lat - DEGREES(@distance/@R)

							SET @maxLon = @lon + DEGREES((@distance/@R/COS(RADIANS(@lat))))
							SET @minLon = @lon - DEGREES((@distance/@R/COS(RADIANS(@lat)))) 

				END

				SELECT e.Id
					  ,e.UserId
					  ,e.EventType
					  ,e.IsPublic
					  ,e.Title
					  ,e.Description
					  ,e.CountYes
					  ,e.CountNo
					  ,e.CountMaybe
					  ,e.CreateDate
					  ,e.ModifiedDate
					  ,e.Start
					  ,e.[End]
					  ,e.MediaId
						  ,m.Id as MediasTableId
						  ,m.CreatedDate
						  ,m.ModifiedDate
						  ,m.MediaType
						  ,m.ContentType
						  ,m.UserId
						  ,m.FilePath
							  ,up.profileName
							  ,up.profileLastName
							  ,up.userId
								  ,n.Id as MediasTableId
								  ,n.CreatedDate
								  ,n.ModifiedDate
								  ,n.MediaType
								  ,n.ContentType
								  ,n.UserId
								  ,n.FilePath
									  ,l.Id
									  ,l.Lat
									  ,l.Lng
									  ,l.Address
									  ,l.City
									  ,l.State
									  ,l.ZipCode

				  FROM dbo.Events as e
					  left join EventLocation as el
							on e.Id = el.EventId
							  left join Location as l
									on el.LocationId = l.Id 
									  left join Medias as m
											on e.MediaId = m.Id
												left join UserProfiles as up
													on e.UserId = up.userId
														left join Medias as n
															on up.MediaId = n.Id

										WHERE e.Id IN (SELECT
															e.Id
											
															FROM Events AS e
																LEFT JOIN EventTags AS et
																	ON et.EventId = e.Id
																		LEFT JOIN Tags AS t
																			On t.Id = et.EventId																

										WHERE 
										(
											(

												(@SearchString  IS NULL)
			
												OR

												(		 
													(e.Title LIKE '%'+@SearchString+'%')
													OR (t.TagName LIKE '%'+@SearchString+'%')
												)
											)

											AND

											(
												(@lat IS NULL)
		
												OR

												(l.Lat between @minLat and @maxLat and l.Lng between @minLon and @maxLon)
		
											)
										))
	
											ORDER BY e.Start ASC

											OFFSET ((@PageNumber - 1) * @PageSize) ROWS
											FETCH NEXT  @PageSize ROWS ONLY
	
END